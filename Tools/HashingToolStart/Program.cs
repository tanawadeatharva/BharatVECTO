using System.Windows.Forms;
using System;

namespace TUGraz.VECTO
{
	class Program
	{
		static void Main(string[] args)
		{
			try {
				StarterHelper.StartVECTO(args, "net45");
			} catch (Exception e) {
				MessageBox.Show(e.Message);
			}
		}

	}
}