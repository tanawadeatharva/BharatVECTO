/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationComponentsDataProviderV10 : AbstractCommonComponentType, IXMLVehicleComponentsDeclaration
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "VehicleComponentsType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IAirdragDeclarationInputData _airdragInputData;
		protected IGearboxDeclarationInputData _gearboxInputData;
		protected IAxleGearInputData _axleGearInputData;
		protected IAngledriveInputData _angledriveInputData;
		protected IEngineDeclarationInputData _engineInputData;
		protected IAuxiliariesDeclarationInputData _auxInputData;
		protected IRetarderInputData _retarderInputData;
		protected IAxlesDeclarationInputData _axleWheels;
		protected IPTOTransmissionInputData _ptoInputData;
		protected IXMLDeclarationVehicleData _vehicle;
		protected ITorqueConverterDeclarationInputData _torqueconverterInputData;


		public XMLDeclarationComponentsDataProviderV10(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) :
			base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
			_vehicle = vehicle;
		}

		#region Implementation of IVehicleComponentsDeclaration

		public virtual IAirdragDeclarationInputData AirdragInputData
		{
			get { return _airdragInputData ?? (_airdragInputData = ComponentReader.AirdragInputData); }
		}

		public virtual IGearboxDeclarationInputData GearboxInputData
		{
			get { return _gearboxInputData ?? (_gearboxInputData = ComponentReader.GearboxInputData); }
		}

		
		public virtual ITorqueConverterDeclarationInputData TorqueConverterInputData
		{
			get { return _torqueconverterInputData ?? (_torqueconverterInputData = ComponentReader.TorqueConverterInputData); }
		}

		public virtual IAxleGearInputData AxleGearInputData
		{
			get { return _axleGearInputData ?? (_axleGearInputData = ComponentReader.AxleGearInputData); }
		}

		public virtual IAngledriveInputData AngledriveInputData
		{
			get { return _angledriveInputData ?? (_angledriveInputData = ComponentReader.AngledriveInputData); }
		}

		public virtual IEngineDeclarationInputData EngineInputData
		{
			get { return _engineInputData ?? (_engineInputData = ComponentReader.EngineInputData); }
		}

		IAuxiliariesDeclarationInputData IVehicleComponentsDeclaration.AuxiliaryInputData
		{
			get { return _auxInputData ?? (_auxInputData = ComponentReader.AuxiliaryData); }
		}

		public virtual IRetarderInputData RetarderInputData
		{
			get { return _retarderInputData ?? (_retarderInputData = ComponentReader.RetarderInputData); }
		}

		public virtual IPTOTransmissionInputData PTOTransmissionInputData
		{
			get { return _vehicle.PTOTransmissionInputData; }
		}

		public virtual IAxlesDeclarationInputData AxleWheels
		{
			get { return _axleWheels ?? (_axleWheels = ComponentReader.AxlesDeclarationInputData); }
		}

		#endregion

		#region Implementation of IXMLVehicleComponentsDeclaration

		public virtual IXMLComponentReader ComponentReader { protected get; set; }

		#endregion


		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationComponentsDataProviderV20 : XMLDeclarationComponentsDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "VehicleComponentsType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationComponentsDataProviderV20(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile) { }

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}
}
