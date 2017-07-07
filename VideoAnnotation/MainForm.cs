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
using System.Security.Cryptography;
using System.Diagnostics;

namespace VideoAnnotation
{
    public partial class MainForm : Form
    {
        private static string[] fileTypes = new string[] { "mp4", "avi", "rmvb", "mkv" };
        //更新UI
        private Action<float> updateUi;
        private Vlc.DotNet.Core.VlcMedia CurrentMedia;

        public MainForm()
        {
            InitializeComponent();
            //this.vlcPlayer.PositionChanged += vlcPlayer_PositionChanged;
            this.DataGridFiles.AutoGenerateColumns = false;
            updateUi = (position) =>
            {
                var p = this.CurrentMedia.Duration.Ticks * position;
                var time = new DateTime((long)p);
                this.labelVideoPosition.Text = string.Format("{0} / {1}:{2}:{3}",
                     time.ToString("T"),
                    this.CurrentMedia.Duration.Hours,
                    this.CurrentMedia.Duration.Minutes.ToString().PadLeft(2, '0'),
                    this.CurrentMedia.Duration.Seconds.ToString().PadLeft(2, '0'));

                this.trackBarPosition.Maximum = (int)this.CurrentMedia.Duration.TotalSeconds;
                this.trackBarPosition.Value = time.Hour * 60 * 60 + time.Minute * 60 + time.Second;
            };
        }
        #region events
        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            var files = DataHelper.GetFiles();
            BindDataToFileListView();
            //try
            //{
            //    Task task = new Task(HashFiles);
            //    task.Start();
            //}
            //catch (Exception ex)
            //{

