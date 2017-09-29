using Microsoft.Win32;

namespace HashingTool.Helper
{
	public class BrowserHelper
	{
		public static string GetDefaultBrowserPath()
		{
			var urlAssociation = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http";
			var browserPathKey = @"$BROWSER$\shell\open\command";

			//	Dim browserPath As String

			//	'Read default browser path from userChoiceLKey
			var userChoiceKey = Registry.CurrentUser.OpenSubKey(urlAssociation + @"\UserChoice", false);

			if (userChoiceKey == null) {
				//		'If user choice was not found, try machine default
				//		'Read default browser path from Win XP registry key, or try Win Vista (and newer) registry key
				var browserKey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false) ??
								Registry.CurrentUser.OpenSubKey(urlAssociation, false);
				if (browserKey == null) {
					return "";
				}

				var path = browserKey.GetValue("").ToString();
				browserKey.Close();
				if (path.Contains(".exe")) {
					return path.Substring(1, path.IndexOf(".exe") + 3);
				} else {
					return path;
				}
			}
			//	' user defined browser choice was found
			var progId = userChoiceKey.GetValue("ProgId").ToString();
			userChoiceKey.Close();

			//	' now look up the path of the executable
			var concreteBrowserKey = browserPathKey.Replace("$BROWSER$", progId);
			var kp = Registry.ClassesRoot.OpenSubKey(concreteBrowserKey, false);
			if (kp == null) {
				return "";
			}
			var browserPath = kp.GetValue("").ToString();
			kp.Close();
			if (browserPath.Contains(".exe")) {
				return browserPath.Substring(1, browserPath.IndexOf(".exe") + 3);
			} else {
				return browserPath;
			}
		}
	}
}
