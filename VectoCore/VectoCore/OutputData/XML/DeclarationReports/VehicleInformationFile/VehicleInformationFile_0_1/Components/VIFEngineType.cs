using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;


namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public class VIFEngineType : AbstractVIFXmlType, IXmlTypeWriter
	{
		public VIFEngineType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var engine = inputData.JobInputData.Vehicle.Components.EngineInputData;
			if (engine == null)	
				return null;

			return new XElement(_vif + XMLNames.Component_Engine,
					new XElement(_vif + XMLNames.ComponentDataWrapper,
						new XAttribute(_xsi + XMLNames.XSIType, "EngineDataVIFType"),
						new XElement(_vif + XMLNames.Component_Manufacturer, engine.Manufacturer),
						new XElement(_vif + XMLNames.Component_Model, engine.Model),
						new XElement(_vif + XMLNames.Component_CertificationNumber, engine.CertificationNumber),
						new XElement(_vif + XMLNames.Component_Date, XmlConvert.ToString(engine.Date, XmlDateTimeSerializationMode.Utc)),
						new XElement(_vif + XMLNames.Component_AppVersion, engine.AppVersion),
						new XElement(_vif + XMLNames.Engine_Displacement,
							engine.Displacement.ConvertToCubicCentiMeter().ToXMLFormat(0)),
						new XElement(_vif + XMLNames.Engine_RatedSpeed, engine.RatedSpeedDeclared.AsRPM.ToXMLFormat(0)),
						new XElement(_vif + XMLNames.Engine_RatedPower, engine.RatedPowerDeclared.ToXMLFormat(0)),
						new XElement(_vif + XMLNames.Engine_MaxTorque, engine.MaxTorqueDeclared.ToXMLFormat(0)),
						new XElement(
							_vif + XMLNames.Engine_WHRType,
							new XElement(_vif + XMLNames.Engine_WHR_MechanicalOutputICE, (engine.WHRType & WHRType.MechanicalOutputICE) != 0),
							new XElement(_vif + XMLNames.Engine_WHR_MechanicalOutputIDrivetrain, (engine.WHRType & WHRType.MechanicalOutputDrivetrain) != 0),
							new XElement(_vif + XMLNames.Engine_WHR_ElectricalOutput, (engine.WHRType & WHRType.ElectricalOutput) != 0)
						),
						GetEngineModes(engine.EngineModes))
					);
		}

		private List<XElement> GetEngineModes(IList<IEngineModeDeclarationInputData> engineModes)
		{
			var modes = new List<XElement>();
			foreach (var mode in engineModes)
			{
				modes.Add(
					new XElement(
						_vif + XMLNames.Engine_FuelModes,
						new XElement(_vif + XMLNames.Engine_IdlingSpeed, mode.IdleSpeed.AsRPM.ToXMLFormat(0)),
						new XElement(
							_vif + XMLNames.Engine_FullLoadAndDragCurve,
							FullLoadCurveReader.Create(mode.FullLoadCurve, true).FullLoadEntries.Select(
								x => new XElement(
									_vif + XMLNames.Engine_FullLoadCurve_Entry,
									new XAttribute(XMLNames.Engine_EngineFullLoadCurve_EngineSpeed_Attr, x.EngineSpeed.AsRPM.ToXMLFormat(2)),
									new XAttribute(XMLNames.Engine_FullLoadCurve_MaxTorque_Attr, x.TorqueFullLoad.ToXMLFormat(2)),
									new XAttribute(XMLNames.Engine_FullLoadCurve_DragTorque_Attr, x.TorqueDrag.ToXMLFormat(2)))
							)),
						new XElement(
							_vif + "Fuels",
							mode.Fuels.Select(x => new XElement(_vif + XMLNames.Engine_FuelType, x.FuelType.ToXMLFormat())))
					)
				);
			}

			return modes;
		}



		#endregion
	}
}
