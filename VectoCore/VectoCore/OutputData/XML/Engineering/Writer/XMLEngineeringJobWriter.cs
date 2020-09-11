using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.OutputData.XML.Engineering.Factory;
using TUGraz.VectoCore.OutputData.XML.Engineering.Writer;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Writer
{
	internal class XMLEngineeringJobWriterV10 : AbstractXMLWriter, IXMLEngineeringJobWriter
	{
		private XNamespace _componentDataNamespace;
		
		public IEngineeringInputDataProvider InputData { protected get; set; }

		[Inject]
		public IEngineeringWriterFactory Factory { protected get; set; }


		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public XMLEngineeringJobWriterV10() : base("VectoJobEngineeringType") { }


		public virtual object[] WriteXML()
		{
			if (InputData.JobInputData.JobType == VectoSimulationJobType.EngineOnlySimulation) {
				return WriteEngineOnlyXML();
			}

			return WriteFullPowertrainXML();
		}

		protected virtual object[] WriteFullPowertrainXML()
		{
			var v10 = ComponentDataNamespace;

			var driverDataWriter = Factory.GetWriter(
				InputData.DriverInputData, Writer, InputData.JobInputData.Vehicle.DataSource);
			
			return new object[] {
				new XElement(v10 + XMLNames.VectoJob_EngineOnlyMode, InputData.JobInputData.JobType != VectoSimulationJobType.EngineOnlySimulation),
				CreateVehicle(InputData.JobInputData.Vehicle),
				new XElement(
					v10 + XMLNames.Component_DriverModel,
					driverDataWriter.GetXMLTypeAttribute(),
					driverDataWriter.WriteXML(InputData)),
				CreateMissions(v10, InputData.JobInputData.Cycles)
			};
		}

		protected virtual object[] WriteEngineOnlyXML()
		{
			var v10 = ComponentDataNamespace;
			return new object[] {
				new XElement(v10 + XMLNames.VectoJob_EngineOnlyMode, InputData.JobInputData.JobType == VectoSimulationJobType.EngineOnlySimulation),
				new XElement(
					v10 + XMLNames.Component_Engine,
					new XElement(v10 + XMLNames.ComponentDataWrapper, 
					Factory.GetWriter(InputData.JobInputData.EngineOnly, Writer).WriteXML(InputData.JobInputData.EngineOnly)
					)
				),
				CreateMissions(v10, InputData.JobInputData.Cycles)
			};
		}

		

		protected virtual XElement CreateVehicle(IVehicleEngineeringInputData vehicle)
		{
			if (Writer.Configuration.SingleFile) {
				var v10 = ComponentDataNamespace;
				var vehicleDataWriter = Factory.GetWriter(vehicle, Writer);
				return new XElement(
					v10 + XMLNames.Component_Vehicle,
					vehicleDataWriter.WriteXML(vehicle)
				);
			}

			var document = Writer.WriteComponent(vehicle);
			return ExtComponent(document , XMLNames.Component_Vehicle, Writer.GetFilename(vehicle));		
		}


		protected virtual XElement CreateMissions(XNamespace tns, IEnumerable<ICycleData> data)
		{
			var retVal = new XElement(tns + XMLNames.VectoJob_MissionCycles);
			foreach (var cycle in data) {
				var filename = cycle.CycleData.Source;
				if (filename == null) {
					continue;
				}

				if (cycle.CycleData.SourceType == DataSourceType.Embedded &&
					filename.StartsWith(DeclarationData.DeclarationDataResourcePrefix)) {
					filename = filename.Replace(DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles.", "")
										.Replace(Constants.FileExtensions.CycleFile, "");
				} else {
					VectoCSVFile.Write(Path.Combine(Writer.Configuration.BasePath, Path.GetFileName(filename)), cycle.CycleData);
				}
				retVal.Add(
					new XElement(
						tns + XMLNames.Missions_Cycle,
						new XAttribute(XMLNames.ExtResource_Type_Attr, XMLNames.ExtResource_Type_Value_CSV),
						new XAttribute(XMLNames.ExtResource_File_Attr, Path.GetFileName(filename))
					)
				);
			}

			return retVal;
		}


		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace
		{
			get { return _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI)); }
		}

		#endregion
	}
}
