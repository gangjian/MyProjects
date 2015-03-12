using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VisualizeMyLife
{
    public partial class TomatoTimerForm : Form
    {
        private int _timerLength = (25 * 60);     // 番茄钟的时长(默认25分钟), 单位是秒
        private int _tickCounter = 0;
        private float _sweepAngle = 0;

        private Graphics _pictureBoxGraphics;
        private Bitmap _backGroundBitmap;
        private Graphics _backGroundBitmapGraphics;

        public TomatoTimerForm(int timerLength = (25 * 60))
        {
            InitializeComponent();
            picBoxResize();
            initGraphics();
            _timerLength = timerLength;
            timer1.Start();
        }

        private void initGraphics()
        {
            _pictureBoxGraphics = this.pictureBox1.CreateGraphics();
            _backGroundBitmapGraphics = Graphics.FromImage(_backGroundBitmap);
        }

        private void picBoxResize()
        {
            int maxLength = this.ClientSize.Width;
            if (maxLength > this.ClientSize.Height)
            {
                maxLength = this.ClientSize.Height;
            }
            this.pictureBox1.Width = maxLength - 5;
            this.pictureBox1.Height = maxLength - 5;
            this.pictureBox1.Location = new Point(2, 2);
            if (null != _backGroundBitmap)
            {
                _backGroundBitmap.Dispose();
            }
            _backGroundBitmap = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            _tickCounter = 0;
            buttonStart.Text = "Reset";
            updateTimerView();
        }

        private void TomatoTimerForm_Paint(object sender, PaintEventArgs e)
        {
            updateTimerView();
        }

        private void TomatoTimerForm_Resize(object sender, EventArgs e)
        {
            picBoxResize();
            initGraphics();
        }

        // 画饼状图
        private void drawPieTimerView()
        {
            Rectangle rect = new Rectangle(0, 0, _backGroundBitmap.Width-1, _backGroundBitmap.Height-1);
            float startAngle = 270.0F;
            // 内圈的区域
            float smallRectWidth = (float)(rect.Width * 0.7);
            Rectangle smallRect = new Rectangle((int)((rect.Width - smallRectWidth) / 2), (int)((rect.Width - smallRectWidth) / 2), (int)smallRectWidth, (int)smallRectWidth);
            SolidBrush srBrush = new SolidBrush(Color.Black);
            if ((_tickCounter > 0)
                && (_tickCounter < _timerLength))
            {
                _backGroundBitmapGraphics.FillPie(new SolidBrush(Color.Gray), rect, 270.0F, _sweepAngle);
                _backGroundBitmapGraphics.FillPie(new SolidBrush(Color.DarkGreen), rect, startAngle + _sweepAngle, 360.0F - _sweepAngle);
                int countDown = (int)(_timerLength - _tickCounter);
                // 最后十秒内圈区域闪烁提示
                if (countDown <= 10)
                {
                    if (0 == (countDown % 2))
                    {
                        srBrush = new SolidBrush(Color.Red);
                    }
                    else
                    {
                        srBrush = new SolidBrush(Color.Yellow);
                    }
                    _backGroundBitmapGraphics.FillPie(srBrush, smallRect, 0, 360);
                    SizeF sizef = _backGroundBitmapGraphics.MeasureString(countDown.ToString(), new Font("Verdana", 22));
                    _backGroundBitmapGraphics.DrawString(countDown.ToString(), new Font("Verdana", 22), new SolidBrush(Color.Black),
                                                            (rect.Width - sizef.Width) / 2, (rect.Height - sizef.Height) / 2);
                }
                else
                {
                    _backGroundBitmapGraphics.FillPie(srBrush, smallRect, 0, 360);
                }
            }
            else
            {
                _backGroundBitmapGraphics.FillPie(new SolidBrush(Color.DarkGray), rect, 0.0F, 360.0F);
                _backGroundBitmapGraphics.FillPie(srBrush, smallRect, 0, 360);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if ("Reset" == buttonStart.Text)
            {
                if (_tickCounter < _timerLength)
                {
                    _tickCounter += 1;
                    _sweepAngle = (float)(360 * _tickCounter) / _timerLength;
                }
                else
                {
                    buttonStart.Text = "Start";
                }
            }
            updateTimerView();
        }

        private void updateTimerView()
        {
            _backGroundBitmapGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            _pictureBoxGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            _backGroundBitmapGraphics.Clear(Color.Black);

            drawPieTimerView();
            drawTimeString();
            _pictureBoxGraphics.DrawImage(_backGroundBitmap, 0, 0);
            this.Validate();
        }

        private void drawTimeString()
        {
            string timeStr = DateTime.Now.Hour.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Second.ToString().PadLeft(2, '0');
            int emSize = 16;
            Font ft = new System.Drawing.Font("Verdana", emSize);
            SizeF sizef = _backGroundBitmapGraphics.MeasureString(timeStr, ft);
            while (sizef.Width/(float)this.pictureBox1.Width < 0.25)
            {
                emSize += 1;
                ft = new System.Drawing.Font("Verdana", emSize);
                sizef = _backGroundBitmapGraphics.MeasureString(timeStr, ft);
            }
            _backGroundBitmapGraphics.DrawString(timeStr, ft, new SolidBrush(Color.LightPink),
                                                    (this.pictureBox1.Width - sizef.Width) / 2, (this.pictureBox1.Width - sizef.Height) / 2);
        }
    }
}
