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

namespace TUGraz.VectoCommon.InputData
{
	public interface IInputDataProvider {}

	public interface IDeclarationInputDataProvider : IInputDataProvider
	{
		IDeclarationJobInputData JobInputData();

		IVehicleDeclarationInputData VehicleInputData { get; }

		IGearboxDeclarationInputData GearboxInputData { get; }

		ITorqueConverterDeclarationInputData TorqueConverterInputData { get; }

		IAxleGearInputData AxleGearInputData { get; }

		IAngularGearInputData AngularGearInputData { get; }

		IEngineDeclarationInputData EngineInputData { get; }

		IAuxiliariesDeclarationInputData AuxiliaryInputData();

		IRetarderInputData RetarderInputData { get; }

		IDriverDeclarationInputData DriverInputData { get; }
	}

	public interface IEngineeringInputDataProvider : IInputDataProvider
	{
		IEngineeringJobInputData JobInputData();

		IVehicleEngineeringInputData VehicleInputData { get; }

		IGearboxEngineeringInputData GearboxInputData { get; }

		ITorqueConverterEngineeringInputData TorqueConverterInputData { get; }

		IAxleGearInputData AxleGearInputData { get; }

		IAngularGearInputData AngularGearInputData { get; }

		IEngineEngineeringInputData EngineInputData { get; }

		IAuxiliariesEngineeringInputData AuxiliaryInputData();

		IRetarderInputData RetarderInputData { get; }

		IDriverEngineeringInputData DriverInputData { get; }

		IPTOTransmissionInputData PTOTransmissionInputData { get; }
	}
}