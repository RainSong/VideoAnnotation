using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoAnnotation
{
    public partial class AddAnnotation : Form
    {
        public string FileId;
        public Form ParentForm;
        public float Position { get; set; }

        public AddAnnotation()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

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
                if (DataHelper.AddAnnotation(this.FileId, this.Position, strAnnotation))
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
        }

        private void AddAnnotation_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.Position.ToString();
        }
    }
}
