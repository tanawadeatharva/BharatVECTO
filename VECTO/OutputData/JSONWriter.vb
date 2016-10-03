Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json.Linq
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCore.InputData.Impl
Imports TUGraz.VectoCore.Models.SimulationComponent.Data

Public Class JSONFileWriter
	Public Const EngineFormatVersion As Short = 3

	Public Const GearboxFormatVersion As Short = 6

	Public Const VehicleFormatVersion As Short = 7

	Private Const VectoJobFormatVersion As Short = 3

	Private Shared _instance As JSONFileWriter

	Public Shared ReadOnly Property Instance As JSONFileWriter
		Get
			If _instance Is Nothing Then _instance = New JSONFileWriter()
			Return _instance
		End Get
	End Property

	Public Function SaveEngine(eng As IEngineEngineeringInputData, filename As String) As Boolean
		Dim json As New JSONWriter

		'Header
		Dim header As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
		header.Add("CreatedBy", Lic.LicString & " (" & Lic.GUID & ")")
		header.Add("Date", Now.ToUniversalTime().ToString("o"))
		header.Add("AppVersion", VECTOvers)
		header.Add("FileVersion", EngineFormatVersion)

		'Body
		Dim body As Dictionary(Of String, Object) = New Dictionary(Of String, Object)

		body.Add("SavedInDeclMode", Cfg.DeclMode)

		body.Add("ModelName", eng.ModelName)

		body.Add("Displacement", eng.Displacement.Value())
		body.Add("IdlingSpeed", eng.IdleSpeed.Value())
		body.Add("Inertia", eng.Inertia.Value())

		body.Add("FullLoadCurve", eng.FullLoadCurve.Source)

		body.Add("FuelMap", eng.FuelConsumptionMap.Source)

		body.Add("WHTC-Urban", eng.WHTCUrban)
		body.Add("WHTC-Rural", eng.WHTCRural)
		body.Add("WHTC-Motorway", eng.WHTCMotorway)
		body.Add("ColdHotBalancingFactor", eng.ColdHotBalancingFactor)

		json.Content = JToken.FromObject(New Dictionary(Of String, Object) From {{"Header", header}, {"Body", body}})

		Return json.WriteFile(filename)
	End Function

	Public Function SaveGearbox(gbx As IGearboxEngineeringInputData, filename As String) As Boolean

		Dim i As Integer
		Dim json As New JSONWriter

		'Header
		Dim header As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
		header.Add("CreatedBy", Lic.LicString & " (" & Lic.GUID & ")")
		header.Add("Date", Now.ToUniversalTime().ToString("o"))
		header.Add("AppVersion", VECTOvers)
		header.Add("FileVersion", GearboxFormatVersion)


		'Body
		Dim body As Dictionary(Of String, Object) = New Dictionary(Of String, Object)

		body.Add("SavedInDeclMode", Cfg.DeclMode)

		body.Add("ModelName", gbx.ModelName)

		body.Add("Inertia", gbx.Inertia.Value())
		body.Add("TracInt", gbx.TractionInterruption.Value())

		Dim ls As New List(Of Dictionary(Of String, Object))
		For Each gear As ITransmissionInputData In gbx.Gears
			Dim gearDict As New Dictionary(Of String, Object)
			gearDict.Add("Ratio", gear.Ratio)
			If gear.LossMap Is Nothing Then
				gearDict.Add("Efficiency", gear.Efficiency)
			Else
				gearDict.Add("LossMap", gear.LossMap.Source)
			End If
			If i > 0 Then
				gearDict.Add("ShiftPolygon", gear.ShiftPolygon.Source)
				gearDict.Add("MaxTorque", gear.MaxTorque.Value())
			End If

			ls.Add(gearDict)
		Next
		body.Add("Gears", ls)

		body.Add("TqReserve", gbx.TorqueReserve)
		'body.Add("SkipGears", gbx.sk)
		body.Add("ShiftTime", gbx.ShiftTime.Value())
		'body.Add("EaryShiftUp", gbx.ShiftInside)

		body.Add("StartTqReserve", gbx.StartTorqueReserve)
		body.Add("StartSpeed", gbx.StartSpeed)
		body.Add("StartAcc", gbx.StartAcceleration.Value())

		body.Add("GearboxType", gbx.Type)

		Dim torqueConverter As ITorqueConverterEngineeringInputData = gbx.TorqueConverter
		Dim torqueConverterDict As New Dictionary(Of String, Object)
		torqueConverterDict.Add("Enabled", Not torqueConverter Is Nothing)
		If Not torqueConverter Is Nothing Then
			torqueConverterDict.Add("File", torqueConverter.TCData.Source)
			torqueConverterDict.Add("RefRPM", torqueConverter.ReferenceRPM.AsRPM)
			torqueConverterDict.Add("Inertia", torqueConverter.Inertia.Value())
			torqueConverterDict.Add("ShiftPolygon", torqueConverter.ShiftPolygon.Source)
		End If
		body.Add("TorqueConverter", torqueConverterDict)


		body.Add("DownshiftAferUpshiftDelay", gbx.DownshiftAferUpshiftDelay.Value())
		body.Add("UpshiftAfterDownshiftDelay", gbx.UpshiftAfterDownshiftDelay.Value())
		body.Add("UpshiftMinAcceleration", gbx.UpshiftMinAcceleration)

		json.Content = JToken.FromObject(New Dictionary(Of String, Object) From {{"Header", header}, {"Body", body}})

		Return json.WriteFile(filename)
	End Function

	Public Function SaveVehicle(vehicle As IVehicleEngineeringInputData, retarder As IRetarderInputData,
								pto As IPTOTransmissionInputData, angledrive As IAngledriveInputData, filename As String) As Boolean
		Dim json As New JSONWriter
		'Header
		Dim header As Dictionary(Of String, Object) = New Dictionary(Of String, Object) From {
				{"CreatedBy", Lic.LicString & " (" & Lic.GUID & ")"},
				{"Date", Now.ToUniversalTime().ToString("o")},
				{"AppVersion", VECTOvers},
				{"FileVersion", VehicleFormatVersion}}

		'Body


		Dim retarderOut As Dictionary(Of String, Object) = New Dictionary(Of String, Object)()
		If retarder Is Nothing Then
			retarderOut.Add("Type", RetarderType.None.GetName())
		Else
			retarderOut.Add("Type", retarder.Type.GetName())
			retarderOut.Add("Ratio", retarder.Ratio)
			If Not retarder.LossMap Is Nothing Then
				retarderOut.Add("File", retarder.LossMap.Source)
			End If
		End If

		Dim ptoOut As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
		If pto Is Nothing Then
			ptoOut.Add("Type", "None")
		Else
			ptoOut.Add("Type", pto.PTOTransmissionType)
			ptoOut.Add("LossMap", pto.PTOLossMap.Source)
			ptoOut.Add("Cycle", pto.PTOCycle.Source)
		End If

		Dim angledriveOut As Dictionary(Of String, Object) = New Dictionary(Of String, Object) From {
				{"Type", angledrive.Type.ToString()},
				{"Ratio", angledrive.Ratio},
				{"LossMap", angledrive.LossMap.Source}}

		Dim body As Dictionary(Of String, Object) = New Dictionary(Of String, Object) From {
				{"SavedInDeclMode", Cfg.DeclMode},
				{"VehCat", vehicle.VehicleCategory.ToString()},
				{"CurbWeight", vehicle.CurbWeightChassis.Value()},
				{"CurbWeightExtra", vehicle.CurbWeightExtra},
				{"Loading", vehicle.Loading},
				{"MassMax", vehicle.GrossVehicleMassRating.ConvertTo().Ton.Value()},
				{"CdA", vehicle.AirDragArea.Value()},
				{"rdyn", vehicle.DynamicTyreRadius},
				{"CdCorrMode", vehicle.CrossWindCorrectionMode.GetName()},
				{"CdCorrFile", vehicle.CrosswindCorrectionMap.Source},
				{"Retarder", retarderOut},
				{"Angledrive", angledriveOut},
				{"PTO", ptoOut},
				{"AxleConfig", New Dictionary(Of String, Object) From {
				{"Type", vehicle.AxleConfiguration.GetName()},
				{"Axles", (From axle In vehicle.Axles Select New Dictionary(Of String, Object) From {
				{"Inertia", axle.Inertia},
				{"Wheels", axle.Wheels},
				{"AxleWeightShare", axle.AxleWeightShare},
				{"TwinTyres", axle.TwinTyres},
				{"RRCISO", axle.RollResistanceCoefficient},
				{"FzISO", axle.TyreTestLoad.Value()}
				}
				)}
				}
				}
				}

		json.Content = JToken.FromObject(New Dictionary(Of String, Object) From {{"Header", header}, {"Body", body}})
		Return json.WriteFile(filename)
	End Function

	Public Function SaveJob(input As IEngineeringInputDataProvider, filename As String) As Boolean
		Dim json As New JSONWriter

		'Header
		Dim header As Dictionary(Of String, Object) = New Dictionary(Of String, Object) From {
				{"CreatedBy", Lic.LicString & " (" & Lic.GUID & ")"},
				{"Date", Now.ToUniversalTime().ToString("o")},
				{"AppVersion", VECTOvers},
				{"FileVersion", VectoJobFormatVersion}}

		'Body
		Dim body As Dictionary(Of String, Object) = New Dictionary(Of String, Object)

		body.Add("SavedInDeclMode", Cfg.DeclMode)
		'SavedInDeclMode = Cfg.DeclMode

		Dim job As IEngineeringJobInputData = input.JobInputData()
		Dim aux As IAuxiliariesEngineeringInputData = input.AuxiliaryInputData()
		Dim driver As IDriverEngineeringInputData = input.DriverInputData

		body.Add("EngineOnlyMode", job.EngineOnlyMode)

		If job.EngineOnlyMode Then
			body.Add("EngineFile", input.EngineInputData.Source)
			For Each cycle As ICycleData In job.Cycles
				body.Add("Cycles", cycle.CycleData.Source)
			Next
			Return True
		End If

		'Main Files
		body.Add("VehicleFile", job.Vehicle.Source)
		body.Add("EngineFile", input.EngineInputData.Source)
		body.Add("GearboxFile", input.GearboxInputData.Source)

		'Cycles
		For Each cycle As ICycleData In job.Cycles
			body.Add("Cycles", cycle.CycleData.Source)
		Next

		'AA-TB
		'ADVANCED AUXILIARIES 
		body.Add("AuxiliaryAssembly", aux.AuxiliaryAssembly)
		body.Add("AuxiliaryVersion", aux.AuxiliaryVersion)
		body.Add("AdvancedAuxiliaryFilePath", aux.AdvancedAuxiliaryFilePath)

		Dim pAdd As Double = 0.0
		Dim auxOut As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
		For Each auxEntry As IAuxiliaryEngineeringInputData In aux.Auxiliaries
			If auxEntry.AuxiliaryType = AuxiliaryDemandType.Constant Then
				pAdd += auxEntry.ConstantPowerDemand.Value()
				Continue For
			End If
			Dim engineeringAuxEntry As IAuxiliaryDeclarationInputData = TryCast(auxEntry, IAuxiliaryDeclarationInputData)
			If engineeringAuxEntry Is Nothing Then
				auxOut.Add("Type", auxEntry.AuxiliaryType.ToString())
				auxOut.Add("Path", auxEntry.DemandMap.Source)
			Else
				auxOut.Add("ID", auxEntry.ID)
				auxOut.Add("Technology", engineeringAuxEntry.Technology)
			End If

		Next

		body.Add("Aux", auxOut)

		If Not Cfg.DeclMode Then
			body.Add("Padd", pAdd)
		End If
		body.Add("VACC", driver.AccelerationCurve.Source)
		body.Add("StartStop", New Dictionary(Of String, Object) From {
					{"Enabled", driver.StartStop.Enabled},
					{"MaxSpeed", driver.StartStop.MaxSpeed.Value()},
					{"MinTime", driver.StartStop.MinTime.Value()},
					{"Delay", driver.StartStop.Delay.Value()}})
		body.Add("LAC", New Dictionary(Of String, Object) From {
					{"Enabled", driver.Lookahead.Enabled},
					{"PreviewDistanceFactor", driver.Lookahead.LookaheadDistanceFactor},
					{"DF_offset", driver.Lookahead.CoastingDecisionFactorOffset},
					{"DF_scaling", driver.Lookahead.CoastingDecisionFactorScaling},
					{"DF_targetSpeedLookup", driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup.Source},
					{"Df_velocityDropLookup", driver.Lookahead.CoastingDecisionFactorVelocityDropLookup.Source}})

		'Overspeed / EcoRoll
		Dim overspeedDic As Dictionary(Of String, Object) = New Dictionary(Of String, Object)

		overspeedDic.Add("Mode", driver.OverSpeedEcoRoll.Mode.ToString())

		overspeedDic.Add("MinSpeed", driver.OverSpeedEcoRoll.MinSpeed.Value())
		overspeedDic.Add("OverSpeed", driver.OverSpeedEcoRoll.OverSpeed.Value())
		overspeedDic.Add("UnderSpeed", driver.OverSpeedEcoRoll.UnderSpeed.Value())
		body.Add("OverSpeedEcoRoll", overspeedDic)

		json.Content = JToken.FromObject(New Dictionary(Of String, Object) From {{"Header", header}, {"Body", body}})
		Return json.WriteFile(filename)
	End Function
End Class