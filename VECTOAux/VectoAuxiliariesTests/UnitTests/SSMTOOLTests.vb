Imports NUnit.Framework
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliariesTests.Mocks
Imports VectoAuxiliaries
Imports VectoAuxiliaries.Hvac

Namespace UnitTests
	<TestFixture()>
	Public Class _SSMTOOLTests
		'TechBenefitsList - FilePath Constants
		Private Const GOODTechList As String = "TestFiles\testSSMTechBenefits.csv"
		Private Const GOODTechListALLON As String = "TestFiles\testSSMTechBenefitsALLON.csv"
		Private Const GOODTechListALLOFF As String = "TestFiles\testSSMTechBenefitsALLOFF.csv"
		Private Const GOODTechListEMPTYLIST As String = "TestFiles\testSSMTechBenefitsEMPTYLIST.csv"

		'Helpers
		Private Function AddDefaultTechLine(source As ISSMTOOL)

			Dim src As SSMTOOL = DirectCast(source, SSMTOOL)

			Dim newItem As ITechListBenefitLine = New TechListBenefitLine(src.GenInputs)

			newItem.Units = "fraction"
			newItem.Category = "Insulation"
			newItem.BenefitName = "Benefit1"

			newItem.LowFloorH = 0.1
			newItem.LowFloorV = 0.1
			newItem.LowFloorC = 0.1

			newItem.SemiLowFloorH = 0.1
			newItem.SemiLowFloorV = 0.1
			newItem.SemiLowFloorC = 0.1

			newItem.RaisedFloorH = 0.1
			newItem.RaisedFloorV = 0.1
			newItem.RaisedFloorC = 0.1

			newItem.OnVehicle = True
			newItem.ActiveVH = True
			newItem.ActiveVV = True
			newItem.ActiveVC = True
			newItem.LineType = TechLineType.Normal

			Dim feedback As String = String.Empty

			Assert.IsTrue(src.TechList.Add(newItem, feedback))
		End Function

		'SSMGenInputTests
		<Test()> _
		<TestCase("BusParameterisation")> _
		<TestCase("BoundaryConditions")> _
		<TestCase("EnvironmentalConditions")> _
		<TestCase("AC-System")> _
		<TestCase("Ventilation")> _
		<TestCase("AuxHeater")>
		Public Sub InstantiateDefaultSSMGenInputsTest(section As String)

			Dim target As ISSMGenInputs = New SSMGenInputs(True)

			If section = "BusParameterisation" Then
				'BUS Parameterisation
				'********************
				Assert.AreEqual(47, target.BP_NumberOfPassengers)
				Assert.AreEqual("raised floor", target.BP_BusFloorType)
				Assert.AreEqual(24.1102486R, target.BP_BusFloorSurfaceArea, 2)
				Assert.AreEqual(114.42325R, target.BP_BusSurfaceAreaM2)
				Assert.AreEqual(20.98R, Math.Round(target.BP_BusWindowSurface, 2))
				Assert.AreEqual(61.81231875D, Math.Round(target.BP_BusVolume, 8))
				Assert.AreEqual(10.655R, target.BP_BusLength)
				Assert.AreEqual(2.55R, target.BP_BusWidth)
			End If

			If section = "BoundaryConditions" Then
				'BOUNDRY CONDITIONS
				'******************
				Assert.AreEqual(0.95R, target.BC_GFactor)
				Assert.AreEqual(0.8R, target.BC_SolarClouding)
				Assert.AreEqual(80, target.BC_HeatPerPassengerIntoCabinW)
				Assert.AreEqual(12, target.BC_PassengerBoundaryTemperature)
				Assert.AreEqual(3.0R, target.BC_PassengerDensityLowFloor)
				Assert.AreEqual(2.2R, target.BC_PassengerDensitySemiLowFloor)
				Assert.AreEqual(1.4R, target.BC_PassengerDensityRaisedFloor)
				Assert.AreEqual(34.0R, Math.Round(target.BC_CalculatedPassengerNumber, 4))
				Assert.AreEqual(3.0R, target.BC_UValues)
				Assert.AreEqual(18, target.BC_HeatingBoundaryTemperature)
				Assert.AreEqual(23, target.BC_CoolingBoundaryTemperature)
				Assert.AreEqual(20, target.BC_HighVentilation)
				Assert.AreEqual(7, target.BC_lowVentilation)
				Assert.AreEqual(1236.25, Math.Round(target.BC_High, 2))
				Assert.AreEqual(432.69, Math.Round(target.BC_Low, 2))
				Assert.AreEqual(692.3, Math.Round(target.BC_HighVentPowerW, 2))
				Assert.AreEqual(242.3, Math.Round(target.BC_LowVentPowerW, 2))
				Assert.AreEqual(0.56R, target.BC_SpecificVentilationPower)
				Assert.AreEqual(0.84, target.BC_AuxHeaterEfficiency)
				Assert.AreEqual(11.8, target.BC_GCVDieselOrHeatingOil)
				Assert.AreEqual(1.5R, target.BC_WindowAreaPerUnitBusLength)
				Assert.AreEqual(5, target.BC_FrontRearWindowArea)
				Assert.AreEqual(3, target.BC_MaxTemperatureDeltaForLowFloorBusses)
				Assert.AreEqual(0.5R, target.BC_MaxPossibleBenefitFromTechnologyList)
			End If


			If section = "EnvironmentalConditions" Then
				'Environmental Conditions
				'************************
				Assert.AreEqual(25.0, target.EC_EnviromentalTemperature)
				Assert.AreEqual(400.0, target.EC_Solar)

			End If

			If section = "AC-System" Then
				'AC-SYSTEM
				'*********
				Assert.AreEqual("2-stage", target.AC_CompressorType)
				Assert.AreEqual(18, target.AC_CompressorCapacitykW)
				Assert.AreEqual(3.5, target.AC_COP)
			End If

			If section = "Ventilation" Then
				'VENTILATION
				'***********                                                                            
				Assert.Areequal(True, target.VEN_VentilationOnDuringHeating)
				Assert.Areequal(True, target.VEN_VentilationWhenBothHeatingAndACInactive)
				Assert.Areequal(True, target.VEN_VentilationDuringAC)
				Assert.Areequal("high", target.VEN_VentilationFlowSettingWhenHeatingAndACInactive)
				Assert.Areequal("high", target.VEN_VentilationDuringHeating)
				Assert.AreEqual("high", target.VEN_VentilationDuringCooling)

			End If

			If section = "AuxHeater" Then
				'AUX HEATER
				'**********
				Assert.AreEqual(0, target.AH_EngineWasteHeatkW)
				Assert.AreEqual(30, target.AH_FuelFiredHeaterkW)
			End If
		End Sub

		'Basic TechListTests
		<Test()>
		Public Sub Instantiate_TechListTest()


			Dim gen As ISSMGenInputs = New SSMGenInputs(True)

			Dim target As ISSMTechList = New SSMTechList(GOODTechList, gen)


			Assert.IsTrue(target.Initialise())
		End Sub

		<Test()>
		Public Sub Instantiate_TechListTestALLON()


			Dim gen As ISSMGenInputs = New SSMGenInputs(True)

			Dim target As ISSMTechList = New SSMTechList(GOODTechListALLON, gen)

			Dim v As Double = target.CValueVariation


			Assert.IsTrue(target.Initialise())
			Assert.AreEqual(0.142, Math.Round(target.HValueVariation, 3))
			Assert.AreEqual(0.006, Math.Round(target.VHValueVariation, 3))
			Assert.AreEqual(0.006, Math.Round(target.VVValueVariation, 3))
			Assert.AreEqual(0.006, Math.Round(target.VCValueVariation, 3))
			Assert.AreEqual(0.259, Math.Round(target.CValueVariation, 3))

			Assert.AreEqual(0.0, Math.Round(target.VHValueVariationKW, 3))
			Assert.AreEqual(0.0, Math.Round(target.VVValueVariationKW, 3))
			Assert.AreEqual(0.0, Math.Round(target.VCValueVariationKW, 3))
			Assert.AreEqual(0.0, Math.Round(target.VCValueVariationKW, 3))
			Assert.AreEqual(-0.2, Math.Round(target.CValueVariationKW, 3))
		End Sub

		'List Management Methods
		<Test()>
		Public Sub Instantiate_TechListTestEMPTYList()


			Dim gen As ISSMGenInputs = New SSMGenInputs(True)

			Dim target As ISSMTechList = New SSMTechList(GOODTechListEMPTYLIST, gen)

			Assert.IsTrue(target.Initialise())

			Assert.IsTrue(target.TechLines.Count = 0)
		End Sub

		<Test()>
		Public Sub Instantiate_TechListTestEMPTYListADD1()


			Dim gen As ISSMGenInputs = New SSMGenInputs(True)

			Dim target As ISSMTechList = New SSMTechList(GOODTechListEMPTYLIST, gen)

			Dim newItem As ITechListBenefitLine = New TechListBenefitLine(gen)

			newItem.Units = "fraction"
			newItem.Category = "Insulation"
			newItem.BenefitName = "Benefit1"

			newItem.LowFloorH = 0.1
			newItem.LowFloorV = 0.1
			newItem.LowFloorC = 0.1

			newItem.SemiLowFloorH = 0.1
			newItem.SemiLowFloorV = 0.1
			newItem.SemiLowFloorC = 0.1

			newItem.RaisedFloorH = 0.1
			newItem.RaisedFloorV = 0.1
			newItem.RaisedFloorC = 0.1

			newItem.OnVehicle = True
			newItem.ActiveVH = True
			newItem.ActiveVV = True
			newItem.ActiveVC = True
			newItem.LineType = TechLineType.Normal

			Dim feedback As String = String.Empty

			Assert.istrue(target.Add(newItem, feedback))


			Assert.IsTrue(target.TechLines.Count = 1)
		End Sub

		<Test()>
		Public Sub Instantiate_TechListTestEMPTYListADD1Duplicate()


			Dim gen As ISSMGenInputs = New SSMGenInputs(True)

			Dim target As ISSMTechList = New SSMTechList(GOODTechListEMPTYLIST, gen)

			Dim newItem As ITechListBenefitLine = New TechListBenefitLine(gen)

			newItem.Units = "fraction"
			newItem.Category = "Insulation"
			newItem.BenefitName = "Benefit1"

			newItem.LowFloorH = 0.1
			newItem.LowFloorV = 0.1
			newItem.LowFloorC = 0.1

			newItem.SemiLowFloorH = 0.1
			newItem.SemiLowFloorV = 0.1
			newItem.SemiLowFloorC = 0.1

			newItem.RaisedFloorH = 0.1
			newItem.RaisedFloorV = 0.1
			newItem.RaisedFloorC = 0.1

			newItem.OnVehicle = True
			newItem.ActiveVH = True
			newItem.ActiveVV = True
			newItem.ActiveVC = True
			newItem.LineType = TechLineType.Normal

			Dim feedback As String = String.Empty

			Assert.istrue(target.Add(newItem, feedback))
			Assert.isFalse(target.Add(newItem, feedback))

			Assert.IsTrue(target.TechLines.Count = 1)
		End Sub

		<Test()>
		Public Sub Instantiate_TechListTestEMPTYListADD1AndClear()


			Dim gen As ISSMGenInputs = New SSMGenInputs(True)

			Dim target As ISSMTechList = New SSMTechList(GOODTechListEMPTYLIST, gen)

			Dim newItem As ITechListBenefitLine = New TechListBenefitLine(gen)

			newItem.Units = "fraction"
			newItem.Category = "Insulation"
			newItem.BenefitName = "Benefit1"

			newItem.LowFloorH = 0.1
			newItem.LowFloorV = 0.1
			newItem.LowFloorC = 0.1

			newItem.SemiLowFloorH = 0.1
			newItem.SemiLowFloorV = 0.1
			newItem.SemiLowFloorC = 0.1

			newItem.RaisedFloorH = 0.1
			newItem.RaisedFloorV = 0.1
			newItem.RaisedFloorC = 0.1

			newItem.OnVehicle = True
			newItem.ActiveVH = True
			newItem.ActiveVV = True
			newItem.ActiveVC = True
			newItem.LineType = TechLineType.Normal

			Dim feedback As String = String.Empty

			Assert.IsTrue(target.Add(newItem, feedback))
			Assert.IsTrue(target.TechLines.Count = 1)
			target.Clear()
			Assert.IsTrue(target.TechLines.Count = 0)
		End Sub

		<Test()>
		Public Sub Instantiate_TechListTestEMPTYListADD1AndModify()


			Dim gen As ISSMGenInputs = New SSMGenInputs(True)

			Dim target As ISSMTechList = New SSMTechList(GOODTechListEMPTYLIST, gen)

			Dim newItem As ITechListBenefitLine = New TechListBenefitLine(gen)

			newItem.Units = "fraction"
			newItem.Category = "Insulation"
			newItem.BenefitName = "Benefit1"

			newItem.LowFloorH = 0.1
			newItem.LowFloorV = 0.1
			newItem.LowFloorC = 0.1

			newItem.SemiLowFloorH = 0.1
			newItem.SemiLowFloorV = 0.1
			newItem.SemiLowFloorC = 0.1

			newItem.RaisedFloorH = 0.1
			newItem.RaisedFloorV = 0.1
			newItem.RaisedFloorC = 0.1

			newItem.OnVehicle = True
			newItem.ActiveVH = True
			newItem.ActiveVV = True
			newItem.ActiveVC = True
			newItem.LineType = TechLineType.Normal

			Dim feedback As String = String.Empty

			'Add
			Assert.IsTrue(target.Add(newItem, feedback))

			'Modify
			newItem.LowFloorC = 0.99
			Assert.IsTrue(target.TechLines(0).IsEqualTo(newItem))
		End Sub

		<Test()>
		Public Sub Instantiate_TechListTestEMPTYListADD1andDeleteIt()


			Dim gen As ISSMGenInputs = New SSMGenInputs(True)

			Dim target As ISSMTechList = New SSMTechList(GOODTechListEMPTYLIST, gen)

			Dim newItem As ITechListBenefitLine = New TechListBenefitLine(gen)

			newItem.Units = "fraction"
			newItem.Category = "Insulation"
			newItem.BenefitName = "Benefit1"

			newItem.LowFloorH = 0.1
			newItem.LowFloorV = 0.1
			newItem.LowFloorC = 0.1

			newItem.SemiLowFloorH = 0.1
			newItem.SemiLowFloorV = 0.1
			newItem.SemiLowFloorC = 0.1

			newItem.RaisedFloorH = 0.1
			newItem.RaisedFloorV = 0.1
			newItem.RaisedFloorC = 0.1

			newItem.OnVehicle = True
			newItem.ActiveVH = True
			newItem.ActiveVV = True
			newItem.ActiveVC = True
			newItem.LineType = TechLineType.Normal

			Dim feedback As String = String.Empty

			Assert.IsTrue(target.Add(newItem, feedback))
			Assert.IsTrue(target.TechLines.Count = 1)
			Assert.IsTrue(target.Delete(newItem, feedback))
			Assert.IsTrue(target.TechLines.Count = 0)
		End Sub

		<Test()>
		Public Sub Instantiate_TechListTestEMPTYListandDeleteNonExistantItem()


			Dim gen As ISSMGenInputs = New SSMGenInputs(True)

			Dim target As ISSMTechList = New SSMTechList(GOODTechListEMPTYLIST, gen)

			Dim newItem As ITechListBenefitLine = New TechListBenefitLine(gen)

			newItem.Units = "fraction"
			newItem.Category = "Insulation"
			newItem.BenefitName = "Benefit1"

			newItem.LowFloorH = 0.1
			newItem.LowFloorV = 0.1
			newItem.LowFloorC = 0.1

			newItem.SemiLowFloorH = 0.1
			newItem.SemiLowFloorV = 0.1
			newItem.SemiLowFloorC = 0.1

			newItem.RaisedFloorH = 0.1
			newItem.RaisedFloorV = 0.1
			newItem.RaisedFloorC = 0.1

			newItem.OnVehicle = True
			newItem.ActiveVH = True
			newItem.ActiveVV = True
			newItem.ActiveVC = True
			newItem.LineType = TechLineType.Normal

			Dim feedback As String = String.Empty

			Assert.IsFalse(target.Delete(newItem, feedback))
		End Sub

		'TechListLineTests
		<Test()>
		Public Sub Instantiate_NewTechListLine()

			Dim gen As ISSMGenInputs = New SSMGenInputs(True)

			Dim ttl As ITechListBenefitLine = New TechListBenefitLine(gen)

			Assert.IsNotNull(ttl)
		End Sub

		<Test()>
		Public Sub TechBenefitLineCompareAsEqual()

			Dim gen As ISSMGenInputs = New SSMGenInputs(True)

			Dim ttl1 As ITechListBenefitLine = New TechListBenefitLine(gen)
			Dim ttl2 As ITechListBenefitLine = New TechListBenefitLine(gen)

			Assert.IsTrue(ttl1.IsEqualTo(ttl2))
		End Sub

		<Test()> _
		<TestCase("Category")> _
		<TestCase("BenefitName")> _
		<TestCase("ActiveVC")> _
		<TestCase("ActiveVH")> _
		<TestCase("ActiveVV")> _
		<TestCase("LineType")> _
		<TestCase("LowFloorC")> _
		<TestCase("LowFloorV")> _
		<TestCase("LowFloorH")> _
		<TestCase("SemiLowFloorC")> _
		<TestCase("SemiLowFloorH")> _
		<TestCase("SemiLowFloorV")> _
		<TestCase("RaisedFloorC")> _
		<TestCase("RaisedFloorH")> _
		<TestCase("RaisedFloorV")> _
		<TestCase("Units")> _
		<TestCase("OnVehicle")>
		Public Sub TechBenefitLineCompareAsUnequal(prop As String)

			Dim gen As ISSMGenInputs = New SSMGenInputs(True)

			Dim ttl1 As ITechListBenefitLine = New TechListBenefitLine(gen)
			Dim ttl2 As ITechListBenefitLine = New TechListBenefitLine(gen)

			Select Case prop


				Case "Category"
					ttl2.Category = "NOT"
				Case "BenefitName"
					ttl2.BenefitName = "NOT"
				Case "ActiveVC"
					ttl2.ActiveVC = True
				Case "ActiveVH"
					ttl2.ActiveVH = True
				Case "ActiveVV"
					ttl2.ActiveVV = True
				Case "LineType"
					ttl2.LineType = TechLineType.HVCActiveSelection
				Case "LowFloorC"
					ttl2.LowFloorC = 1
				Case "LowFloorV"
					ttl2.LowFloorV = 1
				Case "LowFloorH"
					ttl2.LowFloorH = 1
				Case "SemiLowFloorC"
					ttl2.SemiLowFloorC = 1
				Case "SemiLowFloorH"
					ttl2.SemiLowFloorH = 1
				Case "SemiLowFloorV"
					ttl2.SemiLowFloorH = 1
				Case "RaisedFloorC"
					ttl2.RaisedFloorC = 1
				Case "RaisedFloorH"
					ttl2.RaisedFloorH = 1
				Case "RaisedFloorV"
					ttl2.RaisedFloorV = 1
				Case "Units"
					ttl2.Units = "NONE"
				Case "OnVehicle"
					ttl2.OnVehicle = True

			End Select


			Assert.IsFalse(ttl1.IsEqualTo(ttl2))
		End Sub

		'SSMTOOL Persistance
		<Test()>
		Public Sub SaveAndRetreiveTest()

			Const filePath As String = "SSMTOOLTestSaveRetreive.json"

			Dim target As SSMTOOL = New SSMTOOL(filePath, New HVACConstants(), False, True)

			target.Save(filePath)

			'change something
			target.GenInputs.BP_BusLength = 202.202

			Assert.AreEqual(target.GenInputs.BP_BusLength, 202.202)

			'Retreive
			target.Load(filePath)

			Assert.AreEqual(target.GenInputs.BP_BusLength, 10.655)
		End Sub

		'GenInputs Comparison
		<Test()>
		Public Sub SSMTOOL_COMPARISON_GENINPUTS_EQUAL()

			Const filePath As String = "SSMTOOLTestSaveRetreive.json"


			Dim ssmTool1 As SSMTOOL = New SSMTOOL(filePath, New HVACConstants())
			Dim ssmTool2 As SSMTOOL = New SSMTOOL(filePath, New HVACConstants())


			Assert.IsTrue(ssmTool1.IsEqualTo(ssmTool2))
		End Sub

		<Test()>
		Public Sub SSMTOOL_COMPARISON_GENINPUTS_UNEQUAL()

			Const filePath As String = "SSMTOOLTestSaveRetreive.json"


			Dim ssmTool1 As SSMTOOL = New SSMTOOL(filePath, New HVACConstants())

			'Alter somthing
			ssmTool1.genInputs.BP_BusLength = 11

			Dim ssmTool2 As SSMTOOL = New SSMTOOL(filePath, New HVACConstants())


			Assert.IsFalse(ssmTool1.IsEqualTo(ssmTool2))
		End Sub

		'TechListBenefitLine Comparison
		<Test()>
		Public Sub SSMTOOL_COMPARISON_TECHLIST_EQUAL()

			Const filePath As String = "SSMTOOLTestSaveRetreive.json"


			Dim ssmTool1 As SSMTOOL = New SSMTOOL(filePath, New HVACConstants())
			Dim ssmTool2 As SSMTOOL = New SSMTOOL(filePath, New HVACConstants())


			Assert.IsTrue(ssmTool1.IsEqualTo(ssmTool2))
		End Sub

		<Test()>
		Public Sub SSMTOOL_COMPARISON_TECHLIST_EMPTYLISTS_UNEQUALCOUNT()

			Const filePath As String = "SSMTOOLTestSaveRetreive.json"


			Dim ssmTool1 As SSMTOOL = New SSMTOOL(filePath, New HVACConstants())
			Dim ssmTool2 As SSMTOOL = New SSMTOOL(filePath, New HVACConstants())

			'Change something on techlist
			AddDefaultTechLine(ssmTool1)

			Assert.IsFalse(ssmTool1.IsEqualTo(ssmTool2))
		End Sub

		<Test()>
		Public Sub SSMTOOL_COMPARISON_TECHLIST_IDENTICAL_EQUAL()

			Const filePath As String = "SSMTOOLTestSaveRetreive.json"


			Dim ssmTool1 As SSMTOOL = New SSMTOOL(filePath, New HVACConstants())
			Dim ssmTool2 As SSMTOOL = New SSMTOOL(filePath, New HVACConstants())

			'Change something on techlist
			AddDefaultTechLine(ssmTool1)
			AddDefaultTechLine(ssmTool2)

			Assert.IsTrue(ssmTool1.IsEqualTo(ssmTool2))
		End Sub

		<Test()>
		Public Sub SSMTOOL_COMPARISON_TECHLIST_IDENTICAL_SINGLEKeyValueDifference()

			Const filePath As String = "SSMTOOLTestSaveRetreive.json"


			Dim ssmTool1 As SSMTOOL = New SSMTOOL(filePath, New HVACConstants())
			Dim ssmTool2 As SSMTOOL = New SSMTOOL(filePath, New HVACConstants())

			'Change something on techlist
			AddDefaultTechLine(ssmTool1)
			AddDefaultTechLine(ssmTool2)

			'Make Unequal
			ssmTool2.TechList.TechLines(0).BenefitName = "Doobie"

			Assert.IsFalse(ssmTool1.IsEqualTo(ssmTool2))
		End Sub

		<Test()>
		Public Sub SSMTOOL_COMPARISON_TECHLIST_IDENTICAL_SINGLEValueDifference()

			Const filePath As String = "SSMTOOLTestSaveRetreive.json"


			Dim ssmTool1 As SSMTOOL = New SSMTOOL(filePath, New HVACConstants())
			Dim ssmTool2 As SSMTOOL = New SSMTOOL(filePath, New HVACConstants())

			'Change something on techlist
			AddDefaultTechLine(ssmTool1)
			AddDefaultTechLine(ssmTool2)

			'Make Unequal
			ssmTool2.TechList.TechLines(0).ActiveVV = False

			Assert.IsFalse(ssmTool1.IsEqualTo(ssmTool2))
		End Sub
	End Class
End Namespace


