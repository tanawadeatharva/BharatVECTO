Imports VectoAuxiliaries.Electrics
Imports NUnit.Framework
Imports VectoAuxiliaries


Namespace UnitTests
	<TestFixture()>
	Public Class AlternatorMapTests
		Private Const _GOODMAP As String = "TestFiles\testAlternatorMap.aalt"
		Private Const _GOODMAPORIGINALSINGLEMAP As String = "TestFiles\testAlternatorMapOriginalSingleMap.aalt"
		Private Const _INVALIDRPMMAP As String = "TestFiles\testAlternatorMapWithInvalidRpm.aalt"
		Private Const _INVALIDAMPSMAP As String = "TestFiles\testAlternatorMapWithInvalidAmps.aalt"
		Private Const _IVALIDEFFICIENCYMAP As String = "TestFiles\testAlternatorMapWithInvalidEfficiency.aalt"
		Private Const _INVALIDPOWERMAP As String = "TestFiles\testAlternatorMapWithInvalidPower.aalt"
		Private Const _GOODCOMBINEDMAP As String = "TestFiles\testAlternatorMapCombined.aalt"
		Private Const _ASYMETRICALCOMBINEDROWSMAP As String = "TestFiles\testAlternatorMapAsymetricalRowsCombined.aalt"
		Private Const _ASYMETRICALCOMBINEDXYPAIRSMAP As String = "TestFiles\testAlternatorMapAsymetricalXYPairsCombined.aalt"


		<Test()>
		<TestCase(10, 1500, 0.615F)> _
		<TestCase(10, 4000, 0.64F)> _
		<TestCase(10, 7000, 0.475F)> _
		<TestCase(63, 1500, 0.0F)> _
		<TestCase(63, 4000, 0.74F)> _
		<TestCase(63, 7000, 0.658F)> _
		<TestCase(136, 1500, 0.0F)> _
		<TestCase(136, 4000, 0.6694F)> _
		<TestCase(136, 7000, 0.5953F)>
		Public Sub BoundryTests(ByVal amps As Integer, ByVal rpm As Integer, ByVal expected As Single)

			Dim map As IAlternatorMap = GetInitialisedMap()
			Dim target As IAlternatorMap = GetInitialisedMap()
			Dim actual As Single = map.GetEfficiency(rpm, amps).Efficiency
			Assert.AreEqual(expected, actual)
		End Sub

		<TestCase(55, 1500, 0.15576F)> _
		<TestCase(55, 7000, 0.6304F)> _
		<TestCase(10, 3000, 0.63F)> _
		<TestCase(136, 3000, 0.3347F)>
		Public Sub FourCornerWithInterpolatedOtherTest(ByVal amps As Single, ByVal rpm As Single, ByVal expected As Single)

			Dim map As IAlternatorMap = GetInitialisedMap()
			Dim target As IAlternatorMap = GetInitialisedMap()

			Dim actual As Single = map.GetEfficiency(rpm, amps).Efficiency

			Assert.AreEqual(expected, actual)
		End Sub

		<TestCase(18, 1750, 0.656323552F)> _
		<TestCase(18, 6500, 0.5280294F)> _
		<TestCase(130, 1750, 0.0F)> _
		<TestCase(130.5F, 6500, 0.6144F)>
		Public Sub InterpolatedCornersBothMidPreviousTest(ByVal amps As Single, ByVal rpm As Single, ByVal expected As Single)

			Dim map As IAlternatorMap = GetInitialisedMap()
			Dim target As IAlternatorMap = GetInitialisedMap()

			Dim actual As Single = map.GetEfficiency(rpm, amps).Efficiency

			Assert.AreEqual(expected, actual)
		End Sub


		<Test()> _
		<TestCase(18.5F, 1750, 0.65875F)> _
		<TestCase(40, 1750, 0.473675F)> _
		<TestCase(58, 1750, 0.160225F)> _
		<TestCase(65.5F, 1750, 0.20955F)> _
		<TestCase(96.5F, 1750, 0.173F)> _
		<TestCase(130.5F, 1750, 0.0F)> _
		<TestCase(18.5F, 3000, 0.658025F)> _
		<TestCase(40, 3000, 0.5983F)> _
		<TestCase(58, 3000, 0.476825F)> _
		<TestCase(65.5F, 3000, 0.57835F)> _
		<TestCase(96.5F, 3000, 0.5268F)> _
		<TestCase(130.5F, 3000, 0.33735F)> _
		<TestCase(18.5F, 5000, 0.605475F)> _
		<TestCase(40, 5000, 0.65725F)> _
		<TestCase(58, 5000, 0.7006F)> _
		<TestCase(65.5F, 5000, 0.715125F)> _
		<TestCase(96.5F, 5000, 0.687025F)> _
		<TestCase(130.5F, 5000, 0.650575F)> _
		<TestCase(18.5F, 6500, 0.529625F)> _
		<TestCase(40, 6500, 0.59825F)> _
		<TestCase(58, 6500, 0.6557F)> _
		<TestCase(65.5F, 6500, 0.681425F)> _
		<TestCase(96.5F, 6500, 0.656175F)> _
		<TestCase(130.5F, 6500, 0.6144F)>
		Public Sub InterpolatedAllMidPointsTest(ByVal amps As Single, ByVal rpm As Single, ByVal expected As Single)


			Dim map As IAlternatorMap = GetInitialisedMap()
			Dim target As IAlternatorMap = GetInitialisedMap()

			Dim actual As Single = map.GetEfficiency(rpm, amps).Efficiency

			Assert.AreEqual(expected, CType(Math.Round(actual, 6), Single))
		End Sub


#Region "Helpers"

		Private Function GetInitialisedMap() As AlternatorMap
			Dim target As AlternatorMap = GetMap()
			target.Initialise()
			Return target
		End Function

		Private Function GetMap() As AlternatorMap
			Dim path As String = _GOODMAP
			Dim target As AlternatorMap = New AlternatorMap(path)
			Return target
		End Function

		Private Function GetMap(path As String) As AlternatorMap
			Dim target As AlternatorMap = New AlternatorMap(path)
			Return target
		End Function


#End Region
	End Class
End Namespace