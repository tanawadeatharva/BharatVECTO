using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML;


namespace XMLConverterLibrary
{
	public abstract class AbstractXMLJobConverter : IXMLEntityConverter
	{
		protected const string CREATED_BY = "Created by the VECTO XML Converter tool";

		public ErrorOr<XDocument> Convert(XDocument source)
		{
			var sourceVersion = XMLUtils.GetJobVersion(source);

			if (sourceVersion != SourceVersion)
			{
				return Error.Validation(
					description: $"XMLJobConverter: source job version ({sourceVersion}) should be {SourceVersion}");
			}

			var target = ConvertWithoutVersionValidation(source);

			var targetVersion = XMLUtils.GetJobVersion(target);

			if (targetVersion != TargetVersion)
			{
				return Error.Validation(
					description: $"XMLJobConverter: target job version ({targetVersion}) should be {TargetVersion}");
			}

			return target;
		}

		public abstract string SourceVersion { get; }

		public abstract string TargetVersion { get; }

		public XNamespace TargetNamespace => $"{XMLDeclarationNamespaces.DeclarationDefinition}:{TargetVersion}";

		protected XDocument ConvertWithoutVersionValidation(XDocument doc)
		{
			ConvertRootElement(doc);

			var isExempted = IsVehicleExempted(doc);

			ConvertVehicleBase(doc, isExempted);

			return isExempted ? ConvertExemptedVehicle(doc) : ConvertNonExemptedVehicle(doc);
		}

		protected virtual XDocument ConvertExemptedVehicle(XDocument doc)
		{
			XMLUtils.AddElementAfter(doc, "Vehicle/ChassisConfiguration", "AxleConfiguration", CalculateAxleConfiguration(doc));
			XMLUtils.AddElementAfter(doc, "Vehicle/ZeroEmissionVehicle", "SleeperCab", "true");

			AddSumNetPower(doc);
			AddExemptedTech(doc);

			return doc;
		}

		private void AddExemptedTech(XDocument doc)
		{
			var exemptedTech = GetExemptedTech(doc);

			XMLUtils.DeleteElement(doc, "Vehicle/HybridElectricHDV");
			XMLUtils.DeleteElement(doc, "Vehicle/DualFuelVehicle");

			XMLUtils.AddElementAfter(doc, "Vehicle/SumNetPower", "Technology", exemptedTech);
		}

		private string GetExemptedTech(XDocument doc)
		{
			var hevNode = doc.XPathSelectElement(XMLUtils.QueryLocalName("HybridElectricHDV"));
			bool.TryParse(hevNode?.Value, out bool isHEV);
			if (isHEV)
			{
				return "HEV Article 9 exempted";
			}

			var dualNode = doc.XPathSelectElement(XMLUtils.QueryLocalName("DualFuelVehicle"));
			bool.TryParse(dualNode?.Value, out bool isDualFuel);
			if (isDualFuel)
			{
				return "Dual fuel vehicle Article 9 exempted";
			}

			return "Other technology Article 9 exempted";
		}

		private void AddSumNetPower(XDocument doc)
		{
			var sumNetPower = CalculateSumNetPower(doc);

			XMLUtils.DeleteElement(doc, "Vehicle/MaxNetPower1");
			XMLUtils.DeleteElement(doc, "Vehicle/MaxNetPower2");

			XMLUtils.AddElementAfter(doc, "Vehicle/SleeperCab", "SumNetPower", sumNetPower.ToString());
		}

		private int CalculateSumNetPower(XDocument doc)
		{
			var power1Node = doc.XPathSelectElement(XMLUtils.QueryLocalName("MaxNetPower1"));
			int power1 = 0;

			if (power1Node != null)
			{
				int.TryParse(power1Node.Value, out power1);
			}

			var power2Node = doc.XPathSelectElement(XMLUtils.QueryLocalName("MaxNetPower2"));
			int power2 = 0;

			if (power2Node != null)
			{
				int.TryParse(power2Node.Value, out power2);
			}

			return power1 + power2;
		}

