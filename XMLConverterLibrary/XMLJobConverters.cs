using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCommon.InputData;

namespace XMLConverterLibrary
{
    public abstract class AbstractXMLJobConverter : IXMLEntityConverter
    {
        public abstract string SourceVersion { get; }

        public abstract string TargetVersion { get; }

        public abstract ErrorOr<XDocument> Convert(XDocument source);
    }

    public abstract class AbstractXMLJobConverterSingle : AbstractXMLJobConverter
    {
		protected const string CREATED_BY = "Created by the VECTO XML Converter tool";

		public override ErrorOr<XDocument> Convert(XDocument source)
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

		public virtual XNamespace TNS_Namespace => null;

		public virtual string VehicleNamespaceVersion => null;

        public XNamespace TargetNamespace => $"{XMLDeclarationNamespaces.DeclarationDefinition}:{TargetVersion}";

        protected XDocument ConvertWithoutVersionValidation(XDocument doc)
		{
			ConvertRootElement(doc);

			var isExempted = IsVehicleExempted(doc);

			ConvertVehicleBase(doc, isExempted);

			return isExempted ? ConvertExemptedVehicle(doc) : ConvertNonExemptedVehicle(doc);
		}

        protected abstract XDocument ConvertExemptedVehicle(XDocument doc);

        protected abstract XDocument ConvertNonExemptedVehicle(XDocument doc);

        protected abstract void SetVehicleType(XDocument doc, bool isExempted);

        protected abstract void ConvertVehicleBase(XDocument doc, bool isExempted);

        protected string SelectHighestVersion(XElement node)
		{
            List<string> versions = GetVersions(node, XMLUtils.V2_0);

            return versions.OrderByDescending(x => x).First();
		}

        private List<string> GetVersions(XElement node, string defaultVersion = null)
        {
            List<string> versions = new List<string>();

            if (defaultVersion != null)
            {
                versions.Add(defaultVersion);
            }

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

            return versions;
        }

        protected void SetDefaultNamespace(XDocument doc)
		{
			doc.Root.Attribute("xmlns")?.Remove();

			doc.Root.Name = TNS_Namespace + doc.Root.Name.LocalName;

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
			return doc.XPathSelectElement(XMLUtils.QueryLocalName("Vehicle", "Components")) == null;
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
            if (!doc.DescendantNodes().OfType<XComment>().Any(x => x.Value == CREATED_BY))
            {
                doc.Root.AddBeforeSelf(new XComment(CREATED_BY));
            }
		}

		protected void ChangeValueOfSchemaVersionAttribute(XDocument doc)
		{
			doc.Root.SetAttributeValue("schemaVersion", "2.0");
		}

		protected void SetNamespaceTNS(XDocument doc)
		{
			doc.Root.SetAttributeValue(XNamespace.Xmlns + "tns", TNS_Namespace);
		}

        protected abstract void ConvertEngine(XDocument doc);

        protected abstract void ConvertGearbox(XDocument doc);

        protected abstract void ConvertRetarder(XDocument doc);

        protected abstract void ConvertAxleWheels(XDocument doc);

        protected abstract void ConvertAxlegear(XDocument doc);

        protected abstract void ConvertAuxiliaries(XDocument doc);

        protected abstract void ConvertAirDrag(XDocument doc);

        protected abstract void ConvertTorqueLimits(XDocument doc);

        protected abstract void ConvertTorqueConverter(XDocument doc);

        protected abstract void ConvertAngleDrive(XDocument doc);

        protected abstract void ConvertPTO(XDocument doc);

        protected abstract void ConvertADAS(XDocument doc);

        protected abstract void ConvertComponentsTopElement(XDocument doc);

        protected abstract void ConvertHEVProperties(XDocument doc);

        protected abstract void ConvertElectricMotorTorqueLimits(XDocument doc);

        protected abstract void ConvertBoostingLimitations(XDocument doc);
    }

	public abstract class AbstractTargetV2_4 : AbstractXMLJobConverterSingle
	{
        public override string TargetVersion => XMLUtils.V2_4;

        public override XNamespace TNS_Namespace => XMLDeclarationNamespaces.Tns_v20;

        public override string VehicleNamespaceVersion => XMLUtils.V2_0;

        protected override void SetVehicleType(XDocument doc, bool isExempted)
        {
            XMLUtils.SetElementsType(
                doc,
                "Vehicle",
                isExempted
                    ? "Vehicle_Exempted_HeavyLorryDeclarationType"
                    : "Vehicle_Conventional_HeavyLorryDeclarationType"
            );
        }

