using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
	public interface IResultsWriter
	{
		XElement GenerateResults(List<IResultEntry> results);
	}

	public interface IResultGroupWriter
	{
		XElement GetElement(IResultEntry entry);

		XElement GetElement(IOVCResultEntry entry);
	}

	public interface IFuelConsumptionWriter
	{
		XElement GetElement(IResultEntry entry, IFuelConsumptionCorrection fuelConsumptionCorrection);
		XElement GetElement(IWeightedResult entry, IFuelProperties fuel, Kilogram consumption);

	}

	public interface IElectricEnergyConsumptionWriter
	{
		XElement GetElement(IResultEntry entry);

		XElement GetElement(IWeightedResult weighted);
	}

	public interface ICO2Writer
	{
		XElement[] GetElements(IResultEntry entry);

		XElement[] GetElements(IWeightedResult entry);
	}

	public interface ICifSummaryWriter
	{
		XElement GetElement(IList<IResultEntry> entries);

		XElement GetElement(IList<IOVCResultEntry> entries);
	}

	public interface IElectricRangeWriter
	{
		XElement[] GetElements(IResultEntry weightedResult);

		XElement[] GetElements(IWeightedResult weightedResult);

	}


	public interface ICifResultsWriterFactory
	{
		IResultGroupWriter GetLorryConvSuccessResultWriter();
		IResultGroupWriter GetLorryHEVNonOVCSuccessResultWriter();

		IResultGroupWriter GetLorryHEVOVCSuccessResultWriter();
		IResultGroupWriter GetLorryPEVSuccessResultWriter();

		IResultGroupWriter GetLorryErrorResultWriter();

		IResultGroupWriter GetBusSuccessResultWriter();
		IResultGroupWriter GetBusOVCSuccessResultWriter();
		IResultGroupWriter GetBusErrorResultWriter();
		IResultGroupWriter GetBusOVCErrorResultWriter();

		IResultGroupWriter GetMissionWriter();
		IResultGroupWriter GetLorrySimulationParameterWriter();

		IResultGroupWriter GetLorryConvTotalWriter();
		IResultGroupWriter GetLorryHEVNonOVCTotalWriter();
		IResultGroupWriter GetLorryHEVOVCResultWriterChargeDepleting();
		IResultGroupWriter GetLorryHEVOVCResultWriterChargeSustaining();
		IResultGroupWriter GetLorryHEVOVCTotalWriter();
		IResultGroupWriter GetLorryPEVTotalWriter();

		IFuelConsumptionWriter GetFuelConsumptionLorry();
		IElectricEnergyConsumptionWriter GetElectricEnergyConsumptionLorry();
		ICO2Writer GetCO2ResultLorry();

		ICifSummaryWriter GetLorryConvSummaryWriter();
		ICifSummaryWriter GetLorryHEVNonOVCSummaryWriter();
		ICifSummaryWriter GetLorryHEVOVCSummaryWriter();
		ICifSummaryWriter GetLorryPEVSummaryWriter();


		IResultGroupWriter GetBusSimulationParameterWriter();
		IResultGroupWriter GetBusOVCResultWriterChargeDepleting();
		IResultGroupWriter GetBusOVCResultWriterChargeSustaining();
		IResultGroupWriter GetBusOVCTotalWriter();

		IFuelConsumptionWriter GetFuelConsumptionBus();
		IElectricEnergyConsumptionWriter GetElectricEnergyConsumptionBus();
		ICO2Writer GetCO2ResultBus();

		ICifSummaryWriter GetBusOVCCifSummaryWriter();


		IElectricRangeWriter GetElectricRangeWriter();

	}



}