		protected void ConvertEngine(XDocument doc)
		{
			string version = SelectVersion(doc.XPathSelectElement(XMLUtils.QueryLocalName("Engine/Data".Split('/'))));

			XMLUtils.SetElementsType(doc, "Engine/Data", "EngineDataDeclarationType");
			XMLUtils.SetElementsNamespace(doc, "Engine/Data", XMLUtils.V2_0);
			XMLUtils.SetElementsDescendantsNamespace(doc, "Engine/Data", version);
			XMLUtils.SetElementsAttribute(doc, "Engine/Data", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{version}");
			XMLUtils.SetElementsNamespace(doc, "Engine/Signature", XMLUtils.V2_0);
		}

		protected void ConvertGearbox(XDocument doc)
		{
			XMLUtils.SetElementsType(doc, "Gearbox/Data", "GearboxDataDeclarationType");
			XMLUtils.SetElementsNamespace(doc, "Gearbox/Data", XMLUtils.V2_0);
			XMLUtils.SetElementsDescendantsNamespace(doc, "Gearbox/Data", XMLUtils.V2_0);
			XMLUtils.SetElementsAttribute(doc, "Gearbox/Data", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{XMLUtils.V2_0}");
			XMLUtils.SetElementsType(doc, "Gearbox/Data/Gears", "GearsDeclarationType");
			XMLUtils.SetElementsNamespace(doc, "Gearbox/Signature", XMLUtils.V2_0);
		}

		protected void ConvertRetarder(XDocument doc)
		{
			XMLUtils.SetElementsType(doc, "Retarder/Data", "RetarderDataDeclarationType");
			XMLUtils.SetElementsNamespace(doc, "Retarder/Data", XMLUtils.V2_0);
			XMLUtils.SetElementsDescendantsNamespace(doc, "Retarder/Data", XMLUtils.V2_0);
			XMLUtils.SetElementsAttribute(doc, "Retarder/Data", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{XMLUtils.V2_0}");
			XMLUtils.SetElementsNamespace(doc, "Retarder/Signature", XMLUtils.V2_0);
		}

		protected void ConvertAxlegear(XDocument doc)
		{
			XMLUtils.SetElementsType(doc, "Axlegear/Data", "AxlegearDataDeclarationType");
			XMLUtils.SetElementsNamespace(doc, "Axlegear/Data", XMLUtils.V2_0);
			XMLUtils.SetElementsDescendantsNamespace(doc, "Axlegear/Data", XMLUtils.V2_0);
			XMLUtils.SetElementsAttribute(doc, "Axlegear/Data", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{XMLUtils.V2_0}");
			XMLUtils.SetElementsNamespace(doc, "Axlegear/Signature", XMLUtils.V2_0);
		}

		protected void ConvertAxleWheels(XDocument doc)
		{
			XMLUtils.SetElementsType(doc, "AxleWheels/Data", "AxleWheelsDataDeclarationType");
			XMLUtils.SetElementsNamespace(doc, "AxleWheels/Data", XMLUtils.V2_0);
			XMLUtils.SetElementsDescendantsNamespace(doc, "AxleWheels/Data", XMLUtils.V2_0);
			XMLUtils.SetElementsAttribute(doc, "AxleWheels/Data", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{XMLUtils.V2_0}");
			XMLUtils.SetElementsType(doc, "AxleWheels/Data/Axles/Axle", "AxleDataDeclarationType");
			XMLUtils.SetElementsNamespace(doc, "AxleWheels/Signature", XMLUtils.V2_0);

			ConvertTyres(doc);
		}

		protected void ConvertTyres(XDocument doc)
		{
			XMLUtils.SetElementsType(doc, "Tyre/Data", "TyreDataDeclarationType");

			var nodes = doc.XPathSelectElements(XMLUtils.QueryLocalName("Tyre/Data".Split('/')));

			foreach (var node in nodes)
			{
				var tyreVersion = SelectVersion(node);

				node.SetAttributeValue("xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{tyreVersion}");

				XMLUtils.SetElementDescendantsNamespace(node, tyreVersion);
			}
		}

		protected string SelectVersion(XElement node)
		{
			List<string> versions = new List<string>() { XMLUtils.V2_0 };

			var nsnVersion = node.Name.NamespaceName.Split(':').Last();
			if (nsnVersion != TargetVersion)
			{
				versions.Add(nsnVersion);
			}

			var defnsVersion = node.GetDefaultNamespace().NamespaceName.Split(':').Last();
			if (defnsVersion != TargetVersion)
			{
				versions.Add(defnsVersion);
			}

			return versions.OrderByDescending(x => x).First();
		}

		protected void ConvertAuxiliaries(XDocument doc)
		{
			XMLUtils.SetElementsAttribute(doc, "Auxiliaries", "xmlns", null);
			XMLUtils.SetElementsType(doc, "Auxiliaries", null);
			XMLUtils.SetElementsType(doc, "Auxiliaries/Data", "AUX_Conventional_LorryDataType");
			XMLUtils.SetElementsAttribute(doc, "Auxiliaries/Data/SteeringPump/Technology", "axleNumber", "1");
			XMLUtils.FixElementsValue(doc, "Auxiliaries/Data/SteeringPump/Technology");
		}

		protected void ConvertAirDrag(XDocument doc)
		{
			XMLUtils.SetElementsType(doc, "AirDrag/Data", "AirDragDataDeclarationType");
			XMLUtils.SetElementsNamespace(doc, "AirDrag/Data", XMLUtils.V2_0);
			XMLUtils.SetElementsDescendantsNamespace(doc, "AirDrag/Data", XMLUtils.V2_0);
			XMLUtils.SetElementsAttribute(doc, "AirDrag/Data", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{XMLUtils.V2_0}");
			XMLUtils.SetElementsNamespace(doc, "AirDrag/Signature", XMLUtils.V2_0);
		}

		protected void ConvertTorqueLimits(XDocument doc)
		{
			XMLUtils.SetElementsType(doc, "Vehicle/TorqueLimits", "TorqueLimitsType");
			XMLUtils.SetElementsNamespace(doc, "Vehicle/TorqueLimits", XMLUtils.V2_4);
			XMLUtils.SetElementsDescendantsNamespace(doc, "Vehicle/TorqueLimits", XMLUtils.V2_0);
			XMLUtils.SetElementsAttribute(doc, "Vehicle/TorqueLimits", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{XMLUtils.V2_0}");
		}

		protected virtual void ConvertTorqueConverter(XDocument doc)
		{
			XMLUtils.SetElementsType(doc, "TorqueConverter/Data", "TorqueConverterDataDeclarationType");
			XMLUtils.SetElementsNamespace(doc, "TorqueConverter/Data", XMLUtils.V2_0);
			XMLUtils.SetElementsDescendantsNamespace(doc, "TorqueConverter/Data", XMLUtils.V2_0);
			XMLUtils.SetElementsAttribute(doc, "TorqueConverter/Data", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{XMLUtils.V2_0}");
			XMLUtils.SetElementsNamespace(doc, "TorqueConverter/Signature", XMLUtils.V2_0);
		}

		protected void ConvertAngleDrive(XDocument doc)
		{
			XMLUtils.SetElementsType(doc, "Angledrive/Data", "AngledriveDataDeclarationType");
			XMLUtils.SetElementsNamespace(doc, "Angledrive/Data", XMLUtils.V2_0);
			XMLUtils.SetElementsDescendantsNamespace(doc, "Angledrive/Data", XMLUtils.V2_0);
			XMLUtils.SetElementsAttribute(doc, "Angledrive/Data", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{XMLUtils.V2_0}");
			XMLUtils.SetElementsNamespace(doc, "Angledrive/Signature", XMLUtils.V2_0);
		}

		protected void SetDefaultNamespace(XDocument doc)
		{
			doc.Root.Attribute("xmlns")?.Remove();

			doc.Root.Name = XMLDeclarationNamespaces.Tns_v20 + doc.Root.Name.LocalName;

			doc.Root.SetAttributeValue("xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{TargetVersion}");
		}

		protected void RemoveSchemaLocation(XDocument doc)
		{
			doc.Root.Attribute(XMLDeclarationNamespaces.Xsi + XMLNames.SchemaLocation)?.Remove();
		}

		protected virtual void AddNamespaces(XDocument doc)
		{
			doc.Root.SetAttributeValue(XNamespace.Xmlns + XMLUtils.V2_0, $"{XMLDeclarationNamespaces.DeclarationDefinition}:{XMLUtils.V2_0}");
			doc.Root.SetAttributeValue(XNamespace.Xmlns + XMLUtils.V2_4, $"{XMLDeclarationNamespaces.DeclarationDefinition}:{XMLUtils.V2_4}");
		}

		protected bool IsVehicleExempted(XDocument doc)
		{
			var componentsNode = doc.XPathSelectElement(XMLUtils.QueryLocalName("Vehicle", "Components"));

			return componentsNode == null;
		}

		protected virtual XDocument ConvertNonExemptedVehicle(XDocument doc)
		{
			XMLUtils.SetElementsType(doc, "Vehicle/PTO", "PTOType");
			XMLUtils.SetElementsAttribute(doc, "Vehicle/PTO", "xmlns", null);
			XMLUtils.SetElementsType(doc, "Vehicle/ADAS", "ADAS_Conventional_Type");
			XMLUtils.SetElementsType(doc, "Vehicle/Components", "Components_Conventional_LorryType");

			ConvertEngine(doc);
			ConvertGearbox(doc);
			ConvertRetarder(doc);
			ConvertAxlegear(doc);
			ConvertAxleWheels(doc);
			ConvertAuxiliaries(doc);
			ConvertAirDrag(doc);
			ConvertTorqueLimits(doc);
			ConvertTorqueConverter(doc);
			ConvertAngleDrive(doc);

			return doc;
		}

		protected virtual void ConvertRootElement(XDocument doc)
		{
			ChangeValueOfSchemaVersionAttribute(doc);
			SetDefaultNamespace(doc);
			SetNamespaceTNS(doc);
			AddNamespaces(doc);
			RemoveSchemaLocation(doc);
			AddSourceComment(doc);
		}

		protected void AddSourceComment(XDocument doc)
		{
			doc.Root.AddBeforeSelf(new XComment(CREATED_BY));
		}

		protected void ChangeValueOfSchemaVersionAttribute(XDocument doc)
		{
			doc.Root.SetAttributeValue("schemaVersion", "2.0");
		}

		protected void SetNamespaceTNS(XDocument doc)
		{
			doc.Root.SetAttributeValue(XNamespace.Xmlns + "tns", XMLDeclarationNamespaces.Tns_v20);
		}

		protected void ConvertVehicleBase(XDocument doc, bool isExempted)
		{
			XMLUtils.SetElementsNamespace(doc, "Vehicle", XMLUtils.V2_0);

			XMLUtils.SetElementsType(
				doc,
				"Vehicle",
				isExempted
					? "Vehicle_Exempted_HeavyLorryDeclarationType"
					: "Vehicle_Conventional_HeavyLorryDeclarationType"
			);

			XMLUtils.SetElementsDescendantsNamespace(doc, "Vehicle", TargetVersion);
			XMLUtils.SetElementsAttribute(doc, "Vehicle", "xmlns", null);

			XMLUtils.SetElementName(doc, "Vehicle/LegislativeClass", "LegislativeCategory", TargetNamespace);
			XMLUtils.SetElementName(doc, "Vehicle/VehicleCategory", "ChassisConfiguration", TargetNamespace);
			XMLUtils.FixElementsValue(doc, "Vehicle/ChassisConfiguration");

			XMLUtils.SetElementName(doc, "Vehicle/CurbMassChassis", "CorrectedActualMass", TargetNamespace);
			XMLUtils.SetElementName(doc, "Vehicle/GrossVehicleMass", "TechnicalPermissibleMaximumLadenMass", TargetNamespace);
			XMLUtils.AddElementAfter(doc, "Vehicle/ChassisConfiguration", "AxleConfiguration", CalculateAxleConfiguration(doc)); 
		}

		protected string CalculateAxleConfiguration(XDocument doc)
		{
			var axles = XMLUtils.GetElements(doc, "AxleWheels/Data/Axles/Axle");

			var drivenAxlesCount = axles.Count(x => x.XPathSelectElement(XMLUtils.QueryLocalName("AxleType")).Value == "VehicleDriven");

			return (axles.Count() > 0) ? $"{axles.Count() * 2}x{drivenAxlesCount * 2}" : "4x2";
		}
	}

	public class XMLJobConverter_v1_0_To_v_2_4 : AbstractXMLJobConverter
	{
		public override string SourceVersion => XMLUtils.V1_0;

		public override string TargetVersion => XMLUtils.V2_4;

		protected override void ConvertTorqueConverter(XDocument doc)
		{
			XMLUtils.MoveElementUp(doc, "Gearbox/TorqueConverter");
			base.ConvertTorqueConverter(doc);
		}

	}

	public class XMLJobConverter_v2_0_To_v2_4 : AbstractXMLJobConverter
	{
		public override string SourceVersion => XMLUtils.V2_0;

		public override string TargetVersion => XMLUtils.V2_4;

		protected override XDocument ConvertNonExemptedVehicle(XDocument doc)
		{
			base.ConvertNonExemptedVehicle(doc);

			XMLUtils.AddElementAfter(doc, "Vehicle/PTO", "ZeroEmissionVehicle", "false");
			XMLUtils.AddElementAfter(doc, "Vehicle/ZeroEmissionVehicle", "VocationalVehicle", "false");
			XMLUtils.AddElementAfter(doc, "Vehicle/VocationalVehicle", "SleeperCab", "true");

			AddADAS(doc);

			return doc;
		}

		private void AddADAS(XDocument doc)
		{
			var added = XMLUtils.AddElementAfter(doc, "Vehicle/SleeperCab", "ADAS", "");
			if (!added)
			{
				return;
			}

			XMLUtils.SetElementsType(doc, "Vehicle/ADAS", "ADAS_Conventional_Type");
			XMLUtils.AddElementTo(doc, "Vehicle/ADAS", "EngineStopStart", "false");
			XMLUtils.AddElementTo(doc, "Vehicle/ADAS", "EcoRollWithoutEngineStop", "false");
			XMLUtils.AddElementTo(doc, "Vehicle/ADAS", "EcoRollWithEngineStop", "false");
			XMLUtils.AddElementTo(doc, "Vehicle/ADAS", "PredictiveCruiseControl", "none");
		}

	}

	public class XMLJobConverter_v2_1_To_v2_4 : AbstractXMLJobConverter
	{
		public override string SourceVersion => XMLUtils.V2_1;

		public override string TargetVersion => XMLUtils.V2_4;

		protected override void AddNamespaces(XDocument doc)
		{
			base.AddNamespaces(doc);
			doc.Root.SetAttributeValue(XNamespace.Xmlns + SourceVersion, $"{XMLDeclarationNamespaces.DeclarationDefinition}:{SourceVersion}");
		}

	}

	public class XMLJobConverter_v2_2_1_To_v2_4 : AbstractXMLJobConverter
	{
		public override string SourceVersion => XMLUtils.V2_2_1;

		public override string TargetVersion => XMLUtils.V2_4;

		protected override void AddNamespaces(XDocument doc)
		{
			base.AddNamespaces(doc);
			doc.Root.SetAttributeValue(XNamespace.Xmlns + SourceVersion, $"{XMLDeclarationNamespaces.DeclarationDefinition}:{SourceVersion}");
		}
	}

}

