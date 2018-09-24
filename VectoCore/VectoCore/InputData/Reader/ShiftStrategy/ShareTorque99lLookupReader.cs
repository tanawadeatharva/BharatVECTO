using System.IO;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ShiftStrategy
{
	public class ShareTorque99lLookupReader
	{
		public static ShareTorque99lLookup ReadFromStream(Stream stream)
		{
			return Create(VectoCSVFile.ReadStream(stream));
		}

		private static ShareTorque99lLookup Create(TableData data)
		{
			return new ShareTorque99lLookup(
				new LookupDataReader<MeterPerSecond, double>("Share T_99l", new[] { Fields.Velocity, Fields.ShareT99l })
					.Create(data, x => x.ParseDouble(Fields.Velocity).KMPHtoMeterPerSecond(), y => y.ParseDouble(Fields.ShareT99l))
					.OrderBy(x => x.Key).ToArray());
		}

		public static class Fields
		{
			public const string Velocity = "v";
			public const string ShareT99l = "share T_99l";
		}
	}
}
