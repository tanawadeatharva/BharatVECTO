using System;
using System.Linq;
using System.Reflection;
using Ninject.Extensions.Factory;
using Ninject.Extensions.Factory.Factory;
using Ninject.Parameters;
using TUGraz.VectoCommon.Exceptions;

namespace VECTO3GUI2020.Ninject.Util
{
    /// <summary>
    /// Ninject Instance Provider uses the first argument as name to resolve a named binding.
    /// </summary>
    public class UseFirstArgumentAsNameInstanceProvider : StandardInstanceProvider
    {
        private bool _skip_first_argument = true;
        public UseFirstArgumentAsNameInstanceProvider(bool skip_first_argument) : base()
        {
            _skip_first_argument = skip_first_argument;
        }

        public UseFirstArgumentAsNameInstanceProvider() : base()
        {

        }



        protected override IConstructorArgument[] GetConstructorArguments(MethodInfo methodInfo, object[] arguments)
        {
            return base.GetConstructorArguments(methodInfo, arguments).Skip(_skip_first_argument ? 1 : 0).ToArray();
        }

        protected override string GetName(MethodInfo methodInfo, object[] arguments)
        {
            return arguments[0].ToString();
        }

        public override object GetInstance(IInstanceResolver instanceResolver, MethodInfo methodInfo, object[] arguments)
        {
            try
            {
                return base.GetInstance(instanceResolver, methodInfo, arguments);
            }
            catch (Exception e)
            {
                throw new VectoException("failed to create instance for '{1}' via '{0}' version '{2}'", e, methodInfo.Name, methodInfo.ReturnType.Name, arguments[0]);
                throw e;
            }
        }
    }
}
