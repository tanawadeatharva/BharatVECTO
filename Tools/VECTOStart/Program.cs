using System;
using System.Windows.Forms;

namespace TUGraz.VECTO
{
	class Program
	{
		static void Main(string[] args)
		{
			try {
				StarterHelper.StartVECTO(args);
			} catch (Exception e) {
				MessageBox.Show(e.Message);
			}
		}
	}
}
