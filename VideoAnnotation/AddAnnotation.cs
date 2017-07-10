using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoAnnotation
{
    public partial class AddAnnotation : Form
    {
        public string FileId;
        public string ImgPath;
        private FileSystemWatcher FileWatcher;
        public float Position { get; set; }

        public AddAnnotation()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var strAnnotation = this.txtAnnotation.Text.Trim();
            if (string.IsNullOrEmpty(strAnnotation))
            {
                MessageBox.Show("注解内容不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                if (DataHelper.AddAnnotation(this.FileId, this.Position, strAnnotation, this.ImgPath))
                {

                    var parent = ParentForm as MainForm;
                    if (parent != null)
                    {
                        parent.Start();
                    }
                    this.Close();
                }
                else
                {
                    MessageBox.Show("发生错误，添加注解失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void AddAnnotation_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.Position.ToString();
            if (!string.IsNullOrEmpty(this.ImgPath))
            {
                if (!File.Exists(this.ImgPath))
                {
                    var path = this.ImgPath.Substring(0, this.ImgPath.LastIndexOf("\\") + 1);
                    InitFileWatcher(path);
                }
                else
                {
                    ShowImg();
                }
            }

        }
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private void InitFileWatcher(string path)
        {
            this.FileWatcher = new FileSystemWatcher();
            this.FileWatcher.Path = path;
            this.FileWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            //this.FileWatcher.Filter = "*.jpg";
            this.FileWatcher.Created += new FileSystemEventHandler(FileWatcher_Created);
            //this.FileWatcher.Changed += new FileSystemEventHandler(FileWatcher_Changed);

            this.FileWatcher.EnableRaisingEvents = true;
        }

        //private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        //{
        //    if (e.Name == this.ImgPath)
        //    {
        //        this.pictureBox1.Image = Image.FromFile(this.ImgPath);
        //    }
        //}

        private void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath == this.ImgPath)
            {
                Action action = () =>
                {
                    ShowImg();
                };
                Invoke(action);
            }
        }

        private void ShowImg()
        {
            using (var stram = new FileStream(this.ImgPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                this.pictureBox1.Image = Image.FromStream(stram);
                stram.Close();
            }
        }

        private void AddAnnotation_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }
    }
}
