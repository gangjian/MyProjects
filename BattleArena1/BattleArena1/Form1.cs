using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BattleArena1
{
    public partial class Form1 : Form
    {
        private SolidBrush _tankBrush = new SolidBrush(Color.Blue);
        private Point _position = new Point(50, 50);

        public Form1()
        {
            InitializeComponent();
            timer1.Start();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            drawTank(g, _position);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _position.X += 4;
            _position.Y += 2;
            pictureBox1.Invalidate();
        }

        private void drawTank(Graphics g, Point pt)
        {
            Rectangle tankRect = new Rectangle(pt.X, pt.Y, 25, 40);
            g.FillRectangle(_tankBrush, tankRect);
        }
    }
}
