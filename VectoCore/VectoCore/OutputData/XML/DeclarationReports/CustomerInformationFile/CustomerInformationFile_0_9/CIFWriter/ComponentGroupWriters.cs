using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CIFWriter
{
    public class ComponentGroupWriters : AbstractCIFGroupWriter
    {
		public ComponentGroupWriters(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var engine = inputData.JobInputData.Vehicle.Components.EngineInputData;
			var fuelTypesXElement = new XElement(_cif + XMLNames.Report_Vehicle_FuelTypes);

			var fuelTypes = new HashSet<FuelType>();
            foreach (var engineMode in engine.EngineModes)
            {
				foreach (var fuel in engineMode.Fuels) {
					fuelTypes.Add(fuel.FuelType);
				}
			}
			var sortedFuels = fuelTypes.ToList();
			sortedFuels.Sort((fuelType1, fuelType2) => fuelType1.CompareTo(fuelType2));

			sortedFuels.ForEach(type => fuelTypesXElement.Add(new XElement(_cif + XMLNames.Engine_FuelType, type)));

			return new List<XElement>() {
				new XElement(_cif + XMLNames.Engine_RatedPower, engine.RatedPowerDeclared.ValueAsUnit("kW")),
				new XElement(_cif + "EngineCapacity", engine.Displacement.ValueAsUnit("ltr")),
				fuelTypesXElement
			};
		}

		#endregion
	}


	public class TransmissionGroup : AbstractCIFGroupWriter
	{
		public TransmissionGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var gearbox = inputData.JobInputData.Vehicle.Components.GearboxInputData;
			return new List<XElement>() {
				new XElement(_cif + "TransmissionValues", gearbox.CertificationMethod.ToXMLFormat()),
				new XElement(_cif + XMLNames.Gearbox_TransmissionType, gearbox.Type.ToXMLFormat()),
				new XElement(_cif + "NrOfGears", gearbox.Gears.Count)
			};
		}

		#endregion
	}

	public class AxleWheelsGroup : AbstractCIFGroupWriter
	{
		public AxleWheelsGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var axleWheels = inputData.JobInputData.Vehicle.Components.AxleWheels;
			double averageRRC = 0;
			var result = new List<XElement>();
			int axleCount = 0;
			
			foreach (var axle in axleWheels.AxlesDeclaration) {
				averageRRC += axle.Tyre.RollResistanceCoefficient;
				result.Add(new XElement(_cif + XMLNames.AxleWheels_Axles_Axle, 
					new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, ++axleCount),
					new XElement(_cif + XMLNames.Report_Tyre_TyreDimension, axle.Tyre.Dimension),
					new XElement(_cif + "FuelEfficiencyClass", axle.Tyre.FuelEfficiencyClass),
					new XElement(_cif + XMLNames.Report_Tyre_TyreCertificationNumber, axle.Tyre.CertificationNumber)));
			}
			averageRRC /= axleWheels.AxlesDeclaration.Count;
			result.Insert(0, new XElement(_cif + "AverageRRC", averageRRC));


			return result;
		}

		#endregion
	}

	public class LorryAuxGroup : AbstractCIFGroupWriter
	{
		public LorryAuxGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			return new List<XElement>() {
				new XElement(_cif + "SteeringPumpTechnology",
					inputData.JobInputData.Vehicle.Components.AuxiliaryInputData.Auxiliaries
						.Single(aux => aux.Type == AuxiliaryType.SteeringPump).Technology.Join())
			};
		}

		#endregion
	}

	public class ElectricMachineGroup : AbstractCIFGroupWriter
	{
		public ElectricMachineGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var result = new List<XElement>();
			var totalRatedPropulsionPower =
				inputData.JobInputData.Vehicle.Components.ElectricMachines.Entries
					.Where(e => e.Position != PowertrainPosition.GEN).Sum((e => e.ElectricMachine.R85RatedPower));
			result.Add(new XElement(_cif + "TotalRatedPropulsionPower", totalRatedPropulsionPower.ValueAsUnit("kW")));
			result.Add(new XElement(_cif + "MaxContinousPropulsionPower", "TODO"));



			return result;
		}

		#endregion
	}

	public class REESSGroup : AbstractCIFGroupWriter
	{
		public REESSGroup(ICustomerInformationFileFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractCIFGroupWriter

		public override IList<XElement> GetElements(IDeclarationInputDataProvider inputData)
		{
			var reess = inputData.JobInputData.Vehicle.Components.ElectricStorage;
			var batteries = reess.ElectricStorageElements
				.Where(es => es.REESSPack.StorageType == REESSType.Battery)
				.Select(es => es.REESSPack as IBatteryPackDeclarationInputData);
			var totalStorageCapacity = batteries
				.Sum(bp => bp.Capacity);
			var totalUsableCapacity = batteries.Sum(bp => bp.TotalUsableCapacityInSimulation());
			return new List<XElement>() {
				new XElement(_cif + "TotalStorageCapacity", totalUsableCapacity),
				new XElement(_cif + "UsableStorageCapacity", totalStorageCapacity)
			};
		}

		#endregion
	}
}
