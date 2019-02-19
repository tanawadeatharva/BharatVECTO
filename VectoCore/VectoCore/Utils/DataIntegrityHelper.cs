using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TUGraz.VectoCore.Utils
{
	public class DataIntegrityHelper
	{
		public static string ComputeDigestValue(string[] lines)
		{
			var hash = System.Convert.ToBase64String(GetHash(string.Join("\n", lines)));

			return string.Format("SHA256: {0}", hash);
		}

		public static byte[] GetHash(string inputString)
		{
			HashAlgorithm algorithm = SHA256.Create();  
			return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
		}
	}
}
