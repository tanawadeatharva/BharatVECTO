/*
 * Unit Test Template for Adding/Updating Driving Cycles
 * 
 * This file provides comprehensive test examples for validating new or updated
 * driving cycles in the VECTO system. Use these tests as templates when adding
 * new cycles or modifying existing ones.
 * 
 * Location: Copy tests to appropriate test project:
 *   - Cycle Reader Tests: Testing/UnitTests/Vecto UnitTests/TestCases/ComponentReader/
 *   - Factory Tests: Testing/UnitTests/Vecto UnitTests/TestCases/RunDataFactory/
 *   - Integration Tests: Testing/IntegrationTests/VECTO IntegrationTests/TestCases/Declaration/
 */

using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.Vecto.UnitTests.TestCases.CycleAddition
{
    /// <summary>
    /// Template tests for validating new cycle file reading and parsing
    /// </summary>
    [TestFixture]
    public class NewCycleReaderTests
    {
        #region Basic Cycle Reading Tests

        /// <summary>
        /// Test that a valid distance-based cycle file is parsed correctly
        /// </summary>
        [Test]
        public void ReadCycle_ValidDistanceBasedFormat_ParsesCorrectly()
        {
            // Arrange
            string cycleContent = 
                "<s>,<v>,<grad>,<stop>\n" +
                "0,0,0,10\n" +
                "100,30,0,0\n" +
                "500,50,0.005,0\n" +
                "1000,70,0.01,5\n" +
                "1500,0,0,15";
            
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(cycleContent));
            
            // Act
            var cycle = DrivingCycleDataReader.ReadFromStream(
                stream, 
                CycleType.DistanceBased, 
                "TestCycle", 
                false);
            
            // Assert
            Assert.IsNotNull(cycle, "Cycle should not be null");
            Assert.AreEqual(CycleType.DistanceBased, cycle.CycleType, "Cycle type should be DistanceBased");
            Assert.AreEqual("TestCycle", cycle.Name, "Cycle name should match");
            Assert.AreEqual(5, cycle.Entries.Count, "Should have 5 cycle entries");
            
            // Verify first entry
            Assert.AreEqual(0, cycle.Entries[0].Distance.Value(), "First entry distance should be 0");
            Assert.AreEqual(0, cycle.Entries[0].VehicleTargetSpeed.Value(), "First entry speed should be 0");
            
            // Verify last entry
            Assert.AreEqual(1500, cycle.Entries[4].Distance.Value(), "Last entry distance should be 1500");
            Assert.AreEqual(0, cycle.Entries[4].VehicleTargetSpeed.Value(), "Last entry speed should be 0");
        }

        /// <summary>
        /// Test that cycle type is auto-detected from headers
        /// </summary>
        [TestCase("<s>,<v>,<grad>,<stop>", CycleType.DistanceBased, Description = "Distance-based cycle")]
        [TestCase("<t>,<n>,<Me>", CycleType.EngineOnly, Description = "Engine-only cycle")]
        [TestCase("<t>,<Pwheel>,<gear>,<n>", CycleType.PWheel, Description = "Wheel power cycle")]
        [TestCase("<t>,<v>,<grad>", CycleType.MeasuredSpeed, Description = "Measured speed cycle")]
        [TestCase("<t>,<v>,<grad>,<n>,<gear>", CycleType.MeasuredSpeedGear, Description = "Measured speed with gear")]
        public void ReadCycle_DifferentHeaders_CorrectTypeDetected(string header, CycleType expectedType)
        {
            // Arrange
            string cycleContent = header + "\n0,0,0,0";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(cycleContent));
            
            // Act
            var cycle = DrivingCycleDataReader.ReadFromStream(
                stream, 
                expectedType, 
                "AutoDetectTest", 
                false);
            
            // Assert
            Assert.AreEqual(expectedType, cycle.CycleType, 
                $"Cycle type should be detected as {expectedType}");
        }

        /// <summary>
        /// Test that headers are case-insensitive
        /// </summary>
        [TestCase("<S>,<V>,<GRAD>,<STOP>", Description = "Uppercase headers")]
        [TestCase("<s>,<v>,<grad>,<stop>", Description = "Lowercase headers")]
        [TestCase("<S>,<v>,<Grad>,<Stop>", Description = "Mixed case headers")]
        public void ReadCycle_CaseInsensitiveHeaders_ParsesCorrectly(string header)
        {
            // Arrange
            string cycleContent = header + "\n0,0,0,0\n100,10,0.01,0";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(cycleContent));
            
            // Act
            var cycle = DrivingCycleDataReader.ReadFromStream(
                stream, 
                CycleType.DistanceBased, 
                "CaseTest", 
                false);
            
            // Assert
            Assert.IsNotNull(cycle);
            Assert.AreEqual(2, cycle.Entries.Count);
        }

        #endregion

        #region Validation Tests

        /// <summary>
        /// Test that monotonically increasing distance values are valid
        /// </summary>
        [Test]
        public void ValidateCycle_MonotonicDistance_NoException()
        {
            // Arrange
            string cycleContent = 
                "<s>,<v>,<grad>,<stop>\n" +
                "0,0,0,0\n" +
                "100,10,0,0\n" +
                "200,20,0,0\n" +
                "300,30,0,0";
            
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(cycleContent));
            var cycle = DrivingCycleDataReader.ReadFromStream(
                stream, 
                CycleType.DistanceBased, 
                "MonotonicTest", 
                false);
            
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var result = DrivingCycleData.ValidateCycleData(cycle, null);
                Assert.AreEqual(System.ComponentModel.DataAnnotations.ValidationResult.Success, result);
            }, "Monotonic distance values should be valid");
        }

        /// <summary>
        /// Test that non-monotonic distance values are detected
        /// </summary>
        [Test]
        public void ValidateCycle_NonMonotonicDistance_FailsValidation()
        {
            // Arrange
            string cycleContent = 
                "<s>,<v>,<grad>,<stop>\n" +
                "0,0,0,0\n" +
                "100,10,0,0\n" +
                "200,20,0,0\n" +
                "150,30,0,0";  // Distance decreases - invalid!
            
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(cycleContent));
            var cycle = DrivingCycleDataReader.ReadFromStream(
                stream, 
                CycleType.DistanceBased, 
                "NonMonotonicTest", 
                false);
            
            // Act
            var result = DrivingCycleData.ValidateCycleData(cycle, null);
            
            // Assert
            Assert.AreNotEqual(System.ComponentModel.DataAnnotations.ValidationResult.Success, result,
                "Non-monotonic distance should fail validation");
        }

        /// <summary>
        /// Test that realistic speed values are accepted
        /// </summary>
        [TestCase(0, Description = "Zero speed")]
        [TestCase(30, Description = "Low speed (30 km/h)")]
        [TestCase(80, Description = "Highway speed (80 km/h)")]
        [TestCase(90, Description = "Maximum typical speed (90 km/h)")]
        public void ReadCycle_RealisticSpeeds_ParsesCorrectly(double speedKmh)
        {
            // Arrange
            string cycleContent = $"<s>,<v>,<grad>,<stop>\n0,0,0,0\n100,{speedKmh},0,0";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(cycleContent));
            
            // Act
            var cycle = DrivingCycleDataReader.ReadFromStream(
                stream, 
                CycleType.DistanceBased, 
                "SpeedTest", 
                false);
            
            // Assert
            Assert.IsNotNull(cycle);
            Assert.AreEqual(speedKmh / 3.6, cycle.Entries[1].VehicleTargetSpeed.Value(), 0.01,
                "Speed should be converted from km/h to m/s");
        }

        /// <summary>
        /// Test that realistic gradient values are accepted
        /// </summary>
        [TestCase(0, Description = "Flat road")]
        [TestCase(0.01, Description = "1% uphill")]
        [TestCase(-0.01, Description = "1% downhill")]
        [TestCase(0.05, Description = "5% uphill")]
        [TestCase(-0.05, Description = "5% downhill")]
        public void ReadCycle_RealisticGradients_ParsesCorrectly(double gradientRad)
        {
            // Arrange
            string cycleContent = $"<s>,<v>,<grad>,<stop>\n0,0,0,0\n100,50,{gradientRad},0";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(cycleContent));
            
            // Act
            var cycle = DrivingCycleDataReader.ReadFromStream(
                stream, 
                CycleType.DistanceBased, 
                "GradientTest", 
                false);
            
            // Assert
            Assert.IsNotNull(cycle);
            Assert.AreEqual(gradientRad, cycle.Entries[1].RoadGradient.Value(), 0.0001,
                "Gradient should match input value");
        }

        #endregion

        #region Edge Case Tests

        /// <summary>
        /// Test handling of empty cycle
        /// </summary>
        [Test]
        public void ReadCycle_EmptyData_ThrowsException()
        {
            // Arrange
            string cycleContent = "<s>,<v>,<grad>,<stop>\n";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(cycleContent));
            
            // Act & Assert
            Assert.Throws<Exception>(() => {
                DrivingCycleDataReader.ReadFromStream(
                    stream, 
                    CycleType.DistanceBased, 
                    "EmptyTest", 
                    false);
            }, "Empty cycle should throw exception");
        }

        /// <summary>
        /// Test handling of single data point
        /// </summary>
        [Test]
        public void ReadCycle_SingleDataPoint_ParsesCorrectly()
        {
            // Arrange
            string cycleContent = "<s>,<v>,<grad>,<stop>\n0,0,0,0";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(cycleContent));
            
            // Act
            var cycle = DrivingCycleDataReader.ReadFromStream(
                stream, 
                CycleType.DistanceBased, 
                "SinglePointTest", 
                false);
            
            // Assert
            Assert.IsNotNull(cycle);
            Assert.AreEqual(1, cycle.Entries.Count);
        }

        /// <summary>
        /// Test handling of very long cycle
        /// </summary>
        [Test]
        public void ReadCycle_LongCycle_ParsesCorrectly()
        {
            // Arrange
            var sb = new StringBuilder();
            sb.AppendLine("<s>,<v>,<grad>,<stop>");
            
            // Create 1000 data points
            for (int i = 0; i <= 1000; i++)
            {
                sb.AppendLine($"{i * 100},50,0.01,0");
            }
            
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
            
            // Act
            var cycle = DrivingCycleDataReader.ReadFromStream(
                stream, 
                CycleType.DistanceBased, 
                "LongCycleTest", 
                false);
            
            // Assert
            Assert.IsNotNull(cycle);
            Assert.AreEqual(1001, cycle.Entries.Count, "Should parse all 1001 entries");
        }

        #endregion

        #region Different Cycle Type Tests

        /// <summary>
        /// Test reading engine-only cycle format
        /// </summary>
        [Test]
        public void ReadCycle_EngineOnlyFormat_ParsesCorrectly()
        {
            // Arrange
            string cycleContent = 
                "<t>,<n>,<Me>\n" +
                "0,800,0\n" +
                "10,1000,500\n" +
                "20,1200,800\n" +
                "30,1400,1000";
            
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(cycleContent));
            
            // Act
            var cycle = DrivingCycleDataReader.ReadFromStream(
                stream, 
                CycleType.EngineOnly, 
                "EngineOnlyTest", 
                false);
            
            // Assert
            Assert.AreEqual(CycleType.EngineOnly, cycle.CycleType);
            Assert.AreEqual(4, cycle.Entries.Count);
        }

        /// <summary>
        /// Test reading PWheel cycle format
        /// </summary>
        [Test]
        public void ReadCycle_PWheelFormat_ParsesCorrectly()
        {
            // Arrange
            string cycleContent = 
                "<t>,<Pwheel>,<gear>,<n>\n" +
                "0,0,1,800\n" +
                "10,5000,2,1000\n" +
                "20,10000,3,1200";
            
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(cycleContent));
            
            // Act
            var cycle = DrivingCycleDataReader.ReadFromStream(
                stream, 
                CycleType.PWheel, 
                "PWheelTest", 
                false);
            
            // Assert
            Assert.AreEqual(CycleType.PWheel, cycle.CycleType);
            Assert.AreEqual(3, cycle.Entries.Count);
        }

        #endregion
    }

    /// <summary>
    /// Template tests for validating DeclarationCycleFactory with new cycles
    /// </summary>
    [TestFixture]
    public class DeclarationCycleFactoryTests
    {
        private IDeclarationCycleFactory _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new DeclarationCycleFactory();
        }

        #region Factory Retrieval Tests

        /// <summary>
        /// Test that factory retrieves existing mission cycles
        /// </summary>
        [TestCase(MissionType.LongHaul, Description = "Long Haul mission")]
        [TestCase(MissionType.RegionalDelivery, Description = "Regional Delivery mission")]
        [TestCase(MissionType.UrbanDelivery, Description = "Urban Delivery mission")]
        [TestCase(MissionType.Construction, Description = "Construction mission")]
        [TestCase(MissionType.Urban, Description = "Urban bus mission")]
        [TestCase(MissionType.Coach, Description = "Coach mission")]
        public void GetDeclarationCycle_ExistingMission_ReturnsCycle(MissionType missionType)
        {
            // Arrange
            var mission = new Mission { MissionType = missionType };
            
            // Act
            var cycle = _factory.GetDeclarationCycle(mission);
            
            // Assert
            Assert.IsNotNull(cycle, $"Cycle should exist for {missionType}");
            Assert.AreEqual(CycleType.DistanceBased, cycle.CycleType, 
                "Declaration cycles should be distance-based");
            Assert.Greater(cycle.Entries.Count, 0, "Cycle should have data points");
        }

        /// <summary>
        /// Test cycle properties for specific mission
        /// </summary>
        [Test]
        public void GetDeclarationCycle_LongHaul_HasExpectedProperties()
        {
            // Arrange
            var mission = new Mission { MissionType = MissionType.LongHaul };
            
            // Act
            var cycle = _factory.GetDeclarationCycle(mission);
            
            // Assert
            Assert.IsNotNull(cycle);
            Assert.AreEqual(CycleType.DistanceBased, cycle.CycleType);
            
            // Long haul should have significant distance
            var totalDistance = cycle.Entries[cycle.Entries.Count - 1].Distance.Value();
            Assert.Greater(totalDistance, 10000, "Long haul cycle should cover significant distance");
            
            // Should have high-speed sections
            var maxSpeed = 0.0;
            foreach (var entry in cycle.Entries)
            {
                if (entry.VehicleTargetSpeed.Value() > maxSpeed)
                    maxSpeed = entry.VehicleTargetSpeed.Value();
            }
            Assert.Greater(maxSpeed, 20, "Long haul should have highway speeds");
        }

        /// <summary>
        /// Test that factory caching works (if implemented)
        /// </summary>
        [Test]
        public void GetDeclarationCycle_SameMissionTwice_ReturnsConsistentData()
        {
            // Arrange
            var mission = new Mission { MissionType = MissionType.LongHaul };
            
            // Act
            var cycle1 = _factory.GetDeclarationCycle(mission);
            var cycle2 = _factory.GetDeclarationCycle(mission);
            
            // Assert
            Assert.IsNotNull(cycle1);
            Assert.IsNotNull(cycle2);
            Assert.AreEqual(cycle1.Entries.Count, cycle2.Entries.Count, 
                "Same mission should return same cycle data");
            Assert.AreEqual(cycle1.Name, cycle2.Name, 
                "Cycle names should match");
        }

        #endregion

        #region Cycle Content Validation Tests

        /// <summary>
        /// Test that retrieved cycle has valid structure
        /// </summary>
        [Test]
        public void GetDeclarationCycle_AnyCycle_HasValidStructure()
        {
            // Arrange
            var mission = new Mission { MissionType = MissionType.UrbanDelivery };
            
            // Act
            var cycle = _factory.GetDeclarationCycle(mission);
            
            // Assert
            Assert.Greater(cycle.Entries.Count, 1, "Cycle should have multiple entries");
            
            // Check first entry
            var firstEntry = cycle.Entries[0];
            Assert.IsNotNull(firstEntry, "First entry should exist");
            
            // Check monotonicity
            for (int i = 1; i < cycle.Entries.Count; i++)
            {
                Assert.GreaterOrEqual(
                    cycle.Entries[i].Distance.Value(),
                    cycle.Entries[i - 1].Distance.Value(),
                    $"Distance should be monotonic at entry {i}");
            }
        }

        /// <summary>
        /// Test that cycle starts and ends at zero speed
        /// </summary>
        [Test]
        public void GetDeclarationCycle_StandardMission_StartsAndEndsAtRest()
        {
            // Arrange
            var mission = new Mission { MissionType = MissionType.RegionalDelivery };
            
            // Act
            var cycle = _factory.GetDeclarationCycle(mission);
            
            // Assert
            var firstSpeed = cycle.Entries[0].VehicleTargetSpeed.Value();
            var lastSpeed = cycle.Entries[cycle.Entries.Count - 1].VehicleTargetSpeed.Value();
            
            Assert.AreEqual(0, firstSpeed, 0.01, "Cycle should start at rest");
            Assert.AreEqual(0, lastSpeed, 0.01, "Cycle should end at rest");
        }

        #endregion

        #region Performance Tests

        /// <summary>
        /// Test that factory responds quickly
        /// </summary>
        [Test]
        public void GetDeclarationCycle_Performance_CompletesQuickly()
        {
            // Arrange
            var mission = new Mission { MissionType = MissionType.LongHaul };
            var startTime = DateTime.Now;
            
            // Act
            var cycle = _factory.GetDeclarationCycle(mission);
            var elapsed = DateTime.Now - startTime;
            
            // Assert
            Assert.IsNotNull(cycle);
            Assert.Less(elapsed.TotalSeconds, 2.0, 
                "Cycle retrieval should complete within 2 seconds");
        }

        #endregion
    }

    /// <summary>
    /// Template tests for integration testing with new cycles
    /// Note: These are more complex and may need actual vehicle configuration
    /// </summary>
    [TestFixture]
    public class CycleIntegrationTests
    {
        /// <summary>
        /// Template for full simulation test with custom cycle
        /// Note: Requires proper setup of simulation infrastructure
        /// </summary>
        [Test]
        [Ignore("Template - needs full simulation setup")]
        public void FullSimulation_WithNewCycle_CompletesSuccessfully()
        {
            // Arrange
            // TODO: Set up complete vehicle configuration
            // TODO: Configure simulation with new cycle
            
            // Act
            // TODO: Run simulation
            
            // Assert
            // TODO: Validate results
            Assert.Pass("Template test - implement based on specific requirements");
        }

        /// <summary>
        /// Template for comparing results between cycles
        /// </summary>
        [Test]
        [Ignore("Template - needs baseline cycle for comparison")]
        public void Simulation_NewVsOldCycle_ResultsWithinExpectedRange()
        {
            // Arrange
            // TODO: Run simulation with old cycle
            // TODO: Run simulation with new cycle
            
            // Act
            // TODO: Compare results
            
            // Assert
            // TODO: Verify differences are within acceptable tolerance
            Assert.Pass("Template test - implement based on specific requirements");
        }
    }
}
