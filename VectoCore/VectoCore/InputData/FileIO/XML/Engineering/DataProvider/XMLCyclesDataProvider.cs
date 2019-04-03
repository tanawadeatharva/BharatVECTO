using System.Collections.Generic;
using System.IO;
using System.Xml;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLCyclesDataProviderV07 : AbstractXMLType, IXMLCyclesDataProvider
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		protected string BasePath;

		public XMLCyclesDataProviderV07(IEngineeringJobInputData jobData, XmlNode baseNode, string basePath) : base(baseNode)
		{
			BasePath = basePath;
		}


		public IList<ICycleData> Cycles
		{
			get {
				var retVal = new List<ICycleData>();
				var cycleNodes = GetNodes(XMLNames.Missions_Cycle);
				if (cycleNodes == null || cycleNodes.Count == 0) {
					return retVal;
				}

				foreach (XmlNode cycleNode in cycleNodes) {
					var file = cycleNode.Attributes?.GetNamedItem(XMLNames.ExtResource_File_Attr).InnerText;
					if (string.IsNullOrWhiteSpace(file)) {
						continue;
					}

					var fullPath = Path.Combine(BasePath ?? "", file);
					if (File.Exists(fullPath)) {
						retVal.Add(
							new CycleInputData() {
								Name = Path.GetFileNameWithoutExtension(fullPath),
								CycleData = VectoCSVFile.Read(fullPath)
							});
					} else {
						try {
							var resourceName = DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." + file +
												Constants.FileExtensions.CycleFile;
							retVal.Add(
								new CycleInputData() {
									Name = Path.GetFileNameWithoutExtension(file),
									CycleData = VectoCSVFile.ReadStream(RessourceHelper.ReadStream(resourceName), source: resourceName),
								});
						} catch {
							//Log.Debug("Driving Cycle could not be read: " + cycleFile);
							throw new VectoException("Driving Cycle could not be read: " + file);
						}
					}
				}

				return retVal;
			}
		}
	}


	internal class XMLCyclesDataProviderV10 : XMLCyclesDataProviderV07
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public XMLCyclesDataProviderV10(IEngineeringJobInputData jobData, XmlNode baseNode, string basePath) : base(
			jobData, baseNode, basePath) { }
	}
}
