using System;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;

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

		public new CertificationMethod CertificationMethod
		{
			get { return CertificationMethod.Measured; }
		}

		public SquareMeter AirDragArea
		{
			get { return GetDoubleElementValue(XMLNames.AirDrag_DeclaredCdxA).SI<SquareMeter>(); }
		}
	}
}