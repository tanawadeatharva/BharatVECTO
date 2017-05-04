using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace TUGraz.VectoHashing
{
	public class VectoHash : IVectoHash
	{
		public static VectoHash Load(string filename)
		{
			return null;
		}

		public static VectoHash Load(Stream stream)
		{
			return null;
		}

		public static VectoHash Load(XDocument doc)
		{
			return null;
		}

		public IEnumerable<VectoComponents> GetContainigComponents()
		{
			return null;
		}

		public string ComputeHash()
		{
			return null;
		}

		public string ComputeHash(VectoComponents component)
		{
			return null;
		}

		public XDocument AddHash()
		{
			return null;
		}

		public string ReadHash()
		{
			return null;
		}

		public string ReadHash(VectoComponents component)
		{
			return null;
		}

		public bool ValidateHash()
		{
			return false;
		}

		public bool ValidateHash(VectoComponents component)
		{
			return false;
		}
	}
}