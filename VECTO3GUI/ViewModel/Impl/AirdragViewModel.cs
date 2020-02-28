using System;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Impl
{
	public class AirdragViewModel : AbstractComponentViewModel, IAirdragViewModel
	{
		private SquareMeter _cdxA0;
		private SquareMeter _transferredCdxA;
		private SquareMeter _declaredCdxA;
		private bool _useStandardValues;
		private string _appVersion;

		#region Implementation of IAirdragViewModel

		public IAirdragDeclarationInputData ModelData {
			get { return AdapterFactory.AirdragDeclarationAdapter(this); }
		}

		public bool UseMeasuredValues
		{
			get { return _useStandardValues; }
			set { SetProperty(ref _useStandardValues, value); }
		}

		public SquareMeter CdxA_0
		{
			get { return _cdxA0; }
			set { SetProperty(ref _cdxA0, value); }
		}

		public SquareMeter TransferredCdxA
		{
			get { return _transferredCdxA; }
			set { SetProperty(ref _transferredCdxA, value); }
		}

		public SquareMeter DeclaredCdxA
		{
			get { return _declaredCdxA; }
			set { SetProperty(ref _declaredCdxA, value); }
		}

		public string AppVersion
		{
			get { return _appVersion; }
			set { SetProperty(ref _appVersion, value); }
		}


		#endregion

		protected override void InputDataChanged()
		{
			JobViewModel.InputDataProvider.Switch()
						.If<IDeclarationInputDataProvider>(d => SetValues(d.JobInputData.Vehicle.Components.AirdragInputData))
						.If<IEngineeringInputDataProvider>(e => SetValues(e.JobInputData.Vehicle.Components.AirdragInputData));
		}

		private void SetValues(IAirdragDeclarationInputData airdrag)
		{
			UseMeasuredValues = airdrag != null;
			if (airdrag == null) {
				return;
			}
			
			Model = airdrag.Model;
			Manufacturer = airdrag.Manufacturer;
			CertificationNumber = airdrag.CertificationNumber;
			Date = airdrag.Date;
			AppVersion = airdrag.AppVersion;
			DeclaredCdxA = airdrag.AirDragArea;
		}
	}
}
