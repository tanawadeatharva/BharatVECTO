/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
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

using System;
using System.Collections.Generic;
using System.Data;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
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
				table.Columns[col.ColumnName].ColumnName = col.ColumnName.ToLower().RemoveWhitespace();
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
		protected Dictionary<Tuple<TKey1, TKey2>, TValue> Data = new Dictionary<Tuple<TKey1, TKey2>, TValue>();

		public virtual TValue Lookup(TKey1 key1, TKey2 key2)
		{
			return Data[Tuple.Create(key1, key2)];
		}
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