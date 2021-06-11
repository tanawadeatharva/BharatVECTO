using System.Resources;
using Castle.Core.Internal;

namespace VECTO3GUI2020.Helper
{
	public class NameResolver
	{
		public static string ResolveName(string propertyName, params ResourceManager[] resourceManagers)
		{
			foreach (var resourceManager in resourceManagers)
			{
				var resolvedName = resourceManager?.GetString(propertyName);
				if (!resolvedName.IsNullOrEmpty())
				{
					return resolvedName;
					break;
				}
			}


			return propertyName + "*";
		}
	}
}