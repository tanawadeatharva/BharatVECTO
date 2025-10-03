using System.Data;

namespace TUGraz.VectoCore.Models.Declaration
{
    public class PrimaryAndCompletedGroups : LookupData<string, PrimaryAndCompletedGroups.PrimaryAndCompletedGroupsData>
    {
        protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".PrimaryAndCompletedGroups.csv";
        protected override string ErrorMessage => "PrimaryAndCompletedGroups Lookup Error: no value found. Key: '{0}'";

        public struct PrimaryAndCompletedGroupsData 
        {
            public string PrimaryGroup;
            public string CompletedGroup;
        }

        protected override void ParseData(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                var val = new PrimaryAndCompletedGroupsData()
                {
                    CompletedGroup = row[nameof(PrimaryAndCompletedGroupsData.CompletedGroup)].ToString(),
                    PrimaryGroup = row[nameof(PrimaryAndCompletedGroupsData.PrimaryGroup)].ToString()
                };

                Data.Add(val.CompletedGroup, val);
            }
        }

    }
}
