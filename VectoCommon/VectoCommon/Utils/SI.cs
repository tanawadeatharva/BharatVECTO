/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.Exceptions;

// ReSharper disable ClassNeverInstantiated.Global

namespace TUGraz.VectoCommon.Utils
{
	/// <summary>
	/// SI Class for Scalar Values. Converts implicitely to double and is only castable if the SI value has no units.
	/// </summary>
	public class Scalar : SIBase<Scalar>
	{
		[DebuggerHidden]
		private Scalar(double val) : base(val) {}

		public static implicit operator double(Scalar self)
		{
			return self.Val;
		}

		/// <summary>
		/// Implements the operator +.
		/// </summary>
		[DebuggerHidden]
		public static Scalar operator +(Scalar si1, Scalar si2)
		{
			return Create(si1.Val + si2.Val);
		}

		/// <summary>
		/// Implements the operator +.
		/// </summary>
		[DebuggerHidden]
		public static Scalar operator +(Scalar si1, double si2)
		{
			return Create(si1.Val + si2);
		}

		/// <summary>
		/// Implements the operator +.
		/// </summary>
		[DebuggerHidden]
		public static Scalar operator +(double si1, Scalar si2)
		{
			return Create(si1 + si2.Val);
		}

		/// <summary>
		/// Implements the operator -.
		/// </summary>
		[DebuggerHidden]
		public static Scalar operator -(Scalar si1, Scalar si2)
		{
			return Create(si1.Val - si2.Val);
		}

		/// <summary>
		/// Implements the operator -.
		/// </summary>
		[DebuggerHidden]
		public static Scalar operator -(Scalar si1, double si2)
		{
			return Create(si1.Val - si2);
		}

		/// <summary>
		/// Implements the operator -.
		/// </summary>
		[DebuggerHidden]
		public static Scalar operator -(double si1, Scalar si2)
		{
			return Create(si1 - si2.Val);
		}
	}

	/// <summary>
	/// SI Class for Newton [N].
	/// </summary>
	public class Newton : SIBase<Newton>
	{
	    private static readonly int[] SIDefault = new int[7] { 1, 1, -2, 0, 0, 0, 0 };

        [DebuggerHidden]
        private Newton(double val) : base(val, SIDefault) { }

		/// <summary>
		/// Implements the operator *.
		/// </summary>
		/// <param name="newton">The newton.</param>
		/// <param name="meter">The meter.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static NewtonMeter operator *(Newton newton, Meter meter)
		{
			return SIBase<NewtonMeter>.Create(newton.Val * meter.Value());
		}

		[DebuggerHidden]
		public static Watt operator *(Newton newton, MeterPerSecond meterPerSecond)
		{
			return SIBase<Watt>.Create(newton.Val * meterPerSecond.Value());
		}

		[DebuggerHidden]
		public static Watt operator *(MeterPerSecond meterPerSecond, Newton newton)
		{
			return SIBase<Watt>.Create(newton.Val * meterPerSecond.Value());
		}
	}

	/// <summary>
	/// SI Class for Radian [] (rad).
	/// </summary>
	public class Radian : SIBase<Radian>
	{
		[DebuggerHidden]
		private Radian(double val) : base(val) {}
	}

	/// <summary>
	/// SI Class for PerSquareSecond [1/s^2].
	/// </summary>
	public class PerSquareSecond : SIBase<PerSquareSecond>
	{
	    private static readonly int[] SIDefault = new int[7] { 0, 0, -2, 0, 0, 0, 0 };
        [DebuggerHidden]
        private PerSquareSecond(double val) : base(val, SIDefault) { }

		[DebuggerHidden]
		public static PerSecond operator *(PerSquareSecond perSquareSecond, Second second)
		{
			return SIBase<PerSecond>.Create(perSquareSecond.Val * second.Value());
		}
	}

	/// <summary>
	/// SI Class for Meter per square second [m/s^2].
	/// </summary>
	public class MeterPerSquareSecond : SIBase<MeterPerSquareSecond>
	{
	    private static readonly int[] SIDefault = new int[7] { 0, 1, -2, 0, 0, 0, 0 };

        [DebuggerHidden]
        private MeterPerSquareSecond(double val) : base(val, SIDefault) { }

		/// <summary>
		/// Implements the operator *.
		/// </summary>
		[DebuggerHidden]
		public static MeterPerSecond operator *(MeterPerSquareSecond meterPerSecond, Second second)
		{
			return SIBase<MeterPerSecond>.Create(meterPerSecond.Val * second.Value());
		}
	}

	/// <summary>
	/// SI Class for Second [s].
	/// </summary>
	public class Second : SIBase<Second>
	{
	    private static readonly int[] SIDefault = new int[7] { 0, 0, 1, 0, 0, 0, 0 };

        [DebuggerHidden]
        private Second(double val) : base(val, SIDefault) { }
	}

	/// <summary>
	/// SI Class for Meter [m].
	/// </summary>
	public class Meter : SIBase<Meter>
	{
	    private static readonly int[] SIDefault = new int[7] { 0, 1, 0, 0, 0, 0, 0 };

		[DebuggerHidden]
		private Meter(double val) : base(val, SIDefault) {}

		[DebuggerHidden]
		public static MeterPerSecond operator /(Meter meter, Second second)
		{
			return SIBase<MeterPerSecond>.Create(meter.Val / second.Value());
		}

		[DebuggerHidden]
		public static MeterPerSecond operator *(Meter meter, PerSecond perSecond)
		{
			return SIBase<MeterPerSecond>.Create(meter.Val * perSecond.Value());
		}

		/// <summary>
		/// Implements the operator /.
		/// </summary>
		[DebuggerHidden]
		public static Second operator /(Meter second, MeterPerSecond meterPerSecond)
		{
			return SIBase<Second>.Create(second.Val / meterPerSecond.Value());
		}
	}

	/// <summary>
	///  SI Class for KilogramPerMeter [kg/m].
	/// </summary>
	public class KilogramPerMeter : SIBase<KilogramPerMeter>
	{
	    private static readonly int[] SIDefault = new int[7] { 1, -1, 0, 0, 0, 0, 0 };
        [DebuggerHidden]
		private KilogramPerMeter(double val) : base(val, SIDefault) {}
	}

	///// <summary>
	///// SI Class for Gram
	///// </summary>
	//public class Gram : SIBase<Gram>
	//{
	//	private static readonly Unit[] NumeratorDefault = { Unit.g };

	//	[DebuggerHidden]
	//	private Gram(double val) : base(val, NumeratorDefault) {}
	//}

	//public class GramPerSecond : SIBase<GramPerSecond>
	//{
	//	private static readonly Unit[] NumeratorDefault = { Unit.g };
	//	private static readonly Unit[] DenominatorDefault = { Unit.s };

	//	private GramPerSecond(double val) : base(val, NumeratorDefault, DenominatorDefault) { }

	//	public static Gram operator *(GramPerSecond gps, Second s)
	//	{
	//		return SIBase<Gram>.Create(gps.Val * s.Value());
	//	}
	//}

	//public class GramPerLiter : SIBase<GramPerLiter>
	//{
	//	private static readonly Unit[] NumeratorDefault = { Unit.g };
	//	private static readonly Unit[] DenominatorDefault = { Unit.liter };

	//	private GramPerLiter(double val) : base(val, NumeratorDefault, DenominatorDefault) {}
	//}

	public class LiterPerSecond : SIBase<LiterPerSecond>
	{
	    private static readonly int[] SIDefault = new int[7] { 0, 3, -1, 0, 0, 0, 0 };

        private LiterPerSecond(double val) : base(val*0.001, SIDefault) {}
	}

	/// <summary>
	/// SI Class for Kilogram [kg].
	/// </summary>
	public class Kilogram : SIBase<Kilogram>
	{
	    private static readonly int[] SIDefault = new int[7] { 1, 0, 0, 0, 0, 0, 0 };

