
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics

Namespace Mocks

	Public Class AlternatorMapMock
		Implements IAlternatorMap



		Dim failing As Boolean

		Public Sub New(ByVal isFailing As Boolean)
			failing = isFailing
		End Sub


		Public Function GetEfficiency(rpm1 As PerSecond, amps1 As Ampere) As double Implements IAlternatorMap.GetEfficiency
			Return 0.0
		End Function

        Public ReadOnly Property Technologies As IList(Of String) Implements IAlternatorMap.Technologies
        get
                Return New List(Of string)
        End Get
        End Property

        public ReadOnly property Source As String Implements IAlternatorMap.Source
        get
            Return ""
                End Get
        End Property

		'Public Event IAuxiliaryEvent_AuxiliaryEvent As AuxiliaryEventEventHandler Implements IAuxiliaryEvent.AuxiliaryEvent
	End Class

End Namespace
