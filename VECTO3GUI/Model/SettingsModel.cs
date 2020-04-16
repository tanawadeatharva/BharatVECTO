using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VECTO3GUI.Model
{
	public class SettingsModel
	{
		public string SavePathFolder
		{
			get { return Properties.Settings.Default.SavePathFolder; }
			set
			{
				Properties.Settings.Default.SavePathFolder = value;
				Properties.Settings.Default.Save();
			}
		}

		public string XmlFilePathFolder
		{
			get { return Properties.Settings.Default.XMLFilesPathFolder; }
			set
			{
				Properties.Settings.Default.XMLFilesPathFolder = value;
				Properties.Settings.Default.Save();
			}
		}


	}
}
