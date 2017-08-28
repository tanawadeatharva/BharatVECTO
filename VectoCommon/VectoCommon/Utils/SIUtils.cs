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

        private short exponent;
        private short reciproc;

        public enum GrammMode
        {
            NoMass,
            Gramm,
            Kilo,
            KiloGramm
        }
        private GrammMode grammMode;

        public UnitInstance(int[] param_units,
            double param_factor, short param_exponent, short param_reciproc,
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

        private enum Op
        {
            Div,
            Mult
        }


        private void CalcFactorValue(Op workop, double factor)
        {
            if (reciproc == -1)
            {
                if (workop == Op.Div)
                {
                    factorValue *= factor;
                }
                else if (workop == Op.Mult)
                {
                    factorValue /= factor;
                }
            }
            else if (reciproc == 1)
            {
                if (workop == Op.Div)
                {
                    factorValue /= factor;
                }
                else if (workop == Op.Mult)
                {
                    factorValue *= factor;
                }
            }

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
                CalcFactorValue(Op.Div, 1000);
                //factorValue /= 1000;
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
                units[2] += 1 * reciproc * exponent;
                CalcFactorValue(Op.Mult, 3600);
                //factorValue *= 3600;
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
                CalcFactorValue(Op.Mult, 1000);
                //factorValue *= 1000;
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
                //units[1] += 1 * reciproc;
                units[1] += 1 * reciproc * exponent;
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Milli
        {
            get
            {
                //CalcFactorValue(Op.Div, 1000);
                //factorValue /= 1000;
                CalcFactorValue(Op.Mult, Math.Pow(factorValue / 1000, exponent));
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Centi
        {
            get
            {
                //CalcFactorValue(Op.Div, 100);
                //Val *= Math.Pow(factor.Value, (double)_exponent);
                CalcFactorValue(Op.Mult, Math.Pow(factorValue/100, exponent));
                //factorValue /= 100;
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Dezi
        {
            get
            {
                //CalcFactorValue(Op.Div, 10);
                //factorValue /= 10;
                CalcFactorValue(Op.Mult, Math.Pow(factorValue / 10, exponent));
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Minute
        {
            get
            {
                units[2] += 1 * reciproc * exponent;
                CalcFactorValue(Op.Mult, 60);
                //factorValue *= 60;
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Newton
        {
            get
            {
                units[0] += 1 * reciproc * exponent;
                units[1] += 1 * reciproc * exponent;
                units[2] -= 2 * reciproc * exponent;
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Per
        {
            get
            {
                //this = Linear;
                //Linear;

                exponent = 1;


                reciproc = (short)(reciproc * (-1));

                //units = SIUtils.SIUnitsMultFactor(units, -1);

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
                CalcFactorValue(Op.Mult, 2 * Math.PI);
                //factorValue *= 2 * Math.PI;
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
                exponent = (short)(2 * reciproc);
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Ton
        {
            get
            {
                units[0] += 1 * reciproc * exponent;
                CalcFactorValue(Op.Mult, 1000);
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }
        public UnitInstance Watt
        {
            get
            {
                units[0] += 1 * reciproc * exponent;
                units[1] += 2 * reciproc * exponent;
                units[2] -= 3 * reciproc * exponent;
                return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
            }
        }


    }


}
