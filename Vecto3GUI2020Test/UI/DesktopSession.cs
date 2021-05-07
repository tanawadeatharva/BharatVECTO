using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;

namespace Vecto3GUI2020Test
{
	public class DesktopSession
	{
		private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723/";
		WindowsDriver<WindowsElement> desktopSession;

		public DesktopSession()
		{
			var appCapabilities = new AppiumOptions();
			appCapabilities.AddAdditionalCapability("app", "Root");
			appCapabilities.AddAdditionalCapability("deviceName", "WindowsPC");
			desktopSession = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);
		}

		~DesktopSession()
		{
			desktopSession.Quit();
		}

		public WindowsDriver<WindowsElement> DesktopSessionElement
		{
			get { return desktopSession; }
		}

		public WindowsElement FindElementByAbsoluteXPath(string xPath, int nTryCount = 15)
		{
			WindowsElement uiTarget = null;

			while (nTryCount-- > 0) {
				try {
					uiTarget = desktopSession.FindElementByXPath(xPath);
				} catch {
				}

				if (uiTarget != null) {
					break;
				} else {
					System.Threading.Thread.Sleep(200);
				}
			}

			return uiTarget;
		}
	}
}
