/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
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

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using HashingTool.Helper;
using HashingTool.ViewModel.UserControl;
using TUGraz.VectoHashing;

namespace HashingTool.ViewModel
{
	public class VerifyResultDataViewModel : ObservableObject, IMainView
	{
		private readonly VectoJobFile _jobFile;
		private readonly ReportXMLFile _customerReport;
		private readonly ManufacturerReportXMLFile _manufacturerReport;


		public VerifyResultDataViewModel()
		{
			_jobFile = new VectoJobFile("Job File", HashingHelper.IsJobFile, HashingHelper.HashJobFile);
			_manufacturerReport = new ManufacturerReportXMLFile("Manufacturer Report", HashingHelper.IsManufacturerReport,
				HashingHelper.ValidateDocumentHash);
			_manufacturerReport.JobData = _jobFile;
			_customerReport = new ReportXMLFile("Customer Report", HashingHelper.IsCustomerReport,
				HashingHelper.ValidateDocumentHash);
			_customerReport.JobData = _jobFile;
			Files = new ObservableCollection<VectoXMLFile> { _jobFile, _manufacturerReport, _customerReport };

			ErrorsAndWarnings = new CompositeCollection();

			AddErrorCollection(_jobFile.XMLFile.XMLValidationErrors);
			AddErrorCollection(_manufacturerReport.XMLFile.XMLValidationErrors);
			AddErrorCollection(_customerReport.XMLFile.XMLValidationErrors);
			AddErrorCollection(_manufacturerReport.ValidationErrors);
			AddErrorCollection(_customerReport.ValidationErrors);

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


		public string Name
		{
			get { return "Verify Result Data"; }
		}

		public ICommand ShowHomeViewCommand
		{
			get { return ApplicationViewModel.HomeView; }
		}

		public VectoJobFile JobFile
		{
			get { return _jobFile; }
		}


		public ReportXMLFile CustomerReport
		{
			get { return _customerReport; }
		}

		public ManufacturerReportXMLFile ManufacturerReport
		{
			get { return _manufacturerReport; }
		}

		public ObservableCollection<VectoXMLFile> Files { get; private set; }

		public CompositeCollection ErrorsAndWarnings { get; private set; }
	}
}
