
Namespace Hvac
	Public Enum TechLineType

		Normal
		HVCActiveSelection
	End Enum

	Public Enum PowerType

		Mechanical
		Electrical
	End Enum

	'Used by SSMTOOL Class, refer to original spreadsheet model
	'Or PDF Model Document which articulates the same spreadsheet functionality
	'But within the context of the Vecto interpretation of the same.
	Public Class TechListBenefitLine
		Implements ITechListBenefitLine

		Private _h, _vh, _vv, _vc, _c As Single
		Public inputSheet As ISSMGenInputs

		Public Property Units As String Implements ITechListBenefitLine.Units
		Public Property Category As String Implements ITechListBenefitLine.Category
		Public Property BenefitName As String Implements ITechListBenefitLine.BenefitName
		Public Property LowFloorH As New Double Implements ITechListBenefitLine.LowFloorH
		Public Property LowFloorV As New Double Implements ITechListBenefitLine.LowFloorV
		Public Property LowFloorC As New Double Implements ITechListBenefitLine.LowFloorC

		Public Property SemiLowFloorH As New Double Implements ITechListBenefitLine.SemiLowFloorH
		Public Property SemiLowFloorV As New Double Implements ITechListBenefitLine.SemiLowFloorV
		Public Property SemiLowFloorC As New Double Implements ITechListBenefitLine.SemiLowFloorC

		Public Property RaisedFloorH As New Double Implements ITechListBenefitLine.RaisedFloorH
		Public Property RaisedFloorV As New Double Implements ITechListBenefitLine.RaisedFloorV
		Public Property RaisedFloorC As New Double Implements ITechListBenefitLine.RaisedFloorC

		Public Property OnVehicle As Boolean Implements ITechListBenefitLine.OnVehicle
		Public Property ActiveVH As Boolean Implements ITechListBenefitLine.ActiveVH
		Public Property ActiveVV As Boolean Implements ITechListBenefitLine.ActiveVV
		Public Property ActiveVC As Boolean Implements ITechListBenefitLine.ActiveVC
		Public Property LineType As TechLineType Implements ITechListBenefitLine.LineType

		Public ReadOnly Property H As Double Implements ITechListBenefitLine.H
			Get

				Dim returnValue As Double = 0

				'=IF($M49=0,0,IF(AND($M49=1,'INPUT & RESULTS SHEET'!$D$6="low floor"),'TECH LIST INPUT'!D49, IF(AND($M49=1,'INPUT & RESULTS SHEET'!$D$6="semi low floor"),'TECH LIST INPUT'!G49,'TECH LIST INPUT'!J49)))
				If Not OnVehicle Then Return returnValue

				Select Case inputSheet.BP_BusFloorType
					Case "low floor"
						returnValue = LowFloorH
					Case "semi low floor"
						returnValue = SemiLowFloorH
					Case "raised floor"
						returnValue = RaisedFloorH
				End Select

				Return returnValue
			End Get
		End Property

		Public ReadOnly Property VH As Double Implements ITechListBenefitLine.VH
			Get

				Dim floorValue As Double = 0

				If Not OnVehicle Then Return floorValue

				'Get floor value
				Select Case inputSheet.BP_BusFloorType
					Case "low floor"
						floorValue = LowFloorV
					Case "semi low floor"
						floorValue = SemiLowFloorV
					Case "raised floor"
						floorValue = RaisedFloorV
				End Select


				'Active
				If ActiveVH Then ' TechLineType.HVCActiveSelection AndAlso 
					Return floorValue
				Else
					Return 0
				End If
			End Get
		End Property

		Public ReadOnly Property VV As Double Implements ITechListBenefitLine.VV
			Get

				Dim floorValue As Double = 0

				If Not OnVehicle Then Return floorValue

				'Get floor value
				Select Case inputSheet.BP_BusFloorType
					Case "low floor"
						floorValue = LowFloorV
					Case "semi low floor"
						floorValue = SemiLowFloorV
					Case "raised floor"
						floorValue = RaisedFloorV
				End Select


				'Active
				If ActiveVV Then '  TechLineType.HVCActiveSelection AndAlso
					Return floorValue
				Else
					Return 0
				End If
			End Get
		End Property

		Public ReadOnly Property VC As Double Implements ITechListBenefitLine.VC
			Get

				Dim floorValue As Double = 0

				If Not OnVehicle Then Return floorValue

				'Get floor value
				Select Case inputSheet.BP_BusFloorType
					Case "low floor"
						floorValue = LowFloorV
					Case "semi low floor"
						floorValue = SemiLowFloorV
					Case "raised floor"
						floorValue = RaisedFloorV
				End Select


				'Active
				If ActiveVC Then ' TechLineType.HVCActiveSelection AndAlso
					Return floorValue
				Else
					Return 0
				End If
			End Get
		End Property

		Public ReadOnly Property C As Double Implements ITechListBenefitLine.C
			Get

				Dim returnValue As Double = 0


				If Not OnVehicle Then Return returnValue

				Select Case inputSheet.BP_BusFloorType
					Case "low floor"
						returnValue = LowFloorC
					Case "semi low floor"
						returnValue = SemiLowFloorC
					Case "raised floor"
						returnValue = RaisedFloorC
				End Select

				Return returnValue
			End Get
		End Property

		Sub New()
		End Sub

		Sub New(geninputs As ISSMGenInputs)

			Me.inputSheet = geninputs
		End Sub

		Sub New(geninputs As ISSMGenInputs,
				units As String,
				category As String,
				benefitName As String,
				lowFloorH As Double,
				lowFloorV As Double,
				lowFloorC As Double,
				semiLowFloorH As Double,
				semiLowFloorV As Double,
				semiLowFloorC As Double,
				raisedFloorH As Double,
				raisedFloorV As Double,
				raisedFloorC As Double,
				onVehicle As Boolean,
				lineType As TechLineType,
				activeVH As Boolean,
				activeVV As Boolean,
				activeVC As Boolean
				)

			Me.inputSheet = geninputs
			Me.Units = Units
			Me.category = category
			Me.benefitName = benefitName
			Me.lowFloorH = lowFloorH
			Me.lowFloorV = lowFloorV
			Me.lowFloorC = lowFloorC
			Me.semiLowFloorH = semiLowFloorH
			Me.semiLowFloorV = semiLowFloorV
			Me.semiLowFloorC = semiLowFloorC
			Me.raisedFloorH = raisedFloorH
			Me.raisedFloorV = raisedFloorV
			Me.raisedFloorC = raisedFloorC
			Me.OnVehicle = onVehicle
			Me.lineType = lineType
			Me.ActiveVH = activeVH
			Me.ActiveVV = activeVV
			Me.ActiveVC = activeVC
		End Sub

		'Operator Overloads
		Public Shared Operator =(ByVal op1 As TechListBenefitLine, ByVal op2 As TechListBenefitLine) As Boolean

			If (op1.Category = op2.Category AndAlso
				op1.BenefitName = op2.BenefitName AndAlso
				op1.ActiveVC = op2.ActiveVC AndAlso
				op1.ActiveVH = op2.ActiveVH AndAlso
				op1.ActiveVV = op2.ActiveVV AndAlso
				op1.LineType = op2.LineType AndAlso
				op1.LowFloorC = op2.LowFloorC AndAlso
				op1.LowFloorV = op2.LowFloorV AndAlso
				op1.LowFloorH = op2.LowFloorH AndAlso
				op1.SemiLowFloorC = op2.SemiLowFloorC AndAlso
				op1.SemiLowFloorH = op2.SemiLowFloorH AndAlso
				op1.SemiLowFloorV = op2.SemiLowFloorV AndAlso
				op1.RaisedFloorC = op2.RaisedFloorC AndAlso
				op1.RaisedFloorH = op2.RaisedFloorH AndAlso
				op1.RaisedFloorV = op2.RaisedFloorV AndAlso
				op1.OnVehicle = op2.OnVehicle AndAlso
				op1.Units = op2.Units) Then

				Return True

			Else

				Return False

			End If
		End Operator

		Public Shared Operator <>(ByVal op1 As TechListBenefitLine, ByVal op2 As TechListBenefitLine) As Boolean

			If (op1.Category <> op2.Category OrElse
				op1.BenefitName <> op2.BenefitName OrElse
				op1.ActiveVC <> op2.ActiveVC OrElse
				op1.ActiveVH <> op2.ActiveVH OrElse
				op1.ActiveVV <> op2.ActiveVV OrElse
				op1.LineType <> op2.LineType OrElse
				op1.LowFloorC <> op2.LowFloorC OrElse
				op1.LowFloorV <> op2.LowFloorV OrElse
				op1.LowFloorH <> op2.LowFloorH OrElse
				op1.SemiLowFloorC <> op2.SemiLowFloorC OrElse
				op1.SemiLowFloorH <> op2.SemiLowFloorH OrElse
				op1.SemiLowFloorV <> op2.SemiLowFloorV OrElse
				op1.RaisedFloorC <> op2.RaisedFloorC OrElse
				op1.RaisedFloorH <> op2.RaisedFloorH OrElse
				op1.RaisedFloorV <> op2.RaisedFloorV OrElse
				op1.OnVehicle <> op2.OnVehicle OrElse
				op1.Units <> op2.Units) Then

				Return True

			Else

				Return False

			End If
		End Operator

		Public Sub CloneFrom(source As ITechListBenefitLine) Implements ITechListBenefitLine.CloneFrom


			Me.Units = source.Units
			Me.Category = source.Category
			Me.BenefitName = source.BenefitName
			Me.LowFloorH = source.LowFloorH
			Me.LowFloorV = source.LowFloorV
			Me.LowFloorC = source.LowFloorC

			Me.SemiLowFloorH = source.SemiLowFloorH
			Me.SemiLowFloorV = source.SemiLowFloorV
			Me.SemiLowFloorC = source.SemiLowFloorC

			Me.RaisedFloorH = source.RaisedFloorH
			Me.RaisedFloorV = source.RaisedFloorV
			Me.RaisedFloorC = source.RaisedFloorC

			Me.OnVehicle = source.OnVehicle
			Me.ActiveVH = source.ActiveVH
			Me.ActiveVV = source.ActiveVV
			Me.ActiveVC = source.ActiveVC
			Me.LineType = source.LineType
		End Sub

		Public Function IsEqualTo(source As ITechListBenefitLine) As Boolean Implements ITechListBenefitLine.IsEqualTo
			Dim mySource As TechListBenefitLine = CType(source, TechListBenefitLine)
			If mySource Is Nothing Then
				Return False
			End If
			Return Me = mySource
		End Function
	End Class
End Namespace


