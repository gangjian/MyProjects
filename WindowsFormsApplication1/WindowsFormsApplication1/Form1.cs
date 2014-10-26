using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private PictureBox _PictureBox;
        private Bitmap _backgroundMemoryBitmap = null;
        private Graphics _backgroundMemoryGraphic = null;   // 内存Graphic

        public Form1()
        {
            InitializeComponent();
            _PictureBox = this.pictureBox1;

            // 内存Bitmap
            _backgroundMemoryBitmap = new Bitmap(_PictureBox.ClientRectangle.Width, _PictureBox.ClientRectangle.Height);

            // 内存Graphic
            _backgroundMemoryGraphic = Graphics.FromImage(_backgroundMemoryBitmap);
            _backgroundMemoryGraphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            _backgroundMemoryGraphic.Clear(Color.Black);

            // 将位图显示到PictureBox上
            Image displayImage = Image.FromHbitmap(_backgroundMemoryBitmap.GetHbitmap());
            _PictureBox.Image = displayImage;
        }
    }
}
