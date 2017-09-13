using System;

namespace TUGraz.VectoCommon.Utils
{
    public struct SIUtils
    {
        //new method
        public static bool CompareUnits(int[] array1, int[] array2)
        {
            for (int count = 0; count < array1.Length; count++)
            {
                if (array1[count] != array2[count])
                {
                    return false;
                }
            }
            return true;
        }

        //new method
        public static int[] CombineUnits(int[] array1, int[] array2)
        {
            int[] resultarray = new int[array1.Length];
            for (int count = 0; count < array1.Length; count++)
            {
                resultarray[count] = array1[count] + array2[count];
            }
            return resultarray;

        }

        //new method
        public static int[] MultiplyUnits(int[] array1, int factor)
        {
            int[] resultarray = new int[array1.Length];
            for (int count = 0; count < array1.Length; count++)
            {
                if (array1[count] != 0)
                {
                    resultarray[count] = array1[count] * factor;
                }
            }
            return resultarray;
        }

        //new method
        //public static int GetnumberofSIUnits(int[] array)
        //{
        //    int resultCount = 0;
        //    for (int count = 0; count < array.Length; count++)
        //    {
        //        resultCount += array[count] < 0 ? array[count] * (-1) : array[count];

        //    }
        //    return resultCount;
        //}
    }


    public struct Unit
    {
        public static UnitInstance SI
        {
            get
            {
                return new UnitInstance(new int[7] { 0, 0, 0, 0, 0, 0, 0 }, 1, 1, 1);
            }
        }

        /// <summary>
        /// Enum for defining the Units.
        /// </summary>
        //[SuppressMessage("ReSharper", "InconsistentNaming")]
        private enum SIUnit
        {
            kg,
            m,
            s,
            A,
            K,
            mol,
            cd,
        }

        //public static string GetUnitString(int[] SIUnitParam)
        public static string GetUnitString(int[] SIUnitParam,bool isGramm)
        {

            Array unitnames = Enum.GetNames(typeof(SIUnit));
            string numerator = "";
            string denominator = "";
            int potent = 0;
            string potentStr = "";


            for (var i = 0; i < SIUnitParam.Length; i++)
            {
                int currentValue = SIUnitParam[i];


                potent = Math.Abs(currentValue);
                potentStr = "";
                if (currentValue != 0)
                {
                    string currentUnit = (string)unitnames.GetValue(i);
                    if(currentUnit == "kg" && isGramm == true)
                    {
                        currentUnit = "g";
                    }


                    if (potent > 1)
                    {
                        potentStr = "^" + potent;
                    }

                    if (currentValue > 0)
                    {
                        numerator += currentUnit + potentStr;

                    }
                    else if (currentValue < 0)
                    {
                        denominator += currentUnit + potentStr;
                    }
                }
            }
            string result = "";

            if (numerator == "")
            {
                if (denominator == "")
                {
                    result = "-";
                }
                else
                {
                    result = "1/" + denominator;
                }
            }
            else
            {
                if (denominator == "")
                {
                    result = numerator;
                }
                else
                {
                    result = numerator + "/" + denominator;
                }
            }

            return result;
        }


    }

    public struct UnitInstance
    {
        // kg, m, s, A, K, mol, cd

        private int[] units;


        private double factorValue;

        /// <summary>
        /// The current exponent for conversion operations (Square, Cubic, Linear, e.g. new SI(3).Square.Meter).
        /// Can be reseted with Reset, Per, Cast.
        /// </summary>
        private int exponent;

        /// <summary>
        /// A flag indicating if the current SI is in reciprocal mode (used in the <see cref="Per"/> method for reciprocal units: e.g. new SI(2).Meter.Per.Second) ==> [m/s]
        /// Can be reseted with Reset, Per, Cast.
        /// </summary>
        private int reciproc;


        [Flags]
        public enum IsMass
        {
            IsKilo = 0x01, //0001
            IsGramm = 0x02, //0010
            IsKiloGramm = 0x04 //0100
        }
        private IsMass isMassOption;



