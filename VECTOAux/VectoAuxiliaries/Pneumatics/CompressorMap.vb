' Copyright 2017 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.

Imports System.Globalization
Imports System.IO
Imports TUGraz.VectoCommon.Utils

Namespace Pneumatics
	''' <summary>
	''' Compressor Flow Rate and Power Map
	''' </summary>
	''' <remarks></remarks>
	Public Class CompressorMap
		Implements ICompressorMap, 
					IAuxiliaryEvent

		Private ReadOnly filePath As String
		Private _averagePowerDemandPerCompressorUnitFlowRateLitresperSec As Double
		Private _MapBoundariesExceeded As Boolean

		''' <summary>
		''' Dictionary of values keyed by the rpm valaues in the csv file
		''' Values are held as a tuple as follows
		''' Item1 = flow rate
		''' Item2 - power [compressor on] 
		''' Item3 - power [compressor off]
		''' </summary>
		''' <remarks></remarks>
		Private map As Dictionary(Of Integer, CompressorMapValues)

		'Returns the AveragePowerDemand  per unit flow rate in seconds.
		Public Function GetAveragePowerDemandPerCompressorUnitFlowRate() As Double _
			Implements ICompressorMap.GetAveragePowerDemandPerCompressorUnitFlowRate

			Return _averagePowerDemandPerCompressorUnitFlowRateLitresperSec
		End Function


		''' <summary>
		''' Creates a new instance of the CompressorMap class
		''' </summary>
		''' <param name="path">full path to csv data file</param>
		''' <remarks></remarks>
		Public Sub New(ByVal path As String)
			filePath = path
		End Sub

		''' <summary>
		''' Initilaises the map from the supplied csv data
		''' </summary>
		''' <remarks></remarks>
		Public Function Initialise() As Boolean Implements ICompressorMap.Initialise

			If File.Exists(filePath) Then
				Using sr As StreamReader = New StreamReader(filePath)
					'get array of lines from csv
					Dim lines() As String = sr.ReadToEnd().Split(CType(Environment.NewLine, Char()),
																StringSplitOptions.RemoveEmptyEntries)

					'Must have at least 2 entries in map to make it usable [dont forget the header row]
					If lines.Length < 3 Then Throw New ArgumentException("Insufficient rows in csv to build a usable map")

					map = New Dictionary(Of Integer, CompressorMapValues)()
					Dim firstline As Boolean = True

					For Each line As String In lines
						If Not firstline Then
							'split the line
							Dim elements() As String = line.Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries)
							'4 entries per line required
							If (elements.Length <> 4) Then Throw New ArgumentException("Incorrect number of values in csv file")
							'add values to map
							Try
								map.Add(Integer.Parse(elements(0)),
										New CompressorMapValues(Double.Parse(elements(1), CultureInfo.InvariantCulture).SI(Of NormLiterPerSecond),
																Double.Parse(elements(2), CultureInfo.InvariantCulture).SI(Of Watt),
																Double.Parse(elements(3), CultureInfo.InvariantCulture).SI(Of Watt)))
							Catch fe As FormatException
								Throw New InvalidCastException(String.Format("Compresor Map: line '{0}", line), fe)
							End Try
						Else
							firstline = False
						End If
					Next
				End Using

				'*********************************************************************
				'Calculate the Average Power Demand Per Compressor Unit FlowRate / per second.
				Dim powerDividedByFlowRateSum As Double = 0
				For Each speed As KeyValuePair(Of Integer, CompressorMapValues) In map
					powerDividedByFlowRateSum += (speed.Value.PowerCompressorOn - speed.Value.PowerCompressorOff).Value() /
												speed.Value.FlowRate.Value()
				Next

				'Map in Litres Per Minute, so * 60 to get per second, calculated only once at initialisation.
				_averagePowerDemandPerCompressorUnitFlowRateLitresperSec = (powerDividedByFlowRateSum / map.Count) * 60
				'**********************************************************************

			Else
				Throw New ArgumentException("supplied input file does not exist")
			End If

			'If we get here then all should be well and we can return a True value of success.
			Return True
		End Function

		''' <summary>
		''' Returns compressor flow rate at the given rotation speed
		''' </summary>
		''' <param name="rpm">compressor rotation speed</param>
		''' <returns></returns>
		''' <remarks>Single</remarks>
		Public Function GetFlowRate(ByVal rpm As Double) As NormLiterPerSecond Implements ICompressorMap.GetFlowRate
			Dim val As CompressorMapValues = InterpolatedTuple(rpm)
			Return val.FlowRate
		End Function

		''' <summary>
		''' Returns mechanical power at rpm when compressor is on
		''' </summary>
		''' <param name="rpm">compressor rotation speed</param>
		''' <returns></returns>
		''' <remarks>Single</remarks>
		Public Function GetPowerCompressorOn(ByVal rpm As Double) As Watt Implements ICompressorMap.GetPowerCompressorOn
			Dim val As CompressorMapValues = InterpolatedTuple(rpm)
			Return val.PowerCompressorOn
		End Function

		''' <summary>
		''' Returns mechanical power at rpm when compressor is off
		''' </summary>
		''' <param name="rpm">compressor rotation speed</param>
		''' <returns></returns>
		''' <remarks>Single</remarks>
		Public Function GetPowerCompressorOff(ByVal rpm As Double) As Watt Implements ICompressorMap.GetPowerCompressorOff
			Dim val As CompressorMapValues = InterpolatedTuple(rpm)
			Return val.PowerCompressorOff
		End Function

		''' <summary>
		''' Returns an instance of CompressorMapValues containing the values at a key, or interpolated values
		''' </summary>
		''' <returns>CompressorMapValues</returns>
		''' <remarks>Throws exception if rpm are outside map</remarks>
		Private Function InterpolatedTuple(ByVal rpm As Double) As CompressorMapValues
			'check the rpm is within the map
			Dim min As Integer = map.Keys.Min()
			Dim max As Integer = map.Keys.Max()

			If rpm < min OrElse rpm > max Then
				If Not _MapBoundariesExceeded Then
					OnMessage(Me,
							String.Format("Compresser : limited RPM of '{2}' to extent of map - map range is {0} to {1}", min, max, rpm),
							AdvancedAuxiliaryMessageType.Warning)
					_MapBoundariesExceeded = True
				End If

				'Limiting as agreed.
				If rpm > max Then rpm = max
				If rpm < min Then rpm = min

			End If

			'If supplied rpm is a key, we can just return the appropriate tuple
			Dim intRpm As Integer = CType(rpm, Integer)
			If rpm.IsEqual(intRpm) AndAlso map.ContainsKey(intRpm) Then
				Return map(intRpm)
			End If

			'Not a key value, interpolate
			'get the entries before and after the supplied rpm
			Dim pre As KeyValuePair(Of Integer, CompressorMapValues) = (From m In map Where m.Key < rpm Select m).Last()
			Dim post As KeyValuePair(Of Integer, CompressorMapValues) = (From m In map Where m.Key > rpm Select m).First()

			'get the delta values for rpm and the map values
			Dim dRpm As Double = post.Key - pre.Key
			Dim dFlowRate As NormLiterPerSecond = post.Value.FlowRate - pre.Value.FlowRate
			Dim dPowerOn As Watt = post.Value.PowerCompressorOn - pre.Value.PowerCompressorOn
			Dim dPowerOff As Watt = post.Value.PowerCompressorOff - pre.Value.PowerCompressorOff

			'calculate the slopes
			Dim flowSlope As Double = dFlowRate.Value() / dRpm
			Dim powerOnSlope As Double = dPowerOn.Value() / dRpm
			Dim powerOffSlope As Double = dPowerOff.Value() / dRpm

			'calculate the new values
			Dim flowRate As NormLiterPerSecond = (((rpm - pre.Key) * flowSlope).SI(Of NormLiterPerSecond)() + pre.Value.FlowRate)
			Dim powerCompressorOn As Watt = (((rpm - pre.Key) * powerOnSlope).SI(Of Watt)() + pre.Value.PowerCompressorOn)
			Dim powerCompressorOff As Watt = (((rpm - pre.Key) * powerOffSlope).SI(Of Watt)() + pre.Value.PowerCompressorOff)

			'Build and return a new CompressorMapValues instance
			Return New CompressorMapValues(flowRate, powerCompressorOn, powerCompressorOff)
		End Function

		''' <summary>
		''' Encapsulates compressor map values
		''' Flow Rate
		''' Power - Compressor On
		''' Power - Compressor Off
		''' </summary>
		''' <remarks></remarks>
		''' 

		Private Structure CompressorMapValues
			''' <summary>
			''' Compressor flowrate
			''' </summary>
			''' <remarks></remarks>
			Public ReadOnly FlowRate As NormLiterPerSecond

			''' <summary>
			''' Power, compressor on
			''' </summary>
			''' <remarks></remarks>
			Public ReadOnly PowerCompressorOn As Watt

			''' <summary>
			''' Power compressor off
			''' </summary>
			''' <remarks></remarks>
			Public ReadOnly PowerCompressorOff As Watt

			''' <summary>
			''' Creates a new instance of CompressorMapValues
			''' </summary>
			''' <param name="flowRate">flow rate</param>
			''' <param name="powerCompressorOn">power - compressor on</param>
			''' <param name="powerCompressorOff">power - compressor off</param>
			''' <remarks></remarks>
			Public Sub New(ByVal flowRate As NormLiterPerSecond, ByVal powerCompressorOn As Watt,
							ByVal powerCompressorOff As Watt)
				Me.FlowRate = flowRate
				Me.PowerCompressorOn = powerCompressorOn
				Me.PowerCompressorOff = powerCompressorOff
			End Sub
		End Structure


		Public Event Message(ByRef sender As Object, message As String, messageType As AdvancedAuxiliaryMessageType) _
			Implements IAuxiliaryEvent.AuxiliaryEvent

		Private Sub OnMessage(sender As Object, message As String, messageType As AdvancedAuxiliaryMessageType)


			If Not message Is Nothing Then

				RaiseEvent Message(Me, message, messageType)

			End If
		End Sub
	End Class
End Namespace