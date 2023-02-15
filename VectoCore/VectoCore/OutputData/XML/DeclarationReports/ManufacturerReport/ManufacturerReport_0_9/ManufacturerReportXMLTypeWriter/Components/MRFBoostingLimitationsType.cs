using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.Reader.ComponentData;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
	public interface IMrfVehicleType
	{
		XElement GetElement(IVehicleDeclarationInputData inputData);

	}

    internal class MRFBoostingLimitationsType : AbstractMrfXmlType, IMrfVehicleType
    {
		public MRFBoostingLimitationsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMrfVehicleType

		public XElement GetElement(IVehicleDeclarationInputData inputData)
		{
			var boostingLimitations = inputData.BoostingLimitations;
			if (boostingLimitations != null)
			{
				var boostingLimitationsXElement = new XElement(_mrf + XMLNames.Vehicle_BoostingLimitation);
				foreach (DataRow row in boostingLimitations.Rows)
				{
					boostingLimitationsXElement.Add(new XElement(_mrf + XMLNames.BoostingLimitation_Entry,
						new XAttribute(XMLNames.BoostingLimitation_BoostingTorque, row[MaxBoostingTorqueReader.Fields.DrivingTorque]),
						new XAttribute(XMLNames.BoostingLimitation_RotationalSpeed, row[MaxBoostingTorqueReader.Fields.MotorSpeed])));
				}


				return boostingLimitationsXElement;
			}
			return null;
		}

		#endregion
	}


}
