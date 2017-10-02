Imports Microsoft.Win32

Friend Class BrowserUtils
	Public Shared Function GetDefaultBrowserPath() As String

		Dim urlAssociation As String = "Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http"
		Dim browserPathKey As String = "$BROWSER$\shell\open\command"

		Dim browserPath As String

		'Read default browser path from userChoiceLKey
		Dim userChoiceKey As RegistryKey = Registry.CurrentUser.OpenSubKey(urlAssociation + "\UserChoice", False)

		If userChoiceKey Is Nothing Then
			'If user choice was not found, try machine default
			'Read default browser path from Win XP registry key, or try Win Vista (and newer) registry key
			Dim browserKey As RegistryKey = Registry.ClassesRoot.OpenSubKey("HTTP\shell\open\command", False)
			If browserKey Is Nothing Then
				browserKey = Registry.CurrentUser.OpenSubKey(urlAssociation, False)
			End If

			Dim path As String = CType(browserKey.GetValue(""), String)
			browserKey.Close()
			If (path.Contains(".exe")) Then
				Return path.Substring(1, path.IndexOf(".exe") + 3)
			Else
				Return path
			End If

		End If
		' user defined browser choice was found
		Dim progId As String = userChoiceKey.GetValue("ProgId").ToString()
		userChoiceKey.Close()

		' now look up the path of the executable
		Dim concreteBrowserKey As String = browserPathKey.Replace("$BROWSER$", progId)
		Dim kp As RegistryKey = Registry.ClassesRoot.OpenSubKey(concreteBrowserKey, False)
		browserPath = CType(kp.GetValue(""), String)
		kp.Close()
		If (browserPath.Contains(".exe")) Then
			Return browserPath.Substring(1, browserPath.IndexOf(".exe") + 3)
		Else
			Return browserPath
		End If
	End Function
End Class
