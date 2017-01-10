Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.OutputData
Imports TUGraz.VectoCore
Imports TUGraz.VectoCore.Models.Declaration

Public Class JSONFileWriter
	Implements IOutputFileWriter
	Public Const EngineFormatVersion As Integer = 3

	Public Const GearboxFormatVersion As Integer = 6

	Public Const VehicleFormatVersion As Integer = 7

	Private Const VectoJobFormatVersion As Integer = 3

	Private Shared _instance As JSONFileWriter

	Public Shared ReadOnly Property Instance As JSONFileWriter
		Get
			If _instance Is Nothing Then _instance = New JSONFileWriter()
			Return _instance
		End Get
	End Property

	Public Sub SaveEngine(eng As IEngineEngineeringInputData, filename As String) _
		Implements IOutputFileWriter.SaveEngine

		'Header
		Dim header As Dictionary(Of String, Object) = GetHeader(EngineFormatVersion)

		'Body
		Dim body As Dictionary(Of String, Object) = New Dictionary(Of String, Object)

		body.Add("SavedInDeclMode", Cfg.DeclMode)

		body.Add("ModelName", eng.ModelName)

		body.Add("Displacement", eng.Displacement.ConvertTo().Cubic.Centi.Meter.Value().ToString())
		body.Add("IdlingSpeed", eng.IdleSpeed.AsRPM)
		body.Add("Inertia", eng.Inertia.Value())

		body.Add("FullLoadCurve", GetRelativePath(eng.FullLoadCurve.Source, Path.GetDirectoryName(filename)))

		body.Add("FuelMap", GetRelativePath(eng.FuelConsumptionMap.Source, Path.GetDirectoryName(filename)))

		body.Add("WHTC-Urban", eng.WHTCUrban)
		body.Add("WHTC-Rural", eng.WHTCRural)
		body.Add("WHTC-Motorway", eng.WHTCMotorway)
		body.Add("WHTC-Engineering", eng.WHTCEngineering)
		body.Add("ColdHotBalancingFactor", eng.ColdHotBalancingFactor)

		WriteFile(header, body, filename)
	End Sub

	Protected Function GetHeader(fileVersion As Integer) As Dictionary(Of String, Object)
		Dim header As Dictionary(Of String, Object) = New Dictionary(Of String, Object)

		header.Add("CreatedBy", Lic.LicString & " (" & Lic.GUID & ")")
		header.Add("Date", Now.ToUniversalTime().ToString("o"))
		header.Add("AppVersion", VECTOvers)
		header.Add("FileVersion", fileVersion)
		Return header
	End Function

	Public Sub SaveGearbox(gbx As IGearboxEngineeringInputData, axl As IAxleGearInputData, filename As String) _
		Implements IOutputFileWriter.SaveGearbox

		'Header
		Dim header As Dictionary(Of String, Object) = GetHeader(GearboxFormatVersion)


		'Body
		Dim body As Dictionary(Of String, Object) = New Dictionary(Of String, Object)

		body.Add(JsonKeys.SavedInDeclMode, Cfg.DeclMode)
		body.Add(JsonKeys.Gearbox_ModelName, gbx.ModelName)
		body.Add(JsonKeys.Gearbox_Inertia, gbx.Inertia.Value())
		body.Add(JsonKeys.Gearbox_TractionInterruption, gbx.TractionInterruption.Value())

		Dim ls As New List(Of Dictionary(Of String, Object))
		Dim axlgDict As New Dictionary(Of String, Object)
		axlgDict.Add(JsonKeys.Gearbox_Gear_Ratio, axl.Ratio)
		If axl.LossMap Is Nothing Then
			axlgDict.Add(JsonKeys.Gearbox_Gear_Efficiency, axl.Efficiency)
		Else
			axlgDict.Add(JsonKeys.Gearbox_Gear_LossMapFile, GetRelativePath(axl.LossMap.Source, Path.GetDirectoryName(filename)))
		End If
		ls.Add(axlgDict)

		For Each gear As ITransmissionInputData In gbx.Gears
			Dim gearDict As New Dictionary(Of String, Object)
			gearDict.Add(JsonKeys.Gearbox_Gear_Ratio, gear.Ratio)
			If gear.LossMap Is Nothing Then
				gearDict.Add(JsonKeys.Gearbox_Gear_Efficiency, gear.Efficiency)
			Else
				gearDict.Add(JsonKeys.Gearbox_Gear_LossMapFile,
							GetRelativePath(gear.LossMap.Source, Path.GetDirectoryName(filename)))
			End If
			gearDict.Add(JsonKeys.Gearbox_Gear_ShiftPolygonFile, If _
							(Not gbx.SavedInDeclarationMode AndAlso Not gear.ShiftPolygon Is Nothing,
							GetRelativePath(gear.ShiftPolygon.Source, Path.GetDirectoryName(filename)), ""))
			gearDict.Add("MaxTorque", If(gear.MaxTorque Is Nothing, "", gear.MaxTorque.Value().ToString()))

			ls.Add(gearDict)
		Next
		body.Add(JsonKeys.Gearbox_Gears, ls)
		body.Add(JsonKeys.Gearbox_TorqueReserve, gbx.TorqueReserve*100)
		body.Add(JsonKeys.Gearbox_ShiftTime, gbx.MinTimeBetweenGearshift.Value())
		body.Add(JsonKeys.Gearbox_StartTorqueReserve, gbx.StartTorqueReserve*100)
		body.Add(JsonKeys.Gearbox_StartSpeed, gbx.StartSpeed.Value())
		body.Add(JsonKeys.Gearbox_StartAcceleration, gbx.StartAcceleration.Value())
		body.Add(JsonKeys.Gearbox_GearboxType, gbx.Type.ToString())

		Dim torqueConverter As ITorqueConverterEngineeringInputData = gbx.TorqueConverter
		Dim torqueConverterDict As New Dictionary(Of String, Object)
		torqueConverterDict.Add("Enabled", Not torqueConverter Is Nothing AndAlso gbx.Type.AutomaticTransmission())
		If gbx.Type.AutomaticTransmission() AndAlso Not torqueConverter Is Nothing Then
			torqueConverterDict.Add("File", GetRelativePath(torqueConverter.TCData.Source, Path.GetDirectoryName(filename)))
			torqueConverterDict.Add(JsonKeys.Gearbox_TorqueConverter_ReferenceRPM, torqueConverter.ReferenceRPM.AsRPM)
			torqueConverterDict.Add(JsonKeys.Gearbox_TorqueConverter_Inertia, torqueConverter.Inertia.Value())
			torqueConverterDict.Add("MaxTCSpeed", torqueConverter.MaxInputSpeed.AsRPM)
			torqueConverterDict.Add("ShiftPolygon",
									If (Not gbx.SavedInDeclarationMode AndAlso Not torqueConverter.ShiftPolygon Is Nothing,
										GetRelativePath(torqueConverter.ShiftPolygon.Source, Path.GetDirectoryName(filename)), ""))
			torqueConverterDict.Add("CLUpshiftMinAcceleration", torqueConverter.CLUpshiftMinAcceleration.Value())
			torqueConverterDict.Add("CCUpshiftMinAcceleration", torqueConverter.CCUpshiftMinAcceleration.Value())
		End If
		body.Add(JsonKeys.Gearbox_TorqueConverter, torqueConverterDict)

		body.Add("DownshiftAfterUpshiftDelay", gbx.DownshiftAfterUpshiftDelay.Value())
		body.Add("UpshiftAfterDownshiftDelay", gbx.UpshiftAfterDownshiftDelay.Value())
		body.Add("UpshiftMinAcceleration", gbx.UpshiftMinAcceleration.Value())

		body.Add("PowershiftShiftTime", gbx.PowershiftShiftTime.Value())
		body.Add("PowershiftInertiaFactor", gbx.PowerShiftInertiaFactor)

		WriteFile(header, body, filename)
	End Sub

	Public Sub SaveVehicle(vehicle As IVehicleEngineeringInputData, retarder As IRetarderInputData,
							pto As IPTOTransmissionInputData, angledrive As IAngledriveInputData, filename As String) _
		Implements IOutputFileWriter.SaveVehicle
		Dim basePath As String = Path.GetDirectoryName(filename)

		'Header
		Dim header As Dictionary(Of String, Object) = GetHeader(VehicleFormatVersion)

		'Body
		Dim retarderOut As Dictionary(Of String, Object) = New Dictionary(Of String, Object)()
		If retarder Is Nothing Then
			retarderOut.Add("Type", RetarderType.None.GetName())
		Else
			retarderOut.Add("Type", retarder.Type.GetName())
			retarderOut.Add("Ratio", retarder.Ratio)
			retarderOut.Add("File",
							If _
								(retarder.Type.IsDedicatedComponent AndAlso Not retarder.LossMap Is Nothing,
								GetRelativePath(retarder.LossMap.Source, basePath), ""))
		End If

		Dim ptoOut As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
		If pto Is Nothing Then
			ptoOut.Add("Type", "None")
		Else
			ptoOut.Add("Type", pto.PTOTransmissionType)
			ptoOut.Add("LossMap",
						If _
						(pto.PTOTransmissionType <> "None" AndAlso Not pto.PTOLossMap Is Nothing,
						GetRelativePath(pto.PTOLossMap.Source, basePath), ""))
			ptoOut.Add("Cycle",
						If _
						(pto.PTOTransmissionType <> "None" AndAlso Not pto.PTOCycle Is Nothing,
						GetRelativePath(pto.PTOCycle.Source, basePath), ""))
		End If

		Dim angledriveOut As Dictionary(Of String, Object) = New Dictionary(Of String, Object) From {
				{"Type", angledrive.Type.ToString()},
				{"Ratio", angledrive.Ratio},
				{"LossMap",
				If _
				(angledrive.Type = AngledriveType.SeparateAngledrive AndAlso Not angledrive.LossMap Is Nothing,
				GetRelativePath(angledrive.LossMap.Source, basePath), "")}}

		Dim body As Dictionary(Of String, Object) = New Dictionary(Of String, Object) From {
				{"SavedInDeclMode", Cfg.DeclMode},
				{"VehCat", vehicle.VehicleCategory.ToString()},
				{"CurbWeight", vehicle.CurbWeightChassis.Value()},
				{"CurbWeightExtra", vehicle.CurbWeightExtra.Value()},
				{"Loading", vehicle.Loading.Value()},
				{"MassMax", vehicle.GrossVehicleMassRating.ConvertTo().Ton.Value()},
				{"CdA", vehicle.AirDragArea.Value()},
				{"rdyn", vehicle.DynamicTyreRadius.ConvertTo().Milli.Meter.Value()},
				{"CdCorrMode", vehicle.CrossWindCorrectionMode.GetName()},
				{"CdCorrFile",
				If((vehicle.CrossWindCorrectionMode = CrossWindCorrectionMode.SpeedDependentCorrectionFactor OrElse
					vehicle.CrossWindCorrectionMode = CrossWindCorrectionMode.VAirBetaLookupTable) AndAlso
					Not vehicle.CrosswindCorrectionMap Is Nothing, GetRelativePath(vehicle.CrosswindCorrectionMap.Source, basePath),
					"")
				},
				{"Retarder", retarderOut},
				{"Angledrive", angledriveOut},
				{"PTO", ptoOut},
				{"AxleConfig", New Dictionary(Of String, Object) From {
				{"Type", vehicle.AxleConfiguration.GetName()},
				{"Axles", From axle In vehicle.Axles Select New Dictionary(Of String, Object) From {
				{"Inertia", axle.Inertia.Value()},
				{"Wheels", axle.Wheels},
				{"AxleWeightShare", axle.AxleWeightShare},
				{"TwinTyres", axle.TwinTyres},
				{"RRCISO", axle.RollResistanceCoefficient},
				{"FzISO", axle.TyreTestLoad.Value()}
				}}}}}

		WriteFile(header, body, filename)
	End Sub

	Public Sub SaveJob(input As IEngineeringInputDataProvider, filename As String) _
		Implements IOutputFileWriter.SaveJob
		Dim basePath As String = Path.GetDirectoryName(filename)
		'Header
		Dim header As Dictionary(Of String, Object) = GetHeader(VectoJobFormatVersion)

		'Body
		Dim body As Dictionary(Of String, Object) = New Dictionary(Of String, Object)

		'SavedInDeclMode = Cfg.DeclMode

		Dim job As IEngineeringJobInputData = input.JobInputData()
		Dim aux As IAuxiliariesEngineeringInputData = input.AuxiliaryInputData()
		Dim driver As IDriverEngineeringInputData = input.DriverInputData

		body.Add("SavedInDeclMode", job.SavedInDeclarationMode)

		body.Add("EngineOnlyMode", job.EngineOnlyMode)

		If job.EngineOnlyMode Then
			body.Add("EngineFile", GetRelativePath(input.EngineInputData.Source, basePath))
			body.Add("Cycles",
					job.Cycles.Select(Function(x) GetRelativePath(x.CycleData.Source, Path.GetDirectoryName(filename))).ToArray())
			WriteFile(header, body, filename)
			Return
		End If

		'Main Files
		body.Add("VehicleFile", GetRelativePath(job.Vehicle.Source, basePath))
		body.Add("EngineFile", GetRelativePath(input.EngineInputData.Source, basePath))
		body.Add("GearboxFile", GetRelativePath(input.GearboxInputData.Source, basePath))

		'AA-TB
		'ADVANCED AUXILIARIES 
		body.Add("AuxiliaryAssembly", aux.AuxiliaryAssembly.GetName())
		body.Add("AuxiliaryVersion", aux.AuxiliaryVersion)
		body.Add("AdvancedAuxiliaryFilePath", GetRelativePath(aux.AdvancedAuxiliaryFilePath, basePath))

		Dim pAdd As Double = 0.0
		Dim auxList As List(Of Object) = New List(Of Object)
		For Each auxEntry As IAuxiliaryEngineeringInputData In aux.Auxiliaries
			If auxEntry.AuxiliaryType = AuxiliaryDemandType.Constant Then
				pAdd += auxEntry.ConstantPowerDemand.Value()
				Continue For
			End If
			Dim auxOut As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
			Dim engineeringAuxEntry As IAuxiliaryDeclarationInputData = TryCast(auxEntry, IAuxiliaryDeclarationInputData)
			If Not job.SavedInDeclarationMode Then
				auxOut.Add("ID", auxEntry.ID)
				auxOut.Add("Type", AuxiliaryTypeHelper.ParseKey(auxEntry.ID).Name())
				auxOut.Add("Path", GetRelativePath(auxEntry.DemandMap.Source, basePath))
				auxOut.Add("Technology", New String() {})
			Else
				auxOut.Add("ID", auxEntry.ID)
				auxOut.Add("Type", AuxiliaryTypeHelper.ParseKey(auxEntry.ID).Name())
				auxOut.Add("Technology", engineeringAuxEntry.Technology)
			End If
			auxList.Add(auxOut)
		Next

		body.Add("Aux", auxList)

		If Not job.SavedInDeclarationMode Then
			body.Add("Padd", pAdd)
		End If
		If Not job.SavedInDeclarationMode Then
			body.Add("VACC", GetRelativePath(driver.AccelerationCurve.Source, basePath))
		End If
		body.Add("StartStop", New Dictionary(Of String, Object) From {
					{"Enabled", driver.StartStop.Enabled},
					{"MaxSpeed", driver.StartStop.MaxSpeed.AsKmph},
					{"MinTime", driver.StartStop.MinTime.Value()},
					{"Delay", driver.StartStop.Delay.Value()}})
		If Not job.SavedInDeclarationMode Then
			Dim dfTargetSpeed As String = If(
				Not driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup Is Nothing AndAlso
				File.Exists(driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup.Source),
				GetRelativePath(driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup.Source, basePath), "")
			Dim dfVelocityDrop As String = If(
				Not driver.Lookahead.CoastingDecisionFactorVelocityDropLookup Is Nothing AndAlso
				File.Exists(driver.Lookahead.CoastingDecisionFactorVelocityDropLookup.Source),
				GetRelativePath(driver.Lookahead.CoastingDecisionFactorVelocityDropLookup.Source, basePath), "")
			body.Add("LAC", New Dictionary(Of String, Object) From {
						{"Enabled", driver.Lookahead.Enabled},
						{"PreviewDistanceFactor", driver.Lookahead.LookaheadDistanceFactor},
						{"DF_offset", driver.Lookahead.CoastingDecisionFactorOffset},
						{"DF_scaling", driver.Lookahead.CoastingDecisionFactorScaling},
						{"DF_targetSpeedLookup", dfTargetSpeed},
						{"Df_velocityDropLookup", dfVelocityDrop}})
		End If

		'Overspeed / EcoRoll
		Dim overspeedDic As Dictionary(Of String, Object) = New Dictionary(Of String, Object)

		overspeedDic.Add("Mode", driver.OverSpeedEcoRoll.Mode.ToString())

		overspeedDic.Add("MinSpeed", driver.OverSpeedEcoRoll.MinSpeed.AsKmph)
		overspeedDic.Add("OverSpeed", driver.OverSpeedEcoRoll.OverSpeed.AsKmph)
		overspeedDic.Add("UnderSpeed", driver.OverSpeedEcoRoll.UnderSpeed.AsKmph)
		body.Add("OverSpeedEcoRoll", overspeedDic)

		'Cycles
		If Not job.SavedInDeclarationMode Then
			body.Add("Cycles",
					job.Cycles.Select(Function(x) GetRelativePath(x.CycleData.Source, Path.GetDirectoryName(filename))).ToArray())
		End If

		WriteFile(header, body, filename)
	End Sub

	Public Sub ExportJob(input As IEngineeringInputDataProvider, filename As String, separateFiles As Boolean) _
		Implements IOutputFileWriter.ExportJob
		Throw New NotImplementedException
	End Sub

	''' <summary>
	''' Writes the Content variable into a JSON file.
	''' </summary>
	''' <param name="path"></param>
	''' <remarks></remarks>
	Public Shared Sub WriteFile(content As JToken, path As String)
		Dim file As StreamWriter
		Dim str As String

		If content.Count = 0 Then
			Return
		End If

		Try
			str = JsonConvert.SerializeObject(content, Formatting.Indented)
			file = My.Computer.FileSystem.OpenTextFileWriter(path, False)
		Catch ex As Exception
			Throw
		End Try

		file.Write(str)
		file.Close()
	End Sub

	Public Shared Sub WriteFile(content As Dictionary(Of String, Object), path As String)
		WriteFile(JToken.FromObject(content), path)
	End Sub

	Protected Shared Sub WriteFile(header As Dictionary(Of String, Object), body As Dictionary(Of String, Object),
									path As String)
		WriteFile(JToken.FromObject(New Dictionary(Of String, Object) From {{"Header", header}, {"Body", body}}), path)
	End Sub
End Class