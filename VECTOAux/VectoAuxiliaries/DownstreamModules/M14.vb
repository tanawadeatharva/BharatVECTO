Imports TUGraz.VectoCommon.Utils
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac

Namespace DownstreamModules
	Public Class M14
		Implements IM14

		Private m13 As IM13
		Private signals As ISignals
		Private constants As IHVACConstants
		Private ssm As ISSMTOOL


		Public Sub New(m13 As IM13, hvacSSM As ISSMTOOL, constants As IHVACConstants, signals As ISignals)

			If m13 Is Nothing Then Throw New ArgumentException("M14, No M13 Supplied in  arguments")
			If hvacSSM Is Nothing Then Throw New ArgumentException("M14, No SSMTOOL constants Supplied in  arguments")
			If constants Is Nothing Then Throw New ArgumentException("M14, No signals Supplied in  arguments")
			If signals Is Nothing Then Throw New ArgumentException("M14, No signals constants Supplied in  arguments")


			Me.m13 = m13
			Me.signals = signals
			Me.constants = constants
			Me.ssm = hvacSSM
		End Sub

		'Staging Calculations
		Private ReadOnly Property S1 As Joule
			Get
				Return m13.WHTCTotalCycleFuelConsumptionGrams * constants.DieselGCVJperGram
			End Get
		End Property

		Private ReadOnly Property S2 As Joule
			Get
				Return ssm.GenInputs.AH_FuelEnergyToHeatToCoolant * S1
			End Get
		End Property

		Private ReadOnly Property S3 As Joule
			Get
				Return S2 * ssm.GenInputs.AH_CoolantHeatTransferredToAirCabinHeater
			End Get
		End Property

		Private ReadOnly Property S4 As Watt 'kW
			Get
				Return (S3 / signals.CurrentCycleTimeInSeconds.SI(Of Second)())
			End Get
		End Property

		Private ReadOnly Property S5 As Second ' hour
			Get
				Return signals.CurrentCycleTimeInSeconds.SI(Of Second)()  ' / 3600
			End Get
		End Property

		Private ReadOnly Property S6 As Kilogram
			Get
				'Return (S5.Value() * ssm.FuelPerHBaseAsjusted(S4.Value() / 1000) / 3600 * (constants.FuelDensity)).SI(Of Kilogram)()
				Return _
				    (S5 * ssm.FuelPerHBaseAsjusted(S4.Value() / 1000).SI(Unit.SI.Liter.Per.Hour)).Cast(Of Liter)() * constants.FuelDensity
					'(S5 * ssm.FuelPerHBaseAsjusted(S4.Value() / 1000).SI().Liter.Per.Hour).Cast(Of Liter)() * constants.FuelDensity
			End Get
		End Property

		Private ReadOnly Property S7 As Kilogram
			Get
				Return m13.WHTCTotalCycleFuelConsumptionGrams + S6
			End Get
		End Property

		Private ReadOnly Property S8 As Liter
			Get
				Return (S7 / (constants.FuelDensity))
			End Get
		End Property

		Public ReadOnly Property TotalCycleFCGrams As Kilogram Implements IM14.TotalCycleFCGrams
			Get
				Return S7
			End Get
		End Property

		Public ReadOnly Property TotalCycleFCLitres As Liter Implements IM14.TotalCycleFCLitres
			Get
				Return S8
			End Get
		End Property
	End Class
End Namespace

