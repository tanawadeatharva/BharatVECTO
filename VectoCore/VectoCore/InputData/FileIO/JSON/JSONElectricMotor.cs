using System;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.InputData.FileIO.JSON {
	public class JSONElectricMotorV1 : JSONFile, IElectricMotorEngineeringInputData
	{
		public JSONElectricMotorV1(JObject data, string filename, bool tolerateMissing = false) : base(data, filename, tolerateMissing) { }
		public string Manufacturer
		{
			get { return Constants.NOT_AVailABLE; }
		}
		public string Model
		{
			get { return Body.GetEx<string>("Model"); }
		}
		public DateTime Date { get { return DateTime.MinValue; } }
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

		public TableData FullLoadCurve
		{
			get { return ReadTableData(Body.GetEx<string>("FullLoadCurve"), "ElectricMotor FullLoadCurve"); }
		}

		public TableData DragCurve
		{
			get { return ReadTableData(Body.GetEx<string>("DragCurve"), "ElectricMotor DragCurve"); }
		}

		public TableData EfficiencyMap
		{
			get { return ReadTableData(Body.GetEx<string>("EfficiencyMap"), "ElectricMotor Map"); }
		}

		public KilogramSquareMeter Inertia
		{
			get { return Body.GetEx<double>("Inertia").SI<KilogramSquareMeter>(); }
		}

		public Joule OverloadBuffer
		{
			get { return Body.GetValueOrDefault<double>("ThermalOverloadBuffer")?.SI(Unit.SI.Mega.Joule).Cast<Joule>() ?? 1e18.SI<Joule>(); }
		}

		public double OverloadRecoveryFactor
		{
			get
			{
				return Body.GetValueOrDefault<double>("ThermalOverloadRecoveryFactor") ?? 0.9;
			}
		}

		public Watt ContinuousPower
		{
			get { return Body.GetValueOrDefault<double>("ContinuousPower")?.SI<Watt>() ?? 1e12.SI<Watt>(); }
		}

		public PerSecond ContinuousPowerSpeed
		{
			get { return Body.GetValueOrDefault<double>("ContinuousPowerSpeed")?.RPMtoRad() ?? 0.RPMtoRad(); }
		}

		public Second OverloadTime
		{
			get
			{
				return Body.GetValueOrDefault<double>("OverloadTime")?.SI<Second>() ?? 0.SI<Second>();
			}
		}

	}
}