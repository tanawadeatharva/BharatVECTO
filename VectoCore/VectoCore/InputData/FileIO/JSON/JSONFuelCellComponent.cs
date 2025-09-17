using System;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONFuelCellComponent : JSONFile, IFuelCellComponentEngineeringInputData
	{
		public JSONFuelCellComponent(JObject data, string filename, bool tolerateMissing = false) : base(data, filename, tolerateMissing) { }

		#region Implementation of IComponentInputData

		public string Manufacturer => Body.GetEx<string>("Manufacturer");

		public string Model => Body.GetEx<string>("Model");
		public virtual DateTime Date => DateTime.MinValue;

		public CertificationMethod CertificationMethod => CertificationMethod.NotCertified;

		public string CertificationNumber => Constants.NOT_AVAILABLE;

		public DigestData DigestValue => null;

		#endregion

		#region Implementation of IFuelCellComponentEngineeringInputData
		public TableData MassFlowMap => ReadTableData(Body.GetEx<string>("MassFlowMap"), "FuelCell MassFlowMap", true);

		public Watt MaxElectricPower => Body.GetEx<double>("MaxElectricPower").SI(Unit.SI.Kilo.Watt).Cast<Watt>();

        public Watt MinElectricPower => Body.GetEx<double>("MinElectricPower").SI(Unit.SI.Kilo.Watt).Cast<Watt>();

		#endregion
    }


}
