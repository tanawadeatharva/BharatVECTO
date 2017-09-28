using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.OutputData.FileIO
{
	public static class ShiftPolygonExport
	{
		public static void WriteShiftPolygon(ShiftPolygon shiftPolygon, string fileName)
		{
			var lines = new SortedList<double, ShiftPolygonFileEntry>(10);
			foreach (var entry in shiftPolygon.Downshift) {
				lines.Add(Math.Round(entry.Torque.Value(), 4),
					new ShiftPolygonFileEntry {
						Torque = entry.Torque,
						DownShift = entry.AngularSpeed,
						UpShift = shiftPolygon.Upshift.Count > 0 ? shiftPolygon.InterpolateUpshiftSpeed(entry.Torque) : null
					});
			}

			foreach (var entry in shiftPolygon.Upshift) {
				var torque = Math.Round(entry.Torque.Value(), 4);
				if (lines.ContainsKey(torque))
					lines[torque].UpShift = entry.AngularSpeed;
				else {
					lines.Add(torque, new ShiftPolygonFileEntry {
						Torque = entry.Torque,
						DownShift = shiftPolygon.Downshift.Count > 0 ? shiftPolygon.InterpolateDownshiftSpeed(entry.Torque) : null,
						UpShift = entry.AngularSpeed
					});
				}
			}

			var sb = new StringBuilder(lines.Count + 1);
			sb.AppendLine("engine torque [Nm],downshift rpm [1/min],upshift rpm [1/min]");
			foreach (var line in lines.Values) {
				if (line.DownShift == null)
					sb.AppendLine(string.Format("{0},,{1:0.0000}", line.Torque.ToOutputFormat(), line.UpShift.AsRPM));
				else if (line.UpShift == null)
					sb.AppendLine(string.Format("{0},{1:0.0000},", line.Torque.ToOutputFormat(), line.DownShift.AsRPM));
				else
					sb.AppendLine(string.Format("{0},{1:0.0000},{2:0.0000}", line.Torque.ToOutputFormat(), line.DownShift.AsRPM,
						line.UpShift.AsRPM));
			}

			File.WriteAllText(fileName, sb.ToString());
		}

		private class ShiftPolygonFileEntry
		{
			public NewtonMeter Torque;
			public PerSecond DownShift;
			public PerSecond UpShift;
		}
	}
}