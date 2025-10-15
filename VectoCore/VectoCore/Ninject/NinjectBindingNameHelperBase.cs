using System;
using System.Runtime.CompilerServices;

namespace TUGraz.VectoCore.Ninject
{
	public abstract class NinjectBindingNameHelperBase
	{
		protected string CheckArguments<T1>(object[] arguments, Func<T1, string> func, [CallerMemberName] string callerName = "")
		{
			if (arguments.Length == 1 && arguments[0] is T1 p1) {
				return func(p1);
			}
			throw new ArgumentException($"exactly one argument expected for {callerName}: {typeof(T1).Name}");
		}

		protected string CheckArguments<T1, T2>(object[] arguments, Func<T1, T2, string> func, [CallerMemberName] string callerName = "")
		{
			if (arguments.Length == 2 && arguments[0] is T1 p1 && arguments[1] is T2 p2) {
				return func(p1, p2);
			}
			throw new ArgumentException($"exactly two arguments expected for {callerName}: {typeof(T1).Name}, {typeof(T2).Name}");
		}

		protected string CheckArguments<T1, T2, T3>(object[] arguments, Func<T1, T2, T3, string> func, [CallerMemberName] string callerName = "")
		{
			if (arguments.Length == 3 && arguments[0] is T1 p1 && arguments[1] is T2 p2 && arguments[2] is T3 p3) {
				return func(p1, p2, p3);
			}
			throw new ArgumentException($"exactly three arguments expected for {callerName}: {typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}");
		}

		protected string CheckArguments<T1, T2, T3, T4>(object[] arguments, Func<T1, T2, T3, T4, string> func, [CallerMemberName] string callerName = "")
		{
			if (arguments.Length == 4 && arguments[0] is T1 p1 && arguments[1] is T2 p2 && arguments[2] is T3 p3 && arguments[3] is T4 p4) {
				return func(p1, p2, p3, p4);
			}
			throw new ArgumentException($"exactly four arguments expected for {callerName}: {typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name} {typeof(T4).Name}");
		}
	}
}