Imports NUnit.Framework
Imports TUGraz.VectoCommon.Utils
Imports VectoAuxiliaries.Electrics

<TestFixture()>
Public Class ResultCardTests
	Private results As New List(Of SmartResult)
	Private unorderedResults As New List(Of SmartResult)

	Private resultCard As ResultCard
	Private unorderedResultCard As ResultCard


	Public Sub New()

		results.Add(New SmartResult(20, 18))
		results.Add(New SmartResult(30, 27))
		results.Add(New SmartResult(40, 36))
		results.Add(New SmartResult(50, 45))

		unorderedResults.Add(New SmartResult(40, 36))
		unorderedResults.Add(New SmartResult(30, 27))
		unorderedResults.Add(New SmartResult(50, 45))
		unorderedResults.Add(New SmartResult(20, 18))

		'results.Add(60, 54)

		resultCard = New ResultCard(results)

		unorderedResultCard = New ResultCard(unorderedResults)
	End Sub


	<Test()>
	Public Sub CreateNewResultsOKTest()

		Dim target As New ResultCard(results)

		Assert.IsNotNull(target)
	End Sub

    <Test()>
    Public Sub CreateNewBanResultsNullTest()

        Dim target As ResultCard
        Assert.That(Sub() target = New ResultCard(Nothing), Throws.InstanceOf(Of ArgumentException))

    End Sub


    <Test()>
	Public Sub GetBotomBoundryValueTest()

		Dim expected As Single = 18
		Dim actual As Ampere = resultCard.GetSmartCurrentResult(20.SI(Of Ampere))

		Assert.AreEqual(expected, actual.Value(), 0.001)
	End Sub


	<Test()>
	Public Sub UnorderedGetBotomBoundryValueTest()

		Dim expected As Single = 18
		Dim actual As Ampere = unorderedResultCard.GetSmartCurrentResult(20.SI(Of Ampere))

		Assert.AreEqual(expected, actual.Value(), 0.001)
	End Sub


	<Test()>
	Public Sub GetCentreBoundayValueTest()

		Dim expected As Single = 36
		Dim actual As Ampere = resultCard.GetSmartCurrentResult(40.SI(Of Ampere))

		Assert.AreEqual(expected, actual.Value(), 0.001)
	End Sub


	<Test()>
	Public Sub UnorderedGetCentreBoundayValueTest()

		Dim expected As Single = 36
		Dim actual As Ampere = unorderedResultCard.GetSmartCurrentResult(40.SI(Of Ampere))

		Assert.AreEqual(expected, actual.Value(), 0.001)
	End Sub


	<Test()>
	Public Sub GetTopBoundaryValueTest()

		Dim expected As Single = 45
		Dim actual As Ampere = resultCard.GetSmartCurrentResult(50.SI(Of Ampere))

		Assert.AreEqual(expected, actual.Value(), 0.001)
	End Sub

	<Test()>
	Public Sub UnorderedGetTopBoundaryValueTest()

		Dim expected As Single = 45
		Dim actual As Ampere = unorderedResultCard.GetSmartCurrentResult(50.SI(Of Ampere))

		Assert.AreEqual(expected, actual.Value(), 0.001)
	End Sub


	<Test()>
	Public Sub GetInterpolatedValue35AmpsTest()

		Dim expected As Single = 31.5
		Dim actual As Ampere = resultCard.GetSmartCurrentResult(35.SI(Of Ampere))

		Assert.AreEqual(expected, actual.Value(), 0.001)
	End Sub


	<Test()>
	Public Sub UnorderedGetInterpolatedValue35AmpsTest()

		Dim expected As Single = 31.5
		Dim actual As Ampere = unorderedResultCard.GetSmartCurrentResult(35.SI(Of Ampere))

		Assert.AreEqual(expected, actual.Value(), 0.001)
	End Sub

	<Test()>
	Public Sub GetExtrapolatedValue60AmpsTest()

		Dim expected As Single = 54
		Dim actual As Ampere = resultCard.GetSmartCurrentResult(60.SI(Of Ampere))

		Assert.AreEqual(expected, actual.Value(), 0.001)
	End Sub

	<Test()>
	Public Sub UnorderedGetExtrapolatedValue60AmpsTest()

		Dim expected As Single = 54
		Dim actual As Ampere = unorderedResultCard.GetSmartCurrentResult(60.SI(Of Ampere))

		Assert.AreEqual(expected, actual.Value(), 0.001)
	End Sub

	<Test()>
	Public Sub GetExtrapolatedValue10AmpsTest()

		Dim expected As Single = 9
		Dim actual As Ampere = resultCard.GetSmartCurrentResult(10.SI(Of Ampere))

		Assert.AreEqual(expected, actual.Value(), 0.001)
	End Sub

	<Test()>
	Public Sub UnorderedGetExtrapolatedValue10AmpsTest()

		Dim expected As Single = 9
		Dim actual As Ampere = unorderedResultCard.GetSmartCurrentResult(10.SI(Of Ampere))

		Assert.AreEqual(expected, actual.Value(), 0.001)
	End Sub


	<Test()>
	Public Sub EmptyOrInsufficientResultsTest()

		Dim resultSet As New List(Of SmartResult)

		Dim expected As Single = 10
		Dim actual As Ampere = (New ResultCard(resultSet)).GetSmartCurrentResult(10.SI(Of Ampere))

		Assert.AreEqual(expected, actual.Value(), 0.001)
	End Sub
End Class

