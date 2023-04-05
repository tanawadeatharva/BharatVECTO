using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;

namespace VECTO3GUI2020.Resources.XML
{
	public static class XMLTypes
	{
		#region Vehicle

		public const string Vehicle_Conventional_CompletedBusDeclarationType =
			XMLDeclarationConventionalCompletedBusDataProviderV24.XSD_TYPE;
		public const string Vehicle_Hev_CompletedBusDeclarationType =
			XMLDeclarationHevCompletedBusDataProviderV24.XSD_TYPE;
		public const string Vehicle_Pev_CompletedBusDeclarationType =
			XMLDeclarationPEVCompletedBusDataProviderV24.XSD_TYPE;
		public const string Vehicle_Iepc_CompletedBusDeclarationType =
			XMLDeclarationConventionalCompletedBusDataProviderV24.XSD_TYPE;

		public const string Vehicle_Exempted_CompletedBusDeclarationType =
			XMLDeclarationExemptedCompletedBusDataProviderV24.XSD_TYPE;


        #endregion

        #region Components
        public const string Components_xEV_CompletedBusType = "Components_xEV_CompletedBusType";
		public const string Components_Conventional_CompletedBusType = "Components_Conventional_CompletedBusType";
		#endregion 


		#region Airdrag
		public const string AirDragDataDeclarationType = "AirDragDataDeclarationType";
		public const string AirDragModifiedUseStandardValueType = "AirDragModifiedUseStandardValueType";

        #endregion


        #region BusAux

        public const string AUX_Conventional_CompletedBusType = "AUX_Conventional_CompletedBusType";
		public const string AUX_xEV_CompletedBusType = "AUX_xEV_CompletedBusType";

        #endregion
    }
}

