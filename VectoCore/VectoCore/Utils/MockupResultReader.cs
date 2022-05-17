using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.XML;

namespace TUGraz.VectoCore.Utils
{


    internal static class MockupResultReader
    {
		private enum ResultType
		{
			CIF,
			MRF
		}
		private static class MockupResultHelper
		{
			private static string _convArch = "Conv";
			private const string mockupResourcePrefix = "TUGraz.VectoCore.Resources.Declaration.Report";

			
			private static HashSet<string> conventional = new HashSet<string>() {
				//MRF
				XMLNames.MRF_OutputDataType_ConventionalLorryManufacturerOutputDataType,
				XMLNames.MRF_OutputDataType_ConventionalPrimaryBusManufacturerOutputDataType,

				//CIF
				XMLNames.CIF_OutputDataType_ConventionalLorryOutputType,
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
			private static HashSet<string> pev = new HashSet<string>() { };
			public static string GetResourceName(string xmlName, XMLDeclarationReport.ResultEntry result, ResultType type, bool ovc)
			{
				//if (result.Status == VectoRun.Status.Success) {
					var arch = GetArch(xmlName, ovc);
					var reportType = type == ResultType.MRF ? "MRF" : "CIF";
					var vehicleType = result.VehicleClass.IsBus() ? "Bus" : "Lorry";
					return $"{mockupResourcePrefix}.{reportType}_MockupResults_{arch}_{vehicleType}.xml";
				//}

				throw new NotImplementedException("Error result not implemented\n");
			}

			private static string GetArch(string xmlName, bool ocv)
			{
				if (conventional.Contains(xmlName)) {
					return "Conv";
				}

				if (hev.Contains(xmlName)) {
					return ocv ? "OVC-HEV" : "non-OVC-HEV";
				}

				if (pev.Contains(xmlName)) {
					return "PEV";
				}

				throw new VectoException($"{xmlName} not mapped to Architecture (Conv/HEV/PEV)");
			}


		}
        
		public static XElement GetMRFMockupResult(string xmlName, XMLDeclarationReport.ResultEntry result, XName resultElementName, bool ovc)
		{
			var resultElement = GetResultElement(resultElementName, MockupResultHelper.GetResourceName(xmlName, result, ResultType.MRF, ovc));
			ReplaceMission(result, resultElement);
			SetFuels(result, resultElement);
			

			return resultElement;
		}



		public static XElement GetCIFMockupResult(string xmlName, XMLDeclarationReport.ResultEntry result, XName resultElementName, bool ovc)
		{
			var resultElement = GetResultElement(resultElementName, MockupResultHelper.GetResourceName(xmlName, result, ResultType.CIF, ovc));
			resultElement.DescendantNodes().OfType<XComment>().Remove();
			ReplaceMission(result, resultElement);
			SetFuels(result, resultElement);

			return resultElement;
		}

		private static XElement GetResultElement(XName resultElementName, string resourceName)
		{
			var xDoc = XDocument.Load(RessourceHelper.ReadStream(resourceName));
			
			var results = xDoc.XPathSelectElements($"//*[name()='{resultElementName.LocalName}']");
			
			return results.First();
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
					ClearFuelConsumptionEntries(fuelProperties.FuelType, fuelElementToAdd, result.VehicleClass);



					insertAction(fuelElementToAdd);


				}
			}
		}

		/// <summary>
		/// Clears fuel consumption entries that are not used for a specified fueltype and vehicle class
		/// </summary>
		private static void ClearFuelConsumptionEntries(FuelType fuelType, XElement fuelElement,
			VehicleClass vehicleClass)
		{
			if(!(vehicleClass.IsHeavyLorry() || vehicleClass.IsMediumLorry()))
			{
				fuelElement.XPathSelectElements("//*[@unit='l/m³-km']").FirstOrDefault()?.Remove();
			}

			if (fuelType.IsGaseous()) {
				//var test = fuelElement.XPathSelectElements("//*[@unit='l/m³-km']");
				fuelElement.XPathSelectElements("//*[@unit='l/m³-km']").FirstOrDefault()?.Remove();
				fuelElement.XPathSelectElements("//*[@unit='l/t-km']").FirstOrDefault()?.Remove();
				fuelElement.XPathSelectElements("//*[@unit='l/100km']").FirstOrDefault()?.Remove();
			}
		}
	}
}
