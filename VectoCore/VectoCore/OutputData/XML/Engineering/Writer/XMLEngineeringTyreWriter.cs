using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	internal class XMLEngineeringTyreWriterV10 : AbstractComponentWriter<ITyreEngineeringInputData>,
		IXmlEngineeringTyreWriter
	{
		private XNamespace _componentDataNamespace;

		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;
		public XMLEngineeringTyreWriterV10() : base("TyreDataEngineeringType") { }

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace
		{
			get { return _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI)); }
		}

		#endregion

		#region Overrides of AbstractComponentWriter<ITyreEngineeringInputData>

		protected override object[] DoWriteXML(ITyreEngineeringInputData inputData)
		{
			var tns = ComponentDataNamespace;

			//var retVal = new list<object> {
			var tyre = new List<object> { 
				GetXMLTypeAttribute(),
				GetDefaultComponentElements(inputData)
				
			};

			if (string.IsNullOrWhiteSpace(inputData.Dimension)) {
				tyre.Add(new XElement(
							tns + XMLNames.AxleWheels_Axles_Axle_DynamicTyreRadius, inputData.DynamicTyreRadius.Value() * 1000));
				tyre.Add(new XElement(tns + XMLNames.AxleWheels_Axles_Axle_Inertia, inputData.Inertia.Value()));
			} else {
				tyre.Add(new XElement(tns + XMLNames.AxleWheels_Axles_Axle_Dimension, inputData.Dimension));
			}
			tyre.Add(new XElement(tns + XMLNames.AxleWheels_Axles_Axle_RRCISO, inputData.RollResistanceCoefficient.ToXMLFormat(4)));
			tyre.Add(new XElement(tns + XMLNames.AxleWheels_Axles_Axle_FzISO, inputData.TyreTestLoad.Value().ToXMLFormat(0)));

			return tyre.ToArray();
		}

		#endregion
	}
}
