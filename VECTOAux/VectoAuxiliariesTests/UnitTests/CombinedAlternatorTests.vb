Option Strict On

Imports NUnit.Framework
Imports TUGraz.VectoCommon.Utils
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliariesTests.Mocks
Imports VectoAuxiliaries
Imports VectoAuxiliaries.Hvac
Imports System.IO

Namespace UnitTests

    <TestFixture()>
    Public Class CombinedAlternatorTests

        Private Alt1ExpectedTable2000 As New List(Of AltUserInput)
        Private Alt1ExpectedTable4000 As New List(Of AltUserInput)
        Private Alt1ExpectedTable6000 As New List(Of AltUserInput)
        Private Alt2ExpectedTable2000 As New List(Of AltUserInput)
        Private Alt2ExpectedTable4000 As New List(Of AltUserInput)
        Private Alt2ExpectedTable6000 As New List(Of AltUserInput)
        Private Alt3ExpectedTable2000 As New List(Of AltUserInput)
        Private Alt3ExpectedTable4000 As New List(Of AltUserInput)
        Private Alt3ExpectedTable6000 As New List(Of AltUserInput)
        Private Alt4ExpectedTable2000 As New List(Of AltUserInput)
        Private Alt4ExpectedTable4000 As New List(Of AltUserInput)
        Private Alt4ExpectedTable6000 As New List(Of AltUserInput)
        Private RangeTableExpected As New List(Of AltUserInput)



        Private Const COMBINEDALT_GOODMAP = "testfiles\testCombinedAlternatorMap.aalt"


        Sub New()

            Alt1ExpectedTable2000 = New List(Of AltUserInput)() From {New AltUserInput(0, 50),
                                                                         New AltUserInput(10, 50),
                                                                         New AltUserInput(40, 50),
                                                                         New AltUserInput(60, 50),
                                                                         New AltUserInput(61, 50),
                                                                         New AltUserInput(200, 50)}

            Alt1ExpectedTable4000 = New List(Of AltUserInput)() From {New AltUserInput(0, 70),
                                                                         New AltUserInput(10, 70),
                                                                         New AltUserInput(40, 70),
                                                                         New AltUserInput(60, 70),
                                                                         New AltUserInput(61, 70),
                                                                         New AltUserInput(200, 70)}


            Alt1ExpectedTable6000 = New List(Of AltUserInput)() From {New AltUserInput(0, 60),
                                                                         New AltUserInput(10, 60),
                                                                         New AltUserInput(40, 60),
                                                                         New AltUserInput(60, 60),
                                                                         New AltUserInput(61, 60),
                                                                         New AltUserInput(200, 60)}

            'ALT 2
            Alt2ExpectedTable2000 = New List(Of AltUserInput)() From {New AltUserInput(0, 80),
                                                                         New AltUserInput(10, 80),
                                                                         New AltUserInput(40, 80),
                                                                         New AltUserInput(60, 80),
                                                                         New AltUserInput(61, 80),
                                                                         New AltUserInput(200, 80)}

            Alt2ExpectedTable4000 = New List(Of AltUserInput)() From {New AltUserInput(0, 40),
                                                                         New AltUserInput(10, 40),
                                                                         New AltUserInput(40, 40),
                                                                         New AltUserInput(60, 40),
                                                                         New AltUserInput(61, 40),
                                                                         New AltUserInput(200, 40)}


            Alt2ExpectedTable6000 = New List(Of AltUserInput)() From {New AltUserInput(0, 60),
                                                                         New AltUserInput(10, 60),
                                                                         New AltUserInput(40, 60),
                                                                         New AltUserInput(60, 60),
                                                                         New AltUserInput(61, 60),
                                                                         New AltUserInput(200, 60)}


            'ALT 3
            Alt3ExpectedTable2000 = New List(Of AltUserInput)() From {New AltUserInput(0, 95),
                                                                         New AltUserInput(10, 95),
                                                                         New AltUserInput(40, 50),
                                                                         New AltUserInput(60, 90),
                                                                         New AltUserInput(62.5, 95),
                                                                         New AltUserInput(200, 95)}

            Alt3ExpectedTable4000 = New List(Of AltUserInput)() From {New AltUserInput(0, 99),
                                                                         New AltUserInput(10, 99),
                                                                         New AltUserInput(40, 1),
                                                                         New AltUserInput(60, 55),
                                                                         New AltUserInput(76.2962963, 99),
                                                                         New AltUserInput(200, 99)}


            Alt3ExpectedTable6000 = New List(Of AltUserInput)() From {New AltUserInput(0, 94),
                                                                         New AltUserInput(10, 94),
                                                                         New AltUserInput(40, 86),
                                                                         New AltUserInput(60, 13),
                                                                         New AltUserInput(63.5616438, 0),
                                                                         New AltUserInput(200, 0)}


            'ALT 4
            Alt4ExpectedTable2000 = New List(Of AltUserInput)() From {New AltUserInput(0, 55),
                                                                         New AltUserInput(10, 55),
                                                                         New AltUserInput(40, 45),
                                                                         New AltUserInput(60, 67),
                                                                         New AltUserInput(61, 67),
                                                                         New AltUserInput(200, 67)}

            Alt4ExpectedTable4000 = New List(Of AltUserInput)() From {New AltUserInput(0, 77),
                                                                         New AltUserInput(10, 77),
                                                                         New AltUserInput(40, 39),
                                                                         New AltUserInput(60, 23),
                                                                         New AltUserInput(88.75, 0),
                                                                         New AltUserInput(200, 0)}


            Alt4ExpectedTable6000 = New List(Of AltUserInput)() From {New AltUserInput(0, 34),
                                                                         New AltUserInput(10, 34),
                                                                         New AltUserInput(40, 67),
                                                                         New AltUserInput(60, 35),
                                                                         New AltUserInput(81.875, 0),
                                                                         New AltUserInput(200, 0)}



            'RangeTable
            RangeTableExpected = New List(Of AltUserInput)() From {New AltUserInput(-3001, 0),
                                                                        New AltUserInput(-3000, 0),
                                                                        New AltUserInput(2000, 50),
                                                                        New AltUserInput(4000, 70),
                                                                        New AltUserInput(6000, 60),
                                                                        New AltUserInput(18000, 0),
                                                                        New AltUserInput(18001, 0)}









        End Sub

        <OneTimeSetUp>
        Public Sub RunBeforeAnyTests()
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)
        End Sub

        <Test()>
        <TestCase(1, 2, 50.0F)>
        <TestCase(1, 4, 70)>
        <TestCase(1, 6, 60)>
        <TestCase(2, 2, 80)>
        <TestCase(2, 4, 40)>
        <TestCase(2, 6, 60)>
        <TestCase(3, 2, 55)>
        <TestCase(3, 4, 7.75F)>
        <TestCase(3, 6, 76.875F)>
        <TestCase(4, 2, 47.75F)>
        <TestCase(4, 4, 37)>
        <TestCase(4, 6, 63)>
        Public Sub Interpolate4Table4(alt As Integer, rpmK As Integer, expected As Single)

            Dim interpValue As Double

            Select Case alt

                Case 1

                    Select Case rpmK

                        Case 2
                            interpValue = Alternator.Iterpolate(Alt1ExpectedTable2000, 42.5)
                        Case 4
                            interpValue = Alternator.Iterpolate(Alt1ExpectedTable4000, 42.5)
                        Case 6
                            interpValue = Alternator.Iterpolate(Alt1ExpectedTable6000, 42.5)

                    End Select


                Case 2

                    Select Case rpmK

                        Case 2
                            interpValue = Alternator.Iterpolate(Alt2ExpectedTable2000, 42.5)
                        Case 4
                            interpValue = Alternator.Iterpolate(Alt2ExpectedTable4000, 42.5)
                        Case 6
                            interpValue = Alternator.Iterpolate(Alt2ExpectedTable6000, 42.5)

                    End Select

                Case 3

                    Select Case rpmK

                        Case 2
                            interpValue = Alternator.Iterpolate(Alt3ExpectedTable2000, 42.5)
                        Case 4
                            interpValue = Alternator.Iterpolate(Alt3ExpectedTable4000, 42.5)
                        Case 6
                            interpValue = Alternator.Iterpolate(Alt3ExpectedTable6000, 42.5)

                    End Select

                Case 4

                    Select Case rpmK

                        Case 2
                            interpValue = Alternator.Iterpolate(Alt4ExpectedTable2000, 42.5)
                        Case 4
                            interpValue = Alternator.Iterpolate(Alt4ExpectedTable4000, 42.5)
                        Case 6
                            interpValue = Alternator.Iterpolate(Alt4ExpectedTable6000, 42.5)

                    End Select

            End Select


            Assert.AreEqual(interpValue, expected, 0.001)


        End Sub


        <Test()>
        Public Sub Alt1TableConstructTest()


            'Arrange
            Dim signals As ICombinedAlternatorSignals = New CombinedAlternatorSignals


            'Act
            Dim alt As New CombinedAlternator(COMBINEDALT_GOODMAP)



            Dim idx As Integer

            For idx = 0 To alt.Alternators(0).InputTable2000.Count - 1
                Assert.IsTrue(alt.Alternators(0).InputTable2000(idx).IsEqual(Alt1ExpectedTable2000(idx)))
            Next

            For idx = 0 To alt.Alternators(0).InputTable4000.Count - 1
                Assert.IsTrue(alt.Alternators(0).InputTable4000(idx).IsEqual(Alt1ExpectedTable4000(idx)))
            Next

            For idx = 0 To alt.Alternators(0).InputTable6000.Count - 1
                Assert.IsTrue(alt.Alternators(0).InputTable6000(idx).IsEqual(Alt1ExpectedTable6000(idx)))
            Next


        End Sub

        <Test()>
        Public Sub Alt2TableConstructTest()



            'Arrange

            'Act
            Dim alt As New CombinedAlternator(COMBINEDALT_GOODMAP)


            Dim idx As Integer

            For idx = 0 To alt.Alternators(1).InputTable2000.Count - 1
                Assert.IsTrue(alt.Alternators(1).InputTable2000(idx).IsEqual(Alt2ExpectedTable2000(idx)))
            Next

            For idx = 0 To alt.Alternators(1).InputTable4000.Count - 1
                Assert.IsTrue(alt.Alternators(1).InputTable4000(idx).IsEqual(Alt2ExpectedTable4000(idx)))
            Next

            For idx = 0 To alt.Alternators(1).InputTable6000.Count - 1
                Assert.IsTrue(alt.Alternators(1).InputTable6000(idx).IsEqual(Alt2ExpectedTable6000(idx)))
            Next


        End Sub
        <Test()>
        Public Sub Alt3TableConstructTest()


            'Arrange
            Dim signals As ICombinedAlternatorSignals = New CombinedAlternatorSignals


            'Act
            Dim alt As New CombinedAlternator(COMBINEDALT_GOODMAP)


            Dim idx As Integer

            For idx = 0 To alt.Alternators(2).InputTable2000.Count - 1
                Assert.IsTrue(alt.Alternators(2).InputTable2000(idx).IsEqual(Alt3ExpectedTable2000(idx)))
            Next

            For idx = 0 To alt.Alternators(2).InputTable4000.Count - 1
                Assert.IsTrue(alt.Alternators(2).InputTable4000(idx).IsEqual(Alt3ExpectedTable4000(idx), 3))
            Next

            For idx = 0 To alt.Alternators(2).InputTable6000.Count - 1
                Assert.IsTrue(alt.Alternators(2).InputTable6000(idx).IsEqual(Alt3ExpectedTable6000(idx), 3))
            Next


        End Sub
        <Test()>
        Public Sub Alt4TableConstructTest()

            'Arrange
            Dim signals As ICombinedAlternatorSignals = New CombinedAlternatorSignals


            'Act
            Dim alt As New CombinedAlternator(COMBINEDALT_GOODMAP)


            Dim idx As Integer

            For idx = 0 To alt.Alternators(3).InputTable2000.Count - 1
                Assert.IsTrue(alt.Alternators(3).InputTable2000(idx).IsEqual(Alt4ExpectedTable2000(idx)))
            Next

            For idx = 0 To alt.Alternators(3).InputTable4000.Count - 1
                Assert.IsTrue(alt.Alternators(3).InputTable4000(idx).IsEqual(Alt4ExpectedTable4000(idx), 3))
            Next

            For idx = 0 To alt.Alternators(3).InputTable6000.Count - 1
                Assert.IsTrue(alt.Alternators(3).InputTable6000(idx).IsEqual(Alt4ExpectedTable6000(idx), 3))
            Next


        End Sub


        'testCombinedAlternatorMap
        <Test()>
        Public Sub InitialiseCombinedAlternatorMapFromFile()


            'Arrange
            Dim signals As ICombinedAlternatorSignals = New CombinedAlternatorSignals


            'Act
            Dim target As New CombinedAlternator(COMBINEDALT_GOODMAP)



            'Assert

            Assert.AreEqual(target.Alternators.Count, 4)



        End Sub

        <Test()>
        Public Sub InitialiseCombinedAlternatorMapFromDefault()

            'Arrange
            Dim signals As ICombinedAlternatorSignals = New CombinedAlternatorSignals

            'Act
            Dim target As New CombinedAlternator("123.aalt")

            'Assert
            Assert.AreEqual(target.Alternators.Count, 2)

        End Sub


        <Test()>
        Public Sub AveragedEfficiency()


            '  Dim signals As ICombinedAlternatorSignals = New CombinedAlternatorSignals() With {.CrankRPM=1750, .CurrentDemandAmps=170}

            Dim ca As New CombinedAlternator("abc.aalt")

            ca.Initialise()


            Dim actual As AlternatorMapValues = ca.GetEfficiency(1750, 170.SI(Of Ampere))

            Assert.AreEqual(0.684354842F, actual.Efficiency)


        End Sub

        '<Test()>
        Public Sub Performance()


            Dim ca As New CombinedAlternator("abc.aalt")

            ca.Initialise()

            Dim startDT As DateTime = DateTime.Now
            Dim endDateDT As DateTime
            Dim crank As Single
            Dim demand As Single
            Dim rand As New Random(50)

            Dim min As Double = 0.1
            Dim max As Double = 0.1

            For x = 1 To 500000

                'crank = rand.Next(500,3000)
                'demand = rand.Next(1,200)

                crank = rand.Next(0, 0)
                demand = rand.Next(0, 0)

                Dim actual As AlternatorMapValues = ca.GetEfficiency(crank, demand.SI(Of Ampere))

                If actual.Efficiency < min Then min = actual.Efficiency

                If actual.Efficiency > max Then max = actual.Efficiency

            Next

            endDateDT = DateTime.Now

            Dim secs As Single = (endDateDT - startDT).Seconds


        End Sub


        <Test()>
        Public Sub AlternatorsAreEqual()


            Dim ca As ICombinedAlternator = New CombinedAlternator("abc.aalt")
            Dim original As ICombinedAlternator = New CombinedAlternator("abc.aalt")

            Assert.IsTrue(ca.IsEqualTo(original))

        End Sub

        <Test()>
        Public Sub AlternatorsUnequalName()


            Dim ca As New CombinedAlternator("abc.aalt")
            Dim original As New CombinedAlternator("abc.aalt")

            ca.Alternators(0).AlternatorName = "ZCZZCZCZCZCXXXYYY"

            Assert.IsFalse(ca.IsEqualTo(original))

        End Sub

        <Test()>
        Public Sub AlternatorsUnequalPulley()


            Dim ca As New CombinedAlternator("abc.aalt")
            Dim original As New CombinedAlternator("abc.aalt")

            ca.Alternators(0).PulleyRatio = 9

            Assert.IsFalse(ca.IsEqualTo(original))

        End Sub

        '<Test()>
        ' Public Sub AlternatorsUnequalEfficiency()


        '  Dim ca As new CombinedAlternator("abc.aalt")
        '  Dim original As new CombinedAlternator("abc.aalt")

        '  ca.Alternators(0).InputTable2000(1).Eff=0.99999

        '  'Only tests efficiency values table row 1-3
        '  Assert.IsFalse(ca.IsEqualTo( original))

        ' End Sub


        <Test()>
        Public Sub AlternatorsUnequalEfficiency()


            Dim ca As New CombinedAlternator("abc.aalt")
            Dim original As New CombinedAlternator("abc.aalt")

            ca.Alternators(0).InputTable2000(1).Eff = 0.99999

            'Only tests efficiency values table row 1-3
            Assert.IsFalse(ca.IsEqualTo(original))

        End Sub






    End Class


End Namespace





