using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using VECTO3.ViewModel.Interfaces;

namespace VECTO3.ViewModel.Adapter.Declaration
{
	public abstract class AbstractDeclarationAdapter
	{
		private ICommonComponentParameters _vm;

		public AbstractDeclarationAdapter(ICommonComponentParameters vm)
		{
			_vm = vm;
		}

		public string Manufacturer { get { return _vm.Manufacturer; } }
		public string Model { get { return _vm.Model; } }
		public string Date { get { return _vm.Date.ToString(); } }
		public bool SavedInDeclarationMode { get { return true; } }
		public string CertificationNumber { get { return _vm.CertificationNumber; } }
		public DataSourceType SourceType { get { return DataSourceType.Embedded; } }
		public DigestData DigestValue { get { return null;} }
	}
}