        [DebuggerHidden]
		private Kilogram(double val) : base(val, SIDefault) {}

		[DebuggerHidden]
		public static KilogramPerSecond operator /(Kilogram kg, Second second)
		{
			return SIBase<KilogramPerSecond>.Create(kg.Val / second.Value());
		}

		[DebuggerHidden]
		public static SI operator /(Kilogram kg, Joule j)
		{
			return (kg as SI) / j;
		}

		[DebuggerHidden]
		public static Scalar operator /(Kilogram kg, Kilogram kg2)
		{
			return SIBase<Scalar>.Create(kg.Val / kg2.Val);
		}

		[DebuggerHidden]
		public static KilogramPerMeter operator /(Kilogram kg, Meter m)
		{
			return SIBase<KilogramPerMeter>.Create(kg.Val / m.Value());
		}

		[DebuggerHidden]
		public static Newton operator *(Kilogram kg, MeterPerSquareSecond m)
		{
			return SIBase<Newton>.Create(kg.Val * m.Value());
		}

		[DebuggerHidden]
		public static Kilogram operator *(Kilogram kg, double d)
		{
			return new Kilogram(kg.Val * d);
		}

		[DebuggerHidden]
		public static Kilogram operator *(double d, Kilogram kg)
		{
			return new Kilogram(d * kg.Val);
		}

		public static Liter operator /(Kilogram kilogram, KilogramPerCubicMeter kilogramPerCubicMeter)
		{
			return SIBase<Liter>.Create(kilogram.Value() / kilogramPerCubicMeter.Value() * 1000);
		}
	}

	public class Liter : SIBase<Liter>
	{
	    private static readonly int[] SIDefault = new int[7] { 0, 3, 0, 0, 0, 0, 0 };

        [DebuggerHidden]
		private Liter(double val) : base(val * 0.001, SIDefault) {}

