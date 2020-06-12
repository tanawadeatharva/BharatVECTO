using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.Models {
	public class TorqueConverterOperatingPoint
	{
		public PerSecond OutAngularVelocity;
		public NewtonMeter OutTorque;

		public PerSecond InAngularVelocity;
		public NewtonMeter InTorque;

		public double SpeedRatio;
		public double TorqueRatio;
		public bool Creeping;

		public override string ToString()
		{
			return string.Format("n_out: {0}, n_in: {1}, tq_out: {2}, tq_in {3}, nu: {4}, my: {5}", OutAngularVelocity,
								InAngularVelocity, OutTorque, InTorque, SpeedRatio, TorqueRatio);
		}
	}
}