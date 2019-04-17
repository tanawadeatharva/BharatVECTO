using System.IO;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	internal class XMLEngineeringLookaheadDataWriterV10 : AbstractXMLWriter, IXMLLookaheadDataWriter
	{
		private XNamespace _componentDataNamespace;

		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public XMLEngineeringLookaheadDataWriterV10() : base("LookAheadCoastingEngineeringType") { }

		#region Implementation of IXMLEngineeringComponentWriter

		public override object[] WriteXML(IDriverModelData inputData)
		{
			var lookahead = inputData as ILookaheadCoastingInputData;
			var ns = ComponentDataNamespace;
			if (lookahead == null) {
				return new object[] { };
			}

			return new object[] {
				new XElement(ns + XMLNames.DriverModel_LookAheadCoasting_Enabled, lookahead.Enabled),
				new XElement(ns + XMLNames.DriverModel_LookAheadCoasting_MinSpeed, lookahead.MinSpeed.AsKmph),
				new XElement(ns + XMLNames.DriverModel_LookAheadCoasting_PreviewDistanceFactor, lookahead.LookaheadDistanceFactor),
				new XElement(
					ns + XMLNames.DriverModel_LookAheadCoasting_DecisionFactorOffset,
					lookahead.CoastingDecisionFactorOffset),
				new XElement(
					ns + XMLNames.DriverModel_LookAheadCoasting_DecisionFactorScaling,
					lookahead.CoastingDecisionFactorScaling),
				lookahead.CoastingDecisionFactorTargetSpeedLookup == null
					? null
					: new XElement(
						ns + XMLNames.DriverModel_LookAheadCoasting_SpeedDependentDecisionFactor,
						Writer.Configuration.SingleFile
							? EmbedDataTable(
								lookahead.CoastingDecisionFactorTargetSpeedLookup,
								AttributeMappings.CoastingDFTargetSpeedLookupMapping)
							: ExtCSVResource(
								lookahead.CoastingDecisionFactorTargetSpeedLookup,
								Path.Combine(
									Writer.Configuration.BasePath, Writer.RemoveInvalidFileCharacters("Driver_LAC_TargetspeedLookup.csv")))),
				lookahead.CoastingDecisionFactorVelocityDropLookup == null
					? null
					: new XElement(
						ns + XMLNames.DriverModel_LookAheadCoasting_VelocityDropDecisionFactor,
						Writer.Configuration.SingleFile
							? EmbedDataTable(
								lookahead.CoastingDecisionFactorVelocityDropLookup,
								AttributeMappings.CoastingDFVelocityDropLookupMapping)
							: ExtCSVResource(
								lookahead.CoastingDecisionFactorVelocityDropLookup,
								Path.Combine(
									Writer.Configuration.BasePath, Writer.RemoveInvalidFileCharacters("Driver_LAC_VelocityDropLookup.csv"))))
			};
		}


		public override XNamespace ComponentDataNamespace
		{
			get { return _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI)); }
		}

		#endregion
	}
}
