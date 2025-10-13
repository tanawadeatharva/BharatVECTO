using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ninject;
using NUnit.Framework;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using Vecto3GUI2020Test.Utils;

namespace Vecto3GUI2020Test.XML.XMLWritingTest;

[TestFixture]
public class XMLCompleteBus
{
	public const string BASEDIR = @"TestData/Integration/Multistage/FactorMethod";


	private ThreadLocal<IKernel> _kernel;
	private IKernel Kernel => _kernel.Value;




	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new ThreadLocal<IKernel>(TestHelper.GetKernel);
		



	}
	




    [TestCase("CompletedBus_41-32b_AT-P.vecto")]
	[TestCase("CompletedBus_42-33b.vecto")]
    public async Task LoadCompletedBus(string completedJob)
	{
		var filePath = Path.GetFullPath(Path.Combine(BASEDIR, completedJob));
		Assert.IsTrue(File.Exists(filePath), "missing file " + filePath);
		var jobList = Kernel.Get<IJobListViewModel>();
		var doc = await jobList.AddJobAsync(filePath);
		Assert.NotNull(doc);
		var editingViewModel = doc.EditViewModel as MultiStageJobViewModel_v0_1;


		var md = Kernel.GetMockDialogHelper();
		md.PushFileName("test.xml");
		editingViewModel.SaveVIFCommand.Execute(null);


		md.AssertNoErrorDialogs();


	}





}