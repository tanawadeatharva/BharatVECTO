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
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
    public class EngineTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
    {
		public EngineTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var engineData = inputData.JobInputData.Vehicle.Components.EngineInputData;
			var result = new XElement(_mrf + XMLNames.Component_Engine,
				new XElement(_mrf + XMLNames.Component_Model, engineData.Model),
				new XElement(_mrf + XMLNames.Component_CertificationNumber, engineData.CertificationNumber),
				new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue, engineData.DigestValue?.DigestValue ?? ""),
				new XElement(_mrf + XMLNames.Engine_RatedPower, engineData.RatedPowerDeclared.ConvertToKiloWatt().ToXMLFormat(0)),
				new XElement(_mrf + XMLNames.Engine_IdlingSpeed, inputData.JobInputData.Vehicle.EngineIdleSpeed.AsRPM.ToXMLFormat(0)),
				new XElement(_mrf + XMLNames.Engine_RatedSpeed, inputData.JobInputData.Vehicle.Components.EngineInputData.RatedSpeedDeclared.AsRPM.ToXMLFormat(0)),
				new XElement(_mrf + "Capacity", SIBase<Liter>.Create(engineData.Displacement.ConvertToCubicDeziMeter()).ToXMLFormat())
			);
			var fuels = new HashSet<FuelType>();
			foreach (var engineMode in engineData.EngineModes) {
				foreach (var fuel in engineMode.Fuels) {
					fuels.Add(fuel.FuelType);
				}
			}

			var sortedFuels = fuels.ToList();
			sortedFuels.Sort((fuelType1, fuelType2) => fuelType1.CompareTo(fuelType2));
			var fuelTypesElement = new XElement(_mrf + XMLNames.Report_Vehicle_FuelTypes);
			result.Add(fuelTypesElement);
			foreach (var fuel in sortedFuels)
			{
				fuelTypesElement.Add(new XElement(_mrf + XMLNames.Engine_FuelType, fuel.ToXMLFormat()));
			}
			result.Add(new XElement(_mrf + "WasteHeatRecoverySystem", engineData.WHRType != WHRType.None));
			foreach (WHRType whrType in Enum.GetValues(typeof(WHRType))) {
				if (whrType == WHRType.None) {
					continue;
				}

				if ((whrType & engineData.WHRType) == whrType) {
					result.Add(_mrf + "WasteHeatRecoverySystemType", whrType.ToXMLFormat());
				}
			}


			return result;
		}

		#endregion
	}
}
