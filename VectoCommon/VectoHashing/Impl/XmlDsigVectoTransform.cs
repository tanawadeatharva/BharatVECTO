using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace TUGraz.VectoHashing
{
	public class XmlDsigVectoTransform : Transform
	{
		//private static readonly Type[] _inputTypes = { typeof(Stream), typeof(XmlDocument), typeof(XmlNodeList) };
		//private static readonly Type[] _outputTypes = { typeof(Stream) };
		//private XmlDocument _doc = new XmlDocument();

		private XmlDsigXsltTransform _transform;

		public XmlDsigVectoTransform()
		{
			Algorithm = "urn:vecto:xml:2017:canonicalization";
			_transform = new XmlDsigXsltTransform();

			XmlDocument doc = new XmlDocument();
			doc.Load(ReadStream("TUGraz.VectoHashing.Resources.XSLT.SortInputData.xslt"));

			_transform.LoadInnerXml(doc.ChildNodes);
		}

		public override void LoadInnerXml(XmlNodeList nodeList) {}

		protected override XmlNodeList GetInnerXml()
		{
			return null;
		}

		public override void LoadInput(object obj)
		{
			_transform.LoadInput(obj);
		}

		public override object GetOutput()
		{
			return _transform.GetOutput();
		}

		public override object GetOutput(Type type)
		{
			return _transform.GetOutput(type);
		}

		public override Type[] InputTypes
		{
			get { return _transform.InputTypes; }
		}

		public override Type[] OutputTypes
		{
			get { return _transform.OutputTypes; }
		}

		private static Stream ReadStream(string resourceName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resource = assembly.GetManifestResourceStream(resourceName);
			if (resource == null) {
				throw new Exception("Resource file not found: " + resourceName);
			}
			return resource;
		}
	}
}