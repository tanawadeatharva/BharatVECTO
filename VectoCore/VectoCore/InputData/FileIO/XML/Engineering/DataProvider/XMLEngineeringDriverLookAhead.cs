using System.Xml;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringDriverLookAheadV07 : AbstractXMLType, IXMLLookaheadData
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		protected IXMLEngineeringDriverData DriverData;

		public XMLEngineeringDriverLookAheadV07(IXMLEngineeringDriverData driverData, XmlNode node) : base(node)
		{
			DriverData = driverData;
		}

		#region Implementation of ILookaheadCoastingInputData

		public virtual bool Enabled
		{
			get { return GetBool(XMLNames.DriverModel_LookAheadCoasting_Enabled); }
		}

		public virtual MeterPerSecond MinSpeed
		{
			get {
				return GetNode(XMLNames.DriverModel_Overspeed_MinSpeed, required: false)
							?.InnerText.ToDouble().KMPHtoMeterPerSecond() ??
						DeclarationData.Driver.LookAhead.MinimumSpeed;
			}
		}

		public virtual double CoastingDecisionFactorOffset
		{
			get {
				return GetNode(XMLNames.DriverModel_LookAheadCoasting_DecisionFactorOffset, required: false)
							?.InnerText.ToDouble() ??
						DeclarationData.Driver.LookAhead.DecisionFactorCoastingOffset;
			}
		}

		public virtual double CoastingDecisionFactorScaling
		{
			get {
				return GetNode(XMLNames.DriverModel_LookAheadCoasting_DecisionFactorScaling, required: false)
							?.InnerText.ToDouble() ??
						DeclarationData.Driver.LookAhead.DecisionFactorCoastingScaling;
			}
		}

		public virtual double LookaheadDistanceFactor
		{
			get {
				return GetNode(XMLNames.DriverModel_LookAheadCoasting_PreviewDistanceFactor, required: false)
							?.InnerText.ToDouble() ??
						DeclarationData.Driver.LookAhead.LookAheadDistanceFactor;
			}
		}

		public virtual TableData CoastingDecisionFactorTargetSpeedLookup
		{
			get {
				return XMLHelper.ReadEntriesOrResource(
					BaseNode, DriverData.DataSource.SourcePath, XMLNames.DriverModel_LookAheadCoasting_SpeedDependentDecisionFactor,
					XMLNames.LookAheadCoasting_SpeedDependentDecisionFactor_Entry,
					AttributeMappings.CoastingDFTargetSpeedLookupMapping);
			}
		}

		public virtual TableData CoastingDecisionFactorVelocityDropLookup
		{
			get {
				return XMLHelper.ReadEntriesOrResource(
					BaseNode, DriverData.DataSource.SourcePath, XMLNames.DriverModel_LookAheadCoasting_VelocityDropDecisionFactor,
					XMLNames.LookAheadCoasting_VelocityDropDecisionFactor_Entry,
					AttributeMappings.CoastingDFVelocityDropLookupMapping);
			}
		}

		#endregion
	}

	internal class XMLEngineeringDriverLookAheadV10 : XMLEngineeringDriverLookAheadV07
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public XMLEngineeringDriverLookAheadV10(IXMLEngineeringDriverData driverData, XmlNode node) :
			base(driverData, node) { }
	}
}
