using System;
using System.Linq;
using System.Reflection;
using Ninject.Extensions.Factory;
using Ninject.Extensions.Factory.Factory;
using Ninject.Parameters;
using TUGraz.VectoCommon.Exceptions;

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

		#region Overrides of StandardInstanceProvider

		public override object GetInstance(IInstanceResolver instanceResolver, MethodInfo methodInfo, object[] arguments)
		{
			try {
				return base.GetInstance(instanceResolver, methodInfo, arguments);
			} catch (Exception e) {
				throw new VectoException("failed to create instance for '{1}' via '{0}' version '{2}'", e, methodInfo.Name, methodInfo.ReturnType.Name, arguments[0]);
				//throw e;
			}
		}

		#endregion

		#endregion
	}
}
