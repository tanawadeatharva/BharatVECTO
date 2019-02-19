using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Algorithms
{
	[TestFixture]
	public class CSVDigestValueTest
	{

		[TestCase]
		public void TestDigestValueCreation()
		{
			var tbl = CreateDataTable(new[] { "t", "dt", "v" }, 5);

			var str = new MemoryStream();
			var writer = new StreamWriter(str);
			VectoCSVFile.Write(writer, tbl, true, true);

			writer.Flush();
			str.Flush();
			str.Seek(0, SeekOrigin.Begin);

			var reader = new StreamReader(str);
			var lines = new List<string>();
			while (!reader.EndOfStream)
				lines.Add(reader.ReadLine());

			var last = lines.Last();

			Assert.IsTrue(last.StartsWith("#@"), "Digest Identifier not found");
			Assert.IsTrue(last.Contains("SHA256"), "Digest descriptor SHA256 not found");
		}

		[TestCase]
		public void TestDigestValueValidation()
		{
			var tbl = CreateDataTable(new[] { "t", "dt", "v" }, 5);

			var str = new MemoryStream();
			var writer = new StreamWriter(str);
			VectoCSVFile.Write(writer, tbl, true, true);

			writer.Flush();
			str.Flush();
			str.Seek(0, SeekOrigin.Begin);

			var reader = new StreamReader(str);
			var lines = new List<string>();
			while (!reader.EndOfStream)
				lines.Add(reader.ReadLine());

			var last = lines.Last();

			Assert.IsTrue(last.StartsWith("#@"), "Digest Identifier not found");
			Assert.IsTrue(last.Contains("SHA256"), "Digest descriptor SHA256 not found");

			var otherLines = lines.Where(x => !x.StartsWith("#@")).ToArray();

			var digest = DataIntegrityHelper.ComputeDigestValue(otherLines);

			Assert.AreEqual(string.Format("#@ {0}", digest), last);
		}

		private static DataTable CreateDataTable(string[] cols, int numRows)
		{
			var tbl = new DataTable();
			
			foreach (var col in cols) {
				tbl.Columns.Add(col, typeof(double));
			}

			var rnd = new Randomizer(873);
			for (var i = 0; i < numRows; i++) {
				var row = tbl.NewRow();
				foreach (var col in cols) {
					row[col] = rnd.NextDouble(100);
				}

				tbl.Rows.Add(row);
			}

			return tbl;
		}


		
	}
}
