Namespace Electrics
	'This class is reflective of the stored entries for the combined alternator
	'And is used by the Combined Alternator Form and any related classes.

	Public Class CombinedAlternatorMapRow
		Implements ICombinedAlternatorMapRow

		Public Property AlternatorName As String Implements ICombinedAlternatorMapRow.AlternatorName
		Public Property RPM As Single Implements ICombinedAlternatorMapRow.RPM
		Public Property Amps As Single Implements ICombinedAlternatorMapRow.Amps
		Public Property Efficiency As Single Implements ICombinedAlternatorMapRow.Efficiency
		Public Property PulleyRatio As Single Implements ICombinedAlternatorMapRow.PulleyRatio

		'Constructors
		Sub New()
		End Sub

		Sub New(AlternatorName As String, RPM As Single, Amps As Single, Efficiency As Single, PulleyRatio As Single)

			'Sanity Check
			If AlternatorName.Trim.Length = 0 Then Throw New ArgumentException("Alternator name cannot be zero length")
			If Efficiency < 0 Or Efficiency > 100 Then _
				Throw New ArgumentException("Alternator Efficiency must be between 0 and 100")
			If PulleyRatio <= 0 Then Throw New ArgumentException("Alternator Pully ratio must be a positive number")

			'Assignments
			Me.AlternatorName = AlternatorName
			Me.RPM = RPM
			Me.Amps = Amps
			Me.Efficiency = Efficiency
			Me.PulleyRatio = PulleyRatio
		End Sub
	End Class
End Namespace


