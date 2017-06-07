using System.Collections.Generic;
using System.Xml.Linq;

namespace TUGraz.VectoHashing
{
	public interface IVectoHash
	{
		IList<VectoComponents> GetContainigComponents();

		/**
		 * Computes the hash-value of the top-level Data element (or vehicle)
		 * Note: the top-level Data element is required to have an id attribute!
		 * @return base64 encoded hash value
		 */
		string ComputeHash();

		/**
		 * Computes the hash-value for the given component. If a component can exist multiple times
		 * (i.e., Tyres) the index specifies for which component the hash is computed
		 * Note: the Data element is required to have an id attribute!
		 * @return base64 encoded hash value
		 */
		string ComputeHash(VectoComponents component, int index = 0);

		/**
		 * Computes the hash-value of the outer Data element and adds the according Signature element 
		 * after the Data element.
		 * Note: the id attribute is added to the Data element automatically. if an id attribute is already
		 * present its value is overwritten.
		 * @return returns the document including the Signature element with the hash of the Data block
		 */
		XDocument AddHash();

		/**
		 * Reads the hash-value of the top-level Signature element
		 * @return base64 encoded hash value
		 */
		string ReadHash();

		/**
		 * Reads the hash-value of the Signature element for the given component. If a component can exist 
		 * multiple times (i.e., Tyres), the index specifies for which component the hash is computed
		 * @return base64 encoded hash value
		 */
		string ReadHash(VectoComponents component, int index = 0);

		/**
		 * Validates the hash of the top-level component (or vehicle)
		 */
		bool ValidateHash();

		/**
		 * Validates the hash for the given component.
		 */
		bool ValidateHash(VectoComponents component, int index = 0);
	}
}