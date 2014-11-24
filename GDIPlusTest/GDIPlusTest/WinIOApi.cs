using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SuperKeys;

namespace GDIPlusTest
{
    public class WinIOApi
    {
        private WinIoSys m_WinIoSys;

        public WinIOApi()
        {
            m_WinIoSys = new WinIoSys();
            m_WinIoSys.InitSuperKeys();
        }

        public void Dispose()
        {
            m_WinIoSys.CloseSuperKeys();
        }

        public void KeyPress(WinIoSys.Key vkey, int ms)
        {
            m_WinIoSys.KeyPress(vkey, ms);
        }
    }
}
