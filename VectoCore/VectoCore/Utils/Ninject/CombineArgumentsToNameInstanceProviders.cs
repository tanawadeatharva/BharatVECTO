using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject.Extensions.Factory;
using Ninject.Extensions.Factory.Factory;
using Ninject.Parameters;
using TUGraz.VectoCommon.Exceptions;

namespace TUGraz.VectoCore.Utils.Ninject
{
    internal class CombineArgumentsToNameInstanceProvider : StandardInstanceProvider
	{

		public delegate string CombineToName(params object[] arguments);

		private CombineToName _combinationDelegate = null;
		private readonly int _skipArgumentsNr;
		private readonly int _numberOfArguments;

		private HashSet<MethodInfo> _methodInfos = new HashSet<MethodInfo>();
		/// <summary>
		/// Constructor for CombineArgumentsToNameInstanceProvider
		/// </summary>
		/// <param name="combinationDelegate">this delegate is to combine numberOfArguments arguments to a name</param>
		/// <param name="numberOfArguments">the number of arguments that are used to create the name</param>
		/// <param name="methods">the name is only resolved with the combinationDelegate if one of these methods was called, otherwise the standard instance provider is used</param>
		/// <param name="skipArgumentsNr">defines the number of arguments that are skipped and not passed to the constructor</param>
		public CombineArgumentsToNameInstanceProvider(CombineToName combinationDelegate, int numberOfArguments, int skipArgumentsNr, params MethodInfo[] methods)
		{
			_numberOfArguments = numberOfArguments;
			_skipArgumentsNr = skipArgumentsNr;
			_combinationDelegate = combinationDelegate;

			if (methods != null) {
				foreach (var method in methods) {
					_methodInfos.Add(method);
				}
			}
		}

		#region Overrides of StandardInstanceProvider

		public override object GetInstance(IInstanceResolver instanceResolver, MethodInfo methodInfo, object[] arguments)
		{
			try
			{
				return base.GetInstance(instanceResolver, methodInfo, arguments);
			}
			catch (Exception e)
			{
				throw new VectoException("failed to create instance for '{1}' via '{0}' version '{2}'", e, methodInfo.Name, methodInfo.ReturnType.Name, arguments[0]);
				//throw e;
			}
		}

		protected override string GetName(MethodInfo methodInfo, object[] arguments)
		{
			if (!_methodInfos.Contains(methodInfo)) {
				return base.GetName(methodInfo, arguments);
			}

			return _combinationDelegate.Invoke(arguments.Take(_numberOfArguments).ToArray());

		}

		protected override IConstructorArgument[] GetConstructorArguments(MethodInfo methodInfo, object[] arguments)
		{
			if (!_methodInfos.Contains(methodInfo)) {
				return base.GetConstructorArguments(methodInfo, arguments);
			}
			return base.GetConstructorArguments(methodInfo, arguments).Skip(_skipArgumentsNr).ToArray();
		}

		#endregion
	}
}
