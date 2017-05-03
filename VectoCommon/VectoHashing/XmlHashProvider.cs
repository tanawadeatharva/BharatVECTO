using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace VectoDB
{
	public class XmlHashProvider
	{
		protected string File;

		public XmlHashProvider(string file)
		{
			File = file;
		}

		public string ComputeHash(string xpath)
		{
			var xml = new XmlDocument() { PreserveWhitespace = true };
			var manager = new XmlNamespaceManager(xml.NameTable);
			manager.AddNamespace("ved", "urn:tugraz:ivt:VectoAPI:EngineeringDefinitions:v0.6");
			manager.AddNamespace("tns", "urn:tugraz:ivt:VectoAPI:EngineeringInput:v0.6");

			using (var fs = new FileStream(File, FileMode.Open)) {
				using (var sr = new StreamReader(fs)) {
					xml.Load(new LineCleaningTextReader(sr));
				}
			}

			var nodeList = xml.SelectNodes(string.Format("{0}/descendant-or-self::node()|{0}//@*", xpath), manager);
			if (nodeList == null || nodeList.Count == 0) {
				throw new Exception(string.Format("Selected node '{0}' not found in input!", xpath));
			}
			var transform = new XmlDsigC14NTransform();
			transform.LoadInput(nodeList);

			var sha256 = new SHA256CryptoServiceProvider();

			var hash = sha256.ComputeHash((Stream)transform.GetOutput(typeof(Stream)));
			return Convert.ToBase64String(hash);
		}

		private class LineCleaningTextReader : TextReader
		{
			private readonly TextReader _src;

			public LineCleaningTextReader(TextReader src)
			{
				_src = src;
			}

			public override int Read()
			{
				int r = _src.Read();
				switch (r) {
					case 0xD: // \r
						switch (_src.Peek()) {
							case 0xA:
							case 0x85: // \n or NEL char
								_src.Read();
								break;
						}
						return 0xA;
					case 0x85: //NEL
						return 0xA;
					default:
						return r;
				}
			}
		}
	}
}