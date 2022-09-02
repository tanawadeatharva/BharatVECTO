/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
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
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.InputData.Reader.ShiftStrategy;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	
	public partial class DeclarationDataAdapterHeavyLorry : AbstractSimulationDataAdapter, IDeclarationDataAdapter
	{
		[Obsolete("Use DeclarationDataAdapterHeavyLorry.Conventional instead, created automatically with NInject")]
		public DeclarationDataAdapterHeavyLorry()
		{
			
		}
		private IDeclarationDataAdapter _declarationDataAdapterImplementation = new DeclarationDataAdapterHeavyLorry.Conventional();
		public static readonly GearboxType[] SupportedGearboxTypes = Conventional.SupportedGearboxTypes;
		public static List<CrossWindCorrectionCurveReader.CrossWindCorrectionEntry> GetDeclarationAirResistanceCurve(string parameterSet, SquareMeter si, Meter meter)
		{
			return Conventional.GetDeclarationAirResistanceCurve(parameterSet, si, meter);
		}
		#region Implementation of IDeclarationDataAdapter

		public DriverData CreateDriverData()
		{
			return _declarationDataAdapterImplementation.CreateDriverData();
		}

		public VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission,
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
		{
			return _declarationDataAdapterImplementation.CreateVehicleData(vehicle, segment, mission, loading, allowVocational);
		}

		public AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, Segment segment)
		{
			return _declarationDataAdapterImplementation.CreateAirdragData(airdragData, mission, segment);
		}

		public AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData)
		{
			return _declarationDataAdapterImplementation.CreateAxleGearData(axlegearData);
		}

		public AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData)
		{
			return _declarationDataAdapterImplementation.CreateAngledriveData(angledriveData);
		}

		public CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle, IEngineModeDeclarationInputData engineMode,
			Mission mission)
		{
			return _declarationDataAdapterImplementation.CreateEngineData(vehicle, engineMode, mission);
		}

		public GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalc)
		{
			return _declarationDataAdapterImplementation.CreateGearboxData(inputData, runData, shiftPolygonCalc);
		}

		public ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed)
		{
			return _declarationDataAdapterImplementation.CreateGearshiftData(gbx, axleRatio, engineIdlingSpeed);
		}

		public RetarderData CreateRetarderData(IRetarderInputData retarderData)
		{
			return _declarationDataAdapterImplementation.CreateRetarderData(retarderData);
		}

		public PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData)
		{
			return _declarationDataAdapterImplementation.CreatePTOTransmissionData(ptoData);
		}

		public IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData, IBusAuxiliariesDeclarationData busAuxData,
			MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles)
		{
			return _declarationDataAdapterImplementation.CreateAuxiliaryData(auxData, busAuxData, missionType, vehicleClass, vehicleLength, numSteeredAxles);
		}

		public AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData)
		{
			return _declarationDataAdapterImplementation.CreateDummyAxleGearData(gbxData);
		}

		#endregion


	}
}
