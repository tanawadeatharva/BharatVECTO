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
using JetBrains.Annotations;
using TUGraz.VectoCore.Models.Connector.Ports;

namespace TUGraz.VectoCore.Exceptions
{
	public class VectoSimulationException : VectoException
	{
		public VectoSimulationException(string msg) : base(msg) {}
		public VectoSimulationException(string msg, Exception inner) : base(msg, inner) {}

		[StringFormatMethod("message")]
		public VectoSimulationException(string message, params object[] args) : base(message, args) {}

		//[StringFormatMethod("message")]
		public VectoSimulationException(string message, Exception inner, params object[] args) : base(message, inner, args) {}
	}

	public class UnexpectedResponseException : VectoSimulationException
	{
		public IResponse Response;

		public UnexpectedResponseException(string message, IResponse resp)
			: base(message + Environment.NewLine + "{0}", resp)
		{
			Response = resp;
		}
	}

	public class VectoSearchFailedException : VectoException
	{
		public VectoSearchFailedException(string message, params object[] args) : base(message, args) {}
	}
}