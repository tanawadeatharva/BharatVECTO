using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	public class BusDatabase : IBusDatabase
	{
		private List<IBus> buses = new List<IBus>();
		private List<IBus> selectListBuses = new List<IBus>();

		public bool AddBus(IBus bus)
		{
			var result = true;

			try {
				buses.Add(bus);
			} catch (Exception ) {
				result = false;
			}

			return result;
		}

		public List<IBus> GetBuses(string busModel, bool AsSelectList = false)
		{
			if (AsSelectList) {
				selectListBuses = new List<IBus>();
				selectListBuses = buses.Where(v => v.Model == "" || v.Model.ToLower().Contains(busModel.ToLower())).ToList();
				selectListBuses.Insert(0, new Bus(0, "<Select>", "low floor", "gas", 1, 1, 1, 2, false));
				return selectListBuses;
			}

			return buses.Where(v => v.Model == "" || v.Model.ToLower().Contains(busModel.ToLower())).ToList();
		}

		public bool Initialise(string filepath)
		{
			var returnStatus = true;

			if (File.Exists(filepath)) {
				using (var sr = new StreamReader(filepath)) {
					// get array og lines fron csv
					var lines = sr.ReadToEnd().Split(new [] { Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

					// Must have at least 2 entries in map to make it usable [dont forget the header row]
					if (lines.Length < 2) {
						return false;
					}

					var firstline = true;

					var id = 1;

					foreach (var line in lines) {
						if (!firstline) {

							// split the line
							var elements = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							// 7 or 8 entries per line required
							if ((elements.Length != 7 && elements.Length != 8))
								throw new ArgumentException("Incorrect number of values in csv file");

							// Bus
							try {
								var bus = new Bus(id, elements[0], elements[1], elements[2], double.Parse(elements[3], CultureInfo.InvariantCulture), double.Parse(elements[4], CultureInfo.InvariantCulture), double.Parse(elements[5], CultureInfo.InvariantCulture), int.Parse(elements[6], CultureInfo.InvariantCulture), elements.Length == 8 ? bool.Parse(elements[7]) : false);

								buses.Add(bus);
							} catch (Exception ) {

								// Indicate problems
								returnStatus = false;
							}

							id = id + 1;
						} else {
							firstline = false;
						}
					}
				}
			} else {
				returnStatus = false;
			}

			var uniqueBuses = buses.Select(b => b.Model).Distinct().Count();
			if (buses.Count != uniqueBuses) {
				returnStatus = false;
			}

			return returnStatus;
		}

		public bool UpdateBus(int id, IBus bus)
		{
			var result = true;

			try {
				var existingBus = buses.Single(b => b.Id == id);

				existingBus.Model = bus.Model;
				existingBus.RegisteredPassengers = bus.RegisteredPassengers;
				existingBus.FloorType = bus.FloorType;
				existingBus.LengthInMetres = bus.LengthInMetres;
				existingBus.WidthInMetres = bus.WidthInMetres;
				existingBus.HeightInMetres = bus.HeightInMetres;
				existingBus.IsDoubleDecker = bus.IsDoubleDecker;
			} catch (Exception ) {
				result = false;
			}

			return result;
		}

		public bool Save(string filepath)
		{
			var result = true;
			var output = new StringBuilder();

			try {
				output.AppendLine("Bus Model,Type,engine Type,length in m,wide in m,height in m,registered passengers,double decker");

				foreach (var bus in buses)
					output.AppendLine(bus.ToString());

				File.WriteAllText(filepath, output.ToString());
			} catch (Exception ) {
				result = false;
			}

			return result;
		}
	}
}
