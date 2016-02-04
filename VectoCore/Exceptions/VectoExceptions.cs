/*
* Copyright 2015, 2016 Graz University of Technology,
* Institute of Internal Combustion Engines and Thermodynamics,
* Institute of Technical Informatics
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System;
using System.IO;
using JetBrains.Annotations;
using NLog;

namespace TUGraz.VectoCore.Exceptions
{
	public class VectoException : Exception
	{
		public VectoException(string message) : base(message)
		{
			LogManager.Flush();
		}

		public VectoException(string message, Exception innerException) : base(message, innerException)
		{
			LogManager.Flush();
		}

		[StringFormatMethod("message")]
		public VectoException(string message, params object[] args)
			: base(string.Format(message, args))
		{
			LogManager.Flush();
		}

		[StringFormatMethod("message")]
		protected VectoException(string message, Exception inner, params object[] args)
			: base(string.Format(message, args), inner)
		{
			LogManager.Flush();
		}
	}

	public abstract class FileIOException : VectoException
	{
		protected FileIOException(string message) : base(message) {}
		protected FileIOException(string message, Exception inner) : base(message, inner) {}
	}


	public class InvalidFileFormatException : FileIOException
	{
		public InvalidFileFormatException(string message) : base(message) {}
		public InvalidFileFormatException(string message, params object[] args) : base(string.Format(message, args)) {}
		public InvalidFileFormatException(string message, Exception inner) : base(message) {}
	}


	/// <summary>
	///     Exception which gets thrown when the version of a file is not supported.
	/// </summary>
	public class UnsupportedFileVersionException : FileIOException
	{
		public UnsupportedFileVersionException(string message) : base(message) {}
		public UnsupportedFileVersionException(string message, Exception inner) : base(message, inner) {}

		public UnsupportedFileVersionException(string filename, int version, Exception inner = null)
			: base(string.Format("Unsupported Version of {0} file. Got Version {1}",
				Path.GetExtension(filename), version), inner) {}
	}

	/// <summary>
	///     Exception which gets thrown when an error occurred during read of a vecto csv-file.
	/// </summary>
	public class CSVReadException : FileIOException
	{
		public CSVReadException(string message) : base(message) {}
		public CSVReadException(string message, Exception inner) : base(message, inner) {}
	}
}