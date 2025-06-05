using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace XMLConverterLibrary
{
	public class XMLJobConverterFactory : IXMLConverterFactory
	{
		private static readonly List<AbstractXMLJobConverter> _converters;
		
		static XMLJobConverterFactory()
		{
			_converters = new List<AbstractXMLJobConverter> {
					new XMLJobConverter_v1_0_To_v_2_4(),
					new XMLJobConverter_v2_0_To_v2_4(),
					new XMLJobConverter_v2_1_To_v2_4(),
					new XMLJobConverter_v2_2_1_To_v2_4()
			};
		}

		public IEnumerable<Tuple<string, string>> SupportedConversions => 
			_converters.Select(x => new Tuple<string, string>(x.SourceVersion, x.TargetVersion));

		public ErrorOr<IXMLEntityConverter> GetConverter(string fromVersion, string toVersion)
		{
			var converter = _converters.Find(x => (x.SourceVersion == fromVersion) && (x.TargetVersion == toVersion));
			
			if (converter == null) {
				return Error.NotFound(
					description: $"XMLJobConverterFactory: converter from <{fromVersion}> to <{toVersion}> not found!");
			}

			return converter;
		}

		public XmlDocumentType DocumentType => XmlDocumentType.DeclarationJobData;
	}

	
}
