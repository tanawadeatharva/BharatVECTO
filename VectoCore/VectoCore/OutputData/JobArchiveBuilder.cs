using System.IO.Compression;
using System.IO;

using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.Impl;

namespace TUGraz.VectoCore.OutputData
{
    public class JobArchiveBuilder : IJobArchiveBuilder
    {
        private const string _prefix = "JobArchive_";
        private const string _extension = ".zip";

        public JobArchiveBuilder() {}

        public ISimulatorFactory SimulatorFactory { get; set; }

        public void Build()
        { 
            if ((SimulatorFactory == null) || !IsJobSupported()) {
                return;
            }

            var vsumWriter = SimulatorFactory.SumData.SummaryWriter as FileOutputWriter;
            if (vsumWriter == null) {
                return;
            }

            var archiveFileName = _prefix + Path.GetFileNameWithoutExtension(vsumWriter.SumFileName) + _extension;
            var archivePath = Path.Combine(vsumWriter.BasePath, archiveFileName);

            using (var stream = new FileStream(archivePath, FileMode.Create))
            {
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true)) 
                {
                    WriteOutputFilesToZipArchive(archive);
                    WriteInputFilesToZipArchive(archive);
                }
            }
        }

        private bool IsJobSupported()
        {
            return (SimulatorFactory.RunDataFactory is DeclarationVTPModeVectoRunDataFactoryLorries)
                || (SimulatorFactory.RunDataFactory is DeclarationVTPModeVectoRunDataFactoryHeavyBusPrimary);
        }

        private void WriteOutputFilesToZipArchive(ZipArchive archive)
        { 
            var vsumWriter = SimulatorFactory.SumData.SummaryWriter as FileOutputWriter;
            var modWriter = SimulatorFactory.ReportWriter as FileOutputWriter;

            if ((vsumWriter == null) || (modWriter == null)) {
                return;
            }

            WriteFileToZipArchive(vsumWriter.XMLVTPReportName, archive);
            WriteFileToZipArchive(vsumWriter.SumFileName, archive);
                    
            modWriter.ModDataFiles.ForEach(fileName => WriteFileToZipArchive(fileName, archive));
        }

        private void WriteInputFilesToZipArchive(ZipArchive archive)
        {
            var inputDataProvider = SimulatorFactory.RunDataFactory.DataProvider;
            if (inputDataProvider == null) {
                return;
            }

            WriteFileToZipArchive(inputDataProvider.DataSource.SourceFile, archive);
             
            var vtpDeclarationProvider = inputDataProvider as IVTPDeclarationInputDataProvider;

            if (vtpDeclarationProvider != null) {
                WriteVTPDeclarationInputFilesToZipArchive(vtpDeclarationProvider, archive);
            }   
        }

        private void WriteVTPDeclarationInputFilesToZipArchive(IVTPDeclarationInputDataProvider vtpProvider, ZipArchive archive)
        {
            var inputDataProvider = SimulatorFactory.RunDataFactory.DataProvider;
            
            var manufacturerRecord = vtpProvider.JobInputData.ManufacturerReportInputData.Source;
            if (manufacturerRecord != null) {
                var filePath = Path.Combine(inputDataProvider.DataSource.SourcePath, manufacturerRecord);
                WriteFileToZipArchive(filePath, archive);
            }

            var completedVIF = vtpProvider.JobInputData.CompletedVIFInputData?.Source;
            if (completedVIF != null) {
				var filePath = Path.Combine(inputDataProvider.DataSource.SourcePath, completedVIF);
				WriteFileToZipArchive(filePath, archive);
			}

            var declarationVehicle = vtpProvider.JobInputData.Vehicle.DataSource.SourceFile;
            if (declarationVehicle != null) {
                WriteFileToZipArchive(declarationVehicle, archive);
            }

            foreach(var cycle in vtpProvider.JobInputData.Cycles) {
                var fileName = $"{cycle.Name}{Constants.FileExtensions.CycleFile}";
                var filePath = Path.Combine(inputDataProvider.DataSource.SourcePath, fileName);
                WriteFileToZipArchive(filePath, archive);
            }
        }

        private void WriteFileToZipArchive(string filePath, ZipArchive archive)
        {
            if (!File.Exists(filePath)) {
                return;
            }

            ZipArchiveEntry entry = archive.CreateEntry(Path.GetFileName(filePath));

            using (StreamWriter writerManifest = new StreamWriter(entry.Open()))
            {
                writerManifest.Write(File.ReadAllText(filePath));
            }
        }

    }
}
