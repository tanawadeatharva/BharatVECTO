using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms.DataVisualization.Charting;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using Point = TUGraz.VectoCore.Utils.Point;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class ShiftPolygonDrawer
	{
		private static readonly Font AxisLabelFont = new Font("Consolas", 10);
		private static readonly Font AxisTitleFont = new Font("Verdana", 12);
		private static readonly Font LegendFont = new Font("Verdana", 14);

		private static Size _diagramSize = new Size(1000, 800);

		public static void DrawShiftPolygons(string title, EngineFullLoadCurve engineFld, List<ShiftPolygon> polygons,
			string imageFileName, PerSecond speed85kmh, List<List<Point>> upshiftOrig = null,
			List<List<Point>> downshiftTransformed = null)
		{
			var numRows = Math.Ceiling(polygons.Count / 4.0);
			var numCols = Math.Ceiling(polygons.Count / numRows);

			var chart = new Chart() {
				Size = new Size((int)(_diagramSize.Width * numCols), (int)(_diagramSize.Height * numRows))
			};
			var maxX = engineFld.FullLoadEntries.Last().EngineSpeed.Value() / Constants.RPMToRad * 1.1;

			var i = 0;
			foreach (var shiftPolygon in polygons) {
				var chartArea = AddChartArea(chart, "Gear " + (i + 1), "Engine Speed", "Torque", 0, maxX);

				PlotPower(engineFld, chartArea, chart, "engine power " + i);
				PlotFLD(engineFld, speed85kmh, chartArea, chart, "Engine Full Load " + i);

				if (upshiftOrig != null && i < upshiftOrig.Count) {
					PlotShiftLine("UpshiftOrig " + i, chartArea, chart, Color.Gray,
						upshiftOrig[i].Select(pt => pt.X / Constants.RPMToRad).ToList(), upshiftOrig[i].Select(pt => pt.Y).ToList(), true);
				}
				if (downshiftTransformed != null && i < downshiftTransformed.Count) {
					PlotShiftLine("DownTransformed " + i, chartArea, chart, Color.BlueViolet,
						downshiftTransformed[i].Select(pt => pt.X / Constants.RPMToRad).ToList(),
						downshiftTransformed[i].Select(pt => pt.Y).ToList(), true);
				}

				PlotShiftPolygon(i, shiftPolygon, chartArea, chart);

				PositionChartArea(chartArea, 5, i, polygons.Count);


				i++;
			}

			AddTitle(chart, title);
			chart.Invalidate();
			chart.SaveImage(imageFileName, ChartImageFormat.Png);
		}

		private static void PlotPower(EngineFullLoadCurve engineFld, ChartArea chartArea, Chart chart, string name)
		{
			var series = new Series {
				Name = name,
				ChartType = SeriesChartType.Line,
				Color = Color.DarkGoldenrod,
				BorderWidth = 2,
				//Legend = legend.Name,
				IsVisibleInLegend = true,
				ChartArea = chartArea.Name,
				//YAxisType = AxisType.Secondary
			};
			series.BorderDashStyle = ChartDashStyle.Dash;
			series.BorderWidth = 1;

			var x = new List<double>();
			var y = new List<double>();
			for (var i = engineFld.FullLoadEntries.First().EngineSpeed;
				i < engineFld.FullLoadEntries.Last().EngineSpeed;
				i += 5.RPMtoRad()) {
				x.Add(i.Value() / Constants.RPMToRad);
				y.Add(engineFld.FullLoadStationaryPower(i).Value() / 1000);
			}

			chart.Series.Add(series);
			chart.Series[series.Name].Points.DataBindXY(x, y);
		}

		private static void AddTitle(Chart chart, string titleText)
		{
			var title = new Title {
				Text = titleText,
				IsDockedInsideChartArea = false,
				Font = new Font("Verdana", 18, FontStyle.Bold)
			};
			//title.DockedToChartArea = dockToChartArea;
			chart.Titles.Add(title);
		}

		private static void PositionChartArea(ChartArea chartArea, float titleHeight, int i, int count)
		{
			var numRows = Math.Ceiling(count / 4.0);
			var numCols = Math.Ceiling(count / numRows);
			chartArea.Position.Auto = false;
			chartArea.Position.Width = (float)((100.0f) / numCols);
			chartArea.Position.Height = (float)((100.0f - titleHeight) / numRows);
			chartArea.Position.X = (float)(((i) % numCols) * 100.0f / numCols);
			chartArea.Position.Y = (float)(titleHeight + (int)((i) / numCols) * (100 - titleHeight) / numRows);
		}

		private static void PlotShiftPolygon(int gear, ShiftPolygon shiftPolygon, ChartArea chartArea, Chart chart)
		{
			var x = new List<double>();
			var y = new List<double>();
			foreach (var entry in shiftPolygon.Downshift) {
				x.Add(entry.AngularSpeed.Value() / Constants.RPMToRad);
				y.Add(entry.Torque.Value());
			}
			PlotShiftLine("DownShift " + gear, chartArea, chart, Color.DarkRed, x, y);


			var xUp = new List<double>();
			var yUp = new List<double>();
			foreach (var entry in shiftPolygon.Upshift) {
				xUp.Add(entry.AngularSpeed.Value() / Constants.RPMToRad);
				yUp.Add(entry.Torque.Value());
			}
			PlotShiftLine("UpShift " + gear, chartArea, chart, Color.DarkRed, xUp, yUp);


			//return series;
		}

		private static void PlotShiftLine(string name, ChartArea chartArea, Chart chart, Color color, List<double> x,
			List<double> y, bool dashed = false)
		{
			var series = new Series {
				Name = name,
				ChartType = SeriesChartType.Line,
				Color = color,
				BorderWidth = 2,
				//Legend = legend.Name,
				IsVisibleInLegend = true,
				ChartArea = chartArea.Name,
			};
			if (dashed) {
				series.BorderDashStyle = ChartDashStyle.Dash;
				series.BorderWidth = 1;
			}

			chart.Series.Add(series);
			chart.Series[series.Name].Points.DataBindXY(x, y);
		}

		private static void PlotFLD(EngineFullLoadCurve engineFld, PerSecond speed85kmh, ChartArea chartArea, Chart chart,
			string name)
		{
			PlotFullLoad(engineFld, chartArea, chart, name);

			PlotDragLoad(engineFld, chartArea, chart, name);

			PlotNPref(engineFld, chartArea, chart, name);

			PlotN95h(engineFld, chartArea, chart, name);

			PlotN85kmh(engineFld, speed85kmh, chartArea, chart, name);
		}

		private static void PlotN85kmh(EngineFullLoadCurve engineFld, PerSecond speed85kmh, ChartArea chartArea, Chart chart,
			string name)
		{
			var series85kmh = new Series {
				Name = "85kmh-" + name,
				ChartType = SeriesChartType.Line,
				Color = Color.LimeGreen,
				BorderWidth = 2,
				//Legend = legend.Name,
				IsVisibleInLegend = true,
				ChartArea = chartArea.Name,
			};
			chart.Series.Add(series85kmh);
			var x85kmh = new[] { speed85kmh.Value() / Constants.RPMToRad, speed85kmh.Value() / Constants.RPMToRad }.ToList();
			var y85kmh = new[] { engineFld.MaxDragTorque.Value(), engineFld.MaxLoadTorque.Value() }.ToList();
			chart.Series[series85kmh.Name].Points.DataBindXY(x85kmh, y85kmh);
		}

		private static void PlotN95h(EngineFullLoadCurve engineFld, ChartArea chartArea, Chart chart, string name)
		{
			var seriesN95h = new Series {
				Name = "N95h-" + name,
				ChartType = SeriesChartType.Line,
				Color = Color.Red,
				BorderWidth = 2,
				//Legend = legend.Name,
				IsVisibleInLegend = true,
				ChartArea = chartArea.Name,
			};
			chart.Series.Add(seriesN95h);
			var x95h =
				new[] { engineFld.N95hSpeed.Value() / Constants.RPMToRad, engineFld.N95hSpeed.Value() / Constants.RPMToRad }.ToList();
			var y95h = new[] { engineFld.MaxDragTorque.Value(), engineFld.MaxLoadTorque.Value() }.ToList();
			chart.Series[seriesN95h.Name].Points.DataBindXY(x95h, y95h);
		}

		private static void PlotNPref(EngineFullLoadCurve engineFld, ChartArea chartArea, Chart chart, string name)
		{
			var seriesNpref = new Series {
				Name = "NPref-" + name,
				ChartType = SeriesChartType.Line,
				Color = Color.DeepSkyBlue,
				BorderWidth = 2,
				//Legend = legend.Name,
				IsVisibleInLegend = true,
				ChartArea = chartArea.Name,
			};
			chart.Series.Add(seriesNpref);
			var xpref =
				new[]
				{ engineFld.PreferredSpeed.Value() / Constants.RPMToRad, engineFld.PreferredSpeed.Value() / Constants.RPMToRad }
					.ToList();
			var ypref = new[] { engineFld.MaxDragTorque.Value(), engineFld.MaxLoadTorque.Value() }.ToList();
			chart.Series[seriesNpref.Name].Points.DataBindXY(xpref, ypref);
		}

		private static void PlotDragLoad(EngineFullLoadCurve engineFld, ChartArea chartArea, Chart chart, string name)
		{
			var seriesDrag = new Series {
				Name = "Drag-" + name,
				ChartType = SeriesChartType.Line,
				Color = Color.DarkBlue,
				BorderWidth = 2,
				//Legend = legend.Name,
				IsVisibleInLegend = true,
				ChartArea = chartArea.Name,
			};
			chart.Series.Add(seriesDrag);
			var xDrag = new List<double>();
			var yDrag = new List<double>();
			engineFld.FullLoadEntries.ForEach(entry => {
				xDrag.Add(entry.EngineSpeed.Value() / Constants.RPMToRad);
				yDrag.Add(entry.TorqueDrag.Value());
			});
			chart.Series[seriesDrag.Name].Points.DataBindXY(xDrag, yDrag);
		}

		private static void PlotFullLoad(EngineFullLoadCurve engineFld, ChartArea chartArea, Chart chart, string name)
		{
			var series = new Series {
				Name = name,
				ChartType = SeriesChartType.Line,
				Color = Color.DarkBlue,
				BorderWidth = 2,
				//Legend = legend.Name,
				IsVisibleInLegend = true,
				ChartArea = chartArea.Name,
			};
			chart.Series.Add(series);
			var x = new List<double>();
			var y = new List<double>();
			engineFld.FullLoadEntries.ForEach(entry => {
				x.Add(entry.EngineSpeed.Value() / Constants.RPMToRad);
				y.Add(entry.TorqueFullLoad.Value());
			});
			chart.Series[series.Name].Points.DataBindXY(x, y);
		}

		private static ChartArea AddChartArea(Chart chart, string name, string axisXTitle, string axisYTitle, double xMin,
			double xMax, bool discreteValues = false)
		{
			var chartArea = new ChartArea { Name = name };
			chartArea.AxisX.MajorGrid.LineColor = Color.DarkGray;
			chartArea.AxisY.MajorGrid.LineColor = Color.DarkGray;
			chartArea.AxisX.LabelStyle.Font = AxisLabelFont;
			chartArea.AxisY.LabelStyle.Font = AxisLabelFont;

			chartArea.AxisX.Interval = xMax / 20.0;
			chartArea.AxisX.Maximum = xMax;
			chartArea.AxisX.Minimum = xMin;
			chartArea.AxisX.MinorGrid.Enabled = true;
			chartArea.AxisX.MinorGrid.Interval = (xMax - xMin) / 100.0;
			chartArea.AxisX.MinorGrid.LineColor = Color.LightGray;
			chartArea.AxisX.Title = axisXTitle;
			chartArea.AxisX.TitleFont = AxisTitleFont;
			chartArea.AxisX.RoundAxisValues();
			chartArea.AxisX.MajorTickMark.Size = 2 * 100.0f / _diagramSize.Height;

			chartArea.AxisY.Title = axisYTitle;
			chartArea.AxisY.TitleFont = AxisTitleFont;
			chartArea.AxisY.RoundAxisValues();
			if (discreteValues) {
				chartArea.AxisY.MajorGrid.Interval = 1;
				chartArea.AxisY.MinorGrid.Enabled = false;
			} else {
				chartArea.AxisY.MinorGrid.Enabled = true;
			}
			chartArea.AxisY.MinorGrid.LineColor = Color.LightGray;
			chartArea.AxisY.MajorTickMark.Size = 5 * 100.0f / _diagramSize.Width;

			chart.ChartAreas.Add(chartArea);
			return chartArea;
		}
	}
}