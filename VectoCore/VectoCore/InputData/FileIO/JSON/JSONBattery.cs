using System;
using System.IO;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONBatteryV1 : JSONFile, IBatteryPackEngineeringInputData
	{
		public JSONBatteryV1(JObject data, string filename, bool tolerateMissing = false) : base(data, filename,
			tolerateMissing) { }

		public string Manufacturer
		{
			get { return Constants.NOT_AVailABLE; }
		}

		public string Model
		{
			get { return Body.GetEx<string>("Model"); }
		}

		public DateTime Date
		{
			get { return DateTime.MinValue; }
		}

		public CertificationMethod CertificationMethod
		{
			get { return CertificationMethod.NotCertified; }
		}

		public string CertificationNumber
		{
			get { return Constants.NOT_AVailABLE; }
		}

		public DigestData DigestValue
		{
			get { return null; }
		}

		public double MinSOC
		{
			get { return Body.GetEx<double>("SOC_min") / 100.0; }
		}

		public double MaxSOC
		{
			get { return Body.GetEx<double>("SOC_max") / 100.0; }
		}

		public AmpereSecond Capacity
		{
			get { return Body.GetEx<double>("Capacity").SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(); }
		}

		public TableData InternalResistanceCurve
		{
			get
			{
				try {
					return ReadTableData(Body.GetEx<string>("InternalResistanceCurve"), "InternalResistanceCurve");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return
						new TableData(
							Path.Combine(BasePath, Body["InternalResistanceCurve"].ToString()) + MissingFileSuffix,
							DataSourceType.Missing);
				}
			}
		}

		public TableData VoltageCurve
		{
			get
			{
				try {
					return ReadTableData(Body.GetEx<string>("SoCCurve"), "SoC Curve");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return
						new TableData(
							Path.Combine(BasePath, Body["SoCCurve"].ToString()) + MissingFileSuffix,
							DataSourceType.Missing);
				}
			}
		}

		public double MaxCurrentFactor
		{
			get { return Body.GetEx<double>("MaxCurrentFactor"); }
		}
	}
}