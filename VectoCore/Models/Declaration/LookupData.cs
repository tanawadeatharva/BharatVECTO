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

using System.Collections.Generic;
using System.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public abstract class LookupData : LoggingObject
	{
		protected abstract void ParseData(DataTable table);

		protected DataTable ReadCsvResource(string resourceId)
		{
			return VectoCSVFile.ReadStream(RessourceHelper.ReadStream(resourceId));
		}

		protected DataTable ReadCsvFile(string fileName)
		{
			return VectoCSVFile.Read(fileName);
		}

		protected static DataTable NormalizeTable(DataTable table)
		{
			foreach (DataColumn col in table.Columns) {
				table.Columns[col.ColumnName].ColumnName = col.ColumnName.ToLower().Replace(" ", "");
			}
			return table;
		}
	}


	public abstract class LookupData<TKey, TValue> : LookupData
	{
		protected Dictionary<TKey, TValue> Data = new Dictionary<TKey, TValue>();

		public virtual TValue Lookup(TKey key)
		{
			return Data[key];
		}
	}

	public abstract class LookupData<TKey1, TKey2, TValue> : LookupData
	{
		public abstract TValue Lookup(TKey1 key1, TKey2 key2);
	}

	public abstract class LookupData<TKey1, TKey2, TKey3, TValue> : LookupData
	{
		public abstract TValue Lookup(TKey1 key1, TKey2 key2, TKey3 key3);
	}

	public abstract class LookupData<TKey1, TKey2, TKey3, TKey4, TValue> : LookupData
	{
		public abstract TValue Lookup(TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4);
	}
}