/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONComponentInputData : IEngineeringInputDataProvider, IDeclarationInputDataProvider,
		IEngineeringJobInputData, IVehicleEngineeringInputData
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
		private string _filename;


		public JSONComponentInputData(string filename, bool tolerateMissing = false)
		{
			var extension = Path.GetExtension(filename);
			object tmp = null;
			switch (extension) {
				case Constants.FileExtensions.VehicleDataFile:
					tmp = JSONInputDataFactory.ReadJsonVehicle(filename, null, tolerateMissing);
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
			_filename = filename;
		}


		public IEngineeringJobInputData JobInputData
		{
			get { return this; }
		}

		public XElement XMLHash
		{
			get { return new XElement(XMLNames.DI_Signature); }
		}


		IDeclarationJobInputData IDeclarationInputDataProvider.JobInputData
		{
			get { return this; }
		}

		public IDriverEngineeringInputData DriverInputData
		{
			get { return null; }
		}

		public DataSourceType SourceType
		{
			get { return DataSourceType.JSONFile; }
		}

		public string Source
		{
			get { return _filename; }
		}

		public bool SavedInDeclarationMode { get; private set; }
		public string Manufacturer { get; private set; }
		public string Model { get; private set; }
		public string Date { get; private set; }
		public CertificationMethod CertificationMethod { get; private set; }
		public string CertificationNumber { get; private set; }
		public string DigestValue { get; private set; }

		IVehicleDeclarationInputData IDeclarationJobInputData.Vehicle
		{
			get { return Vehicle; }
		}

		public IVehicleEngineeringInputData Vehicle
		{
			get { return VehicleData ?? this; }
		}

		public IList<ICycleData> Cycles { get; private set; }

		public bool EngineOnlyMode { get; private set; }

		public IEngineEngineeringInputData EngineOnly { get; private set; }


		public string JobName
		{
			get { return ""; }
		}

		public string VIN
		{
			get { return Vehicle.VIN; }
		}

		public LegislativeClass LegislativeClass
		{
			get { return Vehicle.LegislativeClass; }
		}

		public VehicleCategory VehicleCategory
		{
			get { return Vehicle.VehicleCategory; }
		}

		public AxleConfiguration AxleConfiguration
		{
			get { return Vehicle.AxleConfiguration; }
		}

		public Kilogram CurbMassChassis
		{
			get { return Vehicle.CurbMassChassis; }
		}

		public Kilogram GrossVehicleMassRating
		{
			get { return Vehicle.GrossVehicleMassRating; }
		}

		public IList<ITorqueLimitInputData> TorqueLimits
		{
			get { return Vehicle.TorqueLimits; }
		}

		IList<IAxleEngineeringInputData> IVehicleEngineeringInputData.Axles
		{
			get { return Vehicle.Axles; }
		}

		public Meter DynamicTyreRadius
		{
			get { return Vehicle.DynamicTyreRadius; }
		}

		public Meter Height
		{
			get { return Vehicle.Height; }
		}

		public IAirdragEngineeringInputData AirdragInputData
		{
			get { return AirdragData; }
		}

		public IGearboxEngineeringInputData GearboxInputData
		{
			get { return Gearbox; }
		}

		public ITorqueConverterEngineeringInputData TorqueConverterInputData
		{
			get { return TorqueConverter; }
		}

		IAxleGearInputData IVehicleEngineeringInputData.AxleGearInputData
		{
			get { return AxleGear; }
		}

		IAngledriveInputData IVehicleEngineeringInputData.AngledriveInputData
		{
			get { return Angledrive; }
		}

		public Kilogram CurbMassExtra
		{
			get { return Vehicle.CurbMassExtra; }
		}

		public Kilogram Loading
		{
			get { return Vehicle.Loading; }
		}

		IList<IAxleDeclarationInputData> IVehicleDeclarationInputData.Axles
		{
			get { return Vehicle.Axles.Cast<IAxleDeclarationInputData>().ToList(); }
		}

		public string ManufacturerAddress
		{
			get { return Vehicle.ManufacturerAddress; }
		}

		public PerSecond EngineIdleSpeed
		{
			get { return Vehicle.EngineIdleSpeed; }
		}

		IAirdragDeclarationInputData IVehicleDeclarationInputData.AirdragInputData
		{
			get { return AirdragInputData; }
		}

		IGearboxDeclarationInputData IVehicleDeclarationInputData.GearboxInputData
		{
			get { return GearboxInputData; }
		}

		ITorqueConverterDeclarationInputData IVehicleDeclarationInputData.TorqueConverterInputData
		{
			get { return TorqueConverterInputData; }
		}

		IAxleGearInputData IVehicleDeclarationInputData.AxleGearInputData
		{
			get { return AxleGear; }
		}

		IAngledriveInputData IVehicleDeclarationInputData.AngledriveInputData
		{
			get { return Angledrive; }
		}

		public IEngineEngineeringInputData EngineInputData
		{
			get { return Engine; }
		}


		IEngineDeclarationInputData IVehicleDeclarationInputData.EngineInputData
		{
			get { return Engine; }
		}

		IAuxiliariesDeclarationInputData IVehicleDeclarationInputData.AuxiliaryInputData()
		{
			throw new NotImplementedException();
		}

		IRetarderInputData IVehicleEngineeringInputData.RetarderInputData
		{
			get { return Retarder; }
		}

		IPTOTransmissionInputData IVehicleEngineeringInputData.PTOTransmissionInputData
		{
			get { return PTOTransmission; }
		}

		IAuxiliariesEngineeringInputData IVehicleEngineeringInputData.AuxiliaryInputData()
		{
			throw new NotImplementedException();
		}

		IRetarderInputData IVehicleDeclarationInputData.RetarderInputData
		{
			get { return Retarder; }
		}

		IPTOTransmissionInputData IVehicleDeclarationInputData.PTOTransmissionInputData
		{
			get { return PTOTransmission; }
		}
	}
}
