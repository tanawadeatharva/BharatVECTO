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
			private const string mockupResourcePrefix = "TUGraz.VectoCore.Mockup.MockupResults";
			
			
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
				if (runData.InputData is IXMLMultistageInputDataProvider mst) {
					ovc = false; //TODO implement
					jobType = mst.JobInputData.JobType; //runData.InputData.JobInputData.JobType;
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
			ReplaceGroup(result, resultElement);
			ReplacePayload(result, resultElement);
			ReplaceFuelMode(result, resultElement);
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
            SetFuels(result, resultElement);

            return resultElement;
		}



		private static void ReplacePayload(XMLDeclarationReport.ResultEntry result, XElement resultElement)
		{
			if (result.Payload == null) {
				return;
			}
			var payload = resultElement.XPathSelectElements($"//*[local-name()='{XMLNames.Report_ResultEntry_Payload}']").ToList();
			if (!payload.Any()) {
				return;
			}
			payload.ForEach(x => x.Value = result.Payload.ToXMLFormat());
		}

		private static void ReplaceGroup(XMLDeclarationReport.ResultEntry result, XElement resultElement)
		{
			var groupElement = resultElement.XPathSelectElements($"//*[local-name()='{XMLNames.Report_Results_PrimaryVehicleSubgroup}']").ToList();
			if (!groupElement.Any()) {
				return;
			}
			groupElement.ForEach(x => x.Value = result.VehicleClass.GetClassNumber());
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
			var fuelElements = resultElement.XPathSelectElements("//*[local-name()='Fuel']").ToList();
			foreach (var fuelElement in fuelElements) {
				var insertPos = fuelElement.NextNode as XElement;
				;
				foreach (var fuelProperties in result.FuelData)
				{
					var fuelElementToAdd = new XElement(fuelElement); //deep copy of fuel element;
					fuelElementToAdd.SetAttributeValue(XMLNames.Report_Results_Fuel_Type_Attr, fuelProperties.FuelType.ToXMLFormat());
					ClearFuelConsumptionEntries(fuelProperties, fuelElementToAdd, result.VehicleClass);
					if (insertPos == null) {
						resultElement.Add(fuelElementToAdd);
					} else {
						insertPos.AddBeforeSelf(fuelElementToAdd);
					}
				}
				fuelElement.Remove();
			}
		}

		private static void ClearGearboxAndAxleGearEntries(XMLDeclarationReport.ResultEntry result,
			XElement resultElement, VectoRunData runData)
		{
			var elementsToRemove = new List<XElement>();
			if (runData.GearboxData == null) {
				//elementsToRemove.AddRange(resultElement.XPathSelectElements("//*[name()='GearshiftCount']"));
				var gearshiftCount = resultElement.XPathSelectElement("//*[name()='GearshiftCount']");
				gearshiftCount.Value = "1";
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
				fuelElement.XPathSelectElements("//*[@unit='l/p-km']").FirstOrDefault()?.Remove();
			}
		}

		
	}
}
