using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationMultistagePrimaryVehicleBusResultsInputDataProviderV01 : AbstractXMLType,
		IXMLResultsInputData
	{
		public static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "ResultsPrimaryVehicleType";

		public static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private XmlNode _resultsNode;
		private IList<IResult> _results;

		public XMLDeclarationMultistagePrimaryVehicleBusResultsInputDataProviderV01(XmlNode resultsNode) : base(
			resultsNode)
		{
			_resultsNode = resultsNode;
		}

		public string Status => GetString(XMLNames.Bus_Status);

		public IList<IResult> Results => _results ?? (_results = ReadResults());


		private IList<IResult> ReadResults()
		{
			if (_resultsNode == null || _resultsNode?.ChildNodes.Count == 0)
				return null;

			var results = new List<IResult>();

			foreach (XmlNode resultNode in _resultsNode.ChildNodes) {
				if (resultNode.LocalName == XMLNames.Report_Result_Result)
					results.AddRange(GetResult(resultNode));
			}

			return results;
		}

		private IList<IResult> GetResult(XmlNode xmlNode)
		{
			var resultStatus = GetAttribute(xmlNode, XMLNames.Result_Status);
			var vehicleGroup = GetString(XMLNames.Report_Results_PrimaryVehicleSubgroup, xmlNode);
			var mission = GetString(XMLNames.Report_Result_Mission, xmlNode).ParseEnum<MissionType>();
			var simulationNode = GetNode(XMLNames.Report_ResultEntry_SimulationParameters, xmlNode);
			var simulationParams = GetSimulationParameter(simulationNode);

			var ovcModes = GetNodes(XMLNames.Report_Results_OVCMode, xmlNode);

			if (ovcModes != null && ovcModes.Count > 0) {
				var retVal = new List<IResult>();
				foreach (XmlNode node in ovcModes) {
					var ovcMode = GetAttribute(node, XMLNames.Results_Report_OVCModeAttr).ParseEnum<OvcHevMode>();
					GetEnergyConsumption(node, out var ovcEnergyConsumption, out var ovcElectricEnergyConsumption);
					retVal.Add(new Result {
						ResultStatus = resultStatus,
						Mission = mission,
						VehicleGroup = VehicleClassHelper.Parse(vehicleGroup),
						SimulationParameter = simulationParams,
						EnergyConsumption = ovcEnergyConsumption,
						ElectricEnergyConsumption = ovcElectricEnergyConsumption,
						CO2 = new Dictionary<string, double>(),
						OvcMode = ovcMode
					});
				}

				return retVal;
			}

			GetEnergyConsumption(xmlNode, out var energyConsumption, out var electricEnergyConsumption);
			return new List<IResult>() {
				new Result {
					ResultStatus = resultStatus,
					Mission = mission,
					VehicleGroup = VehicleClassHelper.Parse(vehicleGroup),
					SimulationParameter = simulationParams,
					EnergyConsumption = energyConsumption,
					ElectricEnergyConsumption = electricEnergyConsumption,
					CO2 = new Dictionary<string, double > (),
					OvcMode = OvcHevMode.NotApplicable
				}
			};
		}

		private void GetEnergyConsumption(XmlNode xmlNode, out Dictionary<FuelType, JoulePerMeter> energyConsumption,
			out JoulePerMeter electricEnergyConsumption)
		{
			energyConsumption = GetNodes(XMLNames.Report_Results_Fuel, xmlNode)
				.Cast<XmlNode>().Select(x => new KeyValuePair<FuelType, JoulePerMeter>(
					GetAttribute(x, XMLNames.Report_Results_Fuel_Type_Attr).ParseEnum<FuelType>(),
					x.SelectSingleNode(
							$".//*[local-name()='{XMLNames.Report_Result_EnergyConsumption}' and @unit='MJ/km']")?.InnerText
						.ToDouble().SI(Unit.SI.Mega.Joule.Per.Kilo.Meter).Cast<JoulePerMeter>())).ToDictionary(x => x.Key, x => x.Value);
			electricEnergyConsumption = GetNode(XMLNames.Report_ResultEntry_VIF_ElectricEnergyConsumption, xmlNode)
				.SelectSingleNode(
					$".//*[local-name()='{XMLNames.Report_Result_EnergyConsumption}' and @unit='MJ/km']")?.InnerText
				.ToDouble().SI(Unit.SI.Mega.Joule.Per.Kilo.Meter).Cast<JoulePerMeter>();

		}


		private ISimulationParameter GetSimulationParameter(XmlNode xmlNode)
		{
			return new SimulationParameter {
				TotalVehicleMass = GetString(XMLNames.Report_ResultEntry_TotalVehicleMass, xmlNode).ToDouble()
					.SI<Kilogram>(),
				Payload = GetString(XMLNames.Report_Result_Payload, xmlNode).ToDouble().SI<Kilogram>(),
				PassengerCount = GetString(XMLNames.Bus_PassengerCount, xmlNode).ToDouble(),
				//FuelMode = GetString(XMLNames.Report_Result_FuelMode, xmlNode)
			};
		}
	}
}
