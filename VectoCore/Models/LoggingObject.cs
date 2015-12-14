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

using NLog;

namespace TUGraz.VectoCore.Models
{
	public class LoggingObject
	{
		protected Logger Log { get; private set; }

		protected Logger GUILogger { get; private set; }

		protected LoggingObject()
		{
			Log = LogManager.GetLogger(GetType().FullName);
			GUILogger = LogManager.GetLogger("GUI");
		}

		protected static Logger Logger<T>()
		{
			return LogManager.GetLogger(typeof(T).ToString());
		}
	}
}