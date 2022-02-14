using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Castle.Components.DictionaryAdapter.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CIFWriter
{
	public abstract class VehicleWriter : IXmlTypeWriter
    {
		protected readonly ICustomerInformationFileFactory _cifFactory;
		protected readonly IManufacturerReportFactory _mrfFactory;
		protected XNamespace _cif = "urn:tugraz:ivt:VectoAPI:CustomerOutput:v0.9";

		public VehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory)
		{
			_cifFactory = cifFactory;
			_mrfFactory = mrfFactory;
		}


		#region Implementation of IXmlTypeWriter

		public abstract XElement GetElement(IDeclarationInputDataProvider inputData);

		#endregion


		protected XElement GetRetarder(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Retarder,
				inputData.JobInputData.Vehicle.Components.RetarderInputData.Type != RetarderType.None);
		}

		protected XElement GetAxleRatio(IDeclarationInputDataProvider inputData, bool optional = false)
		{
			if (!optional || (inputData.JobInputData.Vehicle.Components.AxleGearInputData != null)) {
				return new XElement(_cif + XMLNames.Axlegear_Ratio,
					inputData.JobInputData.Vehicle.Components.AxleGearInputData.Ratio.ToXMLFormat(2));
			} else {
				return null;
			}
			
		}
		
	}


	public class CIFConventionalLorryVehicleWriter : VehicleWriter
	{
		public CIFConventionalLorryVehicleWriter(ICustomerInformationFileFactory cifFactory,
			IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory)
		{

		}

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
					_cifFactory.GetGeneralVehicleSequenceGroupWriter().GetElements(inputData.JobInputData.Vehicle),
					_cifFactory.GetLorryGeneralVehicleSequenceGroupWriter().GetElements(inputData),
					_cifFactory.GetConventionalLorryVehicleSequenceGroupWriter().GetElements(inputData),
					_mrfFactory.GetConventionalADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS),
					_cifFactory.GetEngineGroup().GetElements(inputData),
					_cifFactory.GetTransmissionGroup().GetElements(inputData),
					GetRetarder(inputData),
					GetAxleRatio(inputData),
					_cifFactory.GetAxleWheelsGroup().GetElements(inputData),
					_cifFactory.GetLorryAuxGroup().GetElements(inputData)
			);


			
		}

		#endregion
	}

	public class CIF_HEVPx_LorryVehicleWriter : VehicleWriter
	{
		public CIF_HEVPx_LorryVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				_cifFactory.GetHEV_LorryVehicleTypeGroup().GetElements(inputData),
				_mrfFactory.GetHEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS),
				_cifFactory.GetEngineGroup().GetElements(inputData),
				_cifFactory.GetTransmissionGroup().GetElements(inputData),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetLorryAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}

	public class CIF_HEV_S2_LorryVehicleWriter : VehicleWriter
	{
		public CIF_HEV_S2_LorryVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				_cifFactory.GetHEV_LorryVehicleTypeGroup().GetElements(inputData),
				_mrfFactory.GetHEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS),
				_cifFactory.GetEngineGroup().GetElements(inputData),
				_cifFactory.GetTransmissionGroup().GetElements(inputData),
				GetRetarder(inputData),
				GetAxleRatio(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetLorryAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}

	public class CIF_HEV_S3_LorryVehicleWriter : VehicleWriter
	{
		public CIF_HEV_S3_LorryVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				_cifFactory.GetHEV_LorryVehicleTypeGroup().GetElements(inputData),
				_mrfFactory.GetHEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS),
				_cifFactory.GetEngineGroup().GetElements(inputData),
				GetRetarder(inputData),
				GetAxleRatio(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetLorryAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}

	public class CIF_HEV_S4_LorryVehicleWriter : VehicleWriter
	{
		public CIF_HEV_S4_LorryVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				_cifFactory.GetHEV_LorryVehicleTypeGroup().GetElements(inputData),
				_mrfFactory.GetHEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS),
				_cifFactory.GetEngineGroup().GetElements(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetLorryAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}

	public class CIF_HEV_IEPC_S_LorryVehicleWriter : VehicleWriter
	{
		public CIF_HEV_IEPC_S_LorryVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				_cifFactory.GetHEV_LorryVehicleTypeGroup().GetElements(inputData),
				_mrfFactory.GetHEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS),
				_cifFactory.GetEngineGroup().GetElements(inputData),
				GetRetarder(inputData),
				GetAxleRatio(inputData, true),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetLorryAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}

	public class CIF_PEV_E2_LorryVehicleWriter : VehicleWriter
	{
		public CIF_PEV_E2_LorryVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				_cifFactory.GetPEV_LorryVehicleTypeGroup().GetElements(inputData),
				_mrfFactory.GetPEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS),
				_cifFactory.GetTransmissionGroup().GetElements(inputData),
				GetRetarder(inputData),
				GetAxleRatio(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetLorryAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}

	public class CIF_PEV_E3_LorryVehicleWriter : VehicleWriter
	{
		public CIF_PEV_E3_LorryVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				_cifFactory.GetPEV_LorryVehicleTypeGroup().GetElements(inputData),
				_mrfFactory.GetPEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS),
				GetRetarder(inputData),
				GetAxleRatio(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetLorryAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}

	public class CIF_PEV_E4_LorryVehicleWriter : VehicleWriter
	{
		public CIF_PEV_E4_LorryVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				_cifFactory.GetPEV_LorryVehicleTypeGroup().GetElements(inputData),
				_mrfFactory.GetPEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetLorryAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}

	public class CIF_PEV_IEPC_LorryVehicleWriter : VehicleWriter
	{
		public CIF_PEV_IEPC_LorryVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				_cifFactory.GetPEV_LorryVehicleTypeGroup().GetElements(inputData),
				_mrfFactory.GetPEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS),
				GetRetarder(inputData),
				GetAxleRatio(inputData, true),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetLorryAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}

	public class CIF_ConventionalCompletedBusVehicleWriter : VehicleWriter
	{
		public CIF_ConventionalCompletedBusVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				_cifFactory.GetCompletedBusVehicleTypeGroup().GetElements(inputData));
		}

		#endregion
	}
}
