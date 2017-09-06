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
                return new UnitInstance(new int[7] { 0, 0, 0, 0, 0, 0, 0 }, 1, 1, 1);//, UnitInstance.GrammMode.NoMass);
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
        private int[] units;

        private double factorValue;
        private int exponent;
        private int reciproc;


        [Flags]
        public enum IsMass
        {
            IsKilo = 0x01, //0001
            IsGramm = 0x02, //0010
            IsKiloGramm = 0x04 //0100
        }
        private IsMass isMassOption;

        //public enum GrammMode
        //{
        //    NoMass,
        //    Gramm,
        //    Kilo,
        //    KiloGramm
        //}
        //private GrammMode grammMode;


        public UnitInstance(int[] param_units,
            double param_factor, int param_exponent, int param_reciproc)//,
            //GrammMode param_grammMode)
        {
            units = param_units;
            factorValue = param_factor;
            exponent = param_exponent;
            reciproc = param_reciproc;
            //grammMode = param_grammMode;
            isMassOption = IsMass.IsKiloGramm;
        }

        //public GrammMode GetGrammMode()
        //{
        //    return grammMode;
        //}

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
            //return factorValue;
        }
        public int GetExponent
        {
            get
            {
                return exponent;
            }
        }

        public UnitInstance Gramm
        {
            get
            {
                //if (grammMode == GrammMode.NoMass)
                //{
                //    grammMode = GrammMode.Gramm;
                //}
                //else if (grammMode == GrammMode.Kilo)
                //{
                //    grammMode = GrammMode.KiloGramm;
                //}

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
        public UnitInstance Cubic
        {
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                exponent = 3;
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }
        public UnitInstance Hour
        {
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
        public UnitInstance Kilo
        {
            get
            {
                isMassOption = (isMassOption | IsMass.IsKilo);

                //if (grammMode == GrammMode.NoMass)
                //{
                //    grammMode = GrammMode.Kilo;
                //}
                //else if (grammMode == GrammMode.Gramm)
                //{
                //    grammMode = GrammMode.KiloGramm;
                //}

                factorValue *= Math.Pow(1000, exponent * reciproc);
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }
        public UnitInstance Linear
        {
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                exponent = 1;
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }
        public UnitInstance Meter
        {
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                units[1] += 1 * reciproc * exponent;
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }
        public UnitInstance Milli
        {
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                factorValue /= Math.Pow(1000, exponent * reciproc);
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }
        public UnitInstance Centi
        {
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                factorValue /= Math.Pow(100, exponent * reciproc);
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }
        public UnitInstance Dezi
        {
            get
            {                
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                factorValue /= Math.Pow(10, exponent * reciproc);
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }
        public UnitInstance Minute
        {
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
        public UnitInstance Newton
        {
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
        public UnitInstance Per
        {
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                exponent = 1;
                reciproc = reciproc * (-1);

                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }
        public UnitInstance Radian
        {
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }
        public UnitInstance Rounds
        {
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                factorValue *= Math.Pow(2 * Math.PI, exponent * reciproc);
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }
        public UnitInstance Second
        {
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                units[2] += 1 * reciproc * exponent;
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }
        public UnitInstance Square
        {
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                exponent = 2;
                //return new UnitInstance(units, factorValue, exponent, reciproc, grammMode);
                return this;
            }
        }
        public UnitInstance Ton
        {
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
        public UnitInstance Watt
        {
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
            get
            {
                isMassOption = (isMassOption & ~IsMass.IsKilo);

                int ReciprocAndExponent = reciproc * exponent;
                units[2] += 3 * ReciprocAndExponent;
                factorValue /= Math.Pow(1000, ReciprocAndExponent);
                return this;
            }
        }

    }

}
