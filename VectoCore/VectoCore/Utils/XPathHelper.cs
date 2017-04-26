using System.Linq;
using System.Xml;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.Utils
{
	public class XPathHelper
	{
		protected readonly ExecutionMode Mode;
		protected readonly string DefaultPrefix;

		public XPathHelper(ExecutionMode mode)
		{
			Mode = mode;
			DefaultPrefix = Mode == ExecutionMode.Declaration
				? Constants.XML.DeclarationNSPrefix
				: Constants.XML.EngineeringNSPrefix;
		}

		public string Query(params string[] xpathSections)
		{
			return string.Join("/",
				xpathSections.Select(
					x => string.IsNullOrWhiteSpace(x) || x.Equals("..") || x.Contains(":") || x.StartsWith("@") ? x : NSPrefix(x)))
				;
		}

		public string QueryConstraint(string elementName, string name, string value,
			string prefix = "@")
		{
			return value == null
				? string.Format("{0}[{2}{1}]", elementName, name, prefix)
				: string.Format("{0}[{3}{1}='{2}']", elementName, name, value, prefix);
		}

		public string QueryAbs(params string[] xPathSections)
		{
			return string.Format("/{0}", Query(xPathSections));
		}

		public string NSPrefix(string element, string prefix = null)
		{
			if (prefix == null) {
				prefix = DefaultPrefix;
			}
			return string.Format("{1}:{0}", element, prefix);
		}

		public void AddNamespaces(XmlNamespaceManager manager)
		{
			manager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
			if (Mode == ExecutionMode.Declaration) {
				manager.AddNamespace(Constants.XML.DeclarationNSPrefix, Constants.XML.VectoDeclarationDefinitionsNS);
				manager.AddNamespace(Constants.XML.RootNSPrefix, Constants.XML.VectoDeclarationInputNS);
			} else {
				manager.AddNamespace(Constants.XML.EngineeringNSPrefix, Constants.XML.VectoEngineeringDefinitionsNS);
				manager.AddNamespace(Constants.XML.RootNSPrefix, Constants.XML.VectoEngineeringInputNS);
			}
		}
	}
}