using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.OutputData.XML.Engineering.Writer;

namespace TUGraz.VectoCore.OutputData.XML.Engineering {
	internal static class XMLWriterMapping
	{
		static Dictionary<Type, Entry> mapping = new Dictionary<Type, Entry>() {
			
			{
				typeof(IEngineEngineeringInputData),
				new Entry {
					WriterType = typeof(IXMLEngineeringEngineWriter),
					XMLTag = XMLNames.Component_Engine,
					FilenameTemplate = "Engine-{0}_{1}{2}.xml"
				}
			}, {
				typeof(IVehicleEngineeringInputData),
				new Entry {
					WriterType = typeof(IXMLVehicleDataWriter),
					XMLTag = XMLNames.Component_Vehicle,
					FilenameTemplate = "Vehicle-{0}_{1}{2}.xml"
				}
			}, {
				typeof(IVehicleComponentsEngineering),
				new Entry {
					WriterType = typeof(IXMLEngineeringComponentWriter),
				}
			}, {
				typeof(IGearboxEngineeringInputData),
				new Entry {
					WriterType = typeof(IXMLEngineeringGearboxWriter),
					XMLTag = XMLNames.Component_Gearbox,
					FilenameTemplate = "Gearbox-{0}_{1}{2}.xml"
				}
			}, {
				typeof(ITransmissionInputData),
				new Entry { WriterType = typeof(IXMLEngineeringGearWriter) }
			}, {
				typeof(IDriverEngineeringInputData),
				new Entry {
					WriterType = typeof(IXMLEngineeringDriverDataWriter),
				}
			}, {
				typeof(IRetarderInputData),
				new Entry {
					WriterType = typeof(IXMLEngineeringRetarderWriter),
					XMLTag = XMLNames.Component_Retarder,
					FilenameTemplate = "Retarder-{0}_{1}{2}.xml"
				}
			}, {
				typeof(IAxleGearInputData),
				new Entry {
					WriterType = typeof(IXMLEngineeringAxlegearWriter),
					XMLTag = XMLNames.Component_Axlegear,
					FilenameTemplate = "Axlegear-{0}_{1}{2}.xml"
				}
			}, {
				typeof(IAxlesEngineeringInputData),
				new Entry {
					WriterType = typeof(IXMLEngineeringAxlesWriter),
					XMLTag = XMLNames.Component_AxleWheels,
				}
			}, {
				typeof(IAxleEngineeringInputData),
				new Entry { WriterType = typeof(IXMLEngineeringAxleWriter) }
			}, {
				typeof(ITyreEngineeringInputData),
				new Entry {
					WriterType = typeof(IXmlEngineeringTyreWriter),
					XMLTag = XMLNames.AxleWheels_Axles_Axle_Tyre,
					FilenameTemplate = "Tyre-{0}_{1}{2}.xml"
				}
			}, {
				typeof(ITorqueConverterEngineeringInputData),
				new Entry {
					WriterType = typeof(IXMLEngineeringTorqueconverterWriter),
					XMLTag = XMLNames.Component_TorqueConverter,
					FilenameTemplate = "TorqueConverter-{0}_{1}{2}.xml"
				}
			}, {
				typeof(IAirdragEngineeringInputData),
				new Entry {
					WriterType = typeof(IXMLEngineeringAirdragWriter),
					XMLTag = XMLNames.Component_AirDrag,
					FilenameTemplate = "Airdrag-{0}_{1}{2}.xml"
				}
			}, {
				typeof(IAngledriveInputData),
				new Entry {
					WriterType = typeof(IXMLEngineeringAngledriveWriter),
					XMLTag = XMLNames.Component_Angledrive,
					FilenameTemplate = "Angledrive{0}_{1}{2}.xml"
				}
			}, {
				typeof(ILookaheadCoastingInputData),
				new Entry { WriterType = typeof(IXMLLookaheadDataWriter) }
			}, {
				typeof(IOverSpeedEcoRollEngineeringInputData),
				new Entry { WriterType = typeof(IXMLOverspeedDataWriter) }
			}, {
				typeof(IDriverAccelerationData),
				new Entry { WriterType = typeof(IXMLAccelerationDataWriter) }
			}, {
				typeof(IGearshiftEngineeringInputData),
				new Entry { WriterType = typeof(IXMLGearshiftDataWriter) }
			}, {
				typeof(IAuxiliariesEngineeringInputData),
				new Entry { WriterType = typeof(IXMLAuxiliariesWriter) }
			}, {
				typeof(IAuxiliaryEngineeringInputData),
				new Entry { WriterType = typeof(IXMLAuxiliaryWriter)}
			}, {
				typeof(IAdvancedDriverAssistantSystemsEngineering), new Entry() {WriterType = typeof(IXMLEngineeringADASWriter), XMLTag = XMLNames.Vehicle_AdvancedDriverAssist}
			}

		};

		public class Entry
		{
			public Type WriterType { get; set; }

			public string XMLTag { get; set; }

			public string FilenameTemplate { get; set; }
		}

		public static Type GetWriterType(Type inputType)
		{
			return mapping.ContainsKey(inputType) ? mapping[inputType].WriterType : null;
		}

		public static string GetXMLTag(Type inputType)
		{
			return mapping.ContainsKey(inputType) ? mapping[inputType].XMLTag : null;
		}

		public static string GetFilenameTemplate(Type inputType)
		{
			return mapping.ContainsKey(inputType) ? mapping[inputType].FilenameTemplate : "{0}_{1}.xml";
		}
	}
}