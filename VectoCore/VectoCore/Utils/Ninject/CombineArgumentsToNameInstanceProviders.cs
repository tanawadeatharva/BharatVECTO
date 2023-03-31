using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ninject.Extensions.Factory;
using Ninject.Extensions.Factory.Factory;
using Ninject.Parameters;
using TUGraz.VectoCommon.Exceptions;

namespace TUGraz.VectoCore.Utils.Ninject
{
    public class CombineArgumentsToNameInstanceProvider : StandardInstanceProvider
	{
		public class MethodSettings
		{
			/// <summary>
			/// This delegate is used to create a name out of the parameters
			/// </summary>
			public CombineToName combineToNameDelegate;
			/// <summary>
			/// Specifies the number of arguments that are NOT passed to the constructor
			/// </summary>
			public int skipArguments;
			/// <summary>
			/// Specifies the number of arguments that are passed to the <see cref="combineToNameDelegate"/>
			/// </summary>
			public int takeArguments;

            //TODO: use expressions instead of method info 
			// Expression<Func<int, string>> expression = i => i.ToString();
			// MethodInfo method = ((MethodCallExpression)expression.Body).Method;

            /// <summary>
            /// Sets the methods for which these settings apply, leave empty for default settings
            /// </summary>
            public MethodInfo[] methods;
		}

		public delegate string CombineToName(params object[] arguments);

		private Dictionary<MethodInfo, MethodSettings> _methodSettings = new Dictionary<MethodInfo, MethodSettings>();
		/// <summary>
		/// Constructor for CombineArgumentsToNameInstanceProvider
		/// </summary>
		
		public CombineArgumentsToNameInstanceProvider(params MethodSettings[] settings) : this(false, settings)
		{
			
		}

		public CombineArgumentsToNameInstanceProvider(bool fallback, params MethodSettings[] settings)
		{
			Fallback = fallback;
			if (settings != null && settings.Any(s => s.methods == null))
			{
				throw new ArgumentException($"At least one method has to be specified in the MethodSetting");
			}


			if (settings != null)
			{
				foreach (var setting in settings)
				{
					foreach (var method in setting.methods)
					{
						_methodSettings.Add(method, setting);
					}
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
			catch (Exception e) {
				var name = GetName(methodInfo, arguments);
				throw new VectoException("failed to create instance for '{1}' via '{0}' version '{2}' name'{3}'", e, methodInfo, methodInfo.ReturnType.Name, arguments[0].ToString(), name);
				
				//throw e;
			}
		}

		protected override string GetName(MethodInfo methodInfo, object[] arguments)
		{
			if (!_methodSettings.TryGetValue(methodInfo, out var methodSettings)) {
				return base.GetName(methodInfo, arguments);
			}

			return methodSettings.combineToNameDelegate.Invoke(arguments.Take(methodSettings.takeArguments).ToArray());

		}

		protected override IConstructorArgument[] GetConstructorArguments(MethodInfo methodInfo, object[] arguments)
		{
			if (!_methodSettings.TryGetValue(methodInfo, out var methodSettings)) {
				return base.GetConstructorArguments(methodInfo, arguments);
			}

			return base.GetConstructorArguments(methodInfo, arguments).Skip(methodSettings.skipArguments).ToArray();
		}

		#endregion
	}
}
