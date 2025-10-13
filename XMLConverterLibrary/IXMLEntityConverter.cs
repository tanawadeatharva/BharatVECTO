using ErrorOr;
using System.Xml.Linq;

namespace XMLConverterLibrary
{
	public interface IXMLEntityConverter
	{
		ErrorOr<XDocument> Convert(XDocument source);

        string SourceVersion { get; }

        string TargetVersion { get; }
    }
}
