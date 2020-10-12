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
	public class JSONBatteryV1 : JSONFile, IBatteryPackEngineeringInputData, ISuperCapEngineeringInputData
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

		AmpereSecond IBatteryPackDeclarationInputData.Capacity
		{
			get { return Body.GetEx<double>("Capacity").SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(); }
		}

		Farad ISuperCapDeclarationInputData.Capacity
		{
			get
			{
				return Body.GetEx<double>("Capacity").SI<Farad>();
			}
		}

		public Ohm InternalResistance
		{
			get
			{
				return Body.GetEx<double>("InternalResistance").SI<Ohm>();
			}
		}

		public Volt MinVoltage
		{
			get
			{
				return Body.GetEx<double>("U_min").SI<Volt>();
			}
		}

		public Volt MaxVoltage
		{
			get
			{
				return Body.GetEx<double>("U_max").SI<Volt>();
			}
		}

		public Ampere MaxCurrentCharge
		{
			get{ return Math.Abs(Body.GetEx<double>("I_maxCharge")).SI<Ampere>(); }
		}
		public Ampere MaxCurrentDischarge { get { return Math.Abs(Body.GetEx<double>("I_maxDischarge")).SI<Ampere>(); } }

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

		public TableData MaxCurrentMap
		{
			get
			{
				try {
					return ReadTableData(Body.GetEx<string>("MaxCurrentMap"), "Max Current Map");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return
						new TableData(
							Body["MaxCurrentMap"] == null ? MissingFileSuffix : Path.Combine(BasePath, Body["MaxCurrentMap"].ToString()) + MissingFileSuffix,
							DataSourceType.Missing);
				}
			}
		}


		public REESSType StorageType
		{
			get
			{
				return Body["REESSType"] == null ? REESSType.Battery : Body.GetEx<string>("REESSType").ParseEnum<REESSType>();
			}
		}
	}
}