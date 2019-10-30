using System.Collections.Generic;

namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public interface ISSMTechList
	{
		IReadOnlyList<ISSMTechnology> TechLines { get; set; }

		//void Clear();
		//ITechListBenefitLine Find(string category, string benefitName);
		//bool Add(ITechListBenefitLine item, ref string feedback);
		//bool Delete(ITechListBenefitLine item, ref string feedback);
		//bool Modify(ITechListBenefitLine originalItem, ITechListBenefitLine modifiedItem, ref string feedback);

		//void SetSSMGeneralInputs(ISSMInputs genInputs);

		double HValueVariation { get; }
		double VHValueVariation { get; }
		double VVValueVariation { get; }
		double VCValueVariation { get; }
		double CValueVariation { get; }

		//double HValueVariationKW { get; }
		//double VHValueVariationKW { get; }
		//double VVValueVariationKW { get; }
		//double VCValueVariationKW { get; }
		//double CValueVariationKW { get; }


		//bool Initialise();
		//bool Initialise(string filePath);
	}
}
