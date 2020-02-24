using System;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Impl
{
	public class TyreViewModel : AbstractComponentViewModel, ITyreViewModel
	{
		private string _dimension;
		private double _rollingResistanceCoefficient;
		private Newton _fzIso;

		#region Implementation of ITyreViewModel

		public ITyreDeclarationInputData ModelData { get { return AdapterFactory.TyreDeclarationAdapter(this); } }

		public string Dimension
		{
			get { return _dimension; }
			set { SetProperty(ref _dimension, value); }
		}

		public AllowedEntry<string>[] AllowedDimensions
		{
			get { return DeclarationData.Wheels.GetWheelsDimensions().Select(w => AllowedEntry.Create(w, w)).ToArray(); }
		}

		public double RollingResistanceCoefficient
		{
			get { return _rollingResistanceCoefficient; }
			set { SetProperty(ref _rollingResistanceCoefficient, value); }
		}

		public Newton FzISO
		{
			get { return _fzIso; }
			set { SetProperty(ref _fzIso, value); }
		}

		#endregion

		public void SetValues(ITyreDeclarationInputData tyre)
		{
			Manufacturer = tyre.Manufacturer;
			Model = tyre.Model;
			CertificationNumber = tyre.CertificationNumber;
			//ToDo
			//Date = DateTime.Parse(tyre.Date ?? "1/1/1970");

			Dimension = tyre.Dimension;
			RollingResistanceCoefficient = tyre.RollResistanceCoefficient;
			FzISO = tyre.TyreTestLoad;
		}
	}
}
