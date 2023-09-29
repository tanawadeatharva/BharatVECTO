
#if TRACE
using System.Windows.Forms.DataVisualization.Charting;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;


namespace TUGraz.VectoCore.Tests.InputData
{
	[TestFixture]
	public class IEPCMapReaderTest
	{
		public const string repoRoot = "../../../../../"; //from cwd to repo root
		public const string iepcMapTestFiles = @"TestData/Components/IEPC/IEPCMapReader/";




        [OneTimeSetUp]
		public void OneTimeSetup()
		{
			
		}


		[TestCase(@$"{repoRoot}Generic Vehicles/Engineering Mode/GenericIEPC-S/IEPC-S_Gbx3Speed/IEPC_Gbx3_FLD_max.viepcp", @$"{repoRoot}Generic Vehicles/Engineering Mode/GenericIEPC-S/IEPC-S_Gbx3Speed/IEPC_Gbx3_2.viepco", 4.65f, 2.74f)]
		[TestCase(@$"{repoRoot}Generic Vehicles/Engineering Mode/GenericIEPC-S/IEPC-S_Gbx3Speed/IEPC_Gbx3_FLD_max.viepcp", @$"{repoRoot}Generic Vehicles/Engineering Mode/GenericIEPC-S/IEPC-S_Gbx3Speed/IEPC_Gbx3_1.viepco", 8.31f, 2.74f)]
		[TestCase(@$"{repoRoot}Generic Vehicles/Engineering Mode/GenericIEPC-S/IEPC-S_Gbx3Speed/IEPC_Gbx3_FLD_max.viepcp", @$"{repoRoot}Generic Vehicles/Engineering Mode/GenericIEPC-S/IEPC-S_Gbx3Speed/IEPC_Gbx3_3.viepco", 2.74f, 2.74f)]


		[TestCase(@$"{repoRoot}Generic Vehicles/Engineering Mode/GenericIEPC-S/IEPC-S_Gbx3Speed/IEPC_Gbx3_FLD_max.viepcp", @$"{repoRoot}VectoCore/VectoCoreTest/TestData/Components/IEPC/IEPCMapReader/IEPC_Gbx3_1_different_number_of_points.viepco", 8.31f, 2.74f)]
		[TestCase(@$"{repoRoot}Generic Vehicles/Engineering Mode/GenericIEPC-S/IEPC-S_Gbx3Speed/IEPC_Gbx3_FLD_max.viepcp", @$"{repoRoot}VectoCore/VectoCoreTest/TestData/Components/IEPC/IEPCMapReader/IEPC_Gbx3_1_following_curve.viepco", 8.31f, 2.74f)]


