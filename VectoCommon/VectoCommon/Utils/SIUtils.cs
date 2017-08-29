using System;

namespace TUGraz.VectoCommon.Utils
{
    public struct SIUtils
    {
        //new method
        public static bool CompareSIUnits(int[] array1, int[] array2)
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
        public static int[] AdditionTheSIUnits(int[] array1, int[] array2)
        {
            int[] resultarray = new int[array1.Length];
            for (int count = 0; count < array1.Length; count++)
            {
                resultarray[count] = array1[count] + array2[count];
            }
            return resultarray;

        }

        //new method
        public static int[] SIUnitsMultFactor(int[] array1, int factor)
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
        public static int GetnumberofSIUnits(int[] array)
        {
            int resultCount = 0;
            for (int count = 0; count < array.Length; count++)
            {
                resultCount += array[count] < 0 ? array[count] * (-1) : array[count];

            }
            return resultCount;
        }
    }


    public struct Unit
    {
        public static UnitInstance SI
        {
            get
            {
                return new UnitInstance(new int[7] { 0, 0, 0, 0, 0, 0, 0 }, 1, 1, 1, UnitInstance.GrammMode.NoMass);
            }
        }

    }

    public struct UnitInstance
    {
        private int[] units;
        private double factorValue;

        private int exponent;
        private int reciproc;

        public enum GrammMode
        {
            NoMass,
            Gramm,
            Kilo,
            KiloGramm
        }
        private GrammMode grammMode;

        public UnitInstance(int[] param_units,
            double param_factor, int param_exponent, int param_reciproc,
            GrammMode param_grammMode)
        {
            units = param_units;
            factorValue = param_factor;
            exponent = param_exponent;
            reciproc = param_reciproc;
            grammMode = param_grammMode;
        }

        public GrammMode GetGrammMode()
        {
            return grammMode;
        }

        public int[] GetSIUnits()
        {
            return units;
        }
        public double Getfactor()
        {
            return factorValue;
        }


        public UnitInstance Gramm
        {
            get
            {
                if (grammMode == GrammMode.NoMass)
                {
                    grammMode = GrammMode.Gramm;
                }
                else if (grammMode == GrammMode.Kilo)
                {
                    grammMode = GrammMode.KiloGramm;
                }
                units[0] += 1 * reciproc * exponent;

                factorValue /= Math.Pow(1000, exponent * reciproc);
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                //return this; // not work
            }
        }
        public UnitInstance Cubic
        {
            get
            {
                exponent = 3;
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Hour
        {
            get
            {
                int ReciprocAndExponent = reciproc * exponent;
                units[2] += 1 * ReciprocAndExponent;

                factorValue *= Math.Pow(3600, ReciprocAndExponent);
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Kilo
        {
            get
            {
                if (grammMode == GrammMode.NoMass)
                {
                    grammMode = GrammMode.Kilo;
                }
                else if (grammMode == GrammMode.Gramm)
                {
                    grammMode = GrammMode.KiloGramm;
                }

                factorValue *= Math.Pow(1000, exponent * reciproc);
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Linear
        {
            get
            {
                exponent = 1;
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Meter
        {
            get
            {
                units[1] += 1 * reciproc * exponent;
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Milli
        {
            get
            {

                factorValue /= Math.Pow(1000, exponent * reciproc);
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Centi
        {
            get
            {
                factorValue /= Math.Pow(100, exponent * reciproc);
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Dezi
        {
            get
            {
                factorValue /= Math.Pow(10, exponent * reciproc);
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Minute
        {
            get
            {
                int ReciprocAndExponent = reciproc * exponent;
                units[2] += 1 * ReciprocAndExponent;

                factorValue *= Math.Pow(60, ReciprocAndExponent);
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Newton
        {
            get
            {
                int ReciprocAndExponent = reciproc * exponent;
                units[0] += 1 * ReciprocAndExponent;
                units[1] += 1 * ReciprocAndExponent;
                units[2] -= 2 * ReciprocAndExponent;
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Per
        {
            get
            {
                exponent = 1;
                reciproc = reciproc * (-1);

                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                //return this;
            }
        }
        public UnitInstance Radian
        {
            get
            {
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Rounds
        {
            get
            {
                factorValue *= Math.Pow(2 * Math.PI, exponent * reciproc);
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Second
        {
            get
            {
                units[2] += 1 * reciproc * exponent;
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Square
        {
            get
            {
                exponent = 2 * reciproc;
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Ton
        {
            get
            {
                int ReciprocAndExponent = reciproc * exponent;
                units[0] += 1 * ReciprocAndExponent;
                factorValue *= Math.Pow(1000, ReciprocAndExponent);
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Watt
        {
            get
            {
                int ReciprocAndExponent = reciproc * exponent;
                units[0] += 1 * ReciprocAndExponent;
                units[1] += 2 * ReciprocAndExponent;
                units[2] -= 3 * ReciprocAndExponent;
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }


    }

}
