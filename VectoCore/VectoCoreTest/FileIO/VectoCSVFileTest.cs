using System.Data;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.FileIO
{
	[TestFixture]
	public class VectoCSVFileTest
	{
		[Test]
		public void VectoCSVFile_Read()
		{
			var table = VectoCSVFile.Read(@"TestData\test.csv");
			Assert.AreEqual(3, table.Columns.Count);
			Assert.IsTrue(new[] { "a", "b", "c" }.SequenceEqual(table.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
			Assert.AreEqual(2, table.Rows.Count);

			Assert.IsTrue(new[] { "1", "2", "3" }.SequenceEqual(table.Rows[0].ItemArray));
			Assert.IsTrue(new[] { "4", "5", "6" }.SequenceEqual(table.Rows[1].ItemArray));
		}

		[Test]
		public void VectoCSVFile_Read_RealLossMap()
		{
			var table = VectoCSVFile.Read(@"TestData\Components\Axle.vtlm");
			Assert.AreEqual(3, table.Columns.Count);
			Assert.IsTrue(
				new[] { "Input Speed", "Input Torque", "Torque Loss" }.SequenceEqual(
					table.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
			Assert.AreEqual(285, table.Rows.Count);
		}

		[Test]
		public void VectoCSVFile_ReadStream_Normal()
		{
			var stream = "a,b,c\n1,2,3\n4,5,6".GetStream();
			var table = VectoCSVFile.ReadStream(stream);

			Assert.AreEqual(3, table.Columns.Count);
			Assert.IsTrue(new[] { "a", "b", "c" }.SequenceEqual(table.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
			Assert.AreEqual(2, table.Rows.Count);

			Assert.IsTrue(new[] { "1", "2", "3" }.SequenceEqual(table.Rows[0].ItemArray));
			Assert.IsTrue(new[] { "4", "5", "6" }.SequenceEqual(table.Rows[1].ItemArray));
		}

		[Test]
		public void VectoCSVFile_ReadStream_No_Header()
		{
			var stream = "1,2,3\n4,5,6".GetStream();
			var table = VectoCSVFile.ReadStream(stream);

			Assert.AreEqual(3, table.Columns.Count);
			Assert.IsTrue(new[] { "0", "1", "2" }.SequenceEqual(table.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
			Assert.AreEqual(2, table.Rows.Count);

			Assert.IsTrue(new[] { "1", "2", "3" }.SequenceEqual(table.Rows[0].ItemArray));
			Assert.IsTrue(new[] { "4", "5", "6" }.SequenceEqual(table.Rows[1].ItemArray));
		}

		[Test]
		public void VectoCSVFile_ReadStream_Comments()
		{
			var stream = @"#a,b,c
						   #21,22,23
                           #674,95,96
                           a,b,c
                           #9,8,7
                           1,2,3
                           4,5,6".GetStream();
			var table = VectoCSVFile.ReadStream(stream);

			Assert.AreEqual(3, table.Columns.Count);
			Assert.IsTrue(new[] { "a", "b", "c" }.SequenceEqual(table.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
			Assert.AreEqual(2, table.Rows.Count);

			Assert.IsTrue(new[] { "1", "2", "3" }.SequenceEqual(table.Rows[0].ItemArray));
			Assert.IsTrue(new[] { "4", "5", "6" }.SequenceEqual(table.Rows[1].ItemArray));
		}

		[Test]
		public void VectoCSVFile_Write_Filename()
		{
			const string fileName = "out_test.csv";

			if (File.Exists(fileName))
				File.Delete(fileName);

			var table = new DataTable();
			table.Columns.Add("a");
			table.Columns.Add("b");
			table.Rows.Add("1", "2");

			VectoCSVFile.Write(fileName, table);

			var text = File.ReadAllText(fileName);
			Assert.AreEqual("a,b\r\n1,2\r\n", text);
		}

		[Test]
		public void VectoCSVFile_Write_StreamWriter()
		{
			var table = new DataTable();
			table.Columns.Add("a");
			table.Columns.Add("b");
			var row = table.NewRow();
			row.ItemArray = new[] { "1", "2" };
			table.Rows.Add(row);

			using (var stream = new MemoryStream()) {
				using (var sw = new StreamWriter(stream)) {
					VectoCSVFile.Write(sw, table);
					sw.Flush();

					stream.Position = 0;

					using (var sr = new StreamReader(stream))
						Assert.AreEqual("a,b\r\n1,2\r\n", sr.ReadToEnd());
				}
			}
		}
	}
}