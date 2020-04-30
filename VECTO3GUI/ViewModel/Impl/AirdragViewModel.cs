using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Castle.Core.Internal;
using Ninject;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI.Helper;
using VECTO3GUI.Model.TempDataObject;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Impl
{
	public enum AirdragConfig
	{
		WithoutAirdrag,
		UseMeasurementData,
		Unknown
	}

	public static class AirdragConfigHelper
	{
		public static string GetLabel(this AirdragConfig airdrag)
		{
			switch (airdrag)
			{
				case AirdragConfig.WithoutAirdrag:
					return "Without Airdrag";
				case AirdragConfig.UseMeasurementData:
					return "Use Measurement Data";
			}
			return string.Empty;
		}

		public static AirdragConfig GetAirdragConfig(string name)
		{
			switch (name)
			{
				case "Without Airdrag":
					return AirdragConfig.WithoutAirdrag;
				case "Use Measurement Data":
					return AirdragConfig.UseMeasurementData;
			}
			return AirdragConfig.Unknown;
		}
	}


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
		private bool _isEditable;
		private bool _useMeasurementData;
		private bool _noAirdragData;


		private ICommand _airdragConfig;
		private ICommand _loadFileCommand;


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

		public DigestData DigestValue { get; set; }

		#endregion

		public bool IsEditable
		{
			get { return _isEditable; }
			set { SetProperty(ref _isEditable, value); }
		}

		public bool UseMeasurementData
		{
			get { return _useMeasurementData; }
			set { SetProperty(ref _useMeasurementData, value); }
		}

		public bool NoAirdragData
		{
			get { return _noAirdragData; }
			set { SetProperty(ref _noAirdragData, value); }
		}


		protected override void InputDataChanged()
		{
			var inputData = JobViewModel.InputDataProvider as IDeclarationInputDataProvider;
			_airdragData = inputData?.JobInputData.Vehicle.Components.AirdragInputData;
			SetAirdragValues(_airdragData);
			UseMeasurementData = _airdragData != null;
			NoAirdragData = !UseMeasurementData;
			IsEditable = false;
		}

		private void SetAirdragValues(IAirdragDeclarationInputData airdrag)
		{
			UseMeasuredValues = airdrag != null;
			if (airdrag == null)
			{
				_componentData = new AirdragComponentData(this, true);
				return;
			}

			Model = airdrag.Model;
			Manufacturer = airdrag.Manufacturer;
			CertificationNumber = airdrag.CertificationNumber;
			Date = airdrag.Date;
			AppVersion = airdrag.AppVersion;
			DeclaredCdxA = airdrag.AirDragArea;
			CdxA_0 = DeclaredCdxA;
			TransferredCdxA = DeclaredCdxA;
			DigestValue = airdrag.DigestValue;

			_componentData = new AirdragComponentData(this);
			ClearChangedProperties();
		}

		#region Commands

		public ICommand AirdragConfigCommand
		{
			get
			{
				return _airdragConfig ?? (_airdragConfig = new RelayCommand<AirdragConfig>(DoAirdragConfig));
			}
		}

		private void DoAirdragConfig(AirdragConfig config)
		{
			
		}


		public ICommand LoadFileCommand
		{
			get
			{
				return _loadFileCommand ?? (_loadFileCommand = new RelayCommand(DoLoadFile, CanLoadFile));
			}
		}

		private bool CanLoadFile()
		{
			return UseMeasurementData;
		}

		private void DoLoadFile()
		{
			var filePath = FileDialogHelper.ShowSelectFilesDialog(false)?.FirstOrDefault();
			ReadSelectedXml(filePath);
		}
		#endregion


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

		private void ReadSelectedXml(string filePath)
		{
			if (filePath.IsNullOrEmpty())
				return;
			
			var xmlDocument = XmlHelper.ReadXmlDocument(filePath);
			var nodes = XmlHelper.GetComponentNodes(xmlDocument,
				XMLNames.VectoInputDeclaration, VectoComponents.Airdrag.XMLElementName());

			if(nodes.IsNullOrEmpty())
				return;

			var compReader = new XmlComponentReaderHelper(nodes[0].ParentNode);
			SetLoadedAirdragData(compReader.GetAirdragComponentData());
		}

		private void SetLoadedAirdragData(AirdragComponentData airdrag)
		{
			if (airdrag == null)
				return;

			Model = airdrag.Model;
			Manufacturer = airdrag.Manufacturer;
			CertificationNumber = airdrag.CertificationNumber;
			Date = airdrag.Date;
			AppVersion = airdrag.AppVersion;
			DeclaredCdxA = airdrag.DeclaredCdxA;
			CdxA_0 = airdrag.CdxA_0;
			TransferredCdxA = airdrag.TransferredCdxA;
			DigestValue = airdrag.DigestValue;
		}
	}
}
