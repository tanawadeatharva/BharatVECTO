using System;
using System.IO;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONComponentInputData : IEngineeringInputDataProvider, IDeclarationInputDataProvider
	{
		protected IGearboxEngineeringInputData Gearbox;
		protected IAxleGearInputData AxleGear;
		protected ITorqueConverterEngineeringInputData TorqueConverter;
		protected IAngularGearInputData AngularGear;
		protected IEngineEngineeringInputData Engine;
		protected IVehicleEngineeringInputData VehicleData;
		protected IRetarderInputData Retarder;
		protected IPTOTransmissionInputData PTOTransmission;


		public JSONComponentInputData(string filename)
		{
			var extension = Path.GetExtension(filename);
			object tmp = null;
			switch (extension) {
				case Constants.FileExtensions.VehicleDataFile:
					tmp = JSONInputDataFactory.ReadJsonVehicle(filename);
					break;
				case Constants.FileExtensions.EngineDataFile:
					tmp = JSONInputDataFactory.ReadEngine(filename);
					break;
				case Constants.FileExtensions.GearboxDataFile:
					tmp = JSONInputDataFactory.ReadGearbox(filename);
					break;
			}
			tmp.Switch()
				.If<IVehicleEngineeringInputData>(c => VehicleData = c)
				.If<IEngineEngineeringInputData>(c => Engine = c)
				.If<IGearboxEngineeringInputData>(c => Gearbox = c)
				.If<IAxleGearInputData>(c => AxleGear = c)
				.If<IRetarderInputData>(c => Retarder = c)
				.If<ITorqueConverterEngineeringInputData>(c => TorqueConverter = c)
				.If<IAngularGearInputData>(c => AngularGear = c)
				.If<IPTOTransmissionInputData>(c => PTOTransmission = c);
		}


		public IEngineeringJobInputData JobInputData()
		{
			throw new NotImplementedException();
		}


		IVehicleDeclarationInputData IDeclarationInputDataProvider.VehicleInputData
		{
			get { return VehicleData; }
		}

		IGearboxDeclarationInputData IDeclarationInputDataProvider.GearboxInputData
		{
			get { return GearboxInputData; }
		}

		ITorqueConverterDeclarationInputData IDeclarationInputDataProvider.TorqueConverterInputData
		{
			get { return TorqueConverterInputData; }
		}

		IDeclarationJobInputData IDeclarationInputDataProvider.JobInputData()
		{
			throw new NotImplementedException();
		}

		public IVehicleEngineeringInputData VehicleInputData
		{
			get { return VehicleData; }
		}

		public IGearboxEngineeringInputData GearboxInputData
		{
			get { return Gearbox; }
		}

		public ITorqueConverterEngineeringInputData TorqueConverterInputData
		{
			get { return TorqueConverter; }
		}

		public IAxleGearInputData AxleGearInputData
		{
			get { return AxleGear; }
		}

		public IAngularGearInputData AngularGearInputData
		{
			get { return AngularGear; }
		}

		IEngineDeclarationInputData IDeclarationInputDataProvider.EngineInputData
		{
			get { return EngineInputData; }
		}

		public IEngineEngineeringInputData EngineInputData
		{
			get { return Engine; }
		}

		public IAuxiliariesEngineeringInputData AuxiliaryInputData()
		{
			throw new NotImplementedException();
		}

		IAuxiliariesDeclarationInputData IDeclarationInputDataProvider.AuxiliaryInputData()
		{
			throw new NotImplementedException();
		}

		public IRetarderInputData RetarderInputData
		{
			get { return Retarder; }
		}

		IDriverDeclarationInputData IDeclarationInputDataProvider.DriverInputData
		{
			get { throw new NotImplementedException(); }
		}

		public IDriverEngineeringInputData DriverInputData
		{
			get { return DriverInputData; }
		}

		public IPTOTransmissionInputData PTOTransmissionInputData
		{
			get { return PTOTransmission; }
		}
	}
}