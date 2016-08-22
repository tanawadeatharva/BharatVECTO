' Copyright 2014 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.
Option Infer On

Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices

''' <summary>
''' Determines how file extensions are set in the File Browser
''' </summary>
''' <remarks></remarks>
Public Enum tFbExtMode As Integer
	ForceExt = 0
	MultiExt = 1
	SingleExt = 2
End Enum

Public Enum tWorkMsgType
	StatusBar
	StatusListBox
	ProgBars
	JobStatus
	CycleStatus
	InitProgBar
	Abort
End Enum

Public Enum tMsgID
	NewJob
	Normal
	Warn
	Err
End Enum


Public Enum tCalcResult
	Err
	Abort
	Done
End Enum

Public Enum tJobStatus
	Running
	Queued
	OK
	Err
	Warn
	Undef
End Enum

Public Enum tDriComp
	t
	V
	Grad
	Alt
	nU
	Gears
	Padd
	Pe
	VairVres
	VairBeta
	Undefined
	s
	StopTime
	Torque
	Pwheel
End Enum

Public Enum tVehState
	Cruise
	Acc
	Dec
	Stopped
End Enum

Public Enum tEngState
	Idle
	Drag
	FullDrag
	Load
	FullLoad
	Stopped
	Undef
End Enum

Public Enum tEngClutch
	Closed
	Opened
	Slipping
End Enum

Public Enum tAuxComp
	Psupply
	Undefined
End Enum

Public Enum tCdMode
	ConstCd0 = 0
	CdOfVeng = 1
	CdOfVdecl = 2
	CdOfBeta = 3
End Enum

Public Enum tRtType
	None = 0
	Primary = 1
	Secondary = 2
End Enum

Public Enum tGearbox
	Manual = 0
	SemiAutomatic = 1
	AutomaticSerial = 2
	AutomaticPowerSplit = 3
	Custom = 4
End Enum

<Extension>
Module tGearboxExtension
	Public Function AutomaticTransmission(type As tGearbox) As Boolean
		Return type = tGearbox.AutomaticPowerSplit OrElse type = tGearbox.AutomaticSerial
	End Function


	Public Function ManualTransmission(type As tGearbox) As Boolean
		Return type = tGearbox.Manual OrElse type = tGearbox.SemiAutomatic
	End Function
End Module

Public Enum tVehCat As Integer
	Undef = 0
	RigidTruck = 1
	Tractor = 2
	Citybus = 3
	InterurbanBus = 4
	Coach = 5
End Enum

Public Enum tAxleConf As Integer
	Undef = 0
	a4x2 = 1
	a4x4 = 2
	a6x2 = 3
	a6x4 = 4
	a6x6 = 5
	a8x2 = 6
	a8x4 = 7
	a8x6 = 8
	a8x8 = 9
End Enum


Public Enum tPTOType
	None = 0
	OnlyDriveShaftShiftClawSynchronizerSchieberad = 1
	OnlyDriveShaftMultiDiscClutch = 2
	OnlyDriveShaftMultiDiscClutchOilPump = 3
	DriveShaftUpTo2GearWheelsShiftClawSynchronizerSchieberad = 4
	DriveShaftUpTo2GearWheelsMultiDiscClutch = 5
	DriveShaftUpTo2GearWheelsMultiDiscClutchOilPump = 6
	DriveShaftMoreThan2GearWheelsShiftClawSynchronizerSchieberad = 7
	DriveShaftMoreThan2GearWheelsMultiDiscClutch = 8
	DriveShaftMoreThan2GearWheelsMultiDiscClutchOilPump = 9
End Enum

Module PTOType
	Public ReadOnly PtoTypeStrings As New Dictionary(Of tPTOType, String) From {
		{tPTOType.None, "None"},
		{tPTOType.OnlyDriveShaftShiftClawSynchronizerSchieberad,
		"only the drive shaft of the PTO - shift claw, synchronizer, Schieberad"},
		{tPTOType.OnlyDriveShaftMultiDiscClutch, "only the drive shaft of the PTO - multi-disc clutch"},
		{tPTOType.OnlyDriveShaftMultiDiscClutchOilPump, "only the drive shaft of the PTO - multi-disc clutch, oil pump"},
		{tPTOType.DriveShaftUpTo2GearWheelsShiftClawSynchronizerSchieberad,
		"drive shaft and/or up to 2 gear wheels - shift claw, synchronizer, Schieberad"},
		{tPTOType.DriveShaftUpTo2GearWheelsMultiDiscClutch, "drive shaft and/or up to 2 gear wheels - multi-disc clutch"},
		{tPTOType.DriveShaftUpTo2GearWheelsMultiDiscClutchOilPump,
		"drive shaft and/or up to 2 gear wheels - multi-disc clutch, oil pump"},
		{tPTOType.DriveShaftMoreThan2GearWheelsShiftClawSynchronizerSchieberad,
		"drive shaft and/or more than 2 gear wheels - shift claw, synchronizer, Schieberad"},
		{tPTOType.DriveShaftMoreThan2GearWheelsMultiDiscClutch,
		"drive shaft and/or more than 2 gear wheels - multi-disc clutch"},
		{tPTOType.DriveShaftMoreThan2GearWheelsMultiDiscClutchOilPump,
		"drive shaft and/or more than 2 gear wheels - multi-disc clutch, oil pump"}
		}


	Public Function GetPTOString(p As tPTOType) As String
		If Not PtoTypeStrings.ContainsKey(p) Then
			Return PtoTypeStrings(tPTOType.None)
		Else
			Return PtoTypeStrings(p)
		End If
	End Function

	Public Function GetPTOType(p As String) As tPTOType
		If Not PtoTypeStrings.ContainsValue(p) Then
			Return tPTOType.None
		End If
		Return PtoTypeStrings.ToDictionary(Function(pair) pair.Value, Function(pair) pair.Key)(p)
	End Function
End Module


Public Enum tLoading
	FullLoaded
	EmptyLoaded
	RefLoaded
	UserDefLoaded
End Enum

Public Enum tMission
	LongHaul
	RegionalDelivery
	UrbanDelivery
	MunicipalUtility
	Construction
	HeavyUrban
	Urban
	Suburban
	Interurban
	Coach
	Undef
End Enum

Public Enum tWHTCpart
	Urban
	Rural
	Motorway
End Enum

Public Enum tAux
	Fan
	SteerPump
	HVAC
	ElectricSys
	PneumSys
End Enum


