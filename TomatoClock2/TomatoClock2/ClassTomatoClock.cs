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
    class ClassTomatoClock
    {
        private UInt32 m_totalCount = 1500;
        private UInt32 m_currentCount = 0;

        private SolidBrush m_sdBrushPasedTime = new SolidBrush(Color.OrangeRed);
        private SolidBrush m_sdBrushTotalTime = new SolidBrush(Color.DarkCyan);

        private UInt16 m_flashCount = 0;
        private const UInt16 MAX_FLASH_NUM = 10;
        private SolidBrush m_sdBrushFlash0 = new SolidBrush(Color.Yellow);
        private SolidBrush m_sdBrushFlash1 = new SolidBrush(Color.Red);

        private SolidBrush m_sdBrushInvalid0 = new SolidBrush(Color.GhostWhite);
        private SolidBrush m_sdBrushInvalid1 = new SolidBrush(Color.DarkGray);

        private Rectangle m_rect = new Rectangle(10, 5, 200, 200);
        private Size m_formSize = new Size();

        public ClassTomatoClock()
        {
        }

        public void Paint(Graphics g)
        {
            if (m_currentCount <= m_totalCount)
            {
                DrawCountDownPie(g);
            }
            else
            {
                DrawFlashPie(g);
            }
        }

        private void DrawCountDownPie(Graphics g)
        {
            float startAngle = 270.0F;
            float sweepAngle = (float)(m_currentCount * 360) / m_totalCount;

            g.FillPie(m_sdBrushTotalTime, m_rect, startAngle + sweepAngle, 360 - sweepAngle);

            // Draw pie to screen.
            g.FillPie(m_sdBrushPasedTime, m_rect, startAngle, sweepAngle);
            //g.DrawString(m_formSize.ToString(), new Font("Verdana", 10), new SolidBrush(Color.Red), 10, 250);
        }

        private void DrawFlashPie(Graphics g)
        {
            if (m_flashCount <= MAX_FLASH_NUM)
            {
                if (0 == (m_flashCount % 2))
                {
                    g.FillEllipse(m_sdBrushFlash0, m_rect);
                }
                else
                {
                    g.FillEllipse(m_sdBrushFlash1, m_rect);
                }
            }
            else if (m_flashCount == (MAX_FLASH_NUM + 1))
            {
                g.FillEllipse(m_sdBrushTotalTime, m_rect);
            }
            else
            {
            }
        }

        public void TimerTick()
        {
            if (m_currentCount <= m_totalCount)
            {
                m_currentCount += 1;
            }
            else
            {
                if (m_flashCount <= MAX_FLASH_NUM)
                {
                    m_flashCount += 1;
                }
            }
        }

        public void Reset()
        {
            m_currentCount = 0;
            m_flashCount = 0;
        }

        public void FormResize(Size formSize)
        {
            m_formSize = formSize;
        }
    }
}
