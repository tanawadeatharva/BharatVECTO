using System;
using System.Linq;
using Ninject;
using Ninject.Activation;

namespace TUGraz.VectoCore.Tests.Utils.Ninject;

public static class NinjectExtensionMethods
{
	public static void UpdateBinding<T>(this IKernel kernel, string name, Func<IContext, T> instaceCreator)
	{
		var bindings = kernel.GetBindings(typeof(T)).ToList();
		foreach (var binding in bindings) {
			if (!binding.IsConditional && name.Equals(binding.Metadata.Name)) {
				kernel.RemoveBinding(binding);
				Console.WriteLine($"Removed binding for {name}");
			}
		}

		kernel.Bind<T>().ToMethod(instaceCreator).Named(name);
		Console.WriteLine($"added new binding for {name}");

    }
}