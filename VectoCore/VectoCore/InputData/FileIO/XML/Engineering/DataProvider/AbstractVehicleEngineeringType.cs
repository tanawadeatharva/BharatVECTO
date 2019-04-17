using System.Xml;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	public abstract class AbstractVehicleEngineeringType : AbstractCommonComponentType
	{
		protected AbstractVehicleEngineeringType(XmlNode node, string source) : base(node, source) { }
	}
}