        protected override void ConvertVehicleBase(XDocument doc, bool isExempted)
        {
            XMLUtils.SetElementNamespace(doc, "Vehicle", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{VehicleNamespaceVersion}");

            SetVehicleType(doc, isExempted);

            XMLUtils.SetElementsChildrenNamespace(doc, "Vehicle", TargetVersion);
            XMLUtils.SetElementsAttribute(doc, "Vehicle", "xmlns", null);

            XMLUtils.SetElementName(doc, "Vehicle/LegislativeClass", "LegislativeCategory", TargetNamespace);
            XMLUtils.SetElementName(doc, "Vehicle/VehicleCategory", "ChassisConfiguration", TargetNamespace);
            XMLUtils.FixElementsValue(doc, "Vehicle/ChassisConfiguration");

            XMLUtils.SetElementName(doc, "Vehicle/CurbMassChassis", "CorrectedActualMass", TargetNamespace);
            XMLUtils.SetElementName(doc, "Vehicle/GrossVehicleMass", "TechnicalPermissibleMaximumLadenMass", TargetNamespace);
            XMLUtils.AddElementAfter(doc, "Vehicle/ChassisConfiguration", "AxleConfiguration", CalculateAxleConfiguration(doc));
        }

        protected static string CalculateAxleConfiguration(XDocument doc)
        {
            var axles = XMLUtils.GetElements(doc, "AxleWheels/Data/Axles/Axle");

            var drivenAxlesCount = axles.Count(x => x.XPathSelectElement(XMLUtils.QueryLocalName("AxleType")).Value == "VehicleDriven");

            return (axles.Count() > 0) ? $"{axles.Count() * 2}x{drivenAxlesCount * 2}" : "4x2";
        }

        protected override XDocument ConvertExemptedVehicle(XDocument doc)
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

        protected override void ConvertEngine(XDocument doc)
        {
            var dataNode = doc.XPathSelectElement(XMLUtils.QueryLocalName("Vehicle/Components/Engine/Data".Split('/')));

            if (dataNode == null)
            {
                return;
            }

            string version = SelectHighestVersion(dataNode);

            XMLUtils.SetElementsType(doc, "Vehicle/Components/Engine/Data", "EngineDataDeclarationType");
            XMLUtils.SetElementsNamespace(doc, "Vehicle/Components/Engine/Data", XMLUtils.V2_0);
            XMLUtils.SetElementsDescendantsNamespace(doc, "Vehicle/Components/Engine/Data", version);
            XMLUtils.SetElementsAttribute(doc, "Vehicle/Components/Engine/Data", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{version}");
            XMLUtils.SetElementsNamespace(doc, "Vehicle/Components/Engine/Signature", XMLUtils.V2_0);
        }

        protected override void ConvertGearbox(XDocument doc)
        {
            var dataNode = doc.XPathSelectElement(XMLUtils.QueryLocalName("Gearbox/Data".Split('/')));

            if (dataNode == null)
            {
                return;
            }

            string version = SelectHighestVersion(dataNode);
            
            XMLUtils.SetElementsType(doc, "Gearbox/Data", "GearboxDataDeclarationType");
            XMLUtils.SetElementsNamespace(doc, "Gearbox/Data", XMLUtils.V2_0);
            XMLUtils.SetElementsChildrenNamespace(doc, "Gearbox/Data", version);
            XMLUtils.SetElementsAttribute(doc, "Gearbox/Data", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{version}");
            XMLUtils.SetElementsType(doc, "Gearbox/Data/Gears", "GearsDeclarationType");
            XMLUtils.SetElementsDescendantsNamespace(doc, "Gearbox/Data/Gears", XMLUtils.V2_0);
            XMLUtils.SetElementsNamespace(doc, "Gearbox/Signature", XMLUtils.V2_0);
        }

        protected override void ConvertRetarder(XDocument doc)
        {
            XMLUtils.SetElementsType(doc, "Retarder/Data", "RetarderDataDeclarationType");
            XMLUtils.SetElementsNamespace(doc, "Retarder/Data", XMLUtils.V2_0);
            XMLUtils.SetElementsDescendantsNamespace(doc, "Retarder/Data", XMLUtils.V2_0);
            XMLUtils.SetElementsAttribute(doc, "Retarder/Data", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{XMLUtils.V2_0}");
            XMLUtils.SetElementsNamespace(doc, "Retarder/Signature", XMLUtils.V2_0);
        }

        protected override void ConvertAxleWheels(XDocument doc)
        {
            XMLUtils.SetElementsType(doc, "AxleWheels/Data", "AxleWheelsDataDeclarationType");
            XMLUtils.SetElementsNamespace(doc, "AxleWheels/Data", XMLUtils.V2_0);
            XMLUtils.SetElementsDescendantsNamespace(doc, "AxleWheels/Data", XMLUtils.V2_0);
            XMLUtils.SetElementsAttribute(doc, "AxleWheels/Data", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{XMLUtils.V2_0}");
            XMLUtils.SetElementsType(doc, "AxleWheels/Data/Axles/Axle", "AxleDataDeclarationType");
            XMLUtils.SetElementsNamespace(doc, "AxleWheels/Signature", XMLUtils.V2_0);

            ConvertTyres(doc);
        }

        protected virtual void ConvertTyres(XDocument doc)
        {
            XMLUtils.SetElementsType(doc, "Tyre/Data", "TyreDataDeclarationType");

            var nodes = doc.XPathSelectElements(XMLUtils.QueryLocalName("Tyre/Data".Split('/')));

            foreach (var node in nodes)
            {
                var tyreVersion = SelectHighestVersion(node);

                node.SetAttributeValue("xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{tyreVersion}");

                XMLUtils.SetElementDescendantsNamespace(node, tyreVersion);
            }
        }

        protected override void ConvertAxlegear(XDocument doc)
        {
            XMLUtils.SetElementsType(doc, "Axlegear/Data", "AxlegearDataDeclarationType");
            XMLUtils.SetElementsNamespace(doc, "Axlegear/Data", XMLUtils.V2_0);
            XMLUtils.SetElementsDescendantsNamespace(doc, "Axlegear/Data", XMLUtils.V2_0);
            XMLUtils.SetElementsAttribute(doc, "Axlegear/Data", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{XMLUtils.V2_0}");
            XMLUtils.SetElementsNamespace(doc, "Axlegear/Signature", XMLUtils.V2_0);
        }

        protected override void ConvertAuxiliaries(XDocument doc)
        {
            XMLUtils.SetElementsDescendantsNamespace(doc, "Vehicle/Components/Auxiliaries", TargetVersion);
            XMLUtils.SetElementsAttribute(doc, "Vehicle/Components/Auxiliaries", "xmlns", null);
            XMLUtils.SetElementsType(doc, "Vehicle/Components/Auxiliaries", null);
            XMLUtils.SetElementsType(doc, "Vehicle/Components/Auxiliaries/Data", "AUX_Conventional_LorryDataType");
            XMLUtils.SetElementsAttribute(doc, "Vehicle/Components/Auxiliaries/Data/SteeringPump/Technology", "axleNumber", "1");
            XMLUtils.FixElementsValue(doc, "Vehicle/Components/Auxiliaries/Data/SteeringPump/Technology");
        }

        protected override void ConvertAirDrag(XDocument doc)
        {
            XMLUtils.SetElementsType(doc, "AirDrag/Data", "AirDragDataDeclarationType");
            XMLUtils.SetElementsNamespace(doc, "AirDrag/Data", XMLUtils.V2_0);
            XMLUtils.SetElementsDescendantsNamespace(doc, "AirDrag/Data", XMLUtils.V2_0);
            XMLUtils.SetElementsAttribute(doc, "AirDrag/Data", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{XMLUtils.V2_0}");
            XMLUtils.SetElementsNamespace(doc, "AirDrag/Signature", XMLUtils.V2_0);
        }

        protected override void ConvertTorqueConverter(XDocument doc)
        {
            XMLUtils.SetElementsNamespace(doc, "TorqueConverter", TargetVersion);
            XMLUtils.SetElementsType(doc, "TorqueConverter/Data", "TorqueConverterDataDeclarationType");
            XMLUtils.SetElementsNamespace(doc, "TorqueConverter/Data", XMLUtils.V2_0);
            XMLUtils.SetElementsDescendantsNamespace(doc, "TorqueConverter/Data", XMLUtils.V2_0);
            XMLUtils.SetElementsAttribute(doc, "TorqueConverter/Data", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{XMLUtils.V2_0}");
            XMLUtils.SetElementsNamespace(doc, "TorqueConverter/Signature", XMLUtils.V2_0);
        }

        protected override void ConvertAngleDrive(XDocument doc)
        {
            XMLUtils.SetElementsType(doc, "Angledrive/Data", "AngledriveDataDeclarationType");
            XMLUtils.SetElementsNamespace(doc, "Angledrive/Data", XMLUtils.V2_0);
            XMLUtils.SetElementsDescendantsNamespace(doc, "Angledrive/Data", XMLUtils.V2_0);
            XMLUtils.SetElementsAttribute(doc, "Angledrive/Data", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{XMLUtils.V2_0}");
            XMLUtils.SetElementsNamespace(doc, "Angledrive/Signature", XMLUtils.V2_0);
        }

        protected override void ConvertPTO(XDocument doc)
        {
            XMLUtils.SetElementsType(doc, "Vehicle/PTO", "PTOType");
            XMLUtils.SetElementsAttribute(doc, "Vehicle/PTO", "xmlns", null);
            XMLUtils.SetElementNamespace(doc, "Vehicle/PTO", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{TargetVersion}");
            XMLUtils.SetElementsDescendantsNamespace(doc, "Vehicle/PTO", TargetVersion);
        }

        protected override void ConvertADAS(XDocument doc)
        {
            XMLUtils.SetElementsType(doc, "Vehicle/ADAS", "ADAS_Conventional_Type");
            XMLUtils.SetElementNamespace(doc, "Vehicle/ADAS", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{TargetVersion}");
            XMLUtils.SetElementsDescendantsNamespace(doc, "Vehicle/ADAS", TargetVersion);
        }

        protected override void ConvertComponentsTopElement(XDocument doc)
        {
            XMLUtils.SetElementsType(doc, "Vehicle/Components", "Components_Conventional_LorryType");
            XMLUtils.SetElementsChildrenNamespace(doc, "Vehicle/Components", TargetVersion);
        }

        protected override void ConvertTorqueLimits(XDocument doc)
        {
            XMLUtils.SetElementsType(doc, "Vehicle/TorqueLimits", "TorqueLimitsType");
            XMLUtils.SetElementsNamespace(doc, "Vehicle/TorqueLimits", TargetVersion);
            XMLUtils.SetElementsDescendantsNamespace(doc, "Vehicle/TorqueLimits", XMLUtils.V2_0);
            XMLUtils.SetElementsAttribute(doc, "Vehicle/TorqueLimits", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{XMLUtils.V2_0}");
        }

        protected override void ConvertHEVProperties(XDocument doc)
        {
            throw new NotImplementedException();
        }

        protected override void ConvertElectricMotorTorqueLimits(XDocument doc)
        {
            throw new NotImplementedException();
        }

        protected override void ConvertBoostingLimitations(XDocument doc)
        {
            throw new NotImplementedException();
        }

        protected override XDocument ConvertNonExemptedVehicle(XDocument doc)
        {
            ConvertPTO(doc);
            ConvertADAS(doc);
            ConvertTorqueLimits(doc);
            ConvertComponentsTopElement(doc);
            ConvertEngine(doc);
            ConvertGearbox(doc);
            ConvertTorqueConverter(doc);
            ConvertAngleDrive(doc);
            ConvertRetarder(doc);
            ConvertAxlegear(doc);
            ConvertAxleWheels(doc);
            ConvertAuxiliaries(doc);
            ConvertAirDrag(doc);

            return doc;
        }
    }

    public class XMLJobConverter_v1_0_To_v2_4 : AbstractTargetV2_4
    {
		public override string SourceVersion => XMLUtils.V1_0;

		protected override void ConvertTorqueConverter(XDocument doc)
		{
			XMLUtils.MoveElementUp(doc, "Gearbox/TorqueConverter");
			base.ConvertTorqueConverter(doc);
		}
	}

	public class XMLJobConverter_v2_0_To_v2_4 : AbstractTargetV2_4
    {
		public override string SourceVersion => XMLUtils.V2_0;

		protected override XDocument ConvertNonExemptedVehicle(XDocument doc)
		{
			base.ConvertNonExemptedVehicle(doc);

			XMLUtils.AddElementAfter(doc, "Vehicle/PTO", "ZeroEmissionVehicle", "false");
			XMLUtils.AddElementAfter(doc, "Vehicle/ZeroEmissionVehicle", "VocationalVehicle", "false");
			XMLUtils.AddElementAfter(doc, "Vehicle/VocationalVehicle", "SleeperCab", "true");

            if (XMLUtils.GetElements(doc, "Mode/Fuel").Count() > 1)
            {
				XMLUtils.AddElementAfter(doc, "Vehicle/VocationalVehicle", "NgTankSystem", "Compressed");
            }

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

	public class XMLJobConverter_v2_1_To_v2_4 : AbstractTargetV2_4
    {
		public override string SourceVersion => XMLUtils.V2_1;

		protected override void AddNamespaces(XDocument doc)
		{
			base.AddNamespaces(doc);
			doc.Root.SetAttributeValue(XNamespace.Xmlns + SourceVersion, $"{XMLDeclarationNamespaces.DeclarationDefinition}:{SourceVersion}");
		}
	}

	public class XMLJobConverter_v2_2_1_To_v2_4 : AbstractTargetV2_4
    {
		public override string SourceVersion => XMLUtils.V2_2_1;

		public override string TargetVersion => XMLUtils.V2_4;

		protected override void AddNamespaces(XDocument doc)
		{
			base.AddNamespaces(doc);
			doc.Root.SetAttributeValue(XNamespace.Xmlns + SourceVersion, $"{XMLDeclarationNamespaces.DeclarationDefinition}:{SourceVersion}");
		}
	}

	public class XMLJobConverter_v2_4_To_v2_7 : AbstractXMLJobConverterSingle
	{
        private static Dictionary<string, string> _componentsTypes = new Dictionary<string, string>();
        private static List<string> _PEVTypes = new List<string>();

        public override string SourceVersion => XMLUtils.V2_4;

        public override string TargetVersion => XMLUtils.V2_7;

        static XMLJobConverter_v2_4_To_v2_7()
        {
            _componentsTypes["Components_HEV-S2_LorryType"] = "Components_HEV-S2_LorryType";
            _componentsTypes["Components_HEV-S3_LorryType"] = "Components_HEV-S3_LorryType";
            _componentsTypes["Components_HEV-S4_LorryType"] = "Components_HEV-S4_LorryType";
            _componentsTypes["Components_HEV-IEPC-S_LorryType"] = "Components_HEV-IEPC-S_LorryType";
            _componentsTypes["Components_PEV-E2_LorryType"] = "Components_PEV-E2_LorryType";
            _componentsTypes["Components_PEV-E3_LorryType"] = "Components_PEV-E3_LorryType";
            _componentsTypes["Components_PEV-E4_LorryType"] = "Components_PEV-E4_LorryType";
            _componentsTypes["Components_HEV-S2_PrimaryBusType"] = "Components_HEV-S2_PrimaryBusType";
            _componentsTypes["Components_HEV-S3_PrimaryBusType"] = "Components_HEV-S3_PrimaryBusType";
            _componentsTypes["Components_HEV-S4_PrimaryBusType"] = "Components_HEV-S4_PrimaryBusType";
            _componentsTypes["Components_HEV-IEPC-S_PrimaryBusType"] = "Components_HEV-IEPC-S_PrimaryBusType";
            _componentsTypes["Components_PEV-E2_PrimaryBusType"] = "Components_PEV-E2_PrimaryBusType";
            _componentsTypes["Components_PEV-E3_PrimaryBusType"] = "Components_PEV-E3_PrimaryBusType";
            _componentsTypes["Components_PEV-E4_PrimaryBusType"] = "Components_PEV-E4_PrimaryBusType";

            _PEVTypes.Add("Vehicle_IEPC_HeavyLorryDeclarationType");
            _PEVTypes.Add("Vehicle_PEV_HeavyLorryDeclarationType");
            _PEVTypes.Add("Vehicle_IEPC_MediumLorryDeclarationType");
            _PEVTypes.Add("Vehicle_PEV_MediumLorryDeclarationType");
            _PEVTypes.Add("Vehicle_IEPC_PrimaryBusDeclarationType");
            _PEVTypes.Add("Vehicle_PEV_PrimaryBusDeclarationType");
        }

        protected override void AddNamespaces(XDocument doc)
        {
            doc.Root.SetAttributeValue(XNamespace.Xmlns + SourceVersion, $"{XMLDeclarationNamespaces.DeclarationDefinition}:{SourceVersion}");
        }

        protected override void ConvertVehicleBase(XDocument doc, bool isExempted)
        {
            SetVehicleType(doc, isExempted);

            XMLUtils.SetElementsAttribute(doc, "Vehicle", "xmlns", null);

            XMLUtils.SetElementNamespace(doc, "Vehicle", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{VehicleNamespaceVersion}");

            XMLUtils.SetElementsChildrenNamespace(doc, "Vehicle", TargetVersion);
            
            XMLUtils.AddElementAfter(doc, "Vehicle/Date", "SimulationToolLicenseNumber", "x");
        }

        private static string GetComponentsType(XDocument doc)
        {
            var componentsNode = doc.XPathSelectElement(XMLUtils.QueryLocalName("Vehicle/Components".Split('/')));

            var oldType = componentsNode?.Attribute(XMLDeclarationNamespaces.Xsi + XMLNames.XSIType)?.Value.Split(':').Last();

            return ((oldType != null) && _componentsTypes.ContainsKey(oldType)) ? _componentsTypes[oldType] : null;
        }

        protected override void ConvertPTO(XDocument doc)
        {
            var vehicleType = doc.XPathSelectElement(XMLUtils.QueryLocalName("Vehicle")).Attribute(XMLDeclarationNamespaces.Xsi + XMLNames.XSIType).Value;

            if (vehicleType.IsOneOf("Vehicle_HEV-IEPC-S_HeavyLorryDeclarationType", "Vehicle_IEPC_HeavyLorryDeclarationType"))
            {
                XMLUtils.DeleteElement(doc, "Vehicle/PTO");
            }
            else
            {
                XMLUtils.SetElementsAttribute(doc, "Vehicle/PTO", XMLDeclarationNamespaces.Xsi + XMLNames.XSIType, null);
                XMLUtils.SetElementsDescendantsNamespace(doc, "Vehicle/PTO", TargetVersion);
                XMLUtils.SetElementsAttribute(doc, "Vehicle/PTO", "xmlns", null);
            }
        }

        protected override void ConvertADAS(XDocument doc)
        {
            XMLUtils.SetElementsAttribute(doc, "Vehicle/ADAS", XMLDeclarationNamespaces.Xsi + XMLNames.XSIType, null);
            XMLUtils.SetElementsDescendantsNamespace(doc, "Vehicle/ADAS", TargetVersion);
            XMLUtils.SetElementsAttribute(doc, "Vehicle/ADAS", "xmlns", null);

            var isVehicleOVCHV = XMLUtils.GetElements(doc, "Vehicle/OvcHev").All(x => Boolean.Parse(x.Value));
            if (isVehicleOVCHV)
            {
                XMLUtils.SetElementsValue(doc, "Vehicle/ADAS/EngineStopStart", "true");
            }
        }

        protected override void ConvertComponentsTopElement(XDocument doc)
        {
            XMLUtils.SetElementsAttribute(doc, "Vehicle/Components", XMLDeclarationNamespaces.Xsi + XMLNames.XSIType, GetComponentsType(doc));
            XMLUtils.SetElementsChildrenNamespace(doc, "Vehicle/Components", TargetVersion);
        }

        protected override void ConvertAuxiliaries(XDocument doc)
        {
            XMLUtils.SetElementsDescendantsNamespace(doc, "Vehicle/Components/Auxiliaries", TargetVersion);
            XMLUtils.SetElementsAttribute(doc, "Vehicle/Components/Auxiliaries", XMLDeclarationNamespaces.Xsi + XMLNames.XSIType, null);
            XMLUtils.SetElementsAttribute(doc, "Vehicle/Components/Auxiliaries/Data", XMLDeclarationNamespaces.Xsi + XMLNames.XSIType, null);
            XMLUtils.ConvertDashToHyphen(doc, "BatteryTechnology");
        }

        protected override void ConvertHEVProperties(XDocument doc)
        {
            XMLUtils.SetElementName(doc, "Vehicle/OvcHev", "OVC", TargetNamespace);
            XMLUtils.DeleteElement(doc, "Vehicle/MaxChargingPower");

            var vehicleType = doc.XPathSelectElement(XMLUtils.QueryLocalName("Vehicle")).Attribute(XMLDeclarationNamespaces.Xsi + XMLNames.XSIType).Value;
            var isVehiclePEV = _PEVTypes.Contains(vehicleType);
            
            if (isVehiclePEV)
            {
                XMLUtils.AddElementAfter(doc, "Vehicle/ArchitectureID", "OVC", "true");
            }

            XMLUtils.AddElementAfter(doc, "Vehicle/OVC", "BatteryOnlyMode", isVehiclePEV.ToString().ToLower());
            XMLUtils.AddElementAfter(doc, "Vehicle/BatteryOnlyMode", "DynamicChargingTechnology", "None");
        }

        protected override void ConvertTorqueLimits(XDocument doc)
        {
            XMLUtils.SetElementsAttribute(doc, "Vehicle/TorqueLimits", XMLDeclarationNamespaces.Xsi + XMLNames.XSIType, null);
            XMLUtils.SetElementsDescendantsNamespace(doc, "Vehicle/TorqueLimits", TargetVersion);
            XMLUtils.SetElementsAttribute(doc, "Vehicle/TorqueLimits", "xmlns", null);
        }

        protected override void ConvertElectricMotorTorqueLimits(XDocument doc)
        {
            XMLUtils.SetElementsAttribute(doc, "Vehicle/ElectricMotorTorqueLimits", XMLDeclarationNamespaces.Xsi + XMLNames.XSIType, null);
            XMLUtils.SetElementsDescendantsNamespace(doc, "Vehicle/ElectricMotorTorqueLimits", TargetVersion);
            XMLUtils.SetElementsAttribute(doc, "Vehicle/ElectricMotorTorqueLimits", "xmlns", null);
        }

        protected override void ConvertBoostingLimitations(XDocument doc)
        {
            XMLUtils.SetElementsAttribute(doc, "Vehicle/BoostingLimitations", XMLDeclarationNamespaces.Xsi + XMLNames.XSIType, null);
            XMLUtils.SetElementsDescendantsNamespace(doc, "Vehicle/BoostingLimitations", TargetVersion);
            XMLUtils.SetElementsAttribute(doc, "Vehicle/BoostingLimitations", "xmlns", null);
        }

        protected override XDocument ConvertNonExemptedVehicle(XDocument doc)
        {
            var vehicleType = doc.XPathSelectElement(XMLUtils.QueryLocalName("Vehicle")).Attribute(XMLDeclarationNamespaces.Xsi + XMLNames.XSIType).Value;
            var isVehiclePEV = _PEVTypes.Contains(vehicleType);
            var isVehicleOVCHV = XMLUtils.GetElements(doc, "Vehicle/OvcHev").All(x => Boolean.Parse(x.Value));

            ConvertPTO(doc);
            ConvertADAS(doc);
            ConvertHEVProperties(doc);
            ConvertTorqueLimits(doc);
            ConvertElectricMotorTorqueLimits(doc);
            ConvertBoostingLimitations(doc);
            ConvertComponentsTopElement(doc);
            ConvertEngine(doc);
            ConvertElectricMachine(doc);
            ConvertElectricMachineGen(doc);
            ConvertIEPC(doc);
            ConvertBatteries(doc, isVehiclePEV, isVehicleOVCHV);
            ConvertCapacitor(doc);
            ConvertGearbox(doc);
            ConvertTorqueConverter(doc);
            ConvertAngleDrive(doc);
            ConvertRetarder(doc);
            ConvertAxlegear(doc);
            ConvertAxleWheels(doc);
            ConvertAirDrag(doc);
            ConvertAuxiliaries(doc);

            return doc;
        }

        protected void ConvertComponent(XDocument doc, string component)
        {
            var node = doc.XPathSelectElement(XMLUtils.QueryLocalName($"Vehicle/Components/{component}".Split('/')));
            if (node == null) 
            {
                return;
            }

            var dataNode = node.XPathSelectElement(XMLUtils.QueryLocalName("Data"));

            var dataNamespace = XMLUtils.GetDefaultNamespace(dataNode);
            var typeNamespace = XMLUtils.GetTypeNamespaceOfElement(doc, dataNode);

            XMLUtils.ResetTypeNamespace(dataNode, typeNamespace);
            dataNode.SetAttributeValue("xmlns", dataNamespace);

            XMLUtils.SetElementsAttribute(doc, $"Vehicle/Components/{component}", "xmlns", null);
            XMLUtils.SetElementsAttribute(doc, $"Vehicle/Components/{component}", XMLUtils.TypeAttribute, null);
        }

        protected override void ConvertEngine(XDocument doc)
        {
            ConvertComponent(doc, "Engine");
        }

        protected override void ConvertGearbox(XDocument doc)
        {
            ConvertComponent(doc, "Gearbox");
        }

        protected override void ConvertRetarder(XDocument doc)
        {
            var componentsNode = doc.XPathSelectElement(XMLUtils.QueryLocalName("Vehicle/Components".Split('/')));
            var componentsType = componentsNode.Attribute(XMLDeclarationNamespaces.Xsi + XMLNames.XSIType)?.Value;

            if (componentsType.IsOneOf(
                "Components_HEV-S4_LorryType",
                "Components_PEV-E4_LorryType",
                "Components_HEV-S4_PrimaryBusType",
                "Components_PEV-E4_PrimaryBusType"))
            {
                XMLUtils.DeleteElement(doc, "Vehicle/Components/Retarder");
            }
            else
            {
                ConvertComponent(doc, "Retarder");
            }
        }

        protected override void ConvertAngleDrive(XDocument doc)
        {
            ConvertComponent(doc, "Angledrive");
        }

        protected override void ConvertAxlegear(XDocument doc)
        {
            ConvertComponent(doc, "Axlegear");
        }

        protected override void ConvertAxleWheels(XDocument doc)
        {
            ConvertComponent(doc, "AxleWheels");
        }

        protected override void ConvertAirDrag(XDocument doc)
        {
            ConvertComponent(doc, "AirDrag");
        }

        protected override void ConvertTorqueConverter(XDocument doc)
        {
            ConvertComponent(doc, "TorqueConverter");
        }

        public void ConvertBatteries(XDocument doc, bool isVehiclePEV, bool isVehicleOVCHV)
        {
            XMLUtils.SetElementsChildrenNamespace(doc, "Components/ElectricEnergyStorage", TargetVersion);
            XMLUtils.SetElementsChildrenNamespace(doc, "Components/ElectricEnergyStorage/Battery", TargetVersion);

            var dataNodes = doc.XPathSelectElements(XMLUtils.QueryLocalName("Components/ElectricEnergyStorage/Battery/REESS/Data".Split('/')));

            foreach (var dataNode in dataNodes)
            {
                var typeNamespace = XMLUtils.GetTypeNamespaceOfElement(doc, dataNode);
                
                XMLUtils.ResetTypeNamespace(dataNode, typeNamespace);

                dataNode.SetAttributeValue("xmlns", null);

                dataNode.Parent.SetAttributeValue("xmlns", null);

                XMLUtils.SetNodeNamespace(dataNode, $"{XMLDeclarationNamespaces.DeclarationDefinition}:{XMLUtils.V2_3}");

                XMLUtils.SetElementDescendantsNS(dataNode, typeNamespace);
                
                XMLUtils.SetElementNamespace(dataNode.Parent.XPathSelectElement(XMLUtils.QueryLocalName("Signature")), XMLUtils.V2_3);
            }

            ConvertBatterySoCBounds(doc, isVehiclePEV, isVehicleOVCHV);
        }

        protected void ConvertBatterySoCBounds(XDocument doc, bool isVehiclePEV, bool isVehicleOVCHV)
        {
            var batNodes = doc.XPathSelectElements(XMLUtils.QueryLocalName("Components/ElectricEnergyStorage/Battery".Split('/')));

            var boundsSoC = new Dictionary<string, string>()
            {
                { "SOCmin", "SOCMin" },
                { "SOCmax", "SOCMax" }
            };

            var genericSOC = new GenericSOC();

            foreach (var batNode in batNodes)
            {
                foreach (var bound in boundsSoC)
                {
                    var node = batNode.XPathSelectElement(XMLUtils.QueryLocalName(bound.Key));

                    if (node != null)
                    {
                        node.Value = double.Parse(node.Value).ToXMLFormat(1);
                    }
                    else
                    {
                        if (isVehiclePEV || isVehicleOVCHV)
                        {
                            var genericSoCData = genericSOC.Lookup(
                                isVehiclePEV ? VectoSimulationJobType.BatteryElectricVehicle : VectoSimulationJobType.ParallelHybridVehicle,
                                true);

                            double nodeValue = double.Parse(genericSoCData.GetType().GetField(bound.Value).GetValue(genericSoCData).ToString()) * 100;

                            batNode.Add(new XElement(batNode.Name.Namespace + bound.Key, nodeValue.ToXMLFormat(1)));
                        }
                    }
                }
            }
        }

        public void ConvertCapacitor(XDocument doc)
        {
            XMLUtils.SetElementsChildrenNamespace(doc, "Components/ElectricEnergyStorage", TargetVersion);
            XMLUtils.SetElementsChildrenNamespace(doc, "Components/ElectricEnergyStorage/Capacitor", TargetVersion);

            var dataNodes = doc.XPathSelectElements(XMLUtils.QueryLocalName("Components/ElectricEnergyStorage/Capacitor/Data".Split('/')));

            foreach (var dataNode in dataNodes)
            {
                var typeNamespace = XMLUtils.GetTypeNamespaceOfElement(doc, dataNode);

                XMLUtils.ResetTypeNamespace(dataNode, typeNamespace);

                dataNode.SetAttributeValue("xmlns", null);

                dataNode.Parent.SetAttributeValue("xmlns", null);

                XMLUtils.SetNodeNamespace(dataNode, $"{XMLDeclarationNamespaces.DeclarationDefinition}:{XMLUtils.V2_4}");

                XMLUtils.SetElementDescendantsNS(dataNode, typeNamespace);

                XMLUtils.SetElementNamespace(dataNode.Parent.XPathSelectElement(XMLUtils.QueryLocalName("Signature")), XMLUtils.V2_4);
            }
        }

        public void ConvertElectricMachine(XDocument doc)
        {
            ConvertElectricMachineComponent(doc, "ElectricMachine");
        }

        public void ConvertElectricMachineGen(XDocument doc)
        {
            ConvertElectricMachineComponent(doc, "ElectricMachineGEN");
        }

        protected void ConvertElectricMachineComponent(XDocument doc, string component)
        {
            var dataNode = doc.XPathSelectElement(XMLUtils.QueryLocalName($"Components/{component}/ElectricMachineSystem/Data".Split('/')));

            if (dataNode == null)
            {
                return;
            }

            string version = dataNode.Name.NamespaceName.Split(':').Last();

            XMLUtils.SetElementsChildrenNamespace(doc, $"Components/{component}", TargetVersion);
            XMLUtils.SetElementsChildrenNamespace(doc, $"Components/{component}/P2.5GearRatios", TargetVersion);

            XMLUtils.SetElementsAttribute(doc, $"Components/{component}/ElectricMachineSystem", "xmlns", null);
            XMLUtils.SetElementsAttribute(doc, $"Components/{component}/ElectricMachineSystem/Data", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{version}");
            XMLUtils.SetElementsNamespace(doc, $"Components/{component}/ElectricMachineSystem/Signature", version);

            XMLUtils.SetElementsAttribute(doc, $"Components/{component}/ADC", "xmlns", null);
            XMLUtils.SetElementsAttribute(doc, $"Components/{component}/ADC/Data", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{version}");
            XMLUtils.SetElementsNamespace(doc, $"Components/{component}/ADC/Signature", version);
        }

        protected void ConvertIEPC(XDocument doc)
        {
            string version = XMLUtils.V2_3;

            XMLUtils.SetElementsAttribute(doc, $"Components/IEPC", "xmlns", null);
            XMLUtils.SetElementsAttribute(doc, $"Components/IEPC/Data", "xmlns", $"{XMLDeclarationNamespaces.DeclarationDefinition}:{version}");
            XMLUtils.SetElementsNamespace(doc, $"Components/IEPC/Signature", version);
        }

        public override XNamespace TNS_Namespace => XMLDeclarationNamespaces.Tns_v30;

        public override string VehicleNamespaceVersion => TargetVersion;

        protected override void SetVehicleType(XDocument doc, bool isExempted)
        {
            var vehicle = doc.XPathSelectElement(XMLUtils.QueryLocalName("Vehicle"));
            var type = vehicle.Attribute(XMLUtils.TypeAttribute).Value;
            
            if (type.Contains("Vehicle_IEPC_CompletedBusDeclarationType"))
            {
                XMLUtils.SetElementsType(doc, "Vehicle", "Vehicle_PEV_CompletedBusDeclarationType");
            }
            else
            {
                var noPrefixType = type.Split(':').Last();
                XMLUtils.SetElementsType(doc, "Vehicle", noPrefixType);
            }
        }

        protected override XDocument ConvertExemptedVehicle(XDocument doc)
        {
            return doc;
        }
    }

    public class XMLJobCompositeConverter<T1, T2> : AbstractXMLJobConverter 
        where T1 : AbstractXMLJobConverterSingle, new() 
        where T2 : AbstractXMLJobConverterSingle, new()
    {
        private T1 _firstConverter;
        private T2 _secondConverter;

        public override string SourceVersion => _firstConverter.SourceVersion;

        public override string TargetVersion => _secondConverter.TargetVersion;

        public XMLJobCompositeConverter()
        {
            _firstConverter = new T1();
            _secondConverter = new T2();
        }

        public override ErrorOr<XDocument> Convert(XDocument source)
        {
            var firstResult = _firstConverter.Convert(source);

            if (firstResult.IsError)
            {
                return firstResult;
            }

            return firstResult.IsError ? firstResult : _secondConverter.Convert(firstResult.Value); 
        }
    }

}

