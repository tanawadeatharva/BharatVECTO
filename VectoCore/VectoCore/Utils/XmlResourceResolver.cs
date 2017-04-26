using System;
using System.IO;
using System.Xml;

namespace TUGraz.VectoCore.Utils
{
	public class XmlResourceResolver : XmlUrlResolver
	{
		internal const string BaseUri = "schema://";

		public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			if (absoluteUri.Scheme == "schema") {
				return RessourceHelper.LoadResourceAsStream(RessourceHelper.ResourceType.XMLSchema,
					Path.GetFileName(absoluteUri.LocalPath));
			}
			return base.GetEntity(absoluteUri, role, ofObjectToReturn);
		}
	}
}