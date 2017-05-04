using System.Collections.Generic;
using System.Xml.Linq;

namespace TUGraz.VectoHashing
{
	public interface IVectoHash {
		IEnumerable<VectoComponents> GetContainigComponents();
		string ComputeHash();
		string ComputeHash(VectoComponents component);
		XDocument AddHash();
		string ReadHash();
		string ReadHash(VectoComponents component);
		bool ValidateHash();
		bool ValidateHash(VectoComponents component);
	}
}