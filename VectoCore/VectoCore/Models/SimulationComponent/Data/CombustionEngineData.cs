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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	[CustomValidation(typeof(CombustionEngineData), "ValidateData")]
	public class CombustionEngineData : SimulationComponentData
	{
		[Required, SIRange(1000 * 1E-6, 20000 * 1E-6)]
		public CubicMeter Displacement { get; internal set; }

		[Required, SIRange(400 * Constants.RPMToRad, 1000 * Constants.RPMToRad)]
		public PerSecond IdleSpeed { get; internal set; }

		[Required, SIRange(0, 10)]
		public KilogramSquareMeter Inertia { get; internal set; }

		[Required, Range(0.9, 2)]
		public double WHTCUrban { get; internal set; }

		[Required, Range(0.9, 2)]
		public double WHTCRural { get; internal set; }

		[Required, Range(0.9, 2)]
		public double WHTCMotorway { get; internal set; }

		[Required, ValidateObject]
		public FuelConsumptionMap ConsumptionMap { get; internal set; }

		[Required, ValidateObject]
		public Dictionary<uint, EngineFullLoadCurve> FullLoadCurves { get; internal set; }

		[Required, Range(double.MinValue, double.MaxValue)]
		public double ColdHotCorrectionFactor { get; internal set; }

		[Required, Range(double.MinValue, double.MaxValue)]
		public double CorrectionFactorRegPer { get; internal set; }

		[Required, Range(double.MinValue, double.MaxValue)]
		public double CorrectionFactorNCV { get; internal set; }

		public double FuelConsumptionCorrectionFactor { get; internal set; }

		public PerSecond RatedSpeedDeclared { get; internal set; }

		public Watt RatedPowerDeclared { get; internal set; }

		public NewtonMeter MaxTorqueDeclared { get; internal set; }

		public FuelType FuelType { get; internal set; }

		public CombustionEngineData()
		{
			WHTCUrban = 1;
			WHTCMotorway = 1;
			WHTCRural = 1;
			CorrectionFactorNCV = 1;
			CorrectionFactorRegPer = 1;
			FuelConsumptionCorrectionFactor = 1;
		}

		public CombustionEngineData Copy()
		{
			return new CombustionEngineData {
				Manufacturer = Manufacturer,
				ModelName = ModelName,
				Displacement = Displacement,
				IdleSpeed = IdleSpeed,
				Inertia = Inertia,
				WHTCUrban = WHTCUrban,
				WHTCRural = WHTCRural,
				WHTCMotorway = WHTCMotorway,
				ConsumptionMap = ConsumptionMap,
				FullLoadCurves = FullLoadCurves,
				CorrectionFactorRegPer = CorrectionFactorRegPer,
				CorrectionFactorNCV = CorrectionFactorNCV,
				ColdHotCorrectionFactor = ColdHotCorrectionFactor,
				FuelConsumptionCorrectionFactor = FuelConsumptionCorrectionFactor,
				RatedPowerDeclared = RatedPowerDeclared,
				RatedSpeedDeclared = RatedSpeedDeclared,
				MaxTorqueDeclared = MaxTorqueDeclared,
				FuelType = FuelType
			};
		}

		// ReSharper disable once UnusedMember.Global -- used in CustomValidation
		public static ValidationResult ValidateData(CombustionEngineData data, ValidationContext context)
		{
			if (data.Inertia.IsEqual(0)) {
				LogManager.GetLogger(typeof(CombustionEngineData).FullName).Error("Warning: Engine Inertia is 0!");
			}
			return ValidationResult.Success;
		}
	}
}
