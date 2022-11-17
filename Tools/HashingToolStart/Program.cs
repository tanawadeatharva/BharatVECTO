using System.Windows.Forms;
using System;

namespace TUGraz.VECTO
{
	class Program
	{
		static void Main(string[] args)
		{
			try {
				// mk20220707: hashing tool currently only works under net45, therefore we hardcoded the version
				StarterHelper.StartVECTO(args);
			} catch (Exception e) {
				MessageBox.Show(e.Message);
			}
		}

	}
}