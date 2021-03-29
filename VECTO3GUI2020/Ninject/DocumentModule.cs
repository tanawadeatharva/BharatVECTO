using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Model.Interfaces;
using VECTO3GUI2020.Ninject.Util;
using VECTO3GUI2020.Util;
using VECTO3GUI2020.ViewModel.Implementation.Document;
using VECTO3GUI2020.ViewModel.Interfaces.Document;

namespace VECTO3GUI2020.Ninject
{
    public class DocumentModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDocumentViewModelFactory>().ToFactory(() => new UseFirstArgumentAsNameInstanceProvider(false));
            Bind<IDocumentViewModel>().To<DeclarationJobViewModel>().Named(XmlDocumentType.DeclarationJobData.ToString());
            //Bind<IDocumentViewModel>().To<DeclarationTrailerJobDocumentViewModel>().Named(XmlDocumentType.DeclarationTrailerJobData.ToString());
            
        }
    }
}
