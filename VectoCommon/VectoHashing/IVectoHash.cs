/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

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