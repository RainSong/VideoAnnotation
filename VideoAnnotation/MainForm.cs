﻿using System;
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
            this.vlcPlayer.PositionChanged += vlcPlayer_PositionChanged;

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

            HashFiles();
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
        /// 菜单播放按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemPlay_Click(object sender, EventArgs e)
        {
            var selectItems = this.listViewFiles.SelectedItems;
            if (selectItems == null || selectItems.Count == 0)
            {
                MessageBox.Show("请选择要播放的文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            PlayVideo(selectItems[0].Text);
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
        /// <summary>
        /// 开始/暂停播放按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (this.btnStartStop.Tag.Equals("START"))
            {
                this.btnStartStop.Text = "暂停";
                this.btnStartStop.Tag = "STOP";
                var selectItems = this.listViewFiles.SelectedItems;
                if (selectItems == null || selectItems.Count == 0)
                {
                    MessageBox.Show("请选择要播放的文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                PlayVideo(selectItems[0].Text);
            }
            else
            {
                this.btnStartStop.Text = "开始";
                this.btnStartStop.Tag = "START";
                this.vlcPlayer.Stop();
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
            dynamic fileInfo = DataHelper.GetNoHashFile();
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


    }
}
