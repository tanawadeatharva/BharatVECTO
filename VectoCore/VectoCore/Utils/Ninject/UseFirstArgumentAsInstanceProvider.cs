using System.Linq;
using System.Reflection;
using Ninject.Extensions.Factory;
using Ninject.Parameters;

namespace TUGraz.VectoCore.Utils.Ninject
{
	public class UseFirstArgumentAsInstanceProvider : StandardInstanceProvider
	{
		#region Overrides of StandardInstanceProvider

		protected override string GetName(MethodInfo methodInfo, object[] arguments)
		{
			return arguments[0].ToString();
		}


		protected override IConstructorArgument[] GetConstructorArguments(MethodInfo methodInfo, object[] arguments)
		{
			return base.GetConstructorArguments(methodInfo, arguments).Skip(1).ToArray();
		}

		#endregion
	}
}
