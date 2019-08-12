using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringGearshiftDataV07 : AbstractXMLType, IXMLEngineeringGearshiftData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "ShiftStrategyParametersEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		public XMLEngineeringGearshiftDataV07(XmlNode node) : base(node) { }

		#region Implementation of IGearshiftEngineeringInputData

		//public virtual Second TractionInterruption
		//{
		//	get { return GetNode(XMLNames.Gearbox_TractionInterruption)?.InnerText.ToDouble().SI<Second>(); }
		//}

		public virtual Second MinTimeBetweenGearshift
		{
			get {
				return GetNode(XMLNames.DriverModel_ShiftStrategyParameters_TimeBetweenGearshift, required: false)
							?.InnerText.ToDouble().SI<Second>() ?? DeclarationData.Gearbox.MinTimeBetweenGearshifts;
			}
		}

		public virtual double TorqueReserve
		{
			get {
				return GetNode(XMLNames.DriverModel_ShiftStrategyParameters_TorqueReserve, required: false)?.InnerText.ToDouble() ??
						DeclarationData.Gearbox.TorqueReserve;
			}
		}

		public virtual MeterPerSecond StartSpeed
		{
			get {
				return GetNode(XMLNames.DriverModel_ShiftStrategyParameters_StartSpeed, required: false)
							?.InnerText.ToDouble().SI<MeterPerSecond>() ?? DeclarationData.Gearbox.StartSpeed;
			}
		}

		public virtual MeterPerSquareSecond StartAcceleration
		{
			get {
				return GetNode(XMLNames.DriverModel_ShiftStrategyParameters_StartAcceleration, required: false)
							?.InnerText.ToDouble().SI<MeterPerSquareSecond>() ?? DeclarationData.Gearbox.StartAcceleration;
			}
		}

		public virtual double StartTorqueReserve
		{
			get {
				return GetNode(XMLNames.DriverModel_ShiftStrategyParameters_StartTorqueReserve, required: false)
							?.InnerText.ToDouble() ??
						DeclarationData.Gearbox.TorqueReserveStart;
			}
		}

		public virtual Second DownshiftAfterUpshiftDelay
		{
			get {
				return GetNode(XMLNames.DriverModel_ShiftStrategyParameters_DownshiftAfterUpshiftDelay, required: false)
							?.InnerText.ToDouble().SI<Second>() ??
						DeclarationData.Gearbox.DownshiftAfterUpshiftDelay;
			}
		}

		public virtual Second UpshiftAfterDownshiftDelay
		{
			get {
				return GetNode(XMLNames.DriverModel_ShiftStrategyParameters_UpshiftAfterDownshiftDelay, required: false)
							?.InnerText.ToDouble().SI<Second>() ?? DeclarationData.Gearbox.UpshiftAfterDownshiftDelay;
			}
		}

		public virtual MeterPerSquareSecond UpshiftMinAcceleration
		{
			get {
				return GetNode(XMLNames.DriverModel_ShiftStrategyParameters_UpshiftMinAcceleration, required: false)
							?.InnerText.ToDouble().SI<MeterPerSquareSecond>() ?? DeclarationData.Gearbox.UpshiftMinAcceleration;
			}
		}

		public virtual Second PowershiftShiftTime
		{
			get {
				return GetNode(XMLNames.DriverModel_ShiftStrategyParameters_PowershiftShiftTime, required: false)
							?.InnerText.ToDouble().SI<Second>() ?? 0.8.SI<Second>();
			}
		}

		#endregion

		#region Implementation of ITorqueConverterEngineeringShiftParameterInputData

		public virtual MeterPerSquareSecond CLUpshiftMinAcceleration
		{
			get {
				return GetNode(XMLNames.TorqueConverter_CLUpshiftMinAcceleration, required: false)
							?.InnerText.ToDouble().SI<MeterPerSquareSecond>() ??
						DeclarationData.Gearbox.UpshiftMinAcceleration;
			}
		}

		public virtual MeterPerSquareSecond CCUpshiftMinAcceleration
		{
			get {
				return GetNode(XMLNames.TorqueConverter_CCUpshiftMinAcceleration, required: false)
							?.InnerText.ToDouble().SI<MeterPerSquareSecond>() ??
						DeclarationData.Gearbox.UpshiftMinAcceleration;
			}
		}

		#endregion
	}

	internal class XMLEngineeringGearshiftDataV10 : XMLEngineeringGearshiftDataV07
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		//public new const string XSD_TYPE = "ShiftStrategyParametersType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		public XMLEngineeringGearshiftDataV10(XmlNode node) : base(node) { }
	}
}
