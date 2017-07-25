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
using NLog;
namespace VideoAnnotation
{
    public partial class MainForm : Form
    {
        private static string[] fileTypes = new string[] { "mp4", "avi", "rmvb", "mkv" };
        //更新UI
        private Action<float> updateUi;
        private Vlc.DotNet.Core.VlcMedia CurrentMedia;
        private Logger logger;
        private string SelectFileId;
        private AnnotationImage ImgForm;

        private Panel ImagePanel;
        private PictureBox PicBox;

        public MainForm()
        {
            GlobalMouseHandler gmh = new GlobalMouseHandler();
            gmh.MouseMoved+=new MouseMovedEvent(MouseMoved);
            Application.AddMessageFilter(gmh);

            InitializeComponent();
            //this.vlcPlayer.PositionChanged += vlcPlayer_PositionChanged;
            this.DataGridFiles.AutoGenerateColumns =
                this.DataGridAnnotations.AutoGenerateColumns = false;
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
            this.logger = LogManager.GetCurrentClassLogger();
            this.SelectFileId = string.Empty;

            this.PicBox = new PictureBox();
            this.PicBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.PicBox.Dock = DockStyle.Fill;

            this.ImagePanel = new Panel();
            this.ImagePanel.Size = new Size(200, 200);
            this.ImagePanel.Visible = false;
            this.ImagePanel.Controls.Add(this.PicBox);
        }

        private void MouseMoved()
        {
            if (this.ImgForm != null)
            {
                Point mouseLoactioin = System.Windows.Forms.Cursor.Position;
                //this.ImgForm.Location = new Point(mouseLoactioin.X , mouseLoactioin.Y - this.ImgForm.Height);
            }
        }
        #region 事件
        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            //var files = DataHelper.GetFiles();
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
                try
                {
                    Invoke(updateUi, e.NewPosition);
                }
                catch (Exception ex)
                {

                }
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

