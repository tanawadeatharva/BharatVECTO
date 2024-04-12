using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_1_0.CIFWriter
{
	public abstract class VehicleWriter : IXmlTypeWriter
	{
		protected readonly ICustomerInformationFileFactory _cifFactory;
		//protected readonly IManufacturerReportFactory _mrfFactory;
		protected XNamespace _cif = "urn:tugraz:ivt:VectoAPI:CustomerOutput:v1.0";
		protected XNamespace _xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

		public VehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory)
		{
			_cifFactory = cifFactory;
			//_mrfFactory = mrfFactory;
		}


		#region Implementation of IXmlTypeWriter

		public abstract XElement GetElement(IDeclarationInputDataProvider inputData);

		#endregion


		protected XElement GetRetarder(IDeclarationInputDataProvider inputData)
		{
			IRetarderInputData retarder;
			if (inputData is IMultistepBusInputDataProvider multistage) {
				retarder = multistage.JobInputData.PrimaryVehicle.Vehicle.Components.RetarderInputData;
			} else {
				retarder = inputData.JobInputData.Vehicle.Components.RetarderInputData;
			}
			return new XElement(_cif + XMLNames.Component_Retarder,
				retarder.Type != RetarderType.None);
		}

		protected XElement GetAxleRatio(IDeclarationInputDataProvider inputData, bool optional = false)
		{
			IAxleGearInputData axlegear;
			if (inputData is IMultistepBusInputDataProvider multistage) {
				axlegear = multistage.JobInputData.PrimaryVehicle.Vehicle.Components.AxleGearInputData;
			} else {
				axlegear = inputData.JobInputData.Vehicle.Components.AxleGearInputData;
			}
			if (!optional || (axlegear != null)) {
				return new XElement(_cif + "AxleRatio", axlegear.Ratio.ToXMLFormat(3));
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
					_cifFactory.GetConventionalADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS).WithXName(_cif + "ADAS"),
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
				_cifFactory.GetHEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS).WithXName(_cif + "ADAS"),
				_cifFactory.GetEngineGroup().GetElements(inputData),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetTransmissionGroup().GetElements(inputData),
				GetRetarder(inputData),
				GetAxleRatio(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),

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
				_cifFactory.GetHEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS).WithXName(_cif + "ADAS"),
				_cifFactory.GetEngineGroup().GetElements(inputData),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetTransmissionGroup().GetElements(inputData),
				GetRetarder(inputData),
				GetAxleRatio(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),

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
				_cifFactory.GetHEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS).WithXName(_cif + "ADAS"),
				_cifFactory.GetEngineGroup().GetElements(inputData),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetTransmissionGroupNoGearbox().GetElements(inputData),
				GetRetarder(inputData),
				GetAxleRatio(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),

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
				_cifFactory.GetHEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS).WithXName(_cif + "ADAS"),
				_cifFactory.GetEngineGroup().GetElements(inputData),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetTransmissionGroupNoGearbox().GetElements(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),
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
				_cifFactory.GetHEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS).WithXName(_cif + "ADAS"),
				_cifFactory.GetEngineGroup().GetElements(inputData),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetIEPCTransmissionGroup().GetElements(inputData),
				GetRetarder(inputData),
				GetAxleRatio(inputData, true),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),

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
				_cifFactory.GetPEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS).WithXName(_cif + "ADAS"),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetTransmissionGroup().GetElements(inputData),
				GetRetarder(inputData),
				GetAxleRatio(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),

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
				_cifFactory.GetPEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS).WithXName(_cif + "ADAS"),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetTransmissionGroupNoGearbox().GetElements(inputData),
				GetRetarder(inputData),
				GetAxleRatio(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),

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
				_cifFactory.GetPEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS).WithXName(_cif + "ADAS"),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetTransmissionGroupNoGearbox().GetElements(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),

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
				_cifFactory.GetPEVADASType().GetXmlType(inputData.JobInputData.Vehicle.ADAS).WithXName(_cif + "ADAS"),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetIEPCTransmissionGroup().GetElements(inputData),
				GetRetarder(inputData),
				GetAxleRatio(inputData, true),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),

				_cifFactory.GetLorryAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}

	public class CIF_Exempted_LorryVehicleWriter : VehicleWriter
	{
		public CIF_Exempted_LorryVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var vehicleData = inputData.JobInputData.Vehicle;
			var tech = new string(vehicleData.ExemptedTechnology
				.Where(c => char.IsLetterOrDigit(c) || c == ' ' || c == '.').ToArray());
			return new XElement(_cif + XMLNames.Component_Vehicle,
				
				new XElement(_cif + XMLNames.Component_Manufacturer, vehicleData.Manufacturer),
				new XElement(_cif + XMLNames.Component_ManufacturerAddress, vehicleData.ManufacturerAddress),
				new XElement(_cif + XMLNames.Component_Model, vehicleData.Model),
				_cifFactory.GetGeneralVehicleSequenceGroupWriter().GetElements(vehicleData),

                new XElement(_cif + XMLNames.CorrectedActualMass, vehicleData.CurbMassChassis.ToXMLFormat(0)),
				new XElement(_cif + XMLNames.Vehicle_SleeperCab, vehicleData.SleeperCab),
				new XElement(_cif + XMLNames.Vehicle_ZeroEmissionVehicle, vehicleData.ZeroEmissionVehicle),
				new XElement(_cif + "VehicleTechnologyExempted", tech)
			);
		}

		#endregion
	}

	public class CIF_Conventional_CompletedBusVehicleWriter : VehicleWriter
	{
		public CIF_Conventional_CompletedBusVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "Conventional_CompletedBusVehicleType"),
				_cifFactory.GetCompletedBusVehicleTypeGroup().GetElements(inputData),
				_cifFactory.GetConventionalADASType().GetXmlType(((IMultistepBusInputDataProvider)inputData)
					.JobInputData
					.ConsolidateManufacturingStage.Vehicle.ADAS).WithXName(_cif + "ADAS"), 
				_cifFactory.GetEngineGroup().GetElements(inputData),
				_cifFactory.GetTransmissionGroup().GetElements(inputData),
				GetRetarder(inputData),
				GetAxleRatio(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),
				
				_cifFactory.GetConventionalCompletedBusAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}

	public class CIF_HEV_Px_CompletedBusVehicleWriter : VehicleWriter
	{
		public CIF_HEV_Px_CompletedBusVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "HEV_Px_IHPC_CompletedBusVehicleType"),
				_cifFactory.GetCompletedBusVehicleTypeGroup().GetElements(inputData),
				_cifFactory.GetHEV_CompletedBusVehicleSequenceGroupWriter().GetElements(inputData),
				_cifFactory.GetHEVADASType().GetXmlType(((IMultistepBusInputDataProvider)inputData)
					.JobInputData
					.ConsolidateManufacturingStage.Vehicle.ADAS).WithXName(_cif + "ADAS"), 
				_cifFactory.GetEngineGroup().GetElements(inputData),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetTransmissionGroup().GetElements(inputData),
				GetRetarder(inputData),
				GetAxleRatio(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),

				_cifFactory.GetHEV_Px_IHPC_CompletedBusAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}

	public class CIF_HEV_IHPC_CompletedBusVehicleWriter : CIF_HEV_Px_CompletedBusVehicleWriter
	{
		public CIF_HEV_IHPC_CompletedBusVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }
	}

	public class CIF_HEV_S2_CompletedBusVehicleWriter : CIF_HEV_Px_CompletedBusVehicleWriter
	{
		public CIF_HEV_S2_CompletedBusVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "HEV_S2_CompletedBusVehicleType"),
				_cifFactory.GetCompletedBusVehicleTypeGroup().GetElements(inputData),
				_cifFactory.GetHEV_CompletedBusVehicleSequenceGroupWriter().GetElements(inputData),
				_cifFactory.GetHEVADASType().GetXmlType(((IMultistepBusInputDataProvider)inputData)
					.JobInputData
					.ConsolidateManufacturingStage.Vehicle.ADAS).WithXName(_cif + "ADAS"),
				_cifFactory.GetEngineGroup().GetElements(inputData),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetTransmissionGroup().GetElements(inputData),
				GetRetarder(inputData),
				GetAxleRatio(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),

				_cifFactory.GetHEV_Sx_CompletedBusAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}

	public class CIF_HEV_S3_CompletedBusVehicleWriter : CIF_HEV_Px_CompletedBusVehicleWriter
	{
		public CIF_HEV_S3_CompletedBusVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }
		
		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "HEV_S3_CompletedBusVehicleType"),
				_cifFactory.GetCompletedBusVehicleTypeGroup().GetElements(inputData),
				_cifFactory.GetHEV_CompletedBusVehicleSequenceGroupWriter().GetElements(inputData),
				_cifFactory.GetHEVADASType().GetXmlType(((IMultistepBusInputDataProvider)inputData)
					.JobInputData
					.ConsolidateManufacturingStage.Vehicle.ADAS).WithXName(_cif + "ADAS"),
				_cifFactory.GetEngineGroup().GetElements(inputData),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetTransmissionGroupNoGearbox().GetElements(inputData),
				GetRetarder(inputData),
				GetAxleRatio(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),

				_cifFactory.GetHEV_Sx_CompletedBusAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}

	public class CIF_HEV_S4_CompletedBusVehicleWriter : CIF_HEV_Px_CompletedBusVehicleWriter
	{
		public CIF_HEV_S4_CompletedBusVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "HEV_S4_CompletedBusVehicleType"),
				_cifFactory.GetCompletedBusVehicleTypeGroup().GetElements(inputData),
				_cifFactory.GetHEV_CompletedBusVehicleSequenceGroupWriter().GetElements(inputData),
				_cifFactory.GetHEVADASType().GetXmlType(((IMultistepBusInputDataProvider)inputData)
					.JobInputData
					.ConsolidateManufacturingStage.Vehicle.ADAS).WithXName(_cif + "ADAS"),
				_cifFactory.GetEngineGroup().GetElements(inputData),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetTransmissionGroupNoGearbox().GetElements(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),

				_cifFactory.GetHEV_Sx_CompletedBusAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}

	public class CIF_HEV_IEPC_S_CompletedBusVehicleWriter : CIF_HEV_Px_CompletedBusVehicleWriter
	{
		public CIF_HEV_IEPC_S_CompletedBusVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "HEV_S4_CompletedBusVehicleType"),
				_cifFactory.GetCompletedBusVehicleTypeGroup().GetElements(inputData),
				_cifFactory.GetHEV_CompletedBusVehicleSequenceGroupWriter().GetElements(inputData),
				_cifFactory.GetHEVADASType().GetXmlType(((IMultistepBusInputDataProvider)inputData)
					.JobInputData
					.ConsolidateManufacturingStage.Vehicle.ADAS).WithXName(_cif + "ADAS"),
				_cifFactory.GetEngineGroup().GetElements(inputData),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetIEPCTransmissionGroup().GetElements(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),

				_cifFactory.GetHEV_Sx_CompletedBusAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}

	public class CIF_PEV_E2_CompletedBusVehicleWriter : CIF_HEV_Px_CompletedBusVehicleWriter
	{
		public CIF_PEV_E2_CompletedBusVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "PEV_E2_CompletedBusVehicleType"),
				_cifFactory.GetPEVCompletedBusVehicleTypeGroup().GetElements(inputData),
				_cifFactory.GetPEV_CompletedBusVehicleSequenceGroupWriter().GetElements(inputData),
				_cifFactory.GetPEVADASType().GetXmlType(((IMultistepBusInputDataProvider)inputData)
					.JobInputData
					.ConsolidateManufacturingStage.Vehicle.ADAS).WithXName(_cif + "ADAS"),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetTransmissionGroup().GetElements(inputData),
				GetRetarder(inputData),
				GetAxleRatio(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),

				_cifFactory.GetPEV_CompletedBusAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}

	public class CIF_PEV_E3_CompletedBusVehicleWriter : CIF_HEV_Px_CompletedBusVehicleWriter
	{
		public CIF_PEV_E3_CompletedBusVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "PEV_E3_CompletedBusVehicleType"),
				_cifFactory.GetPEVCompletedBusVehicleTypeGroup().GetElements(inputData),
				_cifFactory.GetPEV_CompletedBusVehicleSequenceGroupWriter().GetElements(inputData),
				_cifFactory.GetPEVADASType().GetXmlType(((IMultistepBusInputDataProvider)inputData)
					.JobInputData
					.ConsolidateManufacturingStage.Vehicle.ADAS).WithXName(_cif + "ADAS"),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetTransmissionGroupNoGearbox().GetElements(inputData),
				GetRetarder(inputData),
				GetAxleRatio(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),

				_cifFactory.GetPEV_CompletedBusAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}

	public class CIF_PEV_E4_CompletedBusVehicleWriter : CIF_HEV_Px_CompletedBusVehicleWriter
	{
		public CIF_PEV_E4_CompletedBusVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "PEV_E4_CompletedBusVehicleType"),
				_cifFactory.GetPEVCompletedBusVehicleTypeGroup().GetElements(inputData),
				_cifFactory.GetPEV_CompletedBusVehicleSequenceGroupWriter().GetElements(inputData),
				_cifFactory.GetPEVADASType().GetXmlType(((IMultistepBusInputDataProvider)inputData)
					.JobInputData
					.ConsolidateManufacturingStage.Vehicle.ADAS).WithXName(_cif + "ADAS"),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetTransmissionGroupNoGearbox().GetElements(inputData),
				//GetRetarder(inputData),
				//GetAxleRatio(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),

				_cifFactory.GetPEV_CompletedBusAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}

	public class CIF_PEV_IEPC_CompletedBusVehicleWriter : CIF_HEV_Px_CompletedBusVehicleWriter
	{
		public CIF_PEV_IEPC_CompletedBusVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "PEV_IEPC_CompletedBusVehicleType"),
				_cifFactory.GetPEVCompletedBusVehicleTypeGroup().GetElements(inputData),
				_cifFactory.GetPEV_CompletedBusVehicleSequenceGroupWriter().GetElements(inputData),
				_cifFactory.GetPEVADASType().GetXmlType(((IMultistepBusInputDataProvider)inputData)
					.JobInputData
					.ConsolidateManufacturingStage.Vehicle.ADAS).WithXName(_cif + "ADAS"),
				//_cifFactory.GetEngineGroup().GetElements(inputData),
				_cifFactory.GetElectricMachineGroup().GetElements(inputData),
				_cifFactory.GetREESSGroup().GetElements(inputData),
				_cifFactory.GetIEPCTransmissionGroup().GetElements(inputData),
				GetRetarder(inputData),
				GetAxleRatio(inputData, true),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),

				_cifFactory.GetPEV_CompletedBusAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}


	public class CIF_ExemptedCompletedBusVehicleWriter : VehicleWriter
	{
		public CIF_ExemptedCompletedBusVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_cif + XMLNames.Component_Vehicle,
				_cifFactory.GetExemptedCompletedBusVehicleTypeGroup().GetElements(inputData)
				//_cifFactory.GetConveCompletedBusVehicleSequenceGroupWriter().GetElements(inputData),
			);
		}

		#endregion
	}


	// ----

	public class CIF_Conventional_SingleBusVehicleWriter : VehicleWriter
	{
		public CIF_Conventional_SingleBusVehicleWriter(ICustomerInformationFileFactory cifFactory, IManufacturerReportFactory mrfFactory) : base(cifFactory, mrfFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var singleBus = (ISingleBusInputDataProvider)inputData;
			var adas = singleBus.CompletedVehicle.ADAS ?? singleBus.PrimaryVehicle.ADAS;
			return new XElement(_cif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + XMLNames.XSIType, "Conventional_CompletedBusVehicleType"),
				_cifFactory.GetSingleBusVehicleTypeGroup().GetElements(inputData),
				_cifFactory.GetConventionalADASType().GetXmlType(adas).WithXName(_cif + "ADAS"),
				_cifFactory.GetEngineGroup().GetElements(inputData),
				_cifFactory.GetTransmissionGroup().GetElements(inputData),
				GetRetarder(inputData),
				GetAxleRatio(inputData),
				_cifFactory.GetAxleWheelsGroup().GetElements(inputData),

				_cifFactory.GetConventionalSingleBusAuxGroup().GetElements(inputData)
			);
		}

		#endregion
	}


}
