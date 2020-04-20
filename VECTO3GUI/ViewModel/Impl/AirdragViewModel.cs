using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;
using VECTO3GUI.Model.TempDataObject;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Impl
{
	public class AirdragViewModel : AbstractComponentViewModel, IAirdragViewModel
	{
		private string _manufacturer;
		private string _model;
		private string _certificationNumber;
		private DateTime? _date;
		private SquareMeter _declaredCdxA;
		private bool _useStandardValues;
		private string _appVersion;

		private IAirdragDeclarationInputData _airdragData;
		private AirdragComponentData _componentData;

		#region Implementation of IAirdragViewModel

		public IAirdragDeclarationInputData ModelData
		{
			get { return AdapterFactory.AirdragDeclarationAdapter(this); }
		}

		public bool UseMeasuredValues
		{
			get { return _useStandardValues; }
			set { SetProperty(ref _useStandardValues, value); }
		}

		public SquareMeter CdxA_0 { get; set; }
		public SquareMeter TransferredCdxA { get; set; }

		public string Manufacturer
		{
			get { return _manufacturer; }
			set
			{
				if (!SetProperty(ref _manufacturer, value))
					return;
				IsDataChanged(_manufacturer, _componentData);
			}
		}

		public string Model
		{
			get { return _model; }
			set
			{
				if (!SetProperty(ref _model, value))
					return;
				IsDataChanged(_model, _componentData);
			}
		}

		public string CertificationNumber
		{
			get { return _certificationNumber; }
			set
			{
				if (!SetProperty(ref _certificationNumber, value))
					return;
				IsDataChanged(_certificationNumber, _componentData);
			}
		}

		public DateTime? Date
		{
			get { return _date; }
			set
			{
				if (!SetProperty(ref _date, value))
					return;

				IsDataChanged(_date, _componentData);
			}
		}

		public SquareMeter DeclaredCdxA
		{
			get { return _declaredCdxA; }
			set
			{
				if (!SetProperty(ref _declaredCdxA, value))
					return;

				IsDataChanged(_declaredCdxA, _componentData);
				SetAirdragArea(_airdragData);
			}
		}

		public string AppVersion
		{
			get { return _appVersion; }
			set
			{
				if (SetProperty(ref _appVersion, value))
					return;
				IsDataChanged(_appVersion, _componentData);
			}
		}


		#endregion

		protected override void InputDataChanged()
		{
			var inputData = JobViewModel.InputDataProvider as IDeclarationInputDataProvider;
			_airdragData = inputData?.JobInputData.Vehicle.Components.AirdragInputData;
			SetAirdragValues(_airdragData);
		}

		private void SetAirdragValues(IAirdragDeclarationInputData airdrag)
		{
			UseMeasuredValues = airdrag != null;
			if (airdrag == null) {
				_componentData = new AirdragComponentData(this, true);
				return;
			}

			Model = airdrag.Model;
			Manufacturer = airdrag.Manufacturer;
			CertificationNumber = airdrag.CertificationNumber;
			Date = airdrag.Date;
			AppVersion = airdrag.AppVersion;
			SetAirdragArea(airdrag);

			_componentData = new AirdragComponentData(this);
			ClearChangedProperties();
		}

		private void SetAirdragArea(IAirdragDeclarationInputData airdrag)
		{
			DeclaredCdxA = airdrag.AirDragArea;
			CdxA_0 = DeclaredCdxA;
			TransferredCdxA = DeclaredCdxA;
		}

		public override void ResetComponentData()
		{
			_componentData.ResetToComponentValues(this);
		}

		public override object SaveComponentData()
		{
			_componentData.UpdateCurrentValues(this);
			ClearChangedProperties();
			return _componentData;
		}
	}
}
