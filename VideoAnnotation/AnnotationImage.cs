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
    public partial class AnnotationImage : Form
    {
        public AnnotationImage(string imagePath)
        {
            InitializeComponent();
            SetImage(imagePath);
        }

        public void SetImage(string imagePath)
        {
            using (var fs = new System.IO.FileStream(imagePath, System.IO.FileMode.Open))
            {
                var img = System.Drawing.Image.FromStream(fs);
                this.PictureBox.Image = img;
            }
        }
    }
}
