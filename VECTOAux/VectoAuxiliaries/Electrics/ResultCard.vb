' Copyright 2015 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.

Imports TUGraz.VectoCommon.Utils
Imports VectoAuxiliaries.Electrics

Namespace Electrics
	Public Class ResultCard
		Implements IResultCard

		Private _results As List(Of SmartResult)

		'Constructor
		Public Sub New(results As List(Of SmartResult))

			If results Is Nothing Then Throw New ArgumentException("A list of smart results must be supplied.")

			_results = results
		End Sub


		'Public class outputs
		Public ReadOnly Property Results As List(Of SmartResult) Implements IResultCard.Results
			Get
				Return _results
			End Get
		End Property

		Public Function GetSmartCurrentResult(ByVal Amps As Ampere) As Ampere Implements IResultCard.GetSmartCurrentResult


			If _results.Count < 2 Then Return 10.SI(Of Ampere)()

			Return GetOrInterpolate(Amps.Value()).SI(Of Ampere)()
		End Function


		'Helpers
		''' <summary>
		''' Gets or interpolates value (A)
		''' </summary>
		''' <param name="amps"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Private Function GetOrInterpolate(amps As Double) As Double

			Dim pre As Double
			Dim post As Double
			Dim dAmps As Double
			Dim dSmartAmps As Double
			Dim smartAmpsSlope As Double
			Dim smartAmps As Double
			Dim maxKey As Double
			Dim minKey As Double

			maxKey = _results.Max.Amps
			minKey = _results.Min.Amps

			Dim compareKey As SmartResult = New SmartResult(amps, 0)

			'Is on boundary check
			If _results.Contains(compareKey) Then _
				Return _results.OrderBy(Function(x) x.Amps).First(Function(x) x.Amps = compareKey.Amps).SmartAmps

			'Is over map - Extrapolate
			If amps > maxKey Then

				'get the entries before and after the supplied key
				pre = (From a In _results Order By a.Amps Where a.Amps < maxKey Select a).Last().Amps
				post = maxKey

				'get the delta values 
				dAmps = post - pre
				dSmartAmps = (From da In _results Order By da.Amps Where da.Amps = post).First().SmartAmps -
							(From da In _results Order By da.Amps Where da.Amps = pre).First().SmartAmps

				'calculate the slopes
				smartAmpsSlope = dSmartAmps / dAmps

				'calculate the new values
				smartAmps = ((amps - post) * smartAmpsSlope) +
							(From da In _results Order By da.Amps Where da.Amps = post).First().SmartAmps

				Return smartAmps

			End If

			'Is under map - Extrapolate
			If amps < minKey Then

				'get the entries before and after the supplied key
				'Post is the first entry and pre is the penultimate to first entry
				post = minKey
				pre = (From k In _results Order By k.Amps Where k.Amps > minKey Select k).First().Amps

				'get the delta values 
				dAmps = post - pre
				dSmartAmps = (From da In _results Order By da.Amps Where da.Amps = post).First().SmartAmps -
							(From da In _results Order By da.Amps Where da.Amps = pre).First().SmartAmps

				'calculate the slopes
				smartAmpsSlope = dSmartAmps / dAmps

				'calculate the new values
				smartAmps = ((amps - post) * smartAmpsSlope) +
							(From da In _results Order By da.Amps Where da.Amps = post).First().SmartAmps

				Return smartAmps
			End If

			'Is Inside map - Interpolate

			'get the entries before and after the supplied rpm
			pre = (From m In _results Order By m.Amps Where m.Amps < amps Select m).Last().Amps
			post = (From m In _results Where m.Amps > amps Select m).First().Amps

			'get the delta values for rpm and the map values
			dAmps = post - pre
			dSmartAmps = (From da In _results Order By da.Amps Where da.Amps = post).First().SmartAmps -
						(From da In _results Order By da.Amps Where da.Amps = pre).First().SmartAmps

			'calculate the slopes
			smartAmpsSlope = dSmartAmps / dAmps

			'calculate the new values
			smartAmps = ((amps - post) * smartAmpsSlope) +
						(From da In _results Order By da.Amps Where da.Amps = post).First().SmartAmps


			Return smartAmps
		End Function
	End Class
End Namespace


