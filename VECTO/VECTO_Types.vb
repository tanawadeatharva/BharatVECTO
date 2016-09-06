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

Public Enum MessageType
	NewJob
	Normal
	Warn
	Err
End Enum




Module PTOType
	'Public ReadOnly PtoTypeStrings As New Dictionary(Of tPTOType, String) From {
	'	{tPTOType.None, "None"},
	'	{tPTOType.OnlyDriveShaftShiftClawSynchronizerSchieberad,
	'	"only the drive shaft of the PTO - shift claw, synchronizer, Schieberad"},
	'	{tPTOType.OnlyDriveShaftMultiDiscClutch, "only the drive shaft of the PTO - multi-disc clutch"},
	'	{tPTOType.OnlyDriveShaftMultiDiscClutchOilPump, "only the drive shaft of the PTO - multi-disc clutch, oil pump"},
	'	{tPTOType.DriveShaftUpTo2GearWheelsShiftClawSynchronizerSchieberad,
	'	"drive shaft and/or up to 2 gear wheels - shift claw, synchronizer, Schieberad"},
	'	{tPTOType.DriveShaftUpTo2GearWheelsMultiDiscClutch, "drive shaft and/or up to 2 gear wheels - multi-disc clutch"},
	'	{tPTOType.DriveShaftUpTo2GearWheelsMultiDiscClutchOilPump,
	'	"drive shaft and/or up to 2 gear wheels - multi-disc clutch, oil pump"},
	'	{tPTOType.DriveShaftMoreThan2GearWheelsShiftClawSynchronizerSchieberad,
	'	"drive shaft and/or more than 2 gear wheels - shift claw, synchronizer, Schieberad"},
	'	{tPTOType.DriveShaftMoreThan2GearWheelsMultiDiscClutch,
	'	"drive shaft and/or more than 2 gear wheels - multi-disc clutch"},
	'	{tPTOType.DriveShaftMoreThan2GearWheelsMultiDiscClutchOilPump,
	'	"drive shaft and/or more than 2 gear wheels - multi-disc clutch, oil pump"}
	'	}

End Module
