using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace Vecto3GUI2020Test.HelperTests
{
	[TestFixture]
	public class BackingStorageTest
	{
		private TestVm _testVm;
		private Dictionary<string, object> _currentValues;
		private Dictionary<string, object> _savedValues;
		private bool _unsavedChangesNotified = false;

		public class TestVm : ObservableObject
		{
			private string _stringTestProperty;

			public string StringTestProperty
			{
				get => _stringTestProperty;
				set => SetProperty(ref _stringTestProperty, value);
			}

			private string _notMonitoredProperty;

			public string NotMonitoredProperty
			{
				get => _notMonitoredProperty;
				set => SetProperty(ref _notMonitoredProperty, value);
			}


			public BackingStorage<TestVm> BackingStorage { get; set; }
			public TestVm()
			{
				BackingStorage = new BackingStorage<TestVm>(this, nameof(StringTestProperty));
			}
		}

		[SetUp]
		public void SetUp()
		{
			_unsavedChangesNotified = false;
			_testVm = new TestVm();
			_currentValues = (Dictionary<string, object>)_testVm.BackingStorage.
				GetType().GetField("_currentValues", BindingFlags.NonPublic | BindingFlags.Instance).
				GetValue(_testVm.BackingStorage);
			_savedValues = (Dictionary<string, object>)_testVm.BackingStorage.
				GetType().GetField("_savedValues", BindingFlags.NonPublic | BindingFlags.Instance).
				GetValue(_testVm.BackingStorage);

            _testVm.BackingStorage.PropertyChanged += BackingStorage_PropertyChanged;
		}

        private void BackingStorage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
			switch (e.PropertyName) {
				case nameof(TestVm.BackingStorage.UnsavedChanges):
					_unsavedChangesNotified = true;
					break;
				default:
					break;
			}
        }

        [Test]
		public void ValueUpdatedOnChange()
		{

			var stopWatch = Stopwatch.StartNew();
			_testVm.StringTestProperty = "Hi i am the new value";
			stopWatch.Stop();
			TestContext.WriteLine($"Ticks ellapsed first update: {stopWatch.ElapsedTicks} ({stopWatch.Elapsed.TotalMilliseconds} ms)");

			
			

			Assert.AreEqual(1, _currentValues.Count);
			Assert.AreEqual(_testVm.StringTestProperty, _currentValues[nameof(_testVm.StringTestProperty)]);



			stopWatch = Stopwatch.StartNew();
			_testVm.StringTestProperty = "Hi i am ANOTHER value";
			stopWatch.Stop();
			TestContext.WriteLine($"Ticks ellapsed second update: {stopWatch.ElapsedTicks}  ({stopWatch.Elapsed.TotalMilliseconds} ms)");

			Assert.AreEqual(1, _currentValues.Count);
			Assert.AreEqual(_testVm.StringTestProperty, _currentValues[nameof(_testVm.StringTestProperty)]);
		}

		[Test]
		public void NotMonitoredProperty()
		{
			_testVm.NotMonitoredProperty = "Hi i should not get stored in the backing storage";
			Assert.IsFalse(_currentValues.ContainsKey(nameof(TestVm.NotMonitoredProperty)));
		}

		[Test]
		public void SaveChanges()
		{
			_testVm.StringTestProperty = "hi";
			Assert.IsTrue(checkNotified(ref _unsavedChangesNotified));
			Assert.IsTrue(_testVm.BackingStorage.UnsavedChanges);

			_testVm.BackingStorage.SaveChanges();
			Assert.IsTrue(checkNotified(ref _unsavedChangesNotified));
			Assert.IsFalse(_testVm.BackingStorage.UnsavedChanges);
		}


		[Test]
		public void ChangeAndUndo()
		{
			var originalString = "original";
			var newValue = "new Value";

			_testVm.StringTestProperty = originalString;
			Assert.IsTrue(checkNotified(ref _unsavedChangesNotified));
			Assert.IsTrue(_testVm.BackingStorage.UnsavedChanges);

			_testVm.BackingStorage.SaveChanges();
			Assert.IsTrue(checkNotified(ref _unsavedChangesNotified));
			Assert.IsFalse(_testVm.BackingStorage.UnsavedChanges);

			_testVm.StringTestProperty = newValue;
			Assert.IsTrue(checkNotified(ref _unsavedChangesNotified));
			Assert.IsTrue(_testVm.BackingStorage.UnsavedChanges);

			_testVm.StringTestProperty = originalString;
			Assert.IsTrue(checkNotified(ref _unsavedChangesNotified));
			Assert.IsFalse(_testVm.BackingStorage.UnsavedChanges);

		}

		public bool checkNotified(ref bool value)
		{
			var result = value;
			value = false;
			return result;
		}

		
	}
}