		[TestCase($"{iepcMapTestFiles}IEPC_Gbx1_FLD_max.viepcp",$"{iepcMapTestFiles}IEPC_Gbx1.viepco", 4.65f, 4.65f)]
		[TestCase($"{iepcMapTestFiles}IEPC_Gbx1_FLD_max.viepcp", $"{iepcMapTestFiles}IEPC_Gbx1_below_fld_outlierpoints.viepco", 4.65f, 4.65f)]
        public void ReadIEPCMap(string fullLoadCurvePath, string powerMapPath, double ratio, double fldMeasuredRatio)
		{
			Assert.That(File.Exists(fullLoadCurvePath), Path.GetFullPath(fullLoadCurvePath));
			Assert.That(File.Exists(powerMapPath), Path.GetFullPath(fullLoadCurvePath));



			var fullLoadCurveData = VectoCSVFile.Read(fullLoadCurvePath);
			var powerMapData = VectoCSVFile.Read(powerMapPath);
			powerMapData.Columns[0].ColumnName = IEPCMapReader.Fields.MotorSpeed;
			powerMapData.Columns[1].ColumnName = IEPCMapReader.Fields.Torque;
			powerMapData.Columns[2].ColumnName = IEPCMapReader.Fields.PowerElectrical;

			var fld = IEPCFullLoadCurveReader.Create(fullLoadCurveData, 1, fldMeasuredRatio);
			var powerMapInput = IEPCMapReader.GetEntries(powerMapData, ratio, ExecutionMode.Engineering);


			var effMap = IEPCMapReader.Create(powerMapData, 1, ratio, fld, ExecutionMode.Engineering);
#if TRACE
			PrintMaps("FLD.png", 
				new SeriesProperties<ElectricMotorFullLoadCurve.FullLoadEntry>() {
					xSelector = (x => x.MotorSpeed.AsRPM),
					ySelector = (y => y.FullDriveTorque.Value()),
					color = Color.Red,
					entries = fld.FullLoadEntries,
				}, new SeriesProperties<ElectricMotorFullLoadCurve.FullLoadEntry>()
				{
					xSelector = (x => x.MotorSpeed.AsRPM),
					ySelector = (y => y.FullGenerationTorque.Value()),
					color = Color.Red,
					entries = fld.FullLoadEntries
				});



			PrintMaps("FLD_input_powermap.png",
				new SeriesProperties<ElectricMotorFullLoadCurve.FullLoadEntry>()
				{
					xSelector = (x => x.MotorSpeed.AsRPM),
					ySelector = (y => y.FullDriveTorque.Value()),
					color = Color.Red,
					entries = fld.FullLoadEntries,
				}, new SeriesProperties<ElectricMotorFullLoadCurve.FullLoadEntry>()
				{
					xSelector = (x => x.MotorSpeed.AsRPM),
					ySelector = (y => y.FullGenerationTorque.Value()),
					color = Color.Red,
					entries = fld.FullLoadEntries
				}, new SeriesProperties<EfficiencyMap.Entry>()
				{
					xSelector = (x => x.MotorSpeed.AsRPM),
					ySelector = (y => y.Torque.Value()),
					color = Color.Blue,
					entries = powerMapInput,
				});


			PrintMaps("FLD_powermap_extrapolated.png",
				new SeriesProperties<ElectricMotorFullLoadCurve.FullLoadEntry>()
				{
					xSelector = (x => x.MotorSpeed.AsRPM),
					ySelector = (y => y.FullDriveTorque.Value()),
					color = Color.Red,
					entries = fld.FullLoadEntries,
				}, new SeriesProperties<ElectricMotorFullLoadCurve.FullLoadEntry>()
				{
					xSelector = (x => x.MotorSpeed.AsRPM),
					ySelector = (y => y.FullGenerationTorque.Value()),
					color = Color.Red,
					entries = fld.FullLoadEntries,
				}, new SeriesProperties<EfficiencyMap.Entry>()
				{
					xSelector = (x => x.MotorSpeed.AsRPM),
					ySelector = (y => y.Torque.Value() * (-1)),
					color = Color.Blue,
					entries = powerMapInput,
				}, new SeriesProperties<EfficiencyMap.Entry>()
				{
					xSelector = (x => x.MotorSpeed.AsRPM),
					ySelector = (y => y.Torque.Value()),
					color = Color.LightCoral,
					entries = effMap.Entries,
				});

#endif

			//Drive
			foreach (var entry in fld.FullLoadEntries) {
				//Assert that this functions returns null when the battery is not restricting the EM
				Assert.IsNull(effMap.LookupTorque(double.MaxValue.SI<Watt>(), entry.MotorSpeed,
					entry.FullDriveTorque * Constants.PowerMapSettings.EfficiencyMapExtrapolationFactor * -1));
				var power_extended = effMap.LookupElectricPower(entry.MotorSpeed, entry.FullDriveTorque * -1 * Constants.PowerMapSettings.EfficiencyMapExtrapolationFactor, true);
				var power = effMap.LookupElectricPower(entry.MotorSpeed, entry.FullDriveTorque * -1, true);
				Assert.IsFalse(power_extended.Extrapolated);
				Assert.AreEqual(power.ElectricalPower.Value() * Constants.PowerMapSettings.EfficiencyMapExtrapolationFactor, power_extended.ElectricalPower.Value(), 1e6);
			}
			//Recuperation
			foreach (var entry in fld.FullLoadEntries)
			{
				//Assert that this functions returns null when the battery is not restricting the EM
				Assert.IsNull(effMap.LookupTorque(double.MinValue.SI<Watt>(), entry.MotorSpeed, entry.FullGenerationTorque * Constants.PowerMapSettings.EfficiencyMapExtrapolationFactor * -1));
				var power_extended = effMap.LookupElectricPower(entry.MotorSpeed, entry.FullGenerationTorque * -1 * Constants.PowerMapSettings.EfficiencyMapExtrapolationFactor, true);
				var power = effMap.LookupElectricPower(entry.MotorSpeed, entry.FullGenerationTorque * -1, true);
				Assert.IsFalse(power_extended.Extrapolated);
				Assert.AreEqual(power.ElectricalPower.Value() * Constants.PowerMapSettings.EfficiencyMapExtrapolationFactor, power_extended.ElectricalPower.Value(), 1e6);

			}
		}

#if TRACE
		public abstract class SeriesProperties
		{
			public abstract IEnumerable<(double x, double y)> GetPoints();
			public Color color;
			public SeriesChartType chartType = SeriesChartType.FastPoint;
			public string legend;
		}


		public class SeriesProperties<T> : SeriesProperties
		{
			public IEnumerable<T> entries;
			public Func<T, double> xSelector;
			public Func<T, double> ySelector;
			


		#region Overrides of SeriesProperties

			public override IEnumerable<(double x, double y)> GetPoints()
			{
				foreach (var entry in entries) {
					yield return (xSelector(entry), ySelector(entry));
				}
			}

		#endregion
		}


        public void PrintMaps(string fileName, params SeriesProperties[] series)
        {
			if (File.Exists(fileName)) {
				File.Delete(fileName);
			}

			//x ... speed
		
			using (var chart = new Chart()) {
				chart.Size = new Size(1000, 1000);
				chart.Legends.Add(new Legend("legend 1"));
				chart.ChartAreas.Add(new ChartArea("maps")
				{
					AxisX = new Axis {  Crossing = 0},
					AxisY = new Axis {  Crossing = 0},
				});

				foreach (var ser in series) {
					var chartSeries = new Series
					{
						ChartType = SeriesChartType.FastPoint,
						Color = ser.color,
						MarkerSize = 10,
					};
					foreach (var entry in ser.GetPoints())
					{
						chartSeries.Points.AddXY(entry.x, entry.y);
					}
					chart.Series.Add(chartSeries);
				}
				
				
		
				


				var dirInfo = Directory.CreateDirectory($@"{nameof(IEPCMapReaderTest)}//{String.Join("", TestContext.CurrentContext.Test.Name.Replace(repoRoot, "").Split(Path.GetInvalidFileNameChars()))}");
				chart.SaveImage($"{dirInfo.FullName}/{(fileName.EndsWith(".png") ? fileName : fileName + ".png")}",
					ChartImageFormat.Png);
				TestContext.WriteLine($"{dirInfo.FullName}");
			}
	



			TestContext.WriteLine($"Map written to {fileName}");
        }
#endif
	}
}