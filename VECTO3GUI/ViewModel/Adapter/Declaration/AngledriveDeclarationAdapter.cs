using System;
using System.Collections.ObjectModel;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using VECTO3.Util;
using VECTO3.ViewModel.Interfaces;

namespace VECTO3.ViewModel.Adapter.Declaration {
	public class AngledriveDeclarationAdapter : AbstractDeclarationAdapter, IAngledriveInputData
	{
		protected IAngledriveViewModel ViewModel;
		public AngledriveDeclarationAdapter(IAngledriveViewModel angledriveViewModel) :base(angledriveViewModel)
		{
			ViewModel = angledriveViewModel;
		}

		#region Implementation of IComponentInputData

		public DataSource DataSource { get; }
		public DateTime Date { get; }
		public string AppVersion { get; }
		public CertificationMethod CertificationMethod { get { return ViewModel.CertificationMethod; } }
		
		#endregion

		#region Implementation of IAngledriveInputData

		public AngledriveType Type { get { return ViewModel.AngledriveType; } }
		public double Ratio { get { return ViewModel.Ratio; } }

		public TableData LossMap
		{
			get { return TableDataConverter.Convert(ViewModel.LossMap); }
		}

		public double Efficiency { get { throw new NotImplementedException(); } }

		#endregion
	}
}