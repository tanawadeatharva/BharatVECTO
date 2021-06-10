using Ninject.Extensions.Factory;
using Ninject.Extensions.Factory.Factory;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace VECTO3GUI2020.Util
{
    public class InheritingConstructorParameterInstanceProvider : StandardInstanceProvider
    {
        public override object GetInstance(IInstanceResolver instanceResolver, MethodInfo methodInfo, object[] arguments)
        {
            return base.GetInstance(instanceResolver, methodInfo, arguments.Skip(1).ToArray();
        }

        protected override IConstructorArgument[] GetConstructorArguments(MethodInfo methodInfo, object[] arguments)
        {
            //https://stackoverflow.com/questions/32138857/inherit-constructorparameter-from-a-factory
            return methodInfo.GetParameters()
                .Select((parameter, index) =>
                new ConstructorArgument(
                    parameter.Name, arguments[index], true)).Cast<IConstructorArgument>().ToArray();
        }
    }
}
