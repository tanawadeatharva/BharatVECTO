using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using VECTO3GUI2020.Helper;

namespace Vecto3GUI2020Test.ViewModelTests
{
	[TestFixture]
	public class ExtensionTests
	{

		[Test]
		public void TestGetValuesAsObservableCollectionEnumExtension()
		{

			VehicleCode? vehicleCode = null;
			var collection1 = EnumHelper.GetValuesAsObservableCollectionExcluding<Enum, VehicleCode>(VehicleCode.NOT_APPLICABLE, VehicleCode.CF);

			Assert.False(collection1.Contains(VehicleCode.NOT_APPLICABLE));
			Assert.False(collection1.Contains(VehicleCode.CF));

			Assert.True(collection1.Contains(VehicleCode.CA));
		
			Assert.True(collection1.Contains(VehicleCode.CB));
			Assert.True(collection1.Contains(VehicleCode.CC));
			Assert.True(collection1.Contains(VehicleCode.CD));
			Assert.True(collection1.Contains(VehicleCode.CE));
			Assert.True(collection1.Contains(VehicleCode.CG));
			Assert.True(collection1.Contains(VehicleCode.CH));
			Assert.True(collection1.Contains(VehicleCode.CI));
			Assert.True(collection1.Contains(VehicleCode.CJ));



			var collection2 = EnumHelper.GetValuesAsObservableCollectionIncluding<Enum, VehicleCode>(VehicleCode.CA);
			Assert.True(collection2.Contains(VehicleCode.CA));
			Assert.False(collection2.Contains(VehicleCode.NOT_APPLICABLE));
		}

		
	}
}