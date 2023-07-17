using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		public Watt PowerLimit => Body.GetEx<double>("PowerLimit").SI<Watt>() * 1000;

		public TableData MassFlowMap => ReadTableData(Body.GetEx<string>("MassFlowMap"), "FuelCell MassFlowMap", true);

        #endregion
    }
}
