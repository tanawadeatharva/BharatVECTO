using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using VECTO3GUI2020.ViewModel.Implementation.Common;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
    public class InterimStageViewModel : ViewModelBase, IManufacturingStageViewModel
	{
		private DigestData _hashPreviousStage;
		private int _stageCount;
		private IVehicleDeclarationInputData _vehicle;
		private IApplicationInformation _applicationInformation;
		private DigestData _signature;

		public DigestData HashPreviousStage => _hashPreviousStage;

		public int StageCount => _stageCount;

		public IVehicleDeclarationInputData Vehicle => _vehicle;

		public IApplicationInformation ApplicationInformation => _applicationInformation;

		public DigestData Signature => _signature;
	}
}
