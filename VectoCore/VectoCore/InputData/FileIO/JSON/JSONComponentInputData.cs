/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.IO;
using System.Xml.Linq;
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
		protected IAngledriveInputData Angledrive;
		protected IEngineEngineeringInputData Engine;
		protected IVehicleEngineeringInputData VehicleData;
		protected IRetarderInputData Retarder;
		protected IPTOTransmissionInputData PTOTransmission;
		private IAirdragEngineeringInputData AirdragData;


		public JSONComponentInputData(string filename, bool tolerateMissing = false)
		{
			var extension = Path.GetExtension(filename);
			object tmp = null;
			switch (extension) {
				case Constants.FileExtensions.VehicleDataFile:
					tmp = JSONInputDataFactory.ReadJsonVehicle(filename, tolerateMissing);
					break;
				case Constants.FileExtensions.EngineDataFile:
					tmp = JSONInputDataFactory.ReadEngine(filename, tolerateMissing);
					break;
				case Constants.FileExtensions.GearboxDataFile:
					tmp = JSONInputDataFactory.ReadGearbox(filename, tolerateMissing);
					break;
			}
			tmp.Switch()
				.If<IVehicleEngineeringInputData>(c => VehicleData = c)
				.If<IAirdragEngineeringInputData>(c => AirdragData = c)
				.If<IEngineEngineeringInputData>(c => Engine = c)
				.If<IGearboxEngineeringInputData>(c => Gearbox = c)
				.If<IAxleGearInputData>(c => AxleGear = c)
				.If<IRetarderInputData>(c => Retarder = c)
				.If<ITorqueConverterEngineeringInputData>(c => TorqueConverter = c)
				.If<IAngledriveInputData>(c => Angledrive = c)
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

		IAirdragDeclarationInputData IDeclarationInputDataProvider.AirdragInputData
		{
			get { return AirdragInputData; }
		}

		public IAirdragEngineeringInputData AirdragInputData
		{
			get { return AirdragData; }
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

		public IAngledriveInputData AngledriveInputData
		{
			get { return Angledrive; }
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

		public XElement XMLHash { get { return null; } }
	}
}