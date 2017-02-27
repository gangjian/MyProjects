using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMI_simulator.Ctrls;

namespace HMI_simulator
{
	public class HMI_PAGE
	{
		public int PageId = -1;
		public string PageName = string.Empty;
		public List<HMI_BUTTON> ButtonList = new List<HMI_BUTTON>();
		public List<HMI_TEXTBOX> TextBoxList = new List<HMI_TEXTBOX>();
		public List<HMI_PROGRESSBAR> ProgressBarList = new List<HMI_PROGRESSBAR>();

		public HMI_PAGE(int id, string name)
		{
			this.PageId = id;
			this.PageName = name;
		}
	}
}
