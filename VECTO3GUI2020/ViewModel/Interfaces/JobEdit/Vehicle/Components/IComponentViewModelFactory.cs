using System;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using VECTO3GUI2020.Ninject.Factories;

namespace VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components
{
	public interface IComponentViewModelFactory
    {
        /// <summary>
        /// Creates a ViewModel for a component.
        /// </summary>
        /// <param name="inputData">The Type of the input Data is used to resolve a Named Binding and is also passed to the constructor</param>
        /// <returns></returns>
		IComponentViewModel CreateComponentViewModel(IComponentInputData inputData);
        IVehicleViewModel CreateVehicleViewModel(IVehicleDeclarationInputData inputData);
        IAdasViewModel CreateAdasViewModel(IAdvancedDriverAssistantSystemDeclarationInputData inputData);
        IEngineModeViewModel CreateEngineModeViewModel(IEngineModeDeclarationInputData inputData);
        IEngineFuelViewModel CreateEngineFuelViewModel(IEngineFuelDeclarationInputData inputData);
        IComponentsViewModel CreateComponentsViewModel(IXMLVehicleComponentsDeclaration inputData);
        ICommonComponentViewModel CreateCommonComponentViewModel(IComponentInputData inputData);


		IComponentViewModel CreateComponentViewModel(ITransmissionInputData inputData);
		IComponentViewModel CreateComponentViewModel(IPTOTransmissionInputData inputData);
		IComponentViewModel CreateComponentViewModel(IAxlesDeclarationInputData inputData);
		IComponentViewModel CreateComponentViewModel(IAxleDeclarationInputData inputData);
		IComponentViewModel CreateComponentViewModel(IAuxiliariesDeclarationInputData inputData);
		IComponentViewModel CreateComponentViewModel(IAuxiliaryDeclarationInputData inputData);



	}

	public class ComponentViewModelFactory : IComponentViewModelFactory
	{
		private readonly IComponentViewModelFactoryInternal _internalFactory;

		#region Implementation of IComponentViewModelFactory

		public ComponentViewModelFactory(IComponentViewModelFactoryInternal internalFactory)
		{
			_internalFactory = internalFactory;
		}

		public IComponentViewModel CreateComponentViewModel(IComponentInputData inputData)
		{
			return CreateComponentViewModel(inputData.DataSource, inputData);
		}


		private IComponentViewModel CreateComponentViewModel<TIn>(DataSource dataSource, TIn inputData)
		{
			return _internalFactory.CreateComponentViewModel(dataSource, inputData);
		}

		public IVehicleViewModel CreateVehicleViewModel(IVehicleDeclarationInputData inputData)
		{
			throw new NotImplementedException();
		}

		public IAdasViewModel CreateAdasViewModel(IAdvancedDriverAssistantSystemDeclarationInputData inputData)
		{
			throw new NotImplementedException();
		}

		public IEngineModeViewModel CreateEngineModeViewModel(IEngineModeDeclarationInputData inputData)
		{
			throw new NotImplementedException();
		}

		public IEngineFuelViewModel CreateEngineFuelViewModel(IEngineFuelDeclarationInputData inputData)
		{
			throw new NotImplementedException();
		}

		public IComponentsViewModel CreateComponentsViewModel(IXMLVehicleComponentsDeclaration inputData)
		{
			throw new NotImplementedException();
		}

		public ICommonComponentViewModel CreateCommonComponentViewModel(IComponentInputData inputData)
		{
			return _internalFactory.CreateCommonComponentViewModel(inputData);
		}

		public IComponentViewModel CreateComponentViewModel(ITransmissionInputData inputData)
		{
			throw new NotImplementedException();
		}

		public IComponentViewModel CreateComponentViewModel(IPTOTransmissionInputData inputData)
		{
			throw new NotImplementedException();
		}

		public IComponentViewModel CreateComponentViewModel(IAxlesDeclarationInputData inputData)
		{
			throw new NotImplementedException();
		}

		public IComponentViewModel CreateComponentViewModel(IAxleDeclarationInputData inputData)
		{
			throw new NotImplementedException();
		}

		public IComponentViewModel CreateComponentViewModel(IAuxiliariesDeclarationInputData inputData)
		{
			throw new NotImplementedException();
		}

		public IComponentViewModel CreateComponentViewModel(IAuxiliaryDeclarationInputData inputData)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