        /// <summary>
        /// 菜单添加注解点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemAddAnnotation_Click(object sender, EventArgs e)
        {
            if (this.vlcPlayer.Position < 0)
            {
                MessageBox.Show("视频还没有开始播放", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            dynamic result = GetSelectRowValues();
            var position = this.vlcPlayer.Position;
            var imgPath = TakeSnapshot(result.id);
            Stop();
            if (result != null)
            {
                var addFrom = new AddAnnotation();
                addFrom.Position = position;
                addFrom.ImgPath = imgPath;
                addFrom.StartPosition = FormStartPosition.CenterParent;
                addFrom.FileId = result.id;
                if (addFrom.ShowDialog() == DialogResult.OK)
                {
                    this.LoadAnnotations(this.SelectFileId);
                    this.Start();
                }

            }
        }

        /// <summary>
        /// 文件列表单元格双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridFiles_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = this.DataGridFiles.Rows[e.RowIndex];
            var result = GetSelectRowValues(row);
            PlayVideo(result.fileName);
            this.Start();
        }


        /// <summary>
        /// 文件列表选中项改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridFiles_SelectionChanged(object sender, EventArgs e)
        {
            dynamic result = GetSelectRowValues();
            if (result == null) return;
            var id = result.id;
            if (!this.SelectFileId.Equals(id))
            {
                this.SelectFileId = id;
                LoadAnnotations(id);
            }
        }
        #endregion

        #region 方法
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
            try
            {
                var dtFiles = DataHelper.GetFiles();
                this.DataGridFiles.DataSource = dtFiles;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "获取文件信息失败");
            }
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
                this.logger.Error(ex, "发生错误，保存失败");
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
        /// <summary>
        /// 计算文件Hash值
        /// </summary>
        private void HashFiles()
        {
            try
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
                        var code = HashFile(fileInfo.file_full_name);
                        DataHelper.UpdateFileHash(fileInfo.id, code);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "计算文件Hash失败");
            }
        }
        /// <summary>
        /// 计算单个文件的Hash值
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 开始播放
        /// </summary>
        public void Start()
        {
            this.btnStartStop.Tag = "Start";
            this.btnStartStop.Text = "暂停";
            this.vlcPlayer.Play();
        }
        /// <summary>
        /// 暂停播放
        /// </summary>
        public void Stop()
        {
            this.btnStartStop.Tag = "Stop";
            this.btnStartStop.Text = "开始";
            this.vlcPlayer.Pause();
        }
        /// <summary>
        /// 获取选中行的数据
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 截屏
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        private string TakeSnapshot(string fileId)
        {
            //string str1 = Process.GetCurrentProcess().MainModule.FileName;//可获得当前执行的exe的文件名。 
            //string str2 = Environment.CurrentDirectory;//获取和设置当前目录（即该进程从中启动的目录）的完全限定路径。(备注:按照定义，如果该进程在本地或网络驱动器的根目录中启动，则此属性的值为驱动器名称后跟一个尾部反斜杠（如“C:\”）。如果该进程在子目录中启动，则此属性的值为不带尾部反斜杠的驱动器和子目录路径[如“C:\mySubDirectory”])。 
            //string str3 = Directory.GetCurrentDirectory(); //获取应用程序的当前工作目录。 
            //string str4 = AppDomain.CurrentDomain.BaseDirectory;//获取基目录，它由程序集冲突解决程序用来探测程序集。 
            //string str5 = Application.StartupPath;//获取启动了应用程序的可执行文件的路径，不包括可执行文件的名称。 
            //string str6 = Application.ExecutablePath;//获取启动了应用程序的可执行文件的路径，包括可执行文件的名称。 
            //string str7 = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;//获取或设置包含该应用程序的目录的名称。
            var path = Directory.GetCurrentDirectory();
            path = Path.Combine(path, "imgs", fileId);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var imgName = Guid.NewGuid().ToString().Replace("-", "") + ".jpg";
            path = Path.Combine(path, imgName);
            try
            {
                //this.vlcPlayer.OnSnapshotTaken(path);

                Action action = () =>
                {
                    this.vlcPlayer.TakeSnapshot(path);
                };
                //Invoke(action, path);

                Task task = new Task(action);
                task.Start();

            }
            catch (Exception ex)
            {
                throw new Exception("截屏失败", ex);
            }
            return path;
        }
        /// <summary>
        /// 跟住选中的文件ID加载备注
        /// </summary>
        /// <param name="fileId"></param>
        private void LoadAnnotations(string fileId)
        {
            try
            {
                var dt = DataHelper.GetAnnotations(fileId);
                this.DataGridAnnotations.DataSource = dt;
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "获取注解信息失败");
            }
        }
        #endregion

        private void DataGridAnnotations_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            var dt = this.DataGridAnnotations.DataSource as DataTable;
            if (dt != null)
            {
                var dr = dt.Rows[e.RowIndex];
                var imgPath = dr.Field<string>("img");
                //if (ImgForm == null)
                //{
                //    this.ImgForm = new AnnotationImage(img);
                //}
                //else
                //{
                //    this.ImgForm.SetImage(img);
                //}

                using (var fs = new System.IO.FileStream(imgPath, System.IO.FileMode.Open))
                {
                    var img = System.Drawing.Image.FromStream(fs);
                    this.PicBox.Image = img;
                }
                this.ImagePanel.Location = new Point(Cursor.Position.X - this.ImgForm.Width, Cursor.Position.Y - this.ImgForm.Height);
                //this.ImgForm.SetDesktopLocation(Cursor.Position.X - this.ImgForm.Width, Cursor.Position.Y - this.ImgForm.Height);
                //this.ImgForm.Show();
                this.ImagePanel.Visible = true;
            }
        }

        private void DataGridAnnotations_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            this.ImagePanel.Visible = false;
            //this.ImgForm.Hide();
        }
    }

    public delegate void MouseMovedEvent();

    public class GlobalMouseHandler : IMessageFilter
    {
        private const int WM_MOUSEMOVE = 0x0200;

        public event MouseMovedEvent MouseMoved;

        #region IMessageFilter Members

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_MOUSEMOVE)
            {
                if (MouseMoved != null)
                {
                    MouseMoved();
                }
            }
            // Always allow message to continue to the next filter control
            return false;
        }

        #endregion
    }
}
