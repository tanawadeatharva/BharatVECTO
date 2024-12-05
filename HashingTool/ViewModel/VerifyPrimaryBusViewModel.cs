/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using HashingTool.Helper;
using HashingTool.ViewModel.UserControl;

namespace HashingTool.ViewModel
{
	public class VerifyPrimaryBusViewModel : ObservableObject, IMainView
	{
		private readonly VectoJobFile _jobFile;
		private readonly VehicleInformationXMLFile _vifFile;
		private readonly ManufacturerReportXMLFile _manufacturerReport;

		public VerifyPrimaryBusViewModel()
		{
			_jobFile = new VectoJobFile(
				"Primary Bus Job File",
				HashingHelper.IsPrimaryJobFile,
				HashingHelper.HashJobFile);
			_manufacturerReport = new ManufacturerReportXMLFile(
				"Primary Bus Manufacturer Report",
				HashingHelper.IsPrimaryManufacturerFile,
				HashingHelper.ValidateDocumentHash);
			_manufacturerReport.JobData = _jobFile;
			_vifFile = new VehicleInformationXMLFile(
				"Primary Bus VIF",
				HashingHelper.IsPrimaryVifFile,
				TUGraz.VectoCore.Utils.XmlDocumentType.MultistepOutputData,
				HashingHelper.ValidateDocumentHash);
			_vifFile.JobData = _jobFile;

			Files = new ObservableCollection<VectoXMLFile> { _jobFile, _manufacturerReport, _vifFile };

			ErrorsAndWarnings = new CompositeCollection();

			AddErrorCollection(_jobFile.XMLFile.XMLValidationErrors);
			AddErrorCollection(_manufacturerReport.XMLFile.XMLValidationErrors);
			AddErrorCollection(_manufacturerReport.ValidationErrors);
			AddErrorCollection(_vifFile.XMLFile.XMLValidationErrors);
			AddErrorCollection(_vifFile.ValidationErrors);

			RaisePropertyChanged("CanonicalizationMethods");
		}

		private void AddErrorCollection(ObservableCollection<string> errorCollection)
		{
			ErrorsAndWarnings.Add(new CollectionContainer { Collection = errorCollection });
			errorCollection.CollectionChanged += UpdateCount;
		}

		private void UpdateCount(object sender, NotifyCollectionChangedEventArgs e)
		{
			RaisePropertyChanged("ErrorCount");
		}

		public int ErrorCount
		{
			get { return ErrorsAndWarnings.Cast<CollectionContainer>().Sum(entry => (entry.Collection as ICollection).Count); }
		}

		public string Name => "Verify Primary Bus Data";

		public ICommand ShowHomeViewCommand => ApplicationViewModel.HomeView;

		public VectoJobFile JobFile => _jobFile;

		public ManufacturerReportXMLFile ManufacturerReport => _manufacturerReport;

		public VehicleInformationXMLFile VIFReport => _vifFile;

		public ObservableCollection<VectoXMLFile> Files { get; private set; }

		public CompositeCollection ErrorsAndWarnings { get; private set; }
	}
}
