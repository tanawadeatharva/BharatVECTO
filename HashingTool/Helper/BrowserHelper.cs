/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using Microsoft.Win32;

namespace HashingTool.Helper
{
	public static class BrowserHelper
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
