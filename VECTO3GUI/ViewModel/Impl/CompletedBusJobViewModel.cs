using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace VECTO3GUI.ViewModel.Impl
{
	public class CompletedBusJobViewModel: AbstractBusJobViewModel
	{
		public CompletedBusJobViewModel(IKernel kernel, JobType jobType):base(kernel, jobType)
		{
			FirstLabelText = $"Select {JobFileType.PIFBusFile.GetLable()}";
		}
	}
}