            //}
        }
        private void MenuItemOpenFile_Click(object sender, EventArgs e)
        {
            if (this.OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(this.OpenFileDialog.FileName);
            }
        }
        /// <summary>
        /// 菜单打开文件夹按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// 设置VLC播放器控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        }
        /// <summary>
        /// 开始/暂停播放按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (this.btnStartStop.Tag.Equals("START"))
            {
                if (this.CurrentMedia == null)
                {
                    var result = GetSelectRowValues();
                    PlayVideo(result.fileName);
                }
                else
                {
                    this.Start();
                }
            }
            else
            {
                this.Stop();
            }
        }
        /// <summary>
        /// 播放器视频播放位置改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void vlcPlayer_PositionChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs e)
        {
            if (this.updateUi != null)
            {
                Invoke(updateUi, e.NewPosition);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBarPosition_Scroll(object sender, EventArgs e)
        {
            //TODO YGJ 此处应该播放时间根据拖动位置改变
            this.vlcPlayer.Position = ((float)this.trackBarPosition.Value) / this.trackBarPosition.Maximum;
        }
        #endregion

        #region methods
        /// <summary>
        /// 从数据库中获取文件信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<string> GetFiles(string path)
        {
            if (fileTypes == null || !fileTypes.Any()) return null;
            Func<string, List<string>> funGetFiles = null;
            funGetFiles = tempPath =>
            {
                var di = new System.IO.DirectoryInfo(tempPath);
                var files = new List<string>();
                foreach (var fileType in fileTypes)
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
        /// <summary>
        /// 给文件列表绑定数据
        /// </summary>
        private void BindDataToFileListView()
        {
            var dtFiles = DataHelper.GetFiles();
            this.DataGridFiles.DataSource = dtFiles;
        }
        /// <summary>
        /// 打开文件夹后，将文件信息保存数据至数据库
        /// </summary>
        /// <param name="fileNames"></param>
        private void SaveFilesToDD(List<string> fileNames)
        {
            var list = new List<dynamic>();
            foreach (string fileName in fileNames)
            {
                dynamic fileInfo = new ExpandoObject();
                fileInfo.fileFullName = fileName;
                var index = fileName.LastIndexOf("\\");
                fileInfo.fileName = fileName.Substring(index + 1, fileName.Length - index - 1);
                fileInfo.id = Guid.NewGuid().ToString().Replace("-", "");
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
        /// <summary>
        /// 播放视频
        /// </summary>
        /// <param name="filePath"></param>
        private void PlayVideo(string filePath)
        {
            this.vlcPlayer.SetMedia(new FileInfo(filePath));
            this.vlcPlayer.Play();
            this.CurrentMedia = this.vlcPlayer.GetCurrentMedia();
        }

        private void HashFiles()
        {
            //dynamic fileInfo = DataHelper.GetNoHashFile();
            dynamic fileInfo;
            while ((fileInfo = DataHelper.GetNoHashFile()) != null)
            {
                if (!File.Exists(fileInfo.file_full_name))
                {
                    DataHelper.UpdateFileUseable(fileInfo.id, false);
                }
                else
                {
                    //var tempFile = Path.Combine(fileInfo.file_full_name, ".an.temp");
                    //var fi = new FileInfo(tempFile);
                    //FileStream stream;
                    //if (!fi.Exists)
                    //{
                    //    stream = fi.Create();
                    //}
                    //else
                    //{
                    //    stream = fi.OpenWrite();
                    //}
                    var code = HashFile(fileInfo.file_full_name);
                    DataHelper.UpdateFileHash(fileInfo.id, code);
                }
            }
        }
        private string HashFile(string path)
        {
            int bufferSize = 1024 * 1024;//自定义缓冲区大小1MB  
            byte[] buffer = new byte[bufferSize];
            string hashCode = null;
            using (var inputStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var hashAlgorithm = new MD5CryptoServiceProvider();
                int readLength = 0;//每次读取长度  
                var output = new byte[bufferSize];
                while ((readLength = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    //计算MD5  
                    hashAlgorithm.TransformBlock(buffer, 0, readLength, output, 0);
                }
                //完成最后计算，必须调用(由于上一部循环已经完成所有运算，所以调用此方法时后面的两个参数都为0)  
                hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
                hashCode = BitConverter.ToString(hashAlgorithm.Hash);
                hashAlgorithm.Clear();
            }
            hashCode = hashCode.Replace("-", "");
            return hashCode;
        }
        #endregion

        private void MenuItemAddAnnotation_Click(object sender, EventArgs e)
        {
            if (this.vlcPlayer.Position < 0)
            {
                MessageBox.Show("视频还没有开始播放", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            JP();
            dynamic result = GetSelectRowValues();
            if (result != null)
            {
                var addFrom = new AddAnnotation();
                addFrom.Position = this.vlcPlayer.Position;
                addFrom.StartPosition = FormStartPosition.CenterParent;
                addFrom.ParentForm = this;
                addFrom.FileId = result.id;
                addFrom.ShowDialog();
            }
        }
        public void Start()
        {
            this.btnStartStop.Tag = "Start";
            this.btnStartStop.Text = "暂停";
            this.vlcPlayer.Play();
        }

        public void Stop()
        {
            this.btnStartStop.Tag = "Stop";
            this.btnStartStop.Text = "开始";
            this.vlcPlayer.Stop();
        }

        public dynamic GetSelectRowValues(DataGridViewRow row = null)
        {
            if (row == null)
            {
                var selectRows = this.DataGridFiles.SelectedRows;
                if (selectRows == null || selectRows.Count == 0) return null;
                row = selectRows[0];
            }
            var id = row.Cells["colID"].Value == null ? string.Empty : row.Cells["colID"].Value.ToString();
            var fileName = row.Cells["colFileFullName"].Value == null ? string.Empty : row.Cells["colFileFullName"].Value.ToString();
            dynamic result = new ExpandoObject();
            result.id = id;
            result.fileName = fileName;
            return result;
        }

        private void DataGridFiles_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = this.DataGridFiles.Rows[e.RowIndex];
            var result = GetSelectRowValues(row);
            PlayVideo(result.fileName);
        }

        private string JP()
        {
            //string str1 = Process.GetCurrentProcess().MainModule.FileName;//可获得当前执行的exe的文件名。 
            //string str2 = Environment.CurrentDirectory;//获取和设置当前目录（即该进程从中启动的目录）的完全限定路径。(备注:按照定义，如果该进程在本地或网络驱动器的根目录中启动，则此属性的值为驱动器名称后跟一个尾部反斜杠（如“C:\”）。如果该进程在子目录中启动，则此属性的值为不带尾部反斜杠的驱动器和子目录路径[如“C:\mySubDirectory”])。 
            //string str3 = Directory.GetCurrentDirectory(); //获取应用程序的当前工作目录。 
            //string str4 = AppDomain.CurrentDomain.BaseDirectory;//获取基目录，它由程序集冲突解决程序用来探测程序集。 
            //string str5 = Application.StartupPath;//获取启动了应用程序的可执行文件的路径，不包括可执行文件的名称。 
            //string str6 = Application.ExecutablePath;//获取启动了应用程序的可执行文件的路径，包括可执行文件的名称。 
            //string str7 = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;//获取或设置包含该应用程序的目录的名称。
            var path = Directory.GetCurrentDirectory().Replace("\\bin\\debug\\", "");
            var result = GetSelectRowValues();
            path = Path.Combine(path, "imgs", result.id);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var imgName = Guid.NewGuid().ToString().Replace("-", "") + ".jpg";
            path = Path.Combine(path, imgName);
            return string.Empty;
        }

    }
}
