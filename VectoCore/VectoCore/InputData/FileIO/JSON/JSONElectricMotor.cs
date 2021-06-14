using System;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.InputData.FileIO.JSON {

	public class JSONElectricMotorV2 : JSONElectricMotorV1
	{
		public JSONElectricMotorV2(JObject data, string filename, bool tolerateMissing = false) : base(data, filename, tolerateMissing) { }

		public override NewtonMeter ContinuousTorque {
			get { return Body.GetEx<double>("ContinuousTorque").SI<NewtonMeter>(); }
		}

		public override PerSecond ContinuousTorqueSpeed {
			get { return Body.GetEx<double>("ContinuousTorqueSpeed").RPMtoRad(); }
		}

		public override NewtonMeter OverloadTorque {
			get { return Body.GetEx<double>("OverloadTorque").SI<NewtonMeter>(); }
		}

		public override PerSecond OverloadTestSpeed {
			get { return Body.GetEx<double>("OverloadTorqueSpeed").RPMtoRad(); }
		}
	}


	public class JSONElectricMotorV1 : JSONFile, IElectricMotorEngineeringInputData
	{
		public JSONElectricMotorV1(JObject data, string filename, bool tolerateMissing = false) : base(data, filename, tolerateMissing) { }
		public virtual string Manufacturer
		{
			get { return Constants.NOT_AVailABLE; }
		}
		public virtual string Model
		{
			get { return Body.GetEx<string>("Model"); }
		}
		public virtual DateTime Date { get { return DateTime.MinValue; } }
		public virtual CertificationMethod CertificationMethod
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

		public virtual TableData FullLoadCurve
		{
			get { return ReadTableData(Body.GetEx<string>("FullLoadCurve"), "ElectricMotor FullLoadCurve"); }
		}

		public virtual TableData DragCurve
		{
			get { return ReadTableData(Body.GetEx<string>("DragCurve"), "ElectricMotor DragCurve"); }
		}

		public virtual TableData EfficiencyMap
		{
			get { return ReadTableData(Body.GetEx<string>("EfficiencyMap"), "ElectricMotor Map"); }
		}

		public virtual KilogramSquareMeter Inertia
		{
			get { return Body.GetEx<double>("Inertia").SI<KilogramSquareMeter>(); }
		}

		public virtual Joule OverloadBuffer
		{
			get { return Body.GetValueOrDefault<double>("ThermalOverloadBuffer")?.SI(Unit.SI.Mega.Joule).Cast<Joule>() ?? 1e18.SI<Joule>(); }
		}

		public virtual double OverloadRecoveryFactor
		{
			get
			{
				return Body.GetValueOrDefault<double>("ThermalOverloadRecoveryFactor") ?? 0.9;
			}
		}

		public virtual NewtonMeter ContinuousTorque
		{
			get { return (Body.GetValueOrDefault<double>("ContinuousPower")?.SI<Watt>() ?? 1e12.SI<Watt>()) / (Body.GetValueOrDefault<double>("ContinuousPowerSpeed")?.RPMtoRad() ?? 1.SI<PerSecond>()) ; }
		}

		public virtual PerSecond ContinuousTorqueSpeed
		{
			get { return Body.GetValueOrDefault<double>("ContinuousPowerSpeed")?.RPMtoRad() ?? 0.RPMtoRad(); }
		}

		public virtual NewtonMeter OverloadTorque {
			get { return null; }
		}

		public virtual PerSecond OverloadTestSpeed
		{
			get { return null; }
		}

		public virtual Second OverloadTime
		{
			get { return Body.GetValueOrDefault<double>("OverloadTime")?.SI<Second>() ?? 0.SI<Second>(); }
		}

	}
}