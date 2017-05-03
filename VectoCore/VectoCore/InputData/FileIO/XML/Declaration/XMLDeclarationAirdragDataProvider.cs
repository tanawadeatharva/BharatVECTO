using System;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Resources;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLDeclarationAirdragDataProvider : AbstractDeclarationXMLComponentDataProvider,
		IAirdragDeclarationInputData
	{
		public XMLDeclarationAirdragDataProvider(XMLDeclarationInputDataProvider xmlInputDataProvider)
			: base(xmlInputDataProvider)
		{
			XBasePath = Helper.Query(VehiclePath,
				XMLNames.Vehicle_Components,
				XMLNames.Component_AirDrag,
				XMLNames.ComponentDataWrapper);
		}

		public SquareMeter AirDragArea
		{
			get { return GetDoubleElementValue(XMLNames.AirDrag_DeclaredCdxA).SI<SquareMeter>(); }
		}
	}
}