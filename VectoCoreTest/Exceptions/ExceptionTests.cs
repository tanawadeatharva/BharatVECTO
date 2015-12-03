/*
* Copyright 2015 European Union
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl5
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.Exceptions;

namespace TUGraz.VectoCore.Tests.Exceptions
{
	[TestClass]
	public class ExceptionTests
	{
		[TestMethod]
		public void Test_VectoExceptions()
		{
			new CSVReadException("Test");
			new CSVReadException("Test", new Exception("Inner"));
			new InvalidFileFormatException("Test");
			new CSVReadException("Test", new Exception("Inner"));
			new UnsupportedFileVersionException("Test");
			new UnsupportedFileVersionException("Test", new Exception("Inner"));
			new InvalidFileFormatException("Test");
			new InvalidFileFormatException("Test", new Exception("Inner"));
			new VectoException("Test");
			new VectoException("Test", new Exception("Inner"));

			new VectoSimulationException("Test");
			new VectoSimulationException("Test", new Exception("Inner"));
		}
	}
}