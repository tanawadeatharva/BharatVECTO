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
'Option Infer On


''' <summary>
''' Determines how file extensions are set in the File Browser
''' </summary>
''' <remarks></remarks>
Public Enum FileBrowserFileExtensionMode As Integer
	ForceExt = 0
	MultiExt = 1
	SingleExt = 2
End Enum

Public Enum WorkerMessageType
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


