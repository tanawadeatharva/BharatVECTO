using System.Collections.Generic;
using System.Data;
using System.Linq;
using iTextSharp.text;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
    public class CrossWindCorrectionCurve : LoggingObject
    {
        public CrossWindCorrectionMode CorrectionMode { get; internal set; }

        protected List<CrossWindCorrectionEntry> Entries;

        public CrossWindCorrectionCurve(List<CrossWindCorrectionEntry> entries, CrossWindCorrectionMode correctionMode)
        {
            CorrectionMode = correctionMode;
            Entries = entries;
        }

        public static CrossWindCorrectionCurve ReadSpeedDependentCorrectionFromFile(string fileName,
            SquareMeter aerodynamicDragArea)
        {
            var data = VectoCSVFile.Read(fileName);
            if (data.Columns.Count != 2) {
                throw new VectoException("Crosswind correction file must consist of 2 columns.");
            }
            if (data.Rows.Count < 2) {
                throw new VectoException("Crosswind correction file must consist of at least two entries");
            }

            if (HeaderIsValid(data.Columns)) {
                return new CrossWindCorrectionCurve(ReadFromColumnNames(data, aerodynamicDragArea),
                    CrossWindCorrectionMode.SpeedDependentCorrectionFactor);
            } else {
                Logger<CrossWindCorrectionCurve>()
                    .Warn(
                        "Crosswind correction file: Header line is not valid. Expected: '{0}, {1}', Got: '{2}'. Falling back to column index.",
                        Fields.Velocity, Fields.Cd,
                        string.Join(", ", data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Reverse()));
                return new CrossWindCorrectionCurve(ReadFromColumnIndizes(data, aerodynamicDragArea),
                    CrossWindCorrectionMode.SpeedDependentCorrectionFactor);
            }
        }

        private static List<CrossWindCorrectionEntry> ReadFromColumnIndizes(DataTable data,
            SquareMeter aerodynamicDragArea)
        {
            return (from DataRow row in data.Rows
                select new CrossWindCorrectionEntry {
                    Velocity = row.ParseDouble(0).KMPHtoMeterPerSecond(),
                    EffectiveCrossSectionArea = row.ParseDouble(1) * aerodynamicDragArea
                }).ToList();
        }

        private static List<CrossWindCorrectionEntry> ReadFromColumnNames(DataTable data,
            SquareMeter aerodynamicDragArea)
        {
            return (from DataRow row in data.Rows
                select new CrossWindCorrectionEntry {
                    Velocity = row.ParseDouble(Fields.Velocity).KMPHtoMeterPerSecond(),
                    EffectiveCrossSectionArea = row.ParseDouble(Fields.Cd) * aerodynamicDragArea
                }).ToList();
        }

        private static bool HeaderIsValid(DataColumnCollection columns)
        {
            return columns.Contains(Fields.Velocity) && columns.Contains(Fields.Cd);
        }

        public static CrossWindCorrectionCurve GetNoCorrectionCurve(SquareMeter aerodynamicDragArea)
        {
            return new CrossWindCorrectionCurve(new[] {
                new CrossWindCorrectionEntry() {
                    Velocity = 0.KMPHtoMeterPerSecond(),
                    EffectiveCrossSectionArea = aerodynamicDragArea
                },
                new CrossWindCorrectionEntry() {
                    Velocity = 100.KMPHtoMeterPerSecond(),
                    EffectiveCrossSectionArea = aerodynamicDragArea
                }
            }.ToList(), CrossWindCorrectionMode.NoCorrection);
        }

        public SquareMeter EffectiveAirDragArea(MeterPerSecond x)
        {
            var p = Entries.GetSection(c => c.Velocity < x);

            if (x < p.Item1.Velocity || p.Item2.Velocity < x) {
                //Log.Error(_data.CrossWindCorrectionMode == CrossWindCorrectionMode.VAirBetaLookupTable
                //    ? string.Format("CdExtrapol β = {0}", x)
                //    : string.Format("CdExtrapol v = {0}", x));
                Log.Error("CdExtrapol v = {0}", x);
            }

            return VectoMath.Interpolate(p.Item1.Velocity, p.Item2.Velocity,
                p.Item1.EffectiveCrossSectionArea, p.Item2.EffectiveCrossSectionArea, x);
        }

        public class Fields
        {
            public static readonly string Velocity = "v";
            public static readonly string Cd = "Cd";
        }

        public class CrossWindCorrectionEntry
        {
            public MeterPerSecond Velocity;
            public SquareMeter EffectiveCrossSectionArea;
        }
    }
}