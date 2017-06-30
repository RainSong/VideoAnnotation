using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using System.Dynamic;

namespace VideoAnnotation
{
    public partial class MainForm : Form
    {
        private string[] fileTypes;
        private List<string> dataSource;
        public MainForm()
        {
            InitializeComponent();
            fileTypes = new string[] { "mp4", "avi", "rmvb", "mkv" };
            dataSource = new List<string>();
            var files = DataHelper.GetFiles();
            BindDataToFileListView();
        }

        #region events

        private void MenuItemOpenFile_Click(object sender, EventArgs e)
        {
            if (this.OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(this.OpenFileDialog.FileName);
            }
        }

        private void MenuItemOpenFolder_Click(object sender, EventArgs e)
        {
            if (this.FolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                var path = this.FolderBrowserDialog.SelectedPath;
                var files = GetFiles(path);
                SaveFilesToDD(files);
                BindDataToFileListView();
            }
        }

        private void MenuItemPlay_Click(object sender, EventArgs e)
        {
            var selectItems = this.listViewFiles.SelectedItems;
            if (selectItems == null || selectItems.Count == 0)
            {
                MessageBox.Show("请选择要播放的文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var filePath = selectItems[0].Text;
            this.vlcPlayer.SetMedia(new FileInfo(filePath));
            this.vlcPlayer.Play();
        }
        private void vlcPlayer_VlcLibDirectoryNeeded(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {

            RegistryKey pregkey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");
            var names = pregkey.GetSubKeyNames();
            var vlcPlayerName = names.FirstOrDefault(o => o.Contains("VLC"));
            if (vlcPlayerName == null)
            {
                MessageBox.Show("错误", "请先安装VLC播放器", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                RegistryKey currentKey = pregkey.OpenSubKey(vlcPlayerName);
                var installLocation = currentKey.GetValue("InstallLocation");
                if (installLocation != null)
                {
                    e.VlcLibDirectory = new DirectoryInfo(installLocation.ToString());
                }
            }
            #region MyRegion
            //var currentAssembly = Assembly.GetEntryAssembly();
            //var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            //if (currentDirectory == null)
            //    return;
            //if (AssemblyName.GetAssemblyName(currentAssembly.Location).ProcessorArchitecture == ProcessorArchitecture.X86)
            //    e.VlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory, @"\lib\x86\"));
            //else
            //    e.VlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory, @"\lib\x64\"));

            //if (!e.VlcLibDirectory.Exists)
            //{
            //    var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            //    folderBrowserDialog.Description = "请选择VLC播放器位置";
            //    folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
            //    folderBrowserDialog.ShowNewFolderButton = true;
            //    if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            //    {
            //        e.VlcLibDirectory = new DirectoryInfo(folderBrowserDialog.SelectedPath);
            //    }
            //} 
            #endregion
        }

        private void listView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            var listbox = sender as ListBox;
            if (listbox == null) return;
            e.DrawBackground();
            e.DrawFocusRectangle();

            ////让文字位于Item的中间
            //float difH = (e.Bounds.Height - e.Font.Height) / 2;
            //RectangleF rf = new RectangleF(e.Bounds.X, e.Bounds.Y + difH, e.Bounds.Width, e.Font.Height);
            //e.Graphics.DrawString(listBox1.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), rf);

            var font = new Font("宋体", 20);
            e.Item.Font = font;
            e.Graphics.DrawString(listbox.Items[e.ItemIndex].ToString(), font, new SolidBrush(Color.Black), e.Bounds);
        }
        #endregion

        #region methods
        private List<string> GetFiles(string path)
        {
            if (fileTypes == null || !fileTypes.Any()) return null;
            Func<string, List<string>> funGetFiles = null;
            funGetFiles = tempPath =>
            {
                var di = new System.IO.DirectoryInfo(tempPath);
                var files = new List<string>();
                foreach (var fileType in this.fileTypes)
                {
                    files.AddRange(di.GetFiles("*." + fileType).Select(o => o.FullName).ToList());
                }
                var dirs = di.GetDirectories();
                if (dirs.Any())
                {
                    foreach (var tempDir in dirs)
                    {
                        files.AddRange(funGetFiles(tempDir.FullName).ToList());
                    }
                }
                return files;
            };
            return funGetFiles(path);

        }

        private void BindDataToFileListView()
        {
            var dtFiles = DataHelper.GetFiles();
            if (dtFiles != null && dtFiles.Rows.Count > 0)
            {
                foreach (DataRow dr in dtFiles.Rows)
                {
                    var fileName = dr.Field<string>("file_full_name");
                    if (fileName != null && !string.IsNullOrEmpty(fileName))
                    {
                        this.listViewFiles.Items.Add(new ListViewItem(fileName));
                    }
                }
            }
        }

        private void SaveFilesToDD(List<string> fileNames)
        {
            var list = new List<dynamic>();
            foreach (string fileName in fileNames)
            {
                dynamic fileInfo = new ExpandoObject();
                fileInfo.fileFullName = fileName;
                var index = fileName.LastIndexOf("\\");
                fileInfo.fileName = fileName.Substring(index + 1, fileName.Length - index - 1);
                list.Add(fileInfo);
            }
            try
            {
                var result = DataHelper.SaveFiles(list);
            }
            catch (Exception ex)
            {
                MessageBox.Show("发生错误，保存失败，" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (this.btnStartStop.Tag.Equals("START"))
            {
                this.btnStartStop.Text = "暂停";
                this.btnStartStop.Tag = "STOP";
            }
            else
            {
                this.btnStartStop.Text = "开始";
                this.btnStartStop.Tag = "START";
            }
        }
    }
}
