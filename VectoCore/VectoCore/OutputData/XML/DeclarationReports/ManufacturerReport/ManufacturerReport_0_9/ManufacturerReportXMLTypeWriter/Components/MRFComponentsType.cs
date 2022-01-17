using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter
{



	internal class MRFConventionalLorryComponentsType : AbstractMrfXmlType
	{
		public MRFConventionalLorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of MRFComponentType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var addTorqueConverterData =
				inputData.JobInputData.Vehicle.Components.GearboxInputData.Type == GearboxType.ATPowerSplit ||
				inputData.JobInputData.Vehicle.Components.GearboxInputData.Type == GearboxType.ATSerial;
			var addRetarderInputData = inputData.JobInputData.Vehicle.Components.RetarderInputData != null;
			var addAngleDriveData = inputData.JobInputData.Vehicle.Components.AngledriveInputData != null;
			var addAirdragdata = inputData.JobInputData.Vehicle.Components.AirdragInputData != null;
			return new XElement(_mrf + XMLNames.Vehicle_Components, 
				_mrfFactory.GetEngineType().GetXmlType(inputData),
				_mrfFactory.GetTransmissionType().GetXmlType(inputData),
				(addRetarderInputData ? _mrfFactory.GetRetarderType().GetXmlType(inputData) : null),
				(addTorqueConverterData ? _mrfFactory.GetTorqueConverterType().GetXmlType(inputData) : null),
				(addAngleDriveData ? _mrfFactory.GetAngleDriveType().GetXmlType(inputData) : null),
				(addAirdragdata ? _mrfFactory.GetAirdragType().GetXmlType(inputData) : null),
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				_mrfFactory.GetConventionalLorryAuxType().GetXmlType(inputData)
			);
			//return new XElement(_mrf + XMLNames.Vehicle_Components, 
			//	_mrfFactory.GetEngineType())
		}

		#endregion
	}

	internal class MRFHEV_Px_IHPC_LorryComponentsType : AbstractMrfXmlType
	{
		public MRFHEV_Px_IHPC_LorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var result = new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetXmlType(inputData),
				_mrfFactory.GetTransmissionType().GetXmlType(inputData),
				_mrfFactory.GetRetarderType().GetXmlType(inputData),
				_mrfFactory.GetTorqueConverterType().GetXmlType(inputData),
				_mrfFactory.GetAngleDriveType().GetXmlType(inputData),
				_mrfFactory.GetAxleGearType().GetXmlType(inputData),
				_mrfFactory.GetHEV_LorryAuxiliariesType().GetXmlType(inputData)
			);
			return result;
		}

		#endregion
	}

	internal class MRFHEV_S2_LorryComponentsType : AbstractMrfXmlType
	{
		public MRFHEV_S2_LorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetXmlType(inputData),
				_mrfFactory.GetTransmissionType().GetXmlType(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetXmlType(inputData) : null,
				components.TorqueConverterInputData != null
					? _mrfFactory.GetTorqueConverterType().GetXmlType(inputData)
					: null,
				_mrfFactory.GetAxleGearType().GetXmlType(inputData),
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				components.AngledriveInputData != null ? _mrfFactory.GetRetarderType().GetXmlType(inputData) : null,
				components.AirdragInputData != null ? _mrfFactory.GetAirdragType().GetXmlType(inputData) : null,
				_mrfFactory.GetElectricMachinesType().GetXmlType(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetXmlType(inputData));
		}

		#endregion
	}

	internal class MRFHEV_S3_LorryComponentsType : AbstractMrfXmlType
	{
		public MRFHEV_S3_LorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetXmlType(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetXmlType(inputData) : null,
				_mrfFactory.GetAxleGearType().GetXmlType(inputData),
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				components.AirdragInputData != null ? _mrfFactory.GetAirdragType().GetXmlType(inputData) : null,
				_mrfFactory.GetElectricMachinesType().GetXmlType(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetXmlType(inputData));
		}

		#endregion
	}

	internal class MRFHEV_S4_LorryComponentsType : AbstractMrfXmlType
	{
		public MRFHEV_S4_LorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetXmlType(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetXmlType(inputData) : null,
				components.AxleGearInputData != null ? _mrfFactory.GetAxleGearType().GetXmlType(inputData) : null,
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				components.AirdragInputData != null ? _mrfFactory.GetAirdragType().GetXmlType(inputData) : null,
				_mrfFactory.GetElectricMachinesType().GetXmlType(inputData),

				_mrfFactory.GetREESSSpecificationsType().GetXmlType(inputData));
		}

		#endregion
	}

	internal class MRFHEV_IEPC_S_LorryComponentsType : AbstractMrfXmlType
	{
		public MRFHEV_IEPC_S_LorryComponentsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var components = inputData.JobInputData.Vehicle.Components;
			return new XElement(_mrf + XMLNames.Vehicle_Components,
				_mrfFactory.GetEngineType().GetXmlType(inputData),
				components.RetarderInputData != null ? _mrfFactory.GetRetarderType().GetXmlType(inputData) : null,
				components.AxleGearInputData != null ? _mrfFactory.GetAxleGearType().GetXmlType(inputData) : null,
				_mrfFactory.GetAxleWheelsType().GetXmlType(inputData),
				components.AirdragInputData != null ? _mrfFactory.GetAirdragType().GetXmlType(inputData) : null,
				_mrfFactory.GetElectricMachinesType().GetXmlType(inputData),
				_mrfFactory.GetIEPCSpecifications().GetXmlType(inputData),
				_mrfFactory.GetREESSSpecificationsType().GetXmlType(inputData));
		}

		#endregion
	}
}
