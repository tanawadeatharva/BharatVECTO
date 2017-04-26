using System;
using System.Collections.Generic;
using System.Xml;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Resources;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLDeclarationVehicleDataProvider : AbstractDeclarationXMLComponentDataProvider,
		IVehicleDeclarationInputData
	{
		public XMLDeclarationVehicleDataProvider(XMLInputDataProvider xmlInputDataProvider) : base(xmlInputDataProvider)
		{
			XBasePath = VehiclePath;
		}

		public VehicleCategory VehicleCategory
		{
			get { return GetElementValue(XMLNames.Vehicle_VehicleCategory).ParseEnum<VehicleCategory>(); }
		}

		public Kilogram CurbWeightChassis
		{
			get { return GetDoubleElementValue(XMLNames.Vehicle_CurbWeightChassis).SI<Kilogram>(); }
		}


		public Kilogram GrossVehicleMassRating
		{
			get { return GetDoubleElementValue(XMLNames.Vehicle_GrossVehicleMass).SI<Kilogram>(); }
		}


		public SquareMeter AirDragArea
		{
			get { return GetDoubleElementValue(XMLNames.Vehicle_AirDragArea).SI<SquareMeter>(); }
		}


		public AxleConfiguration AxleConfiguration
		{
			get { return AxleConfigurationHelper.Parse(GetElementValue(XMLNames.Vehicle_AxleConfiguration)); }
		}

		public IList<IAxleDeclarationInputData> Axles
		{
			get
			{
				var axles = Navigator.Select(Helper.Query(VehiclePath, XMLNames.Vehicle_Components, XMLNames.Component_AxleWheels,
					XMLNames.ComponentDataWrapper, XMLNames.AxleWheels_Axles, XMLNames.AxleWheels_Axles_Axle), Manager);

				var retVal = new IAxleDeclarationInputData[axles.Count];
				while (axles.MoveNext()) {
					//foreach (var axle in axles) {
					var dimension = axles.Current.SelectSingleNode(Helper.NSPrefix(XMLNames.AxleWheels_Axles_Axle_Dimension), Manager);
					var rollResistance = axles.Current.SelectSingleNode(Helper.NSPrefix(XMLNames.AxleWheels_Axles_Axle_RRCISO), Manager);
					var tyreTestLoad = axles.Current.SelectSingleNode(Helper.NSPrefix(XMLNames.AxleWheels_Axles_Axle_FzISO), Manager);
					var axleNumber = axles.Current.GetAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, "").ToInt();
					if (axleNumber < 1 || axleNumber > retVal.Length) {
						throw new VectoException("Axle #{0} exceeds axle count", axleNumber);
					}
					if (retVal[axleNumber - 1] != null) {
						throw new VectoException("Axle #{0} defined multiple times!", axleNumber);
					}
					retVal[axleNumber - 1] = new AxleInputData {
						AxleType = axles.Current.GetAttribute(XMLNames.AxleWheels_Axles_Axle_AxleType_Attr, "").ParseEnum<AxleType>(),
						TwinTyres = XmlConvert.ToBoolean(axles.Current.GetAttribute(XMLNames.AxleWheels_Axles_Axle_TwinTyres_Attr, "")),
						TyreTestLoad = tyreTestLoad == null ? null : tyreTestLoad.Value.ToDouble().SI<Newton>(),
						RollResistanceCoefficient = rollResistance == null ? double.NaN : rollResistance.Value.ToDouble(),
						Wheels = dimension == null ? null : dimension.Value,
					};
				}
				return retVal;
			}
		}

		public double RetarderRatio
		{
			get { return GetDoubleElementValue(XMLNames.Vehicle_RetarderRatio); }
		}

		public RetarderType RetarderType
		{
			get
			{
				var value = GetElementValue(XMLNames.Vehicle_RetarderType); //.ParseEnum<RetarderType>(); 
				switch (value) {
					case "None":
						return RetarderType.None;
					case "Losses included in Gearbox":
						return RetarderType.LossesIncludedInTransmission;
					case "Engine Retarder":
						return RetarderType.EngineRetarder;
					case "Transmission Input Retarder":
						return RetarderType.TransmissionInputRetarder;
					case "Transmission Output Retarder":
						return RetarderType.TransmissionOutputRetarder;
				}
				throw new ArgumentOutOfRangeException(value);
			}
		}

		public AngledriveType AngulargearType
		{
			get { return GetElementValue(XMLNames.Vehicle_AngledriveType).ParseEnum<AngledriveType>(); }
		}
	}
}