using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XMLConverterLibrary
{
	public interface IXMLEntityConverter
	{
		ErrorOr<XDocument> Convert(XDocument source);
	}
}
