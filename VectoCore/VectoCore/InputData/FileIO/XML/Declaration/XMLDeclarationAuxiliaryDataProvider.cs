using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Resources;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLDeclarationAuxiliaryDataProvider : AbstractDeclarationXMLComponentDataProvider,
		IAuxiliariesDeclarationInputData
	{
		public XMLDeclarationAuxiliaryDataProvider(XMLInputDataProvider xmlInputDataProvider) : base(xmlInputDataProvider)
		{
			XBasePath = Helper.Query(VehiclePath,
				XMLNames.Vehicle_Components,
				XMLNames.Component_Auxiliaries,
				XMLNames.ComponentDataWrapper);
		}


		public IList<IAuxiliaryDeclarationInputData> Auxiliaries
		{
			get
			{
				var retVal = new List<IAuxiliaryDeclarationInputData>();
				var auxiliaries = Navigator.Select(Helper.Query(XBasePath,
					Helper.QueryConstraint("*",
						Helper.NSPrefix(XMLNames.Auxiliaries_Auxiliary_Technology), null, "")),
					Manager);

				while (auxiliaries.MoveNext()) {
					var techlistNodes = auxiliaries.Current.Select(Helper.NSPrefix(XMLNames.Auxiliaries_Auxiliary_Technology), Manager);
					var technologyList = new List<string>();
					while (techlistNodes.MoveNext()) {
						technologyList.Add(techlistNodes.Current.Value);
					}
					retVal.Add(new AuxiliaryDataInputData {
						Type = auxiliaries.Current.Name.ParseEnum<AuxiliaryType>(),
						Technology = technologyList,
					});
				}
				return retVal;
			}
		}
	}
}