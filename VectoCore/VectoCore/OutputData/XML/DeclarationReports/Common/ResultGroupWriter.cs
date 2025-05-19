using System;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
{


	public abstract class AbstractResultGroupWriter : AbstractResultWriter, IResultGroupWriter
	{
		protected AbstractResultGroupWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Implementation of IResultGroupWriter

		public abstract XElement GetElement(IResultEntry entry);


		public virtual XElement GetElement(IOVCResultEntry entry)
		{
			throw new NotImplementedException();
		}

		#endregion

	}

	// -------------------------

	public class ResultErrorMissionWriter : AbstractResultWriter, IResultSequenceWriter
	{
		public ResultErrorMissionWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultWriter

		public virtual XElement[] GetElement(IResultEntry entry)
		{
			return new[] { new XElement(TNS + XMLNames.Report_Result_Mission, entry.Mission.ToXMLFormat()) };
		}

		public XElement[] GetElement(IOVCResultEntry entry)
		{
			return null;
		}

		#endregion
	}

	// -------------------------

    public class ResultSimulationParameterLorryWriter : AbstractResultGroupWriter
	{
		public ResultSimulationParameterLorryWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_ResultEntry_SimulationParameters,
				new XElement(TNS + XMLNames.Report_ResultEntry_TotalVehicleMass,
					XMLHelper.ValueAsUnit(entry.TotalVehicleMass, XMLNames.Unit_kg)),
				new XElement(TNS + XMLNames.Report_ResultEntry_Payload,
					XMLHelper.ValueAsUnit(entry.Payload, XMLNames.Unit_kg)));
		}

		#endregion
	}

	// -------------------------

    public class ResultSimulationParameterErrorWriter : AbstractResultGroupWriter
	{
		public ResultSimulationParameterErrorWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_ResultEntry_SimulationParameters,
				new XElement(TNS + XMLNames.Report_ResultEntry_TotalVehicleMass,
					XMLHelper.ValueAsUnit(entry.TotalVehicleMass, XMLNames.Unit_kg)),
				new XElement(TNS + XMLNames.Report_ResultEntry_Payload,
					XMLHelper.ValueAsUnit(entry.Payload, XMLNames.Unit_kg)));
		}

		#endregion
	}

	// -------------------------

    public class ElectricRangeWriter : AbstractResultWriter, IElectricRangeWriter
	{
		public ElectricRangeWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Implementation of IElectricRangeWriter

		public virtual XElement[] GetElements(IResultEntry result)
		{
			if (result.Status == VectoRun.Status.PrimaryBusSimulationIgnore) {
				return new[] {
					new XElement(TNS + XMLNames.Report_ResultEntry_ActualChargeDepletingRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_EquivalentAllElectricRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
				};
            }
			return new[] {
				new XElement(TNS + XMLNames.Report_ResultEntry_ActualChargeDepletingRange,
					result.ActualChargeDepletingRange.ConvertToKiloMeter().ValueAsUnit()),
				new XElement(TNS + XMLNames.Report_ResultEntry_EquivalentAllElectricRange,
					result.EquivalentAllElectricRange.ConvertToKiloMeter().ValueAsUnit()),
				new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
					result.ZeroCO2EmissionsRange.ConvertToKiloMeter().ValueAsUnit()),
			};
		}

		public virtual XElement[] GetElements(IWeightedResult weightedResult)
		{
			if (weightedResult.Status == VectoRun.Status.PrimaryBusSimulationIgnore) {
				return new[] {
					new XElement(TNS + XMLNames.Report_ResultEntry_ActualChargeDepletingRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_EquivalentAllElectricRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
				};
			}
            return new[] {
				new XElement(TNS + XMLNames.Report_ResultEntry_ActualChargeDepletingRange,
					weightedResult.ActualChargeDepletingRange.ConvertToKiloMeter().ValueAsUnit()),
				new XElement(TNS + XMLNames.Report_ResultEntry_EquivalentAllElectricRange,
					weightedResult.EquivalentAllElectricRange.ConvertToKiloMeter().ValueAsUnit()),
				new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
					weightedResult.ZeroCO2EmissionsRange.ConvertToKiloMeter().ValueAsUnit()),
			};
		}

		#endregion
	}

	// -------------------------

    public class ElectricRangeWriter_BOL : AbstractResultWriter, IElectricRangeWriter
	{
		public ElectricRangeWriter_BOL(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Implementation of IElectricRangeWriter

		public XElement[] GetElements(IResultEntry result)
		{
			if (result.Status == VectoRun.Status.PrimaryBusSimulationIgnore) {
				return new[] {
					new XElement(TNS + XMLNames.Report_ResultEntry_Ranges,
						new XAttribute(XMLNames.Report_ResultEntry_Ranges_Lifetime_Attr, XMLNames.Report_ResultEntry_Ranges_Lifetime_BOL_Val),
						new XElement(TNS + XMLNames.Report_ResultEntry_ActualChargeDepletingRange,
							new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
						new XElement(TNS + XMLNames.Report_ResultEntry_EquivalentAllElectricRange,
							new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
						new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
							new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit())
					),
					new XElement(TNS + XMLNames.Report_ResultEntry_Ranges,
						new XAttribute(XMLNames.Report_ResultEntry_Ranges_Lifetime_Attr, XMLNames.Report_ResultEntry_Ranges_Lifetime_EOL_Val),
						new XElement(TNS + XMLNames.Report_ResultEntry_ActualChargeDepletingRange,
							new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
						new XElement(TNS + XMLNames.Report_ResultEntry_EquivalentAllElectricRange,
							new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
						new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
							new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit())
					),
                };
			}
			return new[] {
				// TODO: MQ 20250509: replace NaN with actual computed values
				new XElement(TNS + XMLNames.Report_ResultEntry_Ranges,
					new XAttribute(XMLNames.Report_ResultEntry_Ranges_Lifetime_Attr, XMLNames.Report_ResultEntry_Ranges_Lifetime_BOL_Val),
					new XElement(TNS + XMLNames.Report_ResultEntry_ActualChargeDepletingRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_EquivalentAllElectricRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit())
				),
				new XElement(TNS + XMLNames.Report_ResultEntry_Ranges,
					new XAttribute(XMLNames.Report_ResultEntry_Ranges_Lifetime_Attr, XMLNames.Report_ResultEntry_Ranges_Lifetime_EOL_Val),
					new XElement(TNS + XMLNames.Report_ResultEntry_ActualChargeDepletingRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_EquivalentAllElectricRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit())
				),
            };
        }

		public XElement[] GetElements(IWeightedResult weightedResult)
		{
			if (weightedResult.Status == VectoRun.Status.PrimaryBusSimulationIgnore) {
				return new[] {
					new XElement(TNS + XMLNames.Report_ResultEntry_Ranges,
						new XAttribute(XMLNames.Report_ResultEntry_Ranges_Lifetime_Attr, XMLNames.Report_ResultEntry_Ranges_Lifetime_BOL_Val),
						new XElement(TNS + XMLNames.Report_ResultEntry_ActualChargeDepletingRange,
							new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
						new XElement(TNS + XMLNames.Report_ResultEntry_EquivalentAllElectricRange,
							new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
						new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
							new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit())
					),
					new XElement(TNS + XMLNames.Report_ResultEntry_Ranges,
						new XAttribute(XMLNames.Report_ResultEntry_Ranges_Lifetime_Attr, XMLNames.Report_ResultEntry_Ranges_Lifetime_EOL_Val),
						new XElement(TNS + XMLNames.Report_ResultEntry_ActualChargeDepletingRange,
							new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
						new XElement(TNS + XMLNames.Report_ResultEntry_EquivalentAllElectricRange,
							new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
						new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
							new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit())
					),
				};
            }
			return new[] {
				// TODO: MQ 20250509: replace NaN with actual computed values
				new XElement(TNS + XMLNames.Report_ResultEntry_Ranges,
					new XAttribute(XMLNames.Report_ResultEntry_Ranges_Lifetime_Attr, XMLNames.Report_ResultEntry_Ranges_Lifetime_BOL_Val),
					new XElement(TNS + XMLNames.Report_ResultEntry_ActualChargeDepletingRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_EquivalentAllElectricRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit())
				),
				new XElement(TNS + XMLNames.Report_ResultEntry_Ranges,
					new XAttribute(XMLNames.Report_ResultEntry_Ranges_Lifetime_Attr, XMLNames.Report_ResultEntry_Ranges_Lifetime_EOL_Val),
					new XElement(TNS + XMLNames.Report_ResultEntry_ActualChargeDepletingRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_EquivalentAllElectricRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit())
				),
			};
        }

		#endregion
	}

    // -------------------------

	public class HydrogenRangeWriter : AbstractResultWriter, IHydrogenRangeWriter
	{
		protected readonly IInternalHydrogenRangeWriterFactory _hydrogenRageWriterFactory;

		public HydrogenRangeWriter(ICommonResultsWriterFactory factory, XNamespace ns,
			IInternalHydrogenRangeWriterFactory hydrogenRageWriterFactory) : base(factory, ns)
		{
			_hydrogenRageWriterFactory = hydrogenRageWriterFactory;

		}

		#region Implementation of IHydrogenRangeWriter

		public XElement[] GetElements(IResultEntry results)
		{
			var writer = _hydrogenRageWriterFactory.GetHydrogenRangeWriter(results.VectoRunData.JobType, results.VectoRunData.VehicleData.OffVehicleCharging, results.FuelData);
			return writer.GetElements(results);
		}

		public XElement[] GetElements(IWeightedResult weightedResult)
		{
			var writer = _hydrogenRageWriterFactory.GetHydrogenRangeWriter(weightedResult.JobType, weightedResult.OffVehicleCharging, weightedResult.FuelConsumption.Keys.ToList());
			return writer.GetElements(weightedResult);
		}

        #endregion
    }

	// -------------------------


    public class NullHydrogenRangeWriter : AbstractResultWriter, IHydrogenRangeWriter
	{
		public NullHydrogenRangeWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Implementation of IHydrogenRangeWriter

		public XElement[] GetElements(IResultEntry weightedResult)
		{
			return null;
		}

		public XElement[] GetElements(IWeightedResult weightedResult)
		{
			return null;
		}

		#endregion
	}

    // -------------------------

    public class HydrogenRangeWriterICE : AbstractResultWriter, IHydrogenRangeWriter
	{
		public HydrogenRangeWriterICE(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		public XElement[] GetElements(IResultEntry result)
		{
			return new[] {
				new XElement(TNS + XMLNames.Report_ResultEntry_Ranges,
					new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
                        result.ZeroCO2EmissionsRange.ConvertToKiloMeter().ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_HydrogenRange,
                        result.HydrogenRange.ConvertToKiloMeter().ValueAsUnit())
                )
			};
		}

		public XElement[] GetElements(IWeightedResult weightedResult)
		{
			return new[] {
				new XElement(TNS + XMLNames.Report_ResultEntry_Ranges,
					new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
                        weightedResult.ZeroCO2EmissionsRange.ConvertToKiloMeter().ValueAsUnit()),
                    new XElement(TNS + XMLNames.Report_ResultEntry_HydrogenRange,
                        weightedResult.HydrogenRange.ConvertToKiloMeter().ValueAsUnit())
                )
			};
        }
	}

	// -------------------------

	public class HydrogenRangeWriterHEV : AbstractResultWriter, IHydrogenRangeWriter
	{
		public HydrogenRangeWriterHEV(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		public XElement[] GetElements(IResultEntry result)
		{
			return new[] {
				new XElement(TNS + XMLNames.Report_ResultEntry_Ranges,
					new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
                        result.ZeroCO2EmissionsRange.ConvertToKiloMeter().ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_HydrogenRange,
                        result.HydrogenRange.ConvertToKiloMeter().ValueAsUnit())
                )
			};
        }

		public XElement[] GetElements(IWeightedResult weightedResult)
		{
			return new[] {
				new XElement(TNS + XMLNames.Report_ResultEntry_Ranges,
					new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
                        weightedResult.ZeroCO2EmissionsRange.ConvertToKiloMeter().ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_HydrogenRange,
                        weightedResult.HydrogenRange.ConvertToKiloMeter().ValueAsUnit())
                )
			};
        }
	}

	// -------------------------

	public class HydrogenRangeWriterHEV_OVC : AbstractResultWriter, IHydrogenRangeWriter
	{
		public HydrogenRangeWriterHEV_OVC(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		public XElement[] GetElements(IResultEntry result)
		{
			return new[] {
				new XElement(TNS + XMLNames.Report_ResultEntry_HydrogenRange,
                    result.HydrogenRange.ConvertToKiloMeter().ValueAsUnit())
            };
		}

		public XElement[] GetElements(IWeightedResult weightedResult)
		{
            return new[] {
				new XElement(TNS + XMLNames.Report_ResultEntry_HydrogenRange,
                    weightedResult.HydrogenRange.ConvertToKiloMeter().ValueAsUnit())
            };
        }
	}

	// -------------------------

    public class HydrogenRangeWriterFCHV : AbstractResultWriter, IHydrogenRangeWriter
	{
		public HydrogenRangeWriterFCHV(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		public XElement[] GetElements(IResultEntry result)
		{
			return new[] {
				new XElement(TNS + XMLNames.Report_ResultEntry_Ranges,
					new XAttribute("lifetime", "independent"),
					new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
						result.ZeroCO2EmissionsRange.ConvertToKiloMeter().ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_HydrogenRange,
                        result.HydrogenRange.ConvertToKiloMeter().ValueAsUnit())
                )
			};
        }

		public XElement[] GetElements(IWeightedResult weightedResult)
		{
			return new[] {
				new XElement(TNS + XMLNames.Report_ResultEntry_Ranges,
					new XAttribute("lifetime", "independent"),
					new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
                        weightedResult.ZeroCO2EmissionsRange.ConvertToKiloMeter().ValueAsUnit()),
                    new XElement(TNS + XMLNames.Report_ResultEntry_HydrogenRange,
                        weightedResult.HydrogenRange.ConvertToKiloMeter().ValueAsUnit())
                )
			};
		}
	}

	// -------------------------

	public class HydrogenRangeWriterFCHV_OVC : AbstractResultWriter, IHydrogenRangeWriter
	{
		public HydrogenRangeWriterFCHV_OVC(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		public XElement[] GetElements(IResultEntry result)
		{
			switch(result.OVCMode) {
				case OvcHevMode.ChargeSustaining:
					return new[] {
						new XElement(TNS + XMLNames.Report_ResultEntry_Ranges,
							new XAttribute("lifetime", "independent"),
							new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
								result.ZeroCO2EmissionsRange.ConvertToKiloMeter().ValueAsUnit()),
							new XElement(TNS + XMLNames.Report_ResultEntry_HydrogenRange,
                                result.HydrogenRange.ConvertToKiloMeter().ValueAsUnit())
                        )
					};
				case OvcHevMode.ChargeDepleting:
					return new[] {
						new XElement(TNS + XMLNames.Report_ResultEntry_Ranges,
							new XAttribute(XMLNames.Report_ResultEntry_Ranges_Lifetime_Attr, XMLNames.Report_ResultEntry_Ranges_Lifetime_BOL_Val),
							new XElement(TNS + XMLNames.Report_ResultEntry_ActualChargeDepletingRange,
								result.BeginOfLifeRanges.ActualChargeDepletingRange.ConvertToKiloMeter().ValueAsUnit()),
							new XElement(TNS + XMLNames.Report_ResultEntry_EquivalentAllElectricRange,
                                result.BeginOfLifeRanges.EquivalentAllElectricRange.ConvertToKiloMeter().ValueAsUnit()),
							new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
                                result.BeginOfLifeRanges.ZeroCO2EmissionsRange.ConvertToKiloMeter().ValueAsUnit())
						),
						new XElement(TNS + XMLNames.Report_ResultEntry_Ranges,
							new XAttribute(XMLNames.Report_ResultEntry_Ranges_Lifetime_Attr, XMLNames.Report_ResultEntry_Ranges_Lifetime_EOL_Val),
							new XElement(TNS + XMLNames.Report_ResultEntry_ActualChargeDepletingRange,
                                result.EndOfLifeRanges.ActualChargeDepletingRange.ConvertToKiloMeter().ValueAsUnit()),
                            new XElement(TNS + XMLNames.Report_ResultEntry_EquivalentAllElectricRange,
                                result.EndOfLifeRanges.EquivalentAllElectricRange.ConvertToKiloMeter().ValueAsUnit()),
                            new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
                                result.EndOfLifeRanges.ZeroCO2EmissionsRange.ConvertToKiloMeter().ValueAsUnit())
                        ),
					};
            }
			return null;
		}

		public XElement[] GetElements(IWeightedResult weightedResult)
		{
			throw new NotImplementedException();
		}
	}

    // -------------------------

    public class ErrorResultWriter : AbstractResultGroupWriter
	{
		public ErrorResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }


		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			if (entry.Status == VectoRun.Status.Success) {
				throw new Exception("Simulation run needs to be unsuccessful!");
			}

			return new XElement(TNS + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, XMLNames.Report_Results_Status_Error_Val),
				ResultXMLType != null ? new XAttribute(xsi + XMLNames.XSIType, ResultXMLType) : null,
				_factory.GetErrorMissionWriter(_factory, TNS).GetElement(entry),
				_factory.GetErrorSimulationParameterWriter(_factory, TNS).GetElement(entry),
				_factory.GetErrorDetailsWriter(_factory, TNS).GetElement(entry)
			);
		}

		public override XElement GetElement(IOVCResultEntry entry)
		{
			var errorEntry = new[] { entry.ChargeSustainingResult, entry.ChargeDepletingResult }.FirstOrDefault(x => x.Status != VectoRun.Status.Success);
			if (errorEntry == null) {
				throw new Exception("At least one entry needs to be unsuccessful!");
			}
			return new XElement(TNS + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, XMLNames.Report_Results_Status_Error_Val),
				ResultXMLType != null ? new XAttribute(xsi + XMLNames.XSIType, ResultXMLType) : null,
				_factory.GetErrorMissionWriter(_factory, TNS).GetElement(errorEntry),
				_factory.GetErrorSimulationParameterWriter(_factory, TNS).GetElement(errorEntry),
				_factory.GetErrorDetailsWriter(_factory, TNS).GetElement(errorEntry)
			);
		}

		#endregion

		protected virtual string ResultXMLType => "ResultErrorType";
	}
}