        public UnitInstance(int[] param_units,
            double param_factor, int param_exponent, int param_reciproc)
        {
            units = param_units;
            factorValue = param_factor;
            exponent = param_exponent;
            reciproc = param_reciproc;
            isMassOption = IsMass.IsKiloGramm;
        }



        public IsMass GetGrammMode()
        {
            return isMassOption;
        }

        public int[] GetSIUnits()
        {
            return units;
        }
        public double Getfactor
        {
            get
            {
                return factorValue;
            }
        }

        ///// <summary>
        ///// [g] (to basic unit: [kg])
        ///// </summary>
        //[DebuggerHidden]
        public UnitInstance Gramm
        {
            //[DebuggerHidden]
            get
            {

                units[0] += 1 * reciproc * exponent;

                double factor = Math.Pow(1000, reciproc * exponent);

                if ((isMassOption & IsMass.IsKilo) == IsMass.IsKilo)
                {
                    //is Kilo -> Kilogramm are selected
                    isMassOption = (isMassOption | ~IsMass.IsGramm);
                    isMassOption = (isMassOption | IsMass.IsKiloGramm);
                    factorValue /= factor;

                }
                else
                {
                    //is not kilo -> Gramm are selected
                    isMassOption = (isMassOption | IsMass.IsGramm);
                    isMassOption = (isMassOption | ~IsMass.IsKiloGramm);
                    //factorValue *= factor;
                }
                isMassOption = (isMassOption & ~IsMass.IsKilo);


                //factorValue /= Math.Pow(1000, exponent * reciproc);
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);

                return this; // not work
            }
        }

        ///// <summary>
        ///// Takes all following terms as cubic terms (=to the power of 3).
        ///// </summary>
        //[DebuggerHidden]
        public UnitInstance Cubic
        {
            //[DebuggerHidden]
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                exponent = 3;
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }

        ///// <summary>
        ///// [s] Converts to/from Second. Internally everything is stored in seconds.
        ///// </summary>
        //[DebuggerHidden]
        public UnitInstance Hour
        {
            //[DebuggerHidden]
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                int ReciprocAndExponent = reciproc * exponent;
                units[2] += 1 * ReciprocAndExponent;

                factorValue *= Math.Pow(3600, ReciprocAndExponent);
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }

        ///// <summary>
        ///// Quantifier for Kilo (1000).
        ///// </summary>
        //[DebuggerHidden]
        public UnitInstance Kilo
        {
            //[DebuggerHidden]
            get
            {
                isMassOption = (isMassOption | IsMass.IsKilo);

                factorValue *= Math.Pow(1000, exponent * reciproc);
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }

        ///// <summary>
        ///// Takes all following terms as linear terms (=to the power of 1).
        ///// </summary>
        //[DebuggerHidden]
        public UnitInstance Linear
        {
            //[DebuggerHidden]
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                exponent = 1;
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }

        ///// <summary>
        ///// [m]
        ///// </summary>
        //[DebuggerHidden]
        public UnitInstance Meter
        {
            //[DebuggerHidden]
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                units[1] += 1 * reciproc * exponent;
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }

        ///// <summary>
        ///// Quantifier for milli (1/1000).
        ///// </summary>
        //[DebuggerHidden]
        public UnitInstance Milli
        {
            //[DebuggerHidden]
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                factorValue /= Math.Pow(1000, exponent * reciproc);
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }

        ///// <summary>
        ///// Quantifier for Centi (1/100)
        ///// </summary>
        //[DebuggerHidden]
        public UnitInstance Centi
        {
            //[DebuggerHidden]
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                factorValue /= Math.Pow(100, exponent * reciproc);
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }

        ///// <summary>
        ///// Quantifier for Dezi (1/10)
        ///// </summary>
        //[DebuggerHidden]
        public UnitInstance Dezi
        {
            //[DebuggerHidden]
            get
            {                
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                factorValue /= Math.Pow(10, exponent * reciproc);
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }

        ///// <summary>
        ///// [s] Converts to/from Second. Internally everything is stored in seconds.
        ///// </summary>
        //[DebuggerHidden]
        public UnitInstance Minute
        {
            //[DebuggerHidden]
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                int ReciprocAndExponent = reciproc * exponent;
                units[2] += 1 * ReciprocAndExponent;

                factorValue *= Math.Pow(60, ReciprocAndExponent);
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }

        ///// <summary>
        ///// [N]
        ///// </summary>
        //[DebuggerHidden]
        public UnitInstance Newton
        {
            //[DebuggerHidden]
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                int ReciprocAndExponent = reciproc * exponent;
                units[0] += 1 * ReciprocAndExponent;
                units[1] += 1 * ReciprocAndExponent;
                units[2] -= 2 * ReciprocAndExponent;
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }

        ///// <summary>
        ///// Defines the denominator by the terms following after the Per.
        ///// </summary>
        //[DebuggerHidden]
        public UnitInstance Per
        {
            //[DebuggerHidden]
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                exponent = 1;
                reciproc = reciproc * (-1);

                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }

        ///// <summary>
        ///// [-]. Defines radian. Only virtual. Has no real SI unit.
        ///// </summary>
        //[DebuggerHidden]
        public UnitInstance Radian
        {
            //[DebuggerHidden]
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }

        ///// <summary>
        ///// [-]. Converts to/from Radiant. Internally everything is stored in radian.
        ///// </summary>
        //[DebuggerHidden]
        public UnitInstance Rounds
        {
            //[DebuggerHidden]
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                factorValue *= Math.Pow(2 * Math.PI, exponent * reciproc);
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }

        ///// <summary>
        ///// [s]
        ///// </summary>
        //[DebuggerHidden]
        public UnitInstance Second
        {
            //[DebuggerHidden]
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                units[2] += 1 * reciproc * exponent;
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }

        ///// <summary>
        ///// Takes all following terms as quadratic terms (=to the power of 2).
        ///// </summary>
        //[DebuggerHidden]
        public UnitInstance Square
        {
            //[DebuggerHidden]
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                exponent = 2;
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }

        ///// <summary>
        ///// [t] (to basic unit: [kg])
        ///// </summary>
        //[DebuggerHidden]
        public UnitInstance Ton
        {
            //[DebuggerHidden]
            get
            {
                // remove Gramm and Kilo and KiloGramm is selected.
                isMassOption = (isMassOption & ~IsMass.IsKilo);
                isMassOption = (isMassOption | ~IsMass.IsGramm);
                isMassOption = (isMassOption | IsMass.IsKiloGramm);

                int ReciprocAndExponent = reciproc * exponent;
                units[0] += 1 * ReciprocAndExponent;
                factorValue *= Math.Pow(1000, ReciprocAndExponent);
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }

        ///// <summary>
        ///// [W]
        ///// </summary>
        //[DebuggerHidden]
        public UnitInstance Watt
        {
            //[DebuggerHidden]
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                int ReciprocAndExponent = reciproc * exponent;
                units[0] += 1 * ReciprocAndExponent;
                units[1] += 2 * ReciprocAndExponent;
                units[2] -= 3 * ReciprocAndExponent;
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }
        public UnitInstance Joule
        {
            //[DebuggerHidden]
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                int ReciprocAndExponent = reciproc * exponent;
                units[0] += 1 * ReciprocAndExponent;
                units[1] += 2 * ReciprocAndExponent;
                units[2] -= 2 * ReciprocAndExponent;
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }

        public UnitInstance Liter
        {
            //[DebuggerHidden]
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                int ReciprocAndExponent = reciproc * exponent;
                units[1] += 3 * ReciprocAndExponent;
                factorValue /= Math.Pow(1000, ReciprocAndExponent);
                return this;
            }
        }

    }

}
