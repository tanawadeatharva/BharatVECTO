Namespace Hvac
	Public Interface ITechListBenefitLine
		Property Units As string
		Property Category As String
		Property BenefitName As String

		Property LowFloorH As double
		Property LowFloorV As double
		Property LowFloorC As double

		Property SemiLowFloorH As double
		Property SemiLowFloorV As double
		Property SemiLowFloorC As double

		Property RaisedFloorH As double
		Property RaisedFloorV As double
		Property RaisedFloorC As double

		Property OnVehicle As Boolean
		Property ActiveVH As Boolean
		Property ActiveVV As Boolean
		Property ActiveVC As Boolean
		Property LineType As TechLineType

		ReadOnly Property H As Double
		ReadOnly Property VH As Double
		ReadOnly Property VV As Double
		ReadOnly Property VC As Double
		ReadOnly Property C As Double

		Sub CloneFrom(source As ITechListBenefitLine)

		Function IsEqualTo(source As ITechListBenefitLine) As Boolean
	End Interface
End Namespace


