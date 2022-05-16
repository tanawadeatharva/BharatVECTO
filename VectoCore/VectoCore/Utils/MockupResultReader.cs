using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReport;
using RuntimeArgumentHandle = System.RuntimeArgumentHandle;

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

			private static HashSet<string> hev = new HashSet<string>() { };
			private static HashSet<string> pev = new HashSet<string>() { };
			public static string GetResourceName(string xmlName, XMLDeclarationReport.ResultEntry result, ResultType type)
			{
				var arch = GetArch(xmlName, false);
				var reportType = type == ResultType.MRF ? "MRF" : "CIF";
				var vehicleType = result.VehicleClass.IsBus() ? "Bus" : "Lorry";
				return $"{mockupResourcePrefix}.{reportType}_MockupResults_{arch}_{vehicleType}.xml";
			}

			private static string GetArch(string xmlName, bool ocv)
			{
				if (conventional.Contains(xmlName)) {
					return "Conv";
				}

				if (hev.Contains(xmlName)) {
					if (ocv) {
						return "OCV-HEV";
					} else {
						return "non-OCV-HEV";
					}
					
				}

				if (pev.Contains(xmlName)) {
					return "PEV";
				}

				throw new VectoException($"{xmlName} not mapped to Architecture (Conv/HEV/PEV)");
			}


		}
        
		public static XElement GetMRFMockupResult(string xmlName, XMLDeclarationReport.ResultEntry result, XName resultElementName)
		{
			var resultElement = GetResultElement(resultElementName, MockupResultHelper.GetResourceName(xmlName, result, ResultType.MRF));
			ReplaceMission(result, resultElement);
			SetFuels(result, resultElement);
			

			return resultElement;
		}


		public static XElement GetCIFMockupResult(string xmlName, XMLDeclarationReport.ResultEntry result, XName resultElementName)
		{
			var resultElement = GetResultElement(resultElementName, MockupResultHelper.GetResourceName(xmlName, result, ResultType.CIF));
			ReplaceMission(result, resultElement);
			SetFuels(result, resultElement);

			return resultElement;
		}

		private static XElement GetResultElement(XName resultElementName, string resourceName)
		{
			var xDoc = XDocument.Load(RessourceHelper.ReadStream(resourceName));
			var results = xDoc.XPathSelectElements($"//*[name()='Result']");
			
			return results.First();
		}

		
		private static void ReplaceMission(XMLDeclarationReport.ResultEntry result, XElement resultElement)
		{
			resultElement.Elements().Single(x => x.Name.LocalName == XMLNames.Report_Result_Mission).Value =
				result.Mission.ToXMLFormat();
		}

		private static void SetFuels(XMLDeclarationReport.ResultEntry result, XElement resultElement)
		{
			var fuelElement = resultElement.XPathSelectElements("//*[name()='Fuel']").First();
			var insertAfter = fuelElement.PreviousNode;         //FuelElements added after this element
			fuelElement.Remove();
			
			foreach (var fuelProperties in result.FuelData) {
				
				var fuelElementToAdd = new XElement(fuelElement); //deep copy of fuel element;
				fuelElementToAdd.SetAttributeValue(XMLNames.Report_Results_Fuel_Type_Attr, fuelProperties.FuelType.ToXMLFormat());
				ClearFuelConsumptionEntries(fuelProperties.FuelType, fuelElementToAdd, result.VehicleClass);

				insertAfter.AddAfterSelf(fuelElementToAdd);
				insertAfter = fuelElementToAdd;
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
