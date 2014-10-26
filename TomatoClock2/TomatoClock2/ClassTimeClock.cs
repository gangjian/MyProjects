using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TomatoClock2
{
    class ClassTimeClock
    {
        private SolidBrush m_frameBrush = new SolidBrush(Color.Firebrick);
        private SolidBrush m_backGroundBrush = new SolidBrush(Color.DarkGray);
        private SolidBrush m_handBrush = new SolidBrush(Color.Black);
        private Rectangle m_outerFramRect = new Rectangle(10,  5, 200, 200);
        private Rectangle m_innerFramRect = new Rectangle(25, 20, 170, 170);
        private Rectangle m_centralCircleBigRect = new Rectangle(100, 95, 20, 20);
        private Rectangle m_centralCircleSmallRect = new Rectangle(105, 100, 10, 10);

        public void Paint(Graphics g)
        {
            DrawPanel(g);
        }

        private void DrawPanel(Graphics g)
        {
            g.FillEllipse(m_frameBrush, m_outerFramRect);
            g.FillEllipse(m_backGroundBrush, m_innerFramRect);

            g.FillEllipse(m_handBrush, m_centralCircleBigRect);
            g.FillEllipse(m_backGroundBrush, m_centralCircleSmallRect);

            g.DrawString(GetCurrentTimeString(), new Font("Verdana", 20), new SolidBrush(Color.Red), 10, 220);
        }

        private void DrawHourHand(Graphics g, int hour)
        {
        }

        private void DrawMinuteHand(Graphics g, int minute)
        {
        }

        private void DrawSecondHand(Graphics g, int second)
        {
        }

        private string GetCurrentTimeString()
        {
            string hourStr = DateTime.Now.Hour.ToString().PadLeft(2, '0');
            string minuteStr = DateTime.Now.Minute.ToString().PadLeft(2, '0');
            string secondStr = DateTime.Now.Second.ToString().PadLeft(2, '0');
            return (hourStr + ":" + minuteStr + ":" + secondStr);
        }

        public void FormResize(Size clientSize)
        {
            int height = clientSize.Height;
            int width = clientSize.Width;

            m_outerFramRect = new Rectangle((int)(width * 0.03), (int)(width * 0.03), (int)(width * 0.94), (int)(width * 0.94));
            m_innerFramRect = new Rectangle((int)(width * 0.1), (int)(width * 0.1), (int)(width * 0.8), (int)(width * 0.8));
            m_centralCircleBigRect = new Rectangle(100, 95, 20, 20);
            m_centralCircleSmallRect = new Rectangle(105, 100, 10, 10);
        }
    }
}
