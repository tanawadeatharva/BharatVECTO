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

		public override NewtonMeter ContinuousTorque => Body.GetEx<double>("ContinuousTorque").SI<NewtonMeter>();

		public override PerSecond ContinuousTorqueSpeed => Body.GetEx<double>("ContinuousTorqueSpeed").RPMtoRad();

		public override NewtonMeter OverloadTorque => Body.GetEx<double>("OverloadTorque").SI<NewtonMeter>();

		public override PerSecond OverloadTestSpeed => Body.GetEx<double>("OverloadTorqueSpeed").RPMtoRad();
	}


	public class JSONElectricMotorV1 : JSONFile, IElectricMotorEngineeringInputData
	{
		public JSONElectricMotorV1(JObject data, string filename, bool tolerateMissing = false) : base(data, filename, tolerateMissing) { }
		public virtual string Manufacturer => Constants.NOT_AVAILABLE;

		public virtual string Model => Body.GetEx<string>("Model");
		public virtual DateTime Date => DateTime.MinValue;

		public virtual CertificationMethod CertificationMethod => CertificationMethod.NotCertified;

		public string CertificationNumber => Constants.NOT_AVAILABLE;

		public DigestData DigestValue => null;

		public virtual TableData FullLoadCurve => ReadTableData(Body.GetEx<string>("FullLoadCurve"), "ElectricMotor FullLoadCurve");

		public virtual TableData DragCurve => ReadTableData(Body.GetEx<string>("DragCurve"), "ElectricMotor DragCurve");

		public virtual TableData EfficiencyMap => ReadTableData(Body.GetEx<string>("EfficiencyMap"), "ElectricMotor Map");

		public virtual KilogramSquareMeter Inertia => Body.GetEx<double>("Inertia").SI<KilogramSquareMeter>();

		public virtual Joule OverloadBuffer => Body.GetValueOrDefault<double>("ThermalOverloadBuffer")?.SI(Unit.SI.Mega.Joule).Cast<Joule>() ?? 1e18.SI<Joule>();

		public virtual double OverloadRecoveryFactor => Body.GetValueOrDefault<double>("ThermalOverloadRecoveryFactor") ?? 0.9;

		public virtual NewtonMeter ContinuousTorque => (Body.GetValueOrDefault<double>("ContinuousPower")?.SI<Watt>() ?? 1e12.SI<Watt>()) / (Body.GetValueOrDefault<double>("ContinuousPowerSpeed")?.RPMtoRad() ?? 1.SI<PerSecond>());

		public virtual PerSecond ContinuousTorqueSpeed => Body.GetValueOrDefault<double>("ContinuousPowerSpeed")?.RPMtoRad() ?? 0.RPMtoRad();

		public virtual NewtonMeter OverloadTorque => null;

		public virtual PerSecond OverloadTestSpeed => null;

		public virtual Second OverloadTime => Body.GetValueOrDefault<double>("OverloadTime")?.SI<Second>() ?? 0.SI<Second>();
	}
}