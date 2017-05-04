using System.Collections.Generic;
using System.Xml.Linq;

namespace TUGraz.VectoHashing
{
	public interface IVectoHash
	{
		IEnumerable<VectoComponents> GetContainigComponents();

		string ComputeHash();

		string ComputeHash(VectoComponents component, int index = 0);

		XDocument AddHash();

		string ReadHash();

		string ReadHash(VectoComponents component, int index = 0);

		bool ValidateHash();

		bool ValidateHash(VectoComponents component, int index = 0);
	}
}