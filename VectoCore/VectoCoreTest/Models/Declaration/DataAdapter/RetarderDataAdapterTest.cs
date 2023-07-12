using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Tests.Models.Declaration.DataAdapter
{
    [TestFixture]
	// Issue codeeu vecto/vecto#70
	internal class RetarderDataAdapterTest
	{
		private RetarderDataAdapter _retarderDataAdapter;

		


		[SetUp]
		public void Setup()
		{
			_retarderDataAdapter = new RetarderDataAdapter();

		}


		[Test]
		[Combinatorial]
		public void TestTypeApplicableToArchP_HEV([Values]RetarderType retarderType, [Values(ArchitectureID.P1, ArchitectureID.P2, ArchitectureID.P2_5, ArchitectureID.P3, ArchitectureID.P4, ArchitectureID.P_IHPC)]ArchitectureID archID)
		{
			var retarderMoq = RetarderMoq(retarderType);

			var shouldWork = P_HEVRetarderShouldWork(retarderType, archID);

			CreateRetarderData(archID, shouldWork, retarderMoq, null);
		}

		[Test]
		[Combinatorial]
		public void TestTypeApplicableToArchS_HEV([Values] RetarderType retarderType,
			[Values(ArchitectureID.S_IEPC, ArchitectureID.S2, ArchitectureID.S3, ArchitectureID.S4)]
			ArchitectureID archID)
		{
			var retarderMoq = RetarderMoq(retarderType);

			var shouldWork = S_HEVRetarderShouldWork(retarderType, archID);

			CreateRetarderData(archID, shouldWork, retarderMoq, null);
        }

		[Test]
		[Combinatorial]
		public void TestTypeApplicableToArchPEV([Values] RetarderType retarderType,
			[Values(ArchitectureID.E2, ArchitectureID.E3, ArchitectureID.E4)]
			ArchitectureID archID)
		{
			var retarderMoq = RetarderMoq(retarderType);

			var shouldWork = PEVRetarderShouldWork(retarderType, archID);

			CreateRetarderData(archID, shouldWork, retarderMoq, null);
		}


		[Test]
		[Combinatorial]
		public void TestTypeApplicableToArch_IEPC_E([Values] RetarderType retarderType,
			[Values]bool differentialIncluded, [Values]bool designTypeWheelMotor)
		{
			var retarderMoq = RetarderMoq(retarderType);
			var iepcMoq = IEPCMoq(differentialIncluded, designTypeWheelMotor);

			var shouldWork = IEPCShouldWork(retarderType, differentialIncluded, designTypeWheelMotor);

			CreateRetarderData(ArchitectureID.E_IEPC, shouldWork, retarderMoq, iepcMoq);



		}



		private void CreateRetarderData(ArchitectureID archID, bool shouldWork, Mock<IRetarderInputData> retarderMoq,
			Mock<IIEPCDeclarationInputData> iepcInputData)
		{
			if (shouldWork) {
				TestContext.Progress.WriteLine("Should work");
                var data = _retarderDataAdapter.CreateRetarderData(retarderMoq.Object, archID, iepcInputData?.Object);
				
				Assert.NotNull(data, "Should work");
			} else {
				Assert.Throws<VectoException>((() => { _retarderDataAdapter.CreateRetarderData(retarderMoq.Object, archID, iepcInputData?.Object); }), "Should not work - but does!");
				TestContext.Progress.WriteLine("Doesnt work");
            }
		}


        private bool PEVRetarderShouldWork(RetarderType retarderType, ArchitectureID archId)
		{
			if (retarderType == RetarderType.None)
			{
				return true;
			}
			var s_hev_supported_types = new Dictionary<ArchitectureID, HashSet<RetarderType>>() {
				{ ArchitectureID.E2,
					new HashSet<RetarderType>() {
						RetarderType.LossesIncludedInTransmission,
						RetarderType.TransmissionInputRetarder,
						RetarderType.TransmissionOutputRetarder,
					}},

				{ ArchitectureID.E3, new HashSet<RetarderType>() {
					RetarderType.AxlegearInputRetarder,
				}},

				{ArchitectureID.E4, new HashSet<RetarderType>()},
			};
			return s_hev_supported_types.ContainsKey(archId) && s_hev_supported_types[archId].Contains(retarderType);
		}

        private bool S_HEVRetarderShouldWork(RetarderType retarderType, ArchitectureID archId)
		{
			if (retarderType == RetarderType.None) {
				return true;
			}
			var s_hev_supported_types = new Dictionary<ArchitectureID, HashSet<RetarderType>>() {
				{ ArchitectureID.S2, 
					new HashSet<RetarderType>() {
						RetarderType.LossesIncludedInTransmission,
						RetarderType.TransmissionInputRetarder,
						RetarderType.TransmissionOutputRetarder,
				}},

				{ ArchitectureID.S3, new HashSet<RetarderType>() {
					RetarderType.AxlegearInputRetarder,
				}},

				{ArchitectureID.S4, new HashSet<RetarderType>()},

				{ ArchitectureID.S_IEPC , new HashSet<RetarderType>() {
					RetarderType.AxlegearInputRetarder, 
					RetarderType.LossesIncludedInTransmission
				}}
			};
			return s_hev_supported_types.ContainsKey(archId) && s_hev_supported_types[archId].Contains(retarderType);
		}

		public bool P_HEVRetarderShouldWork(RetarderType type, ArchitectureID arch)
		{
			if (type == RetarderType.TransmissionInputRetarder && arch == ArchitectureID.P_IHPC) {
				return false;
			}


			if (type != RetarderType.AxlegearInputRetarder) {
				return true;

			}

			return false;
		}

		//bool DifferentialIncluded { get; }

		//bool DesignTypeWheelMotor { get; }

		//int? NrOfDesignTypeWheelMotorMeasured { get; }
        public bool IEPCShouldWork(RetarderType retarderType, bool differentialIncluded, bool DesignTypeWheelMotor)
		{
			if (retarderType == RetarderType.None)
			{
				return true;
			}
            if (DesignTypeWheelMotor) {
				//This property "wins"!
				return false;
			}

			if (retarderType == RetarderType.LossesIncludedInTransmission) {
				return true;
			}


			if (retarderType == RetarderType.AxlegearInputRetarder && !differentialIncluded) {
				return true;
			}



			return false;
		}


		private Mock<IRetarderInputData> RetarderMoq(RetarderType retarderType)
		{
			var retarderMoq = new Mock<IRetarderInputData>();
			retarderMoq.Setup(m => m.Type).Returns(retarderType);
			retarderMoq.Setup(m => m.LossMap).Returns(MockLossMap());
			return retarderMoq;
		}

		TableData MockLossMap()
		{
			var tableData = new TableData("Mock")
			{
				Columns = { "c_1", "c_2" }
			};
			for (int i = 0; i < 2; i++)
			{
				var row = tableData.NewRow();
				row[0] = 3.0;
				row[1] = 42.0;
				tableData.Rows.Add(row);
			}
			return tableData;
		}


		private Mock<IIEPCDeclarationInputData> IEPCMoq(bool differentialIncluded, bool designTypeWheelMotor)
		{
			var iepcMoq = new Mock<IIEPCDeclarationInputData>();
			iepcMoq.SetupGet(m => m.DesignTypeWheelMotor).Returns(designTypeWheelMotor);
			iepcMoq.SetupGet(m => m.DifferentialIncluded).Returns(differentialIncluded);
			return iepcMoq;
		}

    }
}
