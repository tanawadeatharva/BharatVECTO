using System;
using System.Diagnostics;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components
{
    public abstract class AirDragViewModel : ViewModelBase, IComponentViewModel, IAirDragViewModel
    {
        private static readonly string _name = "AirDrag";

        protected IAirdragDeclarationInputData _inputData;

		private ICommonComponentViewModel _commonComponentViewModel;
		public ICommonComponentViewModel CommonComponentViewModel
		{
			get => _commonComponentViewModel;
			set => SetProperty(ref _commonComponentViewModel, value);
		}








        public AirDragViewModel(IXMLAirdragDeclarationInputData inputData, IComponentViewModelFactory vmFactory)
        {
			_inputData = inputData as IAirdragDeclarationInputData;
            Debug.Assert(_inputData != null);
            _isPresent = (_inputData?.DataSource?.SourceFile != null);

            _commonComponentViewModel = vmFactory.CreateCommonComponentViewModel(_inputData);
			SetProperties();
		}

		public abstract void SetProperties();

        public string Name { get { return _name; } }
		private bool _isPresent;
		public bool IsPresent { get { return _isPresent; } }

		#region Implementation of IAirDragDeclarationInputData
		
		protected SquareMeter _airDragArea;
		public DataSource DataSource
		{
			get => _commonComponentViewModel.DataSource;
			set => _commonComponentViewModel.DataSource = value;
		}

		public string Manufacturer
		{
			get => _commonComponentViewModel.Manufacturer;
			set => _commonComponentViewModel.Manufacturer = value;
		}

		public string Model
		{
			get => _commonComponentViewModel.Model;
			set => _commonComponentViewModel.Model = value;
		}

		public DateTime Date
		{
			get => _commonComponentViewModel.Date;
			set => _commonComponentViewModel.Date = value;
		}

		public string CertificationNumber
		{
			get => _commonComponentViewModel.CertificationNumber;
			set => _commonComponentViewModel.CertificationNumber = value;
		}

		public CertificationMethod CertificationMethod
		{
			get => _commonComponentViewModel.CertificationMethod;
			set => _commonComponentViewModel.CertificationMethod = value;
		}

		public bool SavedInDeclarationMode
		{
			get => _commonComponentViewModel.SavedInDeclarationMode;
			set => _commonComponentViewModel.SavedInDeclarationMode = value;
		}

		public DigestData DigestValue
		{
			get => _commonComponentViewModel.DigestValue;
			set => _commonComponentViewModel.DigestValue = value;
		}

		public string AppVersion => _commonComponentViewModel.AppVersion;


		public virtual SquareMeter AirDragArea {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public SquareMeter TransferredAirDragArea { get => throw new NotImplementedException(); }
		public SquareMeter AirDragArea_0 { get => throw new NotImplementedException(); }

		#endregion
	}


    public class AirDragViewModel_v1_0 : AirDragViewModel
    {
        public static readonly string VERSION = typeof(XMLDeclarationAirdragDataProviderV10).FullName;
        public AirDragViewModel_v1_0(IXMLAirdragDeclarationInputData inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory)
        {

        }

		public override void SetProperties()
		{
			throw new NotImplementedException();
		}
	}

    public class AirDragViewModel_v2_0 : AirDragViewModel_v1_0
    {
        public static new readonly string VERSION = typeof(XMLDeclarationAirdragDataProviderV20).FullName;

		public AirDragViewModel_v2_0(IXMLAirdragDeclarationInputData inputData, IComponentViewModelFactory vmFactory) : base(inputData, vmFactory)
        {

        }

		public override void SetProperties()
		{
			_airDragArea = _inputData.AirDragArea;
		}

		public override SquareMeter AirDragArea
		{
			get => _airDragArea;
			set => SetProperty(ref _airDragArea, value);
		}
	}
}