		public static Kilogram operator *(Liter liter, KilogramPerCubicMeter kilogramPerCubicMeter)
		{
			return SIBase<Kilogram>.Create(liter.Val / 1000 * kilogramPerCubicMeter.Value());
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class NormLiter : SIBase<NormLiter>
	{
	    private static readonly int[] SIDefault = new int[7] { 0, 3, 0, 0, 0, 0, 0 };

        [DebuggerHidden]
		private NormLiter(double val) : base(val * 0.001, SIDefault) {}

		public static NormLiterPerSecond operator /(NormLiter nl, Second s)
		{
			return SIBase<NormLiterPerSecond>.Create(nl.Val / s.Value());
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class NormLiterPerSecond : SIBase<NormLiterPerSecond>
	{
	    private static readonly int[] SIDefault = new int[7] { 0, 3, -1, 0, 0, 0, 0 };

        [DebuggerHidden]
		private NormLiterPerSecond(double val) : base(val * 0.001, SIDefault) {}

		public static NormLiter operator *(NormLiterPerSecond nips, Second s)
		{
			return SIBase<NormLiter>.Create(nips.Val * s.Value());
		}

		public static NormLiterPerSecond operator *(NormLiterPerSecond nps, double val)
		{
			return Create(nps.Val * val);
		}
	}

	/// <summary>
	/// SI Class for Kilogram per Second [kg].
	/// </summary>
	public class KilogramPerSecond : SIBase<KilogramPerSecond>
	{
	    private static readonly int[] SIDefault = new int[7] { 1, 0, -1, 0, 0, 0, 0 };

        [DebuggerHidden]
		private KilogramPerSecond(double value) : base(value, SIDefault) {}

		[DebuggerHidden]
		public static Kilogram operator *(KilogramPerSecond kilogramPerSecond, Second second)
		{
			return SIBase<Kilogram>.Create(kilogramPerSecond.Val * second.Value());
		}
	}

	/// <summary>
	/// SI Class for Square meter [m^2].
	/// </summary>
	public class SquareMeter : SIBase<SquareMeter>
	{
	    private static readonly int[] SIDefault = new int[7] { 0, 2, 0, 0, 0, 0, 0 };

		[DebuggerHidden]
		private SquareMeter(double value) : base(value, SIDefault) { }
	}

	/// <summary>
	/// SI Class for cubic meter [m^3].
	/// </summary>
	public class CubicMeter : SIBase<CubicMeter>
	{
	    private static readonly int[] SIDefault = new int[7] { 0, 3, 0, 0, 0, 0, 0 };

        [DebuggerHidden]
		private CubicMeter(double value)
			: base(value, SIDefault) {}
	}

	/// <summary>
	/// SI Class for Kilogram Square Meter [kgm^2].
	/// </summary>
	public class KilogramSquareMeter : SIBase<KilogramSquareMeter>
	{
	    private static readonly int[] SIDefault = new int[7] { 1, 2, 0, 0, 0, 0, 0 };

        [DebuggerHidden]
		private KilogramSquareMeter(double value) : base(value, SIDefault) {}

		[DebuggerHidden]
		public static NewtonMeter operator *(KilogramSquareMeter kilogramSquareMeter, PerSquareSecond perSquareSecond)
		{
			return SIBase<NewtonMeter>.Create(kilogramSquareMeter.Val * perSquareSecond.Value());
		}
	}

	/// <summary>
	/// SI Class for Kilogram Square Meter [kgm^2].
	/// </summary>
	public class KilogramPerCubicMeter : SIBase<KilogramPerCubicMeter>
	{
	    private static readonly int[] SIDefault = new int[7] { 1, -3, 0, 0, 0, 0, 0 };

        [DebuggerHidden]
		private KilogramPerCubicMeter(double value) : base(value, SIDefault) {}

		[DebuggerHidden]
		public static Kilogram operator *(KilogramPerCubicMeter kilogramPerCubicMeter, CubicMeter cubicMeter)
		{
			return SIBase<Kilogram>.Create(kilogramPerCubicMeter.Val * cubicMeter.Value());
		}

		public static Kilogram operator *(KilogramPerCubicMeter kilogramPerCubicMeter, Liter liter)
		{
			return SIBase<Kilogram>.Create(kilogramPerCubicMeter.Val * liter.Value() / 1000);
		}

		//public static CubicMeter operator /(Kilogram kg, KilogramPerCubicMeter kgm3)
		//{
		//	return SIBase<CubicMeter>.Create(kg.Value() / kgm3.Val);
		//}
	}

	/// <summary>
	/// SI Class for Kilogramm per watt second [kg/Ws].
	/// W = kgm^2/s^3
	/// </summary>
	public class KilogramPerWattSecond : SIBase<KilogramPerWattSecond>
	{
	    private static readonly int[] SIDefault = new int[7] { 0, -2, 2, 0, 0, 0, 0 };

        [DebuggerHidden]
		private KilogramPerWattSecond(double val) : base(val, SIDefault) {}
	}

    /// <summary>
    /// SI Class for watt second [Ws].
    /// W = kgm^2/s^3
    /// </summary>
    public class WattSecond : SIBase<WattSecond>
	{
	    private static readonly int[] SIDefault = new int[7] { 1, 2, -2, 0, 0, 0, 0 };

        [DebuggerHidden]
		private WattSecond(double val) : base(val, SIDefault) {}

		[DebuggerHidden]
		public static Watt operator /(WattSecond wattSecond, Second second)
		{
			return SIBase<Watt>.Create(wattSecond.Val / second.Value());
		}
	}

	/// <summary>
	/// SI Class for Watt [W].
	/// </summary>
	public class Watt : SIBase<Watt>
	{
	    private static readonly int[] SIDefault = new int[7] { 1, 2, -3, 0, 0, 0, 0 };

        [DebuggerHidden]
		private Watt(double val) : base(val, SIDefault) {}

		/// <summary>
		/// Implements the operator /.
		/// </summary>
		/// <param name="watt">The watt.</param>
		/// <param name="newtonMeter">The newton meter.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static PerSecond operator /(Watt watt, NewtonMeter newtonMeter)
		{
			return SIBase<PerSecond>.Create(watt.Val / newtonMeter.Value());
		}

		[DebuggerHidden]
		public static Newton operator /(Watt watt, MeterPerSecond meterPerSecond)
		{
			return SIBase<Newton>.Create(watt.Val / meterPerSecond.Value());
		}

		/// <summary>
		/// Implements the operator /.
		/// </summary>
		/// <param name="watt">The watt.</param>
		/// <param name="perSecond">The per second.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static NewtonMeter operator /(Watt watt, PerSecond perSecond)
		{
			return SIBase<NewtonMeter>.Create(watt.Val / perSecond.Value());
		}

		[DebuggerHidden]
		public static WattSecond operator *(Watt watt, Second second)
		{
			return SIBase<WattSecond>.Create(watt.Val * second.Value());
		}

		[DebuggerHidden]
		public static Watt operator *(Watt watt, double val)
		{
			return Create(watt.Val * val);
		}
	}

    /// <summary>
    /// SI Class for Watt [J].
    /// J = Ws
    /// W = kgm^2/s^3
    /// </summary>
	public class Joule : SIBase<Joule>
	{
	    private static readonly int[] SIDefault = new int[7] { 1, 2, -2, 0, 0, 0, 0 };

        [DebuggerHidden]
		private Joule(double val) : base(val, SIDefault) {}

		public static implicit operator Joule(WattSecond self)
		{
			return Create(self.Value());
		}

		public static Joule operator +(Joule joule, WattSecond ws)
		{
			return Create(joule.Val + ws.Value());
		}

		public static Watt operator /(Joule joule, Second s)
		{
			return SIBase<Watt>.Create(joule.Val / s.Value());
		}

		public static JoulePerMeter operator /(Joule joule, Meter meter)
		{
			return SIBase<JoulePerMeter>.Create(joule.Val / meter.Value());
		}
	}

    /// <summary>
    /// SI Class for Watt [J/kg].
    /// J = Ws
    /// W = kgm^2/s^3
    /// </summary>
    public class JoulePerKilogramm : SIBase<JoulePerKilogramm>
	{
	    private static readonly int[] SIDefault = new int[7] { 0, 2, -2, 0, 0, 0, 0 };

        private JoulePerKilogramm(double val) : base(val, SIDefault) {}

		public static Joule operator *(Kilogram kg, JoulePerKilogramm jpg)
		{
			return SIBase<Joule>.Create(kg.Value() * jpg.Val);
		}
	}

    /// <summary>
    ///  SI Class for KilogramPerMeter [J/m].
    ///  J = Ws
    ///  W = kgm^2/s^3
    /// </summary>
    public class JoulePerMeter : SIBase<JoulePerMeter>
	{
	    private static readonly int[] SIDefault = new int[7] { 1, 1, -2, 0, 0, 0, 0 };

        [DebuggerHidden]
		private JoulePerMeter(double val) : base(val, SIDefault) {}
	}

	/// <summary>
	/// SI Class for one per second [1/s].
	/// </summary>
	[DebuggerDisplay("rad/s: {Val} | rpm: {AsRPM}")]
	public class PerSecond : SIBase<PerSecond>
	{
	    private static readonly int[] SIDefault = new int[7] { 0, 0, -1, 0, 0, 0, 0 };

		[DebuggerHidden]
		private PerSecond(double val) : base(val, SIDefault) { }

		[DebuggerHidden]
		public static PerSquareSecond operator /(PerSecond perSecond, Second second)
		{
			return SIBase<PerSquareSecond>.Create(perSecond.Val / second.Value());
		}

		public double AsRPM
		{
			get { return Val * 60 / (2 * Math.PI); }
		}
	}

	/// <summary>
	/// SI Class for Meter per second [m/s].
	/// </summary>
	[DebuggerDisplay("{Val} | {AsKmph}")]
	public class MeterPerSecond : SIBase<MeterPerSecond>
	{
	    private static readonly int[] SIDefault = new int[7] { 0, 1, -1, 0, 0, 0, 0 };

        [DebuggerHidden]
		private MeterPerSecond(double val) : base(val, SIDefault) {}

		public double AsKmph
		{
			get { return Val * 3.6; }
		}

		/// <summary>
		/// Implements the operator /.
		/// </summary>
		[DebuggerHidden]
		public static PerSecond operator /(MeterPerSecond meterPerSecond, Meter meter)
		{
			return SIBase<PerSecond>.Create(meterPerSecond.Val / meter.Value());
		}

		/// <summary>
		/// Implements the operator /.
		/// </summary>
		[DebuggerHidden]
		public static Second operator /(MeterPerSecond meterPerSecond, MeterPerSquareSecond meterPerSquareSecond)
		{
			return SIBase<Second>.Create(meterPerSecond.Val / meterPerSquareSecond.Value());
		}

		/// <summary>
		/// Implements the operator /.
		/// </summary>
		[DebuggerHidden]
		public static MeterPerSquareSecond operator /(MeterPerSecond meterPerSecond, Second second)
		{
			return SIBase<MeterPerSquareSecond>.Create(meterPerSecond.Val / second.Value());
		}

		/// <summary>
		/// Implements the operator *.
		/// </summary>
		[DebuggerHidden]
		public static Meter operator *(MeterPerSecond meterPerSecond, Second second)
		{
			return SIBase<Meter>.Create(meterPerSecond.Val * second.Value());
		}

		/// <summary>
		/// Implements the operator *.
		/// </summary>
		[DebuggerHidden]
		public static MeterPerSquareSecond operator *(MeterPerSecond meterPerSecond, PerSecond perSecond)
		{
			return SIBase<MeterPerSquareSecond>.Create(meterPerSecond.Val * perSecond.Value());
		}

		/// <summary>
		/// Implements the operator *.
		/// </summary>
		[DebuggerHidden]
		public static Meter operator *(Second second, MeterPerSecond meterPerSecond)
		{
			return SIBase<Meter>.Create(second.Value() * meterPerSecond.Val);
		}
	}

	/// <summary>
	/// SI Class for NewtonMeter [Nm].
	/// N = kgm/s^2
	/// </summary>
	public class NewtonMeter : SIBase<NewtonMeter>
	{
	    private static readonly int[] SIDefault = new int[7] { 1, 2, -2, 0, 0, 0, 0 };

        [DebuggerHidden]
		private NewtonMeter(double val) : base(val, SIDefault) {}

		[DebuggerHidden]
		public static Watt operator *(NewtonMeter newtonMeter, PerSecond perSecond)
		{
			return SIBase<Watt>.Create(newtonMeter.Val * perSecond.Value());
		}

		[DebuggerHidden]
		public static Watt operator *(PerSecond perSecond, NewtonMeter newtonMeter)
		{
			return SIBase<Watt>.Create(perSecond.Value() * newtonMeter.Val);
		}

		[DebuggerHidden]
		public static Second operator /(NewtonMeter newtonMeter, Watt watt)
		{
			return SIBase<Second>.Create(newtonMeter.Val / watt.Value());
		}

		[DebuggerHidden]
		public static PerSquareSecond operator /(NewtonMeter newtonMeter, KilogramSquareMeter kgKilogramSquareMeter)
		{
			return SIBase<PerSquareSecond>.Create(newtonMeter.Val / kgKilogramSquareMeter.Value());
		}

		[DebuggerHidden]
		public static PerSecond operator /(NewtonMeter newtonMeter, NewtonMeterSecond newtonMeterSecond)
		{
			return SIBase<PerSecond>.Create(newtonMeter.Val / newtonMeterSecond.Value());
		}

		[DebuggerHidden]
		public static Newton operator /(NewtonMeter newtonMeter, Meter meter)
		{
			return SIBase<Newton>.Create(newtonMeter.Val / meter.Value());
		}

		[DebuggerHidden]
		public static NewtonMeterSecond operator /(NewtonMeter newtonMeter, PerSecond perSecond)
		{
			return SIBase<NewtonMeterSecond>.Create(newtonMeter.Val / perSecond.Value());
		}
	}

    /// <summary>
    /// SI Class for NewtonMeterSecond [Nms].
    /// N = kgm/s^2
    /// </summary>
    public class NewtonMeterSecond : SIBase<NewtonMeterSecond>
	{
	    private static readonly int[] SIDefault = new int[7] { 1, 2, -1, 0, 0, 0, 0 };
        private NewtonMeterSecond(double val) : base(val, SIDefault) {}
	}

    /// <summary>
    /// SI Class for Amperer [A].
    /// </summary>
    public class Ampere : SIBase<Ampere>
	{
	    private static readonly int[] SIDefault = new int[7] { 0, 0, 0, 1, 0, 0, 0 };
        private Ampere(double val) : base(val, SIDefault) {}

		public static Watt operator *(Ampere ampere, Volt volt)
		{
			return SIBase<Watt>.Create(volt.Value() * ampere.Val);
		}

		public static Ampere operator *(Ampere ampere, double val)
		{
			return Create(ampere.Val * val);
		}

		public static Volt operator /(Watt watt, Ampere ampere)
		{
			return SIBase<Volt>.Create(watt.Value() / ampere.Value());
		}

		public static Watt operator /(Volt volt, Ampere ampere)
		{
			return SIBase<Watt>.Create(volt.Value() * ampere.Value());
		}
	}

    /// <summary>
    /// SI Class for Amperer [V].
    /// V = kgm^2/As^2
    /// </summary>
    public class Volt : SIBase<Volt>
	{
	    private static readonly int[] SIDefault = new int[7] { 1, 2, -2, -1, 0, 0, 0 };
        private Volt(double val) : base(val, SIDefault) {}

		public static Watt operator *(Volt volt, Ampere ampere)
		{
			return SIBase<Watt>.Create(volt.Val * ampere.Value());
		}

		public static Ampere operator /(Watt watt, Volt volt)
		{
			return SIBase<Ampere>.Create(watt.Value() / volt.Value());
		}
	}

	/// <summary>
	/// Base Class for all special SI Classes. Not intended to be used directly.
	/// Implements templated operators for type safety and convenience.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class SIBase<T> : SI where T : SIBase<T>
	{
		private static readonly T ZeroPrototype;

		static SIBase()
		{
			const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
			var constructorInfo = typeof(T).GetConstructor(bindingFlags, null, new[] { typeof(double) }, null);
			var parameter = Expression.Parameter(typeof(double));
			var lambda = Expression.Lambda<Func<double, T>>(Expression.New(constructorInfo, parameter), parameter);
			Constructor = lambda.Compile();
			ZeroPrototype = Constructor(0);
		}

		/// <summary>
		/// The constructor for the generic type T.
		/// </summary>
		private static readonly Func<double, T> Constructor;

		/// <summary>
		/// Creates the specified special SI object.
		/// </summary>
		/// <param name="val">The value of the SI object.</param>
		[DebuggerStepThrough]
		public static T Create(double val)
		{
			if (val == 0) {
				return ZeroPrototype;
			}

			return Constructor(val);
		}

		[DebuggerStepThrough]
		protected SIBase(double value) : base(value) {}


	    [DebuggerStepThrough]
        protected SIBase(double value, int[] siunits) : base(value)
	    {
	        SIUnits = siunits;
	    }

        [DebuggerStepThrough]
		public new T Abs()
		{
			return Create(Math.Abs(Val));
		}

		#region Operators

		/// <summary>
		/// Implements the operator + for two specialized SI Classes.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <param name="si2">The si2.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static T operator +(SIBase<T> si1, SIBase<T> si2)
		{
			return Create(si1.Val + si2.Val);
		}

		/// <summary>
		/// Implements the operator + for a specialized SI Class and a generic SI Class.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <param name="si2">The si2.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static T operator +(SIBase<T> si1, SI si2)
		{
			return ((si1 as SI) + si2).Cast<T>();
		}

		/// <summary>
		/// Implements the operator + for a generic SI Class and a specialized SI Class.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <param name="si2">The si2.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static T operator +(SI si1, SIBase<T> si2)
		{
			return (si1 + (si2 as SI)).Cast<T>();
		}

		/// <summary>
		/// Implements the unary operator -.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static T operator -(SIBase<T> si1)
		{
			return Create(-si1.Val);
		}

		/// <summary>
		/// Implements the operator - for two specialized SI classes.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <param name="si2">The si2.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static T operator -(SIBase<T> si1, SIBase<T> si2)
		{
			return Create(si1.Val - si2.Val);
		}

		/// <summary>
		/// Implements the operator - for a specialized SI class and a generic SI class.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <param name="si2">The si2.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static T operator -(SIBase<T> si1, SI si2)
		{
			return ((si1 as SI) - si2).Cast<T>();
		}

		/// <summary>
		/// Implements the operator - for a generic SI class and a specialized SI class.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <param name="si2">The si2.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static T operator -(SI si1, SIBase<T> si2)
		{
			return (si1 - (si2 as SI)).Cast<T>();
		}

		/// <summary>
		/// Implements the operator * for a double and a specialized SI class.
		/// </summary>
		/// <param name="d">The double value.</param>
		/// <param name="si">The si.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static T operator *(double d, SIBase<T> si)
		{
			return Create(d * si.Val);
		}

		/// <summary>
		/// Implements the operator * for a specialized SI class and a double.
		/// </summary>
		/// <param name="si">The si.</param>
		/// <param name="d">The double.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static T operator *(SIBase<T> si, double d)
		{
			return Create(si.Val * d);
		}

		/// <summary>
		/// Implements the operator / for a specialized SI class and a double.
		/// </summary>
		/// <param name="si">The si.</param>
		/// <param name="d">The double.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static T operator /(SIBase<T> si, double d)
		{
			return Create(si.Val / d);
		}

		[DebuggerHidden]
		public static Scalar operator /(SIBase<T> si, SIBase<T> si2)
		{
			return SIBase<Scalar>.Create(si.Val / si2.Val);
		}

		#endregion
	}

	/// <summary>
	/// Class for representing generic SI Units.
	/// </summary>
	/// <remarks>
	/// Usage: new SI(1.0).Newton.Meter, new SI(2.3).Rounds.Per.Minute
	/// </remarks>
	[DebuggerDisplay("{Val}")]
	public class SI : IComparable
	{
		/// <summary>
		/// The basic scalar value of the SI.
		/// </summary>
		protected readonly double Val;


        /// <summary>
        /// The array of the SI units.
        /// </summary>
        protected int[] SIUnits;

        protected UnitInstance.IsMass isMass;


        /// <summary>
        /// The current exponent for conversion operations (Square, Cubic, Linear, e.g. new SI(3).Square.Meter).
        /// Can be reseted with Reset, Per, Cast.
        /// </summary>
        //private readonly int _exponent;

        /// <summary>
        /// A flag indicating if the current SI is in reciprocal mode (used in the <see cref="Per"/> method for reciprocal units: e.g. new SI(2).Meter.Per.Second) ==> [m/s]
        /// Can be reseted with Reset, Per, Cast.
        /// </summary>
        //private readonly bool _reciproc;

        /// <summary>
        /// A flag indicating if the current SI is in reverse mode (used for conversions: e.g. new SI(2).Rounds.Per.Minute.ConverTo.Radian.Per.Second ==> [rpm/min] => [rad/s]).
        /// </summary>
        //private readonly bool _reverse;

        ///// <summary>
        ///// Enum for defining the Units.
        ///// </summary>
        //[SuppressMessage("ReSharper", "InconsistentNaming")]
        //protected enum Unit
        //{
        //    kg,
        //    m,
        //    s,
        //    A,
        //    K,
        //    mol,
        //    cd,
        //}


        /// <summary>
        /// Initializes a new instance of the <see cref="SI"/> class without any units (dimensionless, scalar) [-].
        /// </summary>
        /// <param name="val">The value.</param>
        //[DebuggerHidden]
        public SI(double val = 0.0)
        {
            Val = val;

            SIUnits = new int[7] { 0, 0, 0, 0, 0, 0, 0 };


            if (double.IsNaN(val))
            {
                throw new VectoException("NaN [{0}] is not allowed for SI-Values in Vecto.", GetUnitString());
            }

            if (double.IsInfinity(Val))
            {
                throw new VectoException("Infinity [{0}] is not allowed for SI-Values in Vecto.", GetUnitString());
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SI"/> class which allows to construct a new SI with all parameters.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <param name="numerator">The numerator.</param>
        /// <param name="denominator">The denominator.</param>
        /// <param name="reciproc">if set to <c>true</c> then the object is in reciproc mode (1/...)</param>
        /// <param name="reverse">if set to <c>true</c> then the object is in reverse convertion mode (e.g. rpm/min => rad/s).</param>
        /// <param name="exponent">The exponent for further conversions (e.g. Square.Meter).</param>
        //protected SI(double val, int[] siunits, bool reciproc = false,
        //    bool reverse = false, int exponent = 1)
        protected SI(double val, int[] siunits, UnitInstance.IsMass isMassParam)
        {
            Val = val;
            isMass = isMassParam;

            SIUnits = siunits;


            if (double.IsNaN(Val))
            {
                throw new VectoException("NaN [{0}] is not allowed for SI-Values in Vecto.", GetUnitString());
            }

            if (double.IsInfinity(Val))
            {
                throw new VectoException("Infinity [{0}] is not allowed for SI-Values in Vecto.", GetUnitString());
            }
        }

	    public SI(UnitInstance si, double val = 0) : this(val * si.Getfactor, si.GetSIUnits(),si.GetGrammMode()){
       
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SI"/> class which copies the units from an already existing SI.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <param name="unit">The unit.</param>
        [DebuggerHidden]
        private SI(double val, SI unit) : this(val, unit.SIUnits, unit.isMass) {}

        //[DebuggerHidden]
        //protected SI(SI si, double? factor = null, int[] siUnitsParam = null,
        //    bool? reciproc = null, bool? reverse = null, int? exponent = null)
	    protected SI(SI si, double factor , int[] siUnitsParam, UnitInstance.IsMass isMassParam)
        {

            Val = si.Val / factor;
            isMass = isMassParam;
            SIUnits = siUnitsParam;


            if (double.IsNaN(Val))
            {
                throw new VectoException("NaN [{0}] is not allowed for SI-Values in Vecto.", GetUnitString());
            }

            if (double.IsInfinity(Val))
            {
                throw new VectoException("Infinity [{0}] is not allowed for SI-Values in Vecto.", GetUnitString());
            }
        }




		///////// <summary>
		///////// Converts the SI unit to another SI unit, defined by term(s) following after the ConvertTo().
		///////// The Conversion Mode is active until an arithmetic operator is used (+,-,*,/), 
		///////// or the .Value-Method, or the .Cast-Method were called.
		///////// ATTENTION: Before returning an SI Unit, ensure to cancel Conversion Mode (with or Cast).
		///////// </summary>
		///////// <returns></returns>
		//////[DebuggerHidden]
		//////public SI ConvertTo()
		//////{
		//////    return new SI(Linear, reciproc: false, reverse: true);
		//////}

	    public SI ConvertTo(UnitInstance si)
	    {

	        if (!SIUtils.CompareSIUnits(SIUnits, si.GetSIUnits()))
	        {
	            throw new VectoException(
	                "Unit missing. Conversion not possible. [{0}] does not contain a [{1}].",
	                GetUnitString(SIUnits), si.GetSIUnits());
	        }

            double factorValue = si.Getfactor;


	        if ((isMass & UnitInstance.IsMass.IsGramm) == UnitInstance.IsMass.IsGramm)
	        {
	            factorValue *= 1000;
	        }

	        if((si.GetGrammMode() & UnitInstance.IsMass.IsGramm) == UnitInstance.IsMass.IsGramm)
            {
                factorValue /= 1000;
                isMass |= UnitInstance.IsMass.IsGramm;
            }


            return new SI(this, siUnitsParam: si.GetSIUnits(), factor: factorValue,
                isMassParam: isMass);
        }

        /// <summary>
        /// Casts the SI Unit to the concrete unit type (if the units allow such an cast).
        /// </summary>
        /// <typeparam name="T">the specialized SI unit. e.g. Watt, NewtonMeter, Second</typeparam>
        //[DebuggerHidden]
        public T Cast<T>() where T : SIBase<T>
        {
            //MyTrace.Scan(new StackTrace(true));
            var si = ToBasicUnits();
            var t = SIBase<T>.Create(si.Val);
            if (!si.HasEqualUnit(t))
            {
                throw new VectoException("SI Unit Conversion failed: From {0} to {1}", si, t);
            }
            return t;
        }

        /// <summary>
        /// Converts the derived SI units to the basic units and returns this as a new SI object.
        /// </summary>
        public SI ToBasicUnits()
		{
		    return new SI(Val, SIUnits,isMass);
		}


		/// <summary>
		/// Gets the underlying scalar double value.
		/// </summary>
		[DebuggerHidden]
		public double Value()
		{
			return Val;
		}

		/// <summary>
		/// Clones this instance.
		/// </summary>
		public SI Clone()
		{
		    return new SI(Val, SIUnits,isMass);
        }

		/// <summary>
		/// Returns the absolute value.
		/// </summary>
		public virtual SI Abs()
		{
			return new SI(Math.Abs(Val), this);
		}

		/// <summary>
		/// Returns the numerical sign of the SI.
		/// </summary>
		/// <returns>-1 if si &lt; 0. 0 if si==0, 1 if si &gt; 0.</returns>
		[DebuggerHidden]
		public int Sign()
		{
			return Math.Sign(Val);
		}

		#region Unit Definitions

		///// <summary>
		///// Defines the denominator by the terms following after the Per.
		///// </summary>
		//[DebuggerHidden]
		//public SI Per
		//{
		//	[DebuggerHidden] get { return new SI(Linear, reciproc: !_reciproc); }
		//}

		///// <summary>
		///// Takes all following terms as cubic terms (=to the power of 3).
		///// </summary>
		////[DebuggerHidden]
		//public SI Cubic
		//{
		//	//[DebuggerHidden]
  //          get { return new SI(this, exponent: 3); }
		//}

		///// <summary>
		///// Takes all following terms as quadratic terms (=to the power of 2).
		///// </summary>
		//[DebuggerHidden]
		//public SI Square
		//{
		//	[DebuggerHidden] get { return new SI(this, exponent: 2); }
		//}

		///// <summary>
		///// Takes all following terms as linear terms (=to the power of 1).
		///// </summary>
		//[DebuggerHidden]
		//public SI Linear
		//{
		//	[DebuggerHidden] get { return new SI(this, exponent: 1); }
		//}

		///// <summary>
		///// [g] (to basic unit: [kg])
		///// </summary>
		//[DebuggerHidden]
		//public SI Gramm
  //      {
  //          [DebuggerHidden]
		//    get
  //          {
  //              return new SI(this, 0.001, new int[] { 1, 0, 0, 0, 0, 0, 0 });
  //          }
            
		//}

		//[DebuggerHidden]
		//public SI Liter
		//{
  //          [DebuggerHidden]
		//    get
		//    {
		//        return new SI(this, 0.001, new int[] { 0, 3, 0, 0, 0, 0, 0 });
		//    }
  //      }

		//[DebuggerHidden]
		//public SI Joule
		//{
  //          [DebuggerHidden]
		//    get
		//    {
		//        return new SI(this, 0.001, new int[] { 1, 2, -2, 0, 0, 0, 0 });
		//    }
  //      }

		///// <summary>
		///// [t] (to basic unit: [kg])
		///// </summary>
		//[DebuggerHidden]
		//public SI Ton
		//{
  //          [DebuggerHidden]
		//    get
		//    {
		//        return new SI(this, 1000, new int[] { 1, 0, 0, 0, 0, 0, 0 });
		//    }
  //      }

		///// <summary>
		///// [N]
		///// </summary>
		//[DebuggerHidden]
		//public SI Newton
  //      {
  //          [DebuggerHidden]
		//    get
  //          {
  //              return new SI(this, siUnitsParam: new int[] { 1, 1, -2, 0, 0, 0, 0 });
  //          }
		//}

		///// <summary>
		///// [W]
		///// </summary>
		//[DebuggerHidden]
  //      public SI Watt
  //      {
  //          [DebuggerHidden]
  //          get
  //          {
  //              return new SI(this, siUnitsParam: new int[] { 1, 2, -3, 0, 0, 0, 0 });
  //          }
  //      }
		///// <summary>
		///// [m]
		///// </summary>
		////[DebuggerHidden]
  //      public SI Meter
  //      {
  //          //[DebuggerHidden]
  //          get
  //          {
  //              return new SI(this, siUnitsParam: new int[] { 0, 1, 0, 0, 0, 0, 0 });
  //          }
  //      }
		///// <summary>
		///// [s]
		///// </summary>
		//[DebuggerHidden]
  //      public SI Second
  //      {
  //          [DebuggerHidden]
  //          get
  //          {
  //              return new SI(this, siUnitsParam: new int[] { 0, 0, 1, 0, 0, 0, 0 });
  //          }
  //      }
		///// <summary>
		///// [-]. Defines radian. Only virtual. Has no real SI unit.
		///// </summary>
		//[DebuggerHidden]
  //      public SI Radian
  //      {
  //          [DebuggerHidden]
  //          get
  //          {
  //              return new SI(this);
  //          }
  //      }
		///// <summary>
		///// [-]. Converts to/from Radiant. Internally everything is stored in radian.
		///// </summary>
		//[DebuggerHidden]
		//public SI Rounds
		//{
  //          [DebuggerHidden]
		//    get
  //          {
  //              return new SI(this, 2 * Math.PI);
  //          }
		//}

		///// <summary>
		///// [s] Converts to/from Second. Internally everything is stored in seconds.
		///// </summary>
		//[DebuggerHidden]
  //      public SI Hour
  //      {
  //          [DebuggerHidden]
  //          get
  //          {
  //              return new SI(this, 3600, new[] { 0, 0, 1, 0, 0, 0, 0 });
  //          }
  //      }
		///// <summary>
		///// [s] Converts to/from Second. Internally everything is stored in seconds.
		///// </summary>
		//[DebuggerHidden]
  //      public SI Minute
  //      {
  //          [DebuggerHidden]
  //          get
  //          {
  //              return new SI(this, 60.0, new[] { 0, 0, 1, 0, 0, 0, 0 });
  //          }
  //      }
		///// <summary>
		///// Quantifier for milli (1/1000).
		///// </summary>
		//[DebuggerHidden]
  //      public SI Milli
  //      {
  //          [DebuggerHidden]
  //          get
  //          {
  //              return new SI(this, 0.001);
  //          }
  //      }
		///// <summary>
		///// Quantifier for Kilo (1000).
		///// </summary>
		//[DebuggerHidden]
  //      public SI Kilo
  //      {
  //          [DebuggerHidden]
  //          get
  //          {
  //              return new SI(this, 1000.0);
  //          }
  //      }

		//public SI Ampere
  //      {
  //          [DebuggerHidden]
		//    get
		//    {
  //              return new SI(this, siUnitsParam: new int[] { 0, 0, 0, 1, 0, 0, 0 });
  //          }
		//}

		///// <summary>
		///// Quantifier for Dezi (1/10)
		///// </summary>
		//[DebuggerHidden]
  //      public SI Dezi
  //      {
		//    [DebuggerHidden]
		//    get
		//    {
		//        return new SI(this, 0.1);
  //          }
		//}

		///// <summary>
		///// Quantifier for Centi (1/100)
		///// </summary>
		////[DebuggerHidden]
  //      public SI Centi
  //      {
		//    //[DebuggerHidden]
		//    get
		//    {
		//        return new SI(this, 0.01);
  //          }
		//}

		#endregion



		#region Operators

		/// <summary>
		/// Implements the operator +.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <param name="si2">The si2.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		/// <exception cref="VectoException"></exception>
		[DebuggerHidden]
	    public static SI operator +(SI si1, SI si2)
	    {
	        if (!si1.HasEqualUnit(si2))
	        {
	            throw new VectoException("Operator '+' can only operate on SI Objects with the same unit. Got: {0} + {1}", si1, si2);
	        }

	        return new SI(si1.Val + si2.Val)
	        {
	            SIUnits = si1.SIUnits
	        };
	    }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="si1">The si1.</param>
        /// <param name="si2">The si2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="VectoException"></exception>
        [DebuggerHidden]
	    public static SI operator -(SI si1, SI si2)
	    {
	        if (!si1.HasEqualUnit(si2))
	        {
	            throw new VectoException("Operator '-' can only operate on SI Objects with the same unit. Got: {0} - {1}", si1, si2);
	        }
	        return new SI(si1.Val - si2.Val)
	        {
	            SIUnits = si1.SIUnits
	        };
	    }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="si1">The si1.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [DebuggerHidden]
	    public static SI operator -(SI si1)
	    {
	        return new SI(-si1.Val)
	        {
	            SIUnits = si1.SIUnits
	        };
	    }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="si1">The si1.</param>
        /// <param name="si2">The si2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static SI operator *(SI si1, SI si2)
        {
            int[] unitArray = new int[7];
            unitArray = SIUtils.AdditionTheSIUnits(si1.SIUnits, si2.SIUnits);

            return new SI(si1.Val * si2.Val, unitArray, si1.isMass);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="si1">The si1.</param>
        /// <param name="d">The d.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [DebuggerHidden]
	    public static SI operator *(SI si1, double d)
	    {
	        return new SI(si1.Val * d)
	        {
	            SIUnits = si1.SIUnits
	        };
	    }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="si1">The si1.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [DebuggerHidden]
	    public static SI operator *(double d, SI si1)
	    {
	        return new SI(d * si1.Val)
	        {
	            SIUnits = si1.SIUnits
	        };
	    }

        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="si1">The si1.</param>
        /// <param name="si2">The si2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
	    public static SI operator /(SI si1, SI si2)
	    {
	        double result;
	        try
	        {
	            result = si1.Val / si2.Val;

	            // bad cases: Infinity = x / 0.0  (for x != 0), NaN = 0.0 / 0.0
	            if (double.IsInfinity(result) || double.IsNaN(result))
	            {
	                throw new DivideByZeroException();
	            }
	        }
	        catch (DivideByZeroException ex)
	        {
	            throw new VectoException(
	                string.Format("Can not compute division by zero ([{0}] / 0[{1}])", si1.GetUnitString(), si2.GetUnitString()), ex);
	        }

	        int[] unitArray = new int[7];
	        unitArray = SIUtils.AdditionTheSIUnits(
	            si1.SIUnits, SIUtils.SIUnitsMultFactor(si2.SIUnits, -1));

	        return new SI(result, unitArray, si1.isMass);
	    }

        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="si1">The si1.</param>
        /// <param name="d">The d.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [DebuggerHidden]
	    public static SI operator /(SI si1, double d)
	    {
	        if (d.IsEqual(0))
	        {
	            throw new VectoException(string.Format("Can not compute division by zero ([{0}] / 0)", si1.GetUnitString()),
	                new DivideByZeroException());
	        }

	        return new SI(si1.Val / d)
	        {
	            SIUnits = si1.SIUnits
	        };
	    }

        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="si1">The si1.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [DebuggerHidden]
	    public static SI operator /(double d, SI si1)
	    {
	        if (si1.IsEqual(0))
	        {
	            throw new VectoException(string.Format("Can not compute division by zero (x / 0[{0}])", si1.GetUnitString()),
	                new DivideByZeroException());
	        }

	        return new SI(d / si1.Val)
	        {
	            SIUnits = SIUtils.SIUnitsMultFactor(si1.SIUnits, -1)
	        };
	    }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="si1">The si1.</param>
        /// <param name="si2">The si2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="VectoException"></exception>
        [DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <(SI si1, SI si2)
		{
			if (!si1.HasEqualUnit(si2)) {
				throw new VectoException("Operator '<' can only operate on SI Objects with the same unit. Got: {0} < {1}", si1, si2);
			}
			return si1.Val < si2.Val;
		}

		/// <summary>
		/// Implements the operator &lt;.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <param name="d">The d.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static bool operator <(SI si1, double d)
		{
			return si1 != null && si1.Val < d;
		}

		/// <summary>
		/// Implements the operator &gt;.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <param name="si2">The si2.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		/// <exception cref="VectoException"></exception>
		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >(SI si1, SI si2)
		{
			if (!si1.HasEqualUnit(si2)) {
				throw new VectoException("Operator '>' can only operate on SI Objects with the same unit. Got: {0} > {1}", si1, si2);
			}
			return si1.Val > si2.Val;
		}

		/// <summary>
		/// Implements the operator &gt;.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <param name="d">The d.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static bool operator >(SI si1, double d)
		{
			return si1 != null && si1.Val > d;
		}

		/// <summary>
		/// Implements the operator &gt;.
		/// </summary>
		/// <param name="d">The d.</param>
		/// <param name="si1">The si1.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static bool operator >(double d, SI si1)
		{
			return si1 != null && d > si1.Val;
		}

		/// <summary>
		/// Implements the operator &lt;.
		/// </summary>
		/// <param name="d">The d.</param>
		/// <param name="si1">The si1.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static bool operator <(double d, SI si1)
		{
			return si1 != null && d < si1.Val;
		}

		/// <summary>
		/// Implements the operator &lt;=.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <param name="si2">The si2.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		/// <exception cref="VectoException"></exception>
		[DebuggerHidden]
		public static bool operator <=(SI si1, SI si2)
		{
			if (!si1.HasEqualUnit(si2)) {
				throw new VectoException("Operator '<=' can only operate on SI Objects with the same unit. Got: {0} <= {1}", si1,
					si2);
			}
			return si1.Val <= si2.Val;
		}

		/// <summary>
		/// Implements the operator &lt;=.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <param name="d">The d.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static bool operator <=(SI si1, double d)
		{
			return si1 != null && si1.Val <= d;
		}

		/// <summary>
		/// Implements the operator &gt;=.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <param name="si2">The si2.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		/// <exception cref="VectoException"></exception>
		[DebuggerHidden]
		public static bool operator >=(SI si1, SI si2)
		{
			if (!si1.HasEqualUnit(si2)) {
				throw new VectoException("Operator '>=' can only operate on SI Objects with the same unit. Got: {0} >= {1}", si1,
					si2);
			}
			return si1.Val >= si2.Val;
		}

		/// <summary>
		/// Implements the operator &gt;=.
		/// </summary>
		/// <param name="si1">The si1.</param>
		/// <param name="d">The d.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static bool operator >=(SI si1, double d)
		{
			return si1 != null && si1.Val >= d;
		}

		/// <summary>
		/// Implements the operator &gt;=.
		/// </summary>
		/// <param name="d">The d.</param>
		/// <param name="si1">The lower.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		[DebuggerHidden]
		public static bool operator >=(double d, SI si1)
		{
			return si1 != null && d >= si1.Val;
		}

		/// <summary>
		/// Implements the operator &lt;=.
		/// </summary>
		/// <param name="d">The d.</param>
		/// <param name="si1">The lower.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator <=(double d, SI si1)
		{
			return si1 != null && d <= si1.Val;
		}

		/// <summary>
		/// Determines whether the SI is between lower and uppper bound.
		/// </summary>
		/// <param name="lower">The lower bound.</param>
		/// <param name="upper">The upper bound.</param>
		/// <returns></returns>
		public bool IsBetween(SI lower, SI upper)
		{
			return lower <= Val && Val <= upper;
		}

		/// <summary>
		/// Determines whether the SI is between lower and uppper bound.
		/// </summary>
		/// <param name="lower">The lower bound.</param>
		/// <param name="upper">The upper bound.</param>
		/// <returns></returns>
		public bool IsBetween(double lower, double upper)
		{
			return lower <= Val && Val <= upper;
		}

		#endregion

		#region ToString

		/// <summary>
		///     Returns the Unit Part of the SI Unit Expression.
		/// </summary>
	    public string GetUnitString(int[] SIUnitParam = null)
	    {
            if (SIUnitParam == null)
            {
                SIUnitParam = SIUnits;
            }
	        return Unit.GetUnitString(SIUnitParam,
                ((isMass & UnitInstance.IsMass.IsGramm) == UnitInstance.IsMass.IsGramm));
            //return Unit.GetUnitString(SIUnitParam);
            //Array unitnames = Enum.GetNames(typeof(Unit));
            //string numerator = "";
            //string denominator = "";
            //int potent = 0;
            //string potentStr = "";

            //   if (SIUnitParam == null)
            //   {
            //       SIUnitParam = SIUnits;
            //   }
            //for (var i = 0; i < SIUnitParam.Length; i++)
            //{
            //    int currentValue = SIUnitParam[i];
            //    potent = Math.Abs(currentValue);
            //    potentStr = "";
            //    if (currentValue != 0)
            //    {
            //        string currentUnit = (string)unitnames.GetValue(i);

            //        if (potent > 1)
            //        {
            //            potentStr = "^" + potent;
            //        }

            //        if (currentValue > 0)
            //        {
            //            numerator += currentUnit + potentStr;

            //        }
            //        else if (currentValue < 0)
            //        {
            //            denominator += currentUnit + potentStr;
            //        }
            //    }
            //}
            //string result = "";

            //if (numerator == "")
            //{
            //    if (denominator == "")
            //    {
            //        result = "-";
            //    }
            //    else
            //    {
            //        result = "1/" + denominator;
            //    }
            //}
            //else
            //{
            //    if (denominator == "")
            //    {
            //        result = numerator;
            //    }
            //    else
            //    {
            //        result = numerator + "/" + denominator;
            //    }
            //}

            //return result;
        }



        /// <summary>
        ///     Returns the String representation.
        /// </summary>
        public override string ToString()
		{
			return ToString(null);
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		private string ToString(string format)
		{
			if (string.IsNullOrEmpty(format)) {
				format = "F4";
			}

			return string.Format(CultureInfo.InvariantCulture, "{0:" + format + "} [{2}]", Val, format, GetUnitString());
		}

		#endregion

		#region Equality members

		/// <summary>
		/// Compares the Unit-Parts of two SI Units.
		/// </summary>
		/// <param name="si">The si.</param>
		/// <returns></returns>
		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasEqualUnit(SI si)
		{
		    return SIUtils.CompareSIUnits(SIUnits, si.SIUnits);
    	}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			var other = obj as SI;
			return other != null && Val.Equals(other.Val) && HasEqualUnit(other);
		}

		/// <summary>
		/// Determines whether the specified si is equal.
		/// </summary>
		/// <param name="si">The si.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		public bool IsEqual(SI si, SI tolerance = null)
		{
			return (tolerance == null || HasEqualUnit(tolerance)) && HasEqualUnit(si) &&
					Val.IsEqual(si.Val, tolerance == null ? DoubleExtensionMethods.Tolerance : tolerance.Value());
		}

		/// <summary>
		/// Determines whether the specified value is equal.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerHidden]
		public bool IsEqual(double val, double tolerance = DoubleExtensionMethods.Tolerance)
		{
			return Val.IsEqual(val, tolerance);
		}

		/// <summary>
		/// Determines whether the specified si is smaller.
		/// </summary>
		/// <param name="si">The si.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		public bool IsSmaller(SI si, SI tolerance = null)
		{
			if (!HasEqualUnit(si)) {
				throw new VectoException("compared value has to be the same unit. Got: {0} <=> {1}", this, si);
			}
			if (tolerance != null && !HasEqualUnit(tolerance)) {
				throw new VectoException("tolerance has to be the same unit. Got: {0} <=> {1}", this, tolerance);
			}

			return Val.IsSmaller(si.Val, tolerance == null ? DoubleExtensionMethods.Tolerance : tolerance.Value());
		}

		/// <summary>
		/// Determines whether the specified si is smaller.
		/// </summary>
		/// <param name="si">The si.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		public bool IsSmaller(SI si, double tolerance)
		{
			if (!HasEqualUnit(si)) {
				throw new VectoException("compared value has to be the same unit. Got: {0} <=> {1}", this, si);
			}

			return Val.IsSmaller(si.Val, tolerance);
		}

		/// <summary>
		/// Determines whether [is smaller or equal] [the specified si].
		/// </summary>
		/// <param name="si">The si.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		public bool IsSmallerOrEqual(SI si, SI tolerance = null)
		{
			if (!HasEqualUnit(si)) {
				throw new VectoException("compared value has to be the same unit. Got: {0} <=> {1}", this, si);
			}
			if (tolerance != null && !HasEqualUnit(tolerance)) {
				throw new VectoException("tolerance has to be the same unit. Got: {0} <=> {1}", this, tolerance);
			}

			return Val.IsSmallerOrEqual(si.Val, tolerance == null ? DoubleExtensionMethods.Tolerance : tolerance.Value());
		}

		/// <summary>
		/// Determines whether the specified si is greater.
		/// </summary>
		/// <param name="si">The si.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		public bool IsGreater(SI si, SI tolerance = null)
		{
			if (!HasEqualUnit(si)) {
				throw new VectoException("compared value has to be the same unit. Got: {0} <=> {1}", this, si);
			}
			if (tolerance != null && !HasEqualUnit(tolerance)) {
				throw new VectoException("tolerance has to be the same unit. Got: {0} <=> {1}", this, tolerance);
			}

			return Val.IsGreater(si.Val, tolerance == null ? DoubleExtensionMethods.Tolerance : tolerance.Value());
		}

		/// <summary>
		/// Determines whether the specified si is greater.
		/// </summary>
		/// <param name="si">The si.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public bool IsGreater(SI si, double tolerance)
		{
			if (!HasEqualUnit(si)) {
				throw new VectoException("compared value has to be the same unit. Got: {0} <=> {1}", this, si);
			}

			return Val.IsGreater(si.Val, tolerance);
		}

		/// <summary>
		/// Determines whether [is greater or equal] [the specified si].
		/// </summary>
		/// <param name="si">The si.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public bool IsGreaterOrEqual(SI si, SI tolerance = null)
		{
			if (!HasEqualUnit(si)) {
				throw new VectoException("compared value has to be the same unit. Got: {0} <=> {1}", this, si);
			}
			if (tolerance != null && !HasEqualUnit(tolerance)) {
				throw new VectoException("tolerance has to be the same unit. Got: {0} <=> {1}", this, tolerance);
			}

			return Val.IsGreaterOrEqual(si.Val, tolerance == null ? DoubleExtensionMethods.Tolerance : tolerance.Value());
		}

		/// <summary>
		/// Determines whether the specified value is smaller.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public bool IsSmaller(double val, double tolerance = DoubleExtensionMethods.Tolerance)
		{
			return Val.IsSmaller(val, tolerance);
		}

		/// <summary>
		/// Determines whether [is smaller or equal] [the specified value].
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public bool IsSmallerOrEqual(double val, double tolerance = DoubleExtensionMethods.Tolerance)
		{
			return Val.IsSmallerOrEqual(val, tolerance);
		}

		/// <summary>
		/// Determines whether the specified value is greater.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public bool IsGreater(double val, double tolerance = DoubleExtensionMethods.Tolerance)
		{
			return Val.IsGreater(val, tolerance);
		}

		/// <summary>
		/// Determines whether [is greater or equal] [the specified value].
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public bool IsGreaterOrEqual(double val, double tolerance = DoubleExtensionMethods.Tolerance)
		{
			return Val.IsGreaterOrEqual(val, tolerance);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            // ReSharper disable once NonReadonlyMemberInGetHashCode
	            var hashCode = Val.GetHashCode();
	            hashCode = (hashCode * 397) ^ (SIUnits != null ? SIUnits.GetHashCode() : 0);
	            return hashCode;
	        }
	    }
        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" /> in the sort order.
        /// </returns>
	    public int CompareTo(object obj)
	    {
	        var si = obj as SI;
	        if (si == null)
	        {
	            return 1;
	        }

	        if (!HasEqualUnit(si))
	        {
                //if (SIUtils.GetnumberofSIUnits(si.SIUnits) >= SIUtils.GetnumberofSIUnits(SIUnits))
	            if (si.SIUnits.Sum<int>(n => Math.Abs(n)) >= SIUnits.Sum<int>(n => Math.Abs(n)))
                {
	                return -1;
	            }
	            return 1;
	        }

	        if (this > si)
	        {
	            return 1;
	        }
	        return this < si ? -1 : 0;
	    }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(SI left, SI right)
		{
			return Equals(left, right);
		}

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator !=(SI left, SI right)
		{
			return !Equals(left, right);
		}

		#endregion

		/// <summary>
		/// Convert the SI to a string in the wished output format.
		/// </summary>
		/// <param name="decimals">The decimals.</param>
		/// <param name="outputFactor">The output factor.</param>
		/// <param name="showUnit">The show unit.</param>
		/// <returns></returns>
		public string ToOutputFormat(uint? decimals = null, double? outputFactor = null, bool? showUnit = null)
		{
			decimals = decimals ?? 4;
			outputFactor = outputFactor ?? 1.0;
			showUnit = showUnit ?? false;

			if (showUnit.Value) {
				return (Val * outputFactor.Value).ToString("F" + decimals.Value, CultureInfo.InvariantCulture) + " [" +
						GetUnitString() + "]";
			}

			return (Val * outputFactor.Value).ToString("F" + decimals.Value, CultureInfo.InvariantCulture);
		}

		public string ToGUIFormat()
		{
			return Val.ToGUIFormat();
		}

		public string ToXMLFormat(uint? decimals = null)
		{
			decimals = decimals ?? 2;
			return Val.ToString("F" + decimals.Value, CultureInfo.InvariantCulture);
		}

		public class EqualityComparer<T> : IEqualityComparer<T> where T : SI
		{
			private readonly double _precision;

			public EqualityComparer(double precision = DoubleExtensionMethods.Tolerance)
			{
				_precision = precision;
			}

			public bool Equals(T x, T y)
			{
				return x.IsEqual(y.Value(), _precision);
			}

			public int GetHashCode(T obj)
			{
				return obj.Value().GetHashCode();
			}
		}
	}
}