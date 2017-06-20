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

using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	// ReSharper disable once InconsistentNaming
	public class XMLEngineeringJobInputDataProvider : AbstractEngineeringXMLComponentDataProvider, IEngineeringJobInputData
	{
		public XMLEngineeringJobInputDataProvider(XMLEngineeringInputDataProvider xmlEngineeringJobInputDataProvider,
			XPathDocument jobDocument, string xmlBasePath, string fsBasePath)
			: base(xmlEngineeringJobInputDataProvider, jobDocument, xmlBasePath, fsBasePath) {}


		public IVehicleDeclarationInputData Vehicle
		{
			get { return InputData.VehicleInputData; }
		}

		IVehicleEngineeringInputData IEngineeringJobInputData.Vehicle
		{
			get { return InputData.VehicleInputData; }
		}

		//IEngineEngineeringInputData EngineInputData
		//{
		//	get { return new XMLEngineeringEngineDataProvider(InputData, XMLDocument, Helper.Query(XBasePath, XMLNames.Component_Engine, XMLNames.ComponentDataWrapper) ,FSBasePath); } 
		//}

		public IList<ICycleData> Cycles
		{
			get {
				var retVal = new List<ICycleData>();
				var cycles = Navigator.Select(Helper.Query(XBasePath, XMLNames.VectoJob_MissionCycles,
					Helper.QueryConstraint(XMLNames.Missions_Cycle, XMLNames.ExtResource_Type_Attr,
						XMLNames.ExtResource_Type_Value_CSV)), Manager);
				while (cycles.MoveNext()) {
					var file = cycles.Current.GetAttribute(XMLNames.ExtResource_File_Attr, "");
					var fileFull = Path.Combine(FSBasePath ?? "", file);
					if (File.Exists(fileFull)) {
						retVal.Add(new CycleInputData() {
							Name = Path.GetFileNameWithoutExtension(fileFull),
							CycleData = VectoCSVFile.Read(fileFull)
						});
					} else {
						try {
							var resourceName = DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." + file +
												Constants.FileExtensions.CycleFile;
							retVal.Add(new CycleInputData() {
								Name = Path.GetFileNameWithoutExtension(file),
								CycleData = VectoCSVFile.ReadStream(RessourceHelper.ReadStream(resourceName), source: resourceName),
							});
						} catch {
							//Log.Debug("Driving Cycle could not be read: " + cycleFile);
							throw new VectoException("Driving Cycle could not be read: " + file);
						}
					}
				}
				return retVal;
			}
		}

		public bool EngineOnlyMode
		{
			get {
				return ElementExists(XMLNames.VectoJob_EngineOnlyMode) &&
						XmlConvert.ToBoolean(GetElementValue(XMLNames.VectoJob_EngineOnlyMode));
			}
		}

		protected internal Second DownshiftAfterUpshiftDelay
		{
			get {
				return
					ElementExists(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
						"DownshiftAfterUpshiftDelay"))
						? GetDoubleElementValue(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
							"DownshiftAfterUpshiftDelay")).SI<Second>()
						: DeclarationData.Gearbox.DownshiftAfterUpshiftDelay;
			}
		}

		protected internal Second UpshiftAfterDownshiftDelay
		{
			get {
				return
					ElementExists(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
						"UpshiftAfterDownshiftDelay"))
						? GetDoubleElementValue(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
							"UpshiftAfterDownshiftDelay")).SI<Second>()
						: DeclarationData.Gearbox.UpshiftAfterDownshiftDelay;
			}
		}

		protected internal MeterPerSquareSecond UpshiftMinAcceleration
		{
			get {
				return
					ElementExists(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
						"UpshiftMinAcceleration"))
						? GetDoubleElementValue(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
							"UpshiftMinAcceleration"))
							.SI<MeterPerSquareSecond>()
						: DeclarationData.Gearbox.UpshiftMinAcceleration;
			}
		}


		public string JobName
		{
			get {
				return InputData.JobInputData().EngineOnlyMode
					? InputData.EngineInputData.Model
					: InputData._vehicleInputData.GetVehicleID;
			}
		}

		public double TorqueReserve
		{
			get {
				return
					ElementExists(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
						XMLNames.DriverModel_ShiftStrategyParameters_TorqueReserve))
						? GetDoubleElementValue(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
							XMLNames.DriverModel_ShiftStrategyParameters_TorqueReserve))
						: DeclarationData.Gearbox.TorqueReserve;
			}
		}

		public Second MinTimeBetweenGearshift
		{
			get {
				return
					ElementExists(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
						XMLNames.DriverModel_ShiftStrategyParameters_TimeBetweenGearshift))
						? GetDoubleElementValue(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
							XMLNames.DriverModel_ShiftStrategyParameters_TimeBetweenGearshift)).SI<Second>()
						: DeclarationData.Gearbox.MinTimeBetweenGearshifts;
			}
		}

		public MeterPerSecond StartSpeed
		{
			get {
				return
					ElementExists(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
						XMLNames.DriverModel_ShiftStrategyParameters_StartSpeed))
						? GetDoubleElementValue(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
							XMLNames.DriverModel_ShiftStrategyParameters_StartSpeed)).SI<MeterPerSecond>()
						: DeclarationData.Gearbox.StartSpeed;
			}
		}

		public MeterPerSquareSecond StartAcceleration
		{
			get {
				return
					ElementExists(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
						XMLNames.DriverModel_ShiftStrategyParameters_StartAcceleration))
						? GetDoubleElementValue(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
							XMLNames.DriverModel_ShiftStrategyParameters_StartAcceleration)).SI<MeterPerSquareSecond>()
						: DeclarationData.Gearbox.StartAcceleration;
			}
		}

		public double StartTorqueReserve
		{
			get {
				return
					ElementExists(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
						XMLNames.DriverModel_ShiftStrategyParameters_StartTorqueReserve))
						? GetDoubleElementValue(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
							XMLNames.DriverModel_ShiftStrategyParameters_StartTorqueReserve))
						: DeclarationData.Gearbox.TorqueReserveStart;
			}
		}


		public Second PowershiftShiftTime
		{
			get {
				return
					ElementExists(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
						"PowershiftShiftTime"))
						? GetDoubleElementValue(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
							"PowershiftShiftTime")).SI<Second>()
						: 0.8.SI<Second>();
			}
		}

		public MeterPerSquareSecond CCUpshiftMinAcceleration
		{
			get {
				return
					ElementExists(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
						"CCUpshiftMinAcceleration"))
						? GetDoubleElementValue(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
							"CCUpshiftMinAcceleration")).SI<MeterPerSquareSecond>()
						: DeclarationData.Gearbox.UpshiftMinAcceleration;
			}
		}

		public MeterPerSquareSecond CLUpshiftMinAcceleration
		{
			get {
				return
					ElementExists(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
						"CLUpshiftMinAcceleration"))
						? GetDoubleElementValue(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_ShiftStrategyParameters,
							"CLUpshiftMinAcceleration")).SI<MeterPerSquareSecond>()
						: DeclarationData.Gearbox.UpshiftMinAcceleration;
			}
		}

		public XMLEngineeringDriverDataProvider GetDriverData(XmlReaderSettings settings)
		{
			return new XMLEngineeringDriverDataProvider(InputData, XMLDocument, XBasePath, FSBasePath);
		}
	}
}