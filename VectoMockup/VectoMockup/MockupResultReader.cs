using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoMockup
{


    internal static class MockupResultReader
    {
		private enum ResultType
		{
			CIF,
			MRF,
			VIF
		}
		private static class MockupResultHelper
		{
			private static string _convArch = "Conv";
			private const string mockupResourcePrefix = "TUGraz.VectoMockup.MockupResults";
			
			
			private static HashSet<string> conventional = new HashSet<string>() {
				//MRF
				XMLNames.MRF_OutputDataType_ConventionalLorryManufacturerOutputDataType,
				XMLNames.MRF_OutputDataType_ConventionalPrimaryBusManufacturerOutputDataType,

				//CIF
				XMLNames.CIF_OutputDataType_ConventionalLorryOutputType,
				

				//VIF //TODO: seperate namespaces
				"urn:tugraz:ivt:VectoAPI:DeclarationOutput:VehicleInterimFile:v0.1",
			};

			private static HashSet<string> hev = new HashSet<string>() {
				//MRF
				XMLNames.MRF_OutputDataType_HEV_S2_LorryManufacturerOutputDataType,
				XMLNames.MRF_OutputDataType_HEV_S3_LorryManufacturerOutputDataType,
				XMLNames.MRF_OutputDataType_HEV_S4_LorryManufacturerOutputDataType,
				XMLNames.MRF_OutputDataType_HEV_Px_IHPCLorryManufacturerOutputDataType,
				XMLNames.MRF_OutputDataType_HEV_IEPC_S_LorryManufacturerOutputDataType,
				//CIF
				XMLNames.CIF_OutputDataType_HEV_S2_LorryOutputType,
				XMLNames.CIF_OutputDataType_HEV_S3_LorryOutputType,
				XMLNames.CIF_OutputDataType_HEV_S4_LorryOutputType,
				XMLNames.CIF_OutputDataType_HEV_IEPC_S_LorryOutputType,
				XMLNames.CIF_OutputDataType_HEV_Px_LorryOutputType,
			};
			private static HashSet<string> pev = new HashSet<string>() {
				//MRF
				XMLNames.MRF_OutputDataType_PEV_E2_LorryManufacturerOutputDataType,
				XMLNames.MRF_OutputDataType_PEV_E3_LorryManufacturerOutputDataType,
				XMLNames.MRF_OutputDataType_PEV_E4_LorryManufacturerOutputDataType,
				XMLNames.MRF_OutputDataType_PEV_IEPC_LorryManufacturerOutputDataType,

				//CIF
				XMLNames.CIF_OutputDataType_PEV_E2_LorryOutputType,
				XMLNames.CIF_OutputDataType_PEV_E3_LorryOutputType,
				XMLNames.CIF_OutputDataType_PEV_E4_LorryOutputType,
				XMLNames.CIF_OutputDataType_PEV_IEPC_LorryOutputType

			};

			public static string GetResourceName(string xmlName, XMLDeclarationReport.ResultEntry result, ResultType type, VectoRunData runData)
			{
				
				var resNames = Assembly.GetAssembly(typeof(MockupResultReader)).GetManifestResourceNames();
				//if (result.Status == VectoRun.Status.Success) {
					var arch = GetArch(xmlName, runData);
					var reportType = GetReportType(type);
					var vehicleType = result.VehicleClass.IsBus() ? "Bus" : "Lorry";
					return $"{mockupResourcePrefix}.{reportType}_MockupResults_{arch}_{vehicleType}.xml";
				//}

				throw new NotImplementedException("Error result not implemented\n");
			}

			private static object GetReportType(ResultType type)
			{
				switch (type) {
					case ResultType.CIF:
						return "CIF";
					case ResultType.MRF:
						return "MRF";
					case ResultType.VIF:
						return "VIF";
					default:
						throw new ArgumentOutOfRangeException(nameof(type), type, null);
				}
			}

			private static string GetArch(string xmlName, VectoRunData runData)
			{
				bool ovc = false;
				var jobType = VectoSimulationJobType.ConventionalVehicle;
				if (runData.InputData is IXMLMultistageInputDataProvider) {
					ovc = false; //TODO implement
				} else {
					ovc = runData.InputData.JobInputData.Vehicle.OvcHev;
					jobType = runData.InputData.JobInputData.JobType;
				}

				
				if (jobType == VectoSimulationJobType.ConventionalVehicle) {
					return "Conv";
				}

				if (jobType.IsOneOf(VectoSimulationJobType.ParallelHybridVehicle, VectoSimulationJobType.SerialHybridVehicle)) {
					return ovc ? "OVC-HEV" : "non-OVC-HEV";
				}

				if (jobType.IsOneOf(VectoSimulationJobType.BatteryElectricVehicle)) {
					return "PEV";
				}

				throw new VectoException($"{xmlName} not mapped to Architecture (Conv/HEV/PEV)");
			}


		}
        
		public static XElement GetMRFMockupResult(string xmlName, XMLDeclarationReport.ResultEntry result, XName resultElementName, VectoRunData runData)
		{
			var resultElement = GetResultElement(resultElementName, MockupResultHelper.GetResourceName(xmlName, result, ResultType.MRF, runData));
			ReplaceMission(result, resultElement);
			SetFuels(result, resultElement);
			ClearGearboxAndAxleGearEntries(result, resultElement, runData);
			
			return resultElement;
		}



		public static XElement GetCIFMockupResult(string xmlName, XMLDeclarationReport.ResultEntry result, XName resultElementName, VectoRunData runData)
		{
			var resultElement = GetResultElement(resultElementName, MockupResultHelper.GetResourceName(xmlName, result, ResultType.CIF, runData));
			resultElement.DescendantNodes().OfType<XComment>().Remove();
			ReplaceMission(result, resultElement);
			SetFuels(result, resultElement);

			return resultElement;
		}

		public static XElement GetVIFMockupResult(string xmlName, XMLDeclarationReport.ResultEntry result, XName resultElementName, VectoRunData runData)
		{
			var resultElement = GetResultElement(resultElementName, MockupResultHelper.GetResourceName(xmlName, result, ResultType.VIF, runData));
			resultElement.DescendantNodes().OfType<XComment>().Remove();
			ReplaceMission(result, resultElement);
			ReplaceGroup(result, resultElement);
			ReplacePayload(result, resultElement);
			ReplaceFuelMode(result,resultElement);
			//SetFuels(result, resultElement);

			//Results.Add(
			//	new XElement(
			//		tns + XMLNames.Report_Result_Result,
			//		new XAttribute(
			//			XMLNames.Report_Result_Status_Attr,
			//			resultEntry.Status == VectoRun.Status.Success ? "success" : "error"),
			//		new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, resultEntry.VehicleClass.GetClassNumber()),
			//		new XElement(tns + XMLNames.Report_Result_Mission, resultEntry.Mission.ToXMLFormat()),
			//		new XElement(
			//			tns + XMLNames.Report_ResultEntry_SimulationParameters,
			//			new XElement(
			//				tns + XMLNames.Report_ResultEntry_TotalVehicleMass,
			//				XMLHelper.ValueAsUnit(resultEntry.TotalVehicleMass, XMLNames.Unit_kg, 2)),
			//			new XElement(
			//				tns + XMLNames.Report_Result_Payload, XMLHelper.ValueAsUnit(resultEntry.Payload, XMLNames.Unit_kg, 2)),
			//			new XElement(
			//				tns + XMLNames.Report_ResultEntry_PassengerCount,
			//				resultEntry.PassengerCount?.ToXMLFormat(2) ?? "NaN"),
			//			new XElement(
			//				tns + XMLNames.Report_Result_FuelMode,
			//				resultEntry.FuelData.Count > 1
			//					? XMLNames.Report_Result_FuelMode_Val_Dual
			//					: XMLNames.Report_Result_FuelMode_Val_Single)
			//		),
			//		GetResults(resultEntry)));

			return resultElement;
		}



		private static void ReplacePayload(XMLDeclarationReport.ResultEntry result, XElement resultElement)
		{
			var payload = resultElement.XPathSelectElements($"//*[local-name()='{XMLNames.Report_ResultEntry_Payload}']");
			payload.Single().Value = result.Payload.ToXMLFormat();
		}

		private static void ReplaceGroup(XMLDeclarationReport.ResultEntry result, XElement resultElement)
		{
			var groupElement = resultElement.XPathSelectElements($"//*[local-name()='{XMLNames.Report_Vehicle_VehicleGroup}']");
			groupElement.Single().Value = result.VehicleClass.GetClassNumber();
		}

		private static void ReplaceFuelMode(XMLDeclarationReport.ResultEntry result, XElement resultElement)
		{
			var fuelMode = resultElement.XPathSelectElements($"//*[local-name()='{XMLNames.Report_Result_FuelMode}']");
			var fuelModeElement = fuelMode.FirstOrDefault();
			if (fuelModeElement != null) {
				fuelModeElement.Value = result.FuelData.Count > 1
					? XMLNames.Report_Result_FuelMode_Val_Dual
					: XMLNames.Report_Result_FuelMode_Val_Single;
			}
		}


		private static XElement GetResultElement(XName resultElementName, string resourceName)
		{
			var xDoc = XDocument.Load(ReadStream(resourceName));
			
			var results = xDoc.XPathSelectElements($"//*[local-name()='{resultElementName.LocalName}']");
			var resultElement = results.First();
			resultElement.DescendantNodes().OfType<XComment>().Remove();
			return resultElement;
		}
		public static Stream ReadStream(string resourceName)
		{
			var assembly = Assembly.GetAssembly(typeof(MockupResultReader));
			var resource = assembly.GetManifestResourceStream(resourceName);
			if (resource == null)
			{
				throw new VectoException("Resource file not found: " + resourceName);
			}
			return resource;
		}


		private static void ReplaceMission(XMLDeclarationReport.ResultEntry result, XElement resultElement)
		{
			var mission = resultElement.Elements()
				.FirstOrDefault(x => x.Name.LocalName == XMLNames.Report_Result_Mission);
			if (mission != null) {
				mission.Value = result.Mission.ToXMLFormat();
			}
				
		}

		private static void SetFuels(XMLDeclarationReport.ResultEntry result, XElement resultElement)
		{
			//var tmpResultElement = new XElement(resultElement);
			var fuelElements = resultElement.XPathSelectElements("//*[name()='Fuel']").ToList();
			foreach (var fuelElement in fuelElements) {
				XElement lastAdded = null;
				foreach (var fuelProperties in result.FuelData)
				{
					Action<XElement> insertAction = (element) => {
						//FuelElements added after this element
						if (lastAdded != null) {
							fuelElement.AddAfterSelf(element);
							
						} else if (fuelElement.PreviousNode != null) {
							fuelElement.AddAfterSelf(element);
							
						} else {
							fuelElement.Parent.AddFirst(element);
						}
						lastAdded = element;
						fuelElement.Remove();
					};

					var fuelElementToAdd = new XElement(fuelElement); //deep copy of fuel element;
					fuelElementToAdd.SetAttributeValue(XMLNames.Report_Results_Fuel_Type_Attr, fuelProperties.FuelType.ToXMLFormat());
					ClearFuelConsumptionEntries(fuelProperties, fuelElementToAdd, result.VehicleClass);



					insertAction(fuelElementToAdd);


				}
			}
		}

		private static void ClearGearboxAndAxleGearEntries(XMLDeclarationReport.ResultEntry result,
			XElement resultElement, VectoRunData runData)
		{
			var elementsToRemove = new List<XElement>();
			if (runData.GearboxData == null) {
				elementsToRemove.AddRange(resultElement.XPathSelectElements("//*[name()='GearshiftCount']"));
				elementsToRemove.AddRange(resultElement.XPathSelectElements("//*[name()='AverageGearboxEfficiency']"));
			}

			if (runData.AxleGearData == null) {
				elementsToRemove.AddRange(resultElement.XPathSelectElements("//*[name()='AverageAxlegearEfficiency']"));
			}
			foreach (var xElement in elementsToRemove) {
				xElement.Remove();
			}
		}

		/// <summary>
		/// Clears fuel consumption entries that are not used for a specified fueltype and vehicle class
		/// </summary>
		private static void ClearFuelConsumptionEntries(IFuelProperties fuelProperties, XElement fuelElement,
			VehicleClass vehicleClass)
		{
			if(!(vehicleClass.IsHeavyLorry() || vehicleClass.IsMediumLorry()))
			{
				fuelElement.XPathSelectElements("//*[@unit='l/m³-km']").FirstOrDefault()?.Remove();
			}

			if (fuelProperties.FuelDensity == null) {
				//var test = fuelElement.XPathSelectElements("//*[@unit='l/m³-km']");
				fuelElement.XPathSelectElements("//*[@unit='l/m³-km']").FirstOrDefault()?.Remove();
				fuelElement.XPathSelectElements("//*[@unit='l/t-km']").FirstOrDefault()?.Remove();
				fuelElement.XPathSelectElements("//*[@unit='l/100km']").FirstOrDefault()?.Remove();
			}
		}
	}
}
