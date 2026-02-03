# Guide: Adding or Updating Default Cycle for Vehicle Types in BharatVECTO

## Table of Contents
1. [Introduction](#introduction)
2. [Understanding the Current Structure](#understanding-the-current-structure)
3. [Prerequisites](#prerequisites)
4. [Process Overview](#process-overview)
5. [Step-by-Step Guide](#step-by-step-guide)
6. [Testing Your Changes](#testing-your-changes)
7. [Deployment](#deployment)
8. [Troubleshooting](#troubleshooting)
9. [Appendix](#appendix)

---

## Introduction

This guide provides a comprehensive process for adding or updating a new driving cycle as the default cycle for a specific vehicle type in the BharatVECTO (VECTO) repository. VECTO uses standardized driving cycles to simulate energy consumption and CO₂ emissions from Heavy Duty Vehicles (HDVs).

### What is a Driving Cycle?

A driving cycle is a standardized speed-time or speed-distance profile that represents typical vehicle operation patterns. In VECTO, different vehicle types and mission profiles use different driving cycles to accurately simulate real-world conditions.

### When to Use This Guide

Use this guide when you need to:
- Add a new mission profile with a custom driving cycle
- Update an existing default cycle for a vehicle type
- Modify cycle parameters for declaration mode simulations
- Implement new regulatory cycle requirements

---

## Understanding the Current Structure

### Cycle Types in VECTO

VECTO supports several cycle types (defined in `DrivingCycleData.cs`):

| Cycle Type | Description | Use Case |
|------------|-------------|----------|
| `DistanceBased` | Speed and gradient over distance | Declaration mode missions |
| `EngineOnly` | Engine speed and torque profiles | Engine-only testing |
| `PWheel` | Power at wheel measurements | Power-based simulations |
| `MeasuredSpeed` | Time-based speed measurements | Engineering mode |
| `MeasuredSpeedGear` | Speed with gear information | Detailed engineering analysis |
| `PTO` / `EPTO` | Power Take-Off cycles | Auxiliary equipment operations |
| `VTP` | Verification Test Procedure | Regulatory compliance testing |

### Mission Types

Mission types define the operational profile of vehicles (from `MissionType.cs`):

**Truck Missions:**
- `LongHaul` - Long-distance freight transport
- `LongHaul1` - Long-distance freight transport for Rigid truck 4x2 (same as LongHaul)
- `RegionalDelivery` - Regional distribution
- `UrbanDelivery` - City delivery operations
- `MunicipalUtility` - Municipal services
- `Construction` - Construction site operations

**Bus/Coach Missions:**
- `HeavyUrban` - Dense city bus routes
- `Urban` - City bus operations
- `Suburban` - Suburban bus routes
- `Interurban` - Inter-city connections
- `Coach` - Long-distance coach services

### Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    VECTO Application                         │
└─────────────────┬───────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────┐
│          VectoRunDataFactoryFactory                          │
│  (Determines which factory to use based on vehicle type)     │
└─────────────────┬───────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────┐
│           Declaration/Engineering Mode Factory               │
│  - DeclarationModePrimaryBusRunDataFactory                   │
│  - DeclarationModeHeavyLorryRunDataFactory                   │
│  - EngineeringModeVectoRunDataFactory                        │
└─────────────────┬───────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────┐
│           DeclarationCycleFactory                            │
│  (Retrieves default cycles for missions)                     │
└─────────────────┬───────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────┐
│           Embedded Resource Cycles                           │
│  (DeclarationData.MissionCycles.{MissionType}.vdri)         │
└─────────────────────────────────────────────────────────────┘
```

### Key Components

1. **DeclarationCycleFactory** (`IDeclarationCycleFactory.cs`)
   - Retrieves default cycles for declaration mode
   - Caches cycles for performance
   - Reads from embedded resources

2. **DrivingCycleDataReader** (`DrivingCycleDataReader.cs`)
   - Parses cycle files (.vdri format)
   - Auto-detects cycle type from file headers
   - Validates cycle data

3. **VectoRunDataFactoryFactory** (`VectoRunDataFactoryFactory.cs`)
   - Factory of factories pattern
   - Routes to correct vehicle-type specific factory
   - Handles declaration vs. engineering mode

---

## Prerequisites

### Required Knowledge
- C# programming and .NET Framework
- Understanding of VECTO architecture
- Familiarity with vehicle simulation concepts
- Git version control basics

### Required Tools
- Visual Studio 2019 or later
- .NET Framework 4.5+
- Git for version control
- Text editor for cycle files
- NUnit test runner (for testing)

### Required Access
- Read/write access to VECTO repository
- Understanding of regulatory requirements for the cycle
- Access to cycle data (speed, gradient, distance/time profiles)

---

## Process Overview

```
┌──────────────────────┐
│  1. Define Cycle     │
│     Parameters       │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│  2. Create Cycle     │
│     File (.vdri)     │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│  3. Embed Resource   │
│     or Add File      │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│  4. Update Factory   │
│     (if needed)      │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│  5. Write Unit Tests │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│  6. Integration Test │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│  7. Validate Results │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│  8. Deploy & Document│
└──────────────────────┘
```

---

## Step-by-Step Guide

### Step 1: Define Cycle Parameters

Before creating a cycle file, you need to define the complete cycle parameters.

#### 1.1 Determine Cycle Type

Choose the appropriate cycle type based on your use case:

- **Declaration Mode**: Use `DistanceBased` cycle
- **Engineering Mode**: Use `MeasuredSpeed`, `MeasuredSpeedGear`, or `PWheel`
- **Engine Testing**: Use `EngineOnly`
- **Verification**: Use `VTP`

#### 1.2 Define Mission Type

Determine which mission type will use this cycle:
- For new missions: Add to `MissionType` enum in `VectoCommon/Models/MissionType.cs`
- For existing missions: Identify the `MissionType` enum value

#### 1.3 Gather Cycle Data

Collect the required data points for your cycle:

**For Distance-Based Cycles (Declaration Mode):**
- Distance (s) in meters
- Vehicle speed (v) in km/h or m/s
- Road gradient (grad) in radians or %
- Stopping time (stop) in seconds

**For Time-Based Cycles (Engineering Mode):**
- Time (t) in seconds
- Vehicle speed (v) in km/h or m/s
- Road gradient (grad) in radians or %
- Additional power demand (Padd) in Watts
- Auxiliary power demands (optional)

#### 1.4 Validate Cycle Data

Ensure your cycle data meets these requirements:
- Monotonically increasing distance or time values
- Physically realistic speed and gradient values
- Proper units and formatting
- No missing or invalid data points

### Step 2: Create Cycle File (.vdri)

#### 2.1 File Format

Cycle files use CSV format with specific headers. The header determines the cycle type.

**Distance-Based Cycle Example:**
```csv
<s>,<v>,<grad>,<stop>
0,0,0,0
100,10,0.01,0
200,20,0.02,5
300,30,0.01,0
...
```

**Time-Based Cycle Example:**
```csv
<t>,<v>,<grad>,<Padd>
0,0,0,0
1,5,0.01,1000
2,10,0.02,1500
3,15,0.01,2000
...
```

#### 2.2 Header Conventions

Headers must be enclosed in angle brackets `<>` for automatic type detection:

| Header | Description | Unit | Required |
|--------|-------------|------|----------|
| `<s>` | Distance | meters | For distance-based |
| `<t>` | Time | seconds | For time-based |
| `<v>` | Vehicle speed | km/h | Yes (except engine-only) |
| `<grad>` | Road gradient | radians or % | Optional |
| `<stop>` | Stopping time | seconds | For distance-based |
| `<Padd>` | Additional power | Watts | Optional |
| `<n>` | Engine speed | rpm | For engine/gear modes |
| `<gear>` | Gear number | - | For gear modes |
| `<Me>` | Engine torque | Nm | For engine-only |
| `<Pwheel>` | Wheel power | Watts | For PWheel mode |

**Note:** Headers are case-insensitive but must match expected patterns.

#### 2.3 Create the File

1. Create a new text file with `.vdri` extension
2. Add the appropriate header line
3. Add data rows (one per cycle point)
4. Save with UTF-8 encoding

**Example: LongHaul.vdri**
```csv
<s>,<v>,<grad>,<stop>
0,0,0,10
100,30,0,0
500,50,0.005,0
1000,70,0.01,0
1500,85,0.008,5
2000,85,0,0
2500,70,-0.005,0
3000,50,-0.01,0
3500,30,-0.008,0
4000,0,0,15
```

#### 2.4 Validate Syntax

Use the `DrivingCycleDataReader` to validate:
```csharp
using var stream = File.OpenRead("LongHaul.vdri");
var cycle = DrivingCycleDataReader.ReadFromStream(stream, CycleType.DistanceBased, "LongHaul", false);
// If no exception, cycle is valid
```

### Step 3: Embed as Resource or Add to File System

You have two options for making cycles available:

#### Option A: Embed as Resource (Recommended for Declaration Mode)

**For declaration mode default cycles**, embed in the VectoCore assembly:

1. **Add file to project structure:**
   ```
   VectoCore/VectoCore/Models/Declaration/MissionCycles/{MissionType}.vdri
   ```

2. **Set Build Action to Embedded Resource:**
   - Right-click the file in Visual Studio
   - Properties → Build Action → Embedded Resource
   - Or edit `.csproj` file:
   ```xml
   <EmbeddedResource Include="Models\Declaration\MissionCycles\LongHaul.vdri" />
   ```

3. **Verify resource naming:**
   The resource name should be:
   ```
   TUGraz.VectoCore.Models.Declaration.MissionCycles.{MissionType}.vdri
   ```

4. **Update DeclarationCycleFactory if needed:**
   The factory automatically reads from embedded resources using the mission name.

#### Option B: File System (For Engineering Mode)

**For engineering mode or user-provided cycles:**

1. Place cycle files in appropriate directory:
   ```
   Generic Vehicles/Declaration Mode/{VehicleType}/{CycleName}.vdri
   ```

2. Reference in vehicle configuration files (.vveh or job files)

3. Cycle will be loaded at runtime via `DrivingCycleDataReader`

### Step 4: Update Factory Classes (If Adding New Mission Type)

If you're adding a completely new mission type (not just updating an existing cycle):

#### 4.1 Update MissionType Enum

Edit `VectoCommon/VectoCommon/Models/MissionType.cs`:

```csharp
public enum MissionType
{
    // Existing missions...
    Coach,
    
    // Add your new mission
    NewMissionType,
    
    VerificationTest,
    ExemptedMission
}
```

#### 4.2 Update MissionTypeHelper

Edit `VectoCore/VectoCore/Models/Declaration/MissionType.cs`:

```csharp
public static string ToXMLFormat(this MissionType self)
{
    switch (self) {
        // Existing cases...
        case MissionType.Coach:
            return "Coach";
        case MissionType.NewMissionType:
            return "New Mission Type";  // XML display name
        // ...
    }
}

public static Kilogram GetAveragePassengerMass(this MissionType self)
{
    switch (self) {
        // Add case for new mission type
        case MissionType.NewMissionType:
            return 75.SI<Kilogram>();  // Average passenger mass if applicable
        // ...
    }
}
```

#### 4.3 Update DeclarationCycleFactory (If Custom Logic Needed)

Most of the time, `DeclarationCycleFactory` works automatically. However, if you need custom cycle loading logic:

Edit `VectoCore/VectoCore/Models/Declaration/IDeclarationCycleFactory.cs`:

```csharp
protected virtual DrivingCycleData ReadDeclarationCycle(MissionType missionType)
{
    // Special handling for specific mission types
    if (missionType == MissionType.NewMissionType) {
        // Custom logic here if needed
    }
    
    // Default behavior - reads from embedded resource
    var cycle = RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix +
                                        ".MissionCycles." +
                                        missionType.ToString().Replace("EMS", "") +
                                        Constants.FileExtensions.CycleFile);
    
    return DrivingCycleDataReader.ReadFromStream(cycle, CycleType.DistanceBased, "", false);
}
```

#### 4.4 Update VectoRunDataFactoryFactory (If New Vehicle Class)

Only needed if adding a completely new vehicle classification:

Edit `VectoCore/VectoCore/InputData/Reader/VectoRunDataFactoryFactory.cs`:

```csharp
public IVectoRunDataFactory CreateVectoRunDataFactory(...)
{
    // Add logic to route to appropriate factory
    if (isNewVehicleType) {
        return new NewVehicleTypeRunDataFactory(...);
    }
    // ...
}
```

### Step 5: Write Unit Tests

Testing is crucial to ensure cycles work correctly across all scenarios.

#### 5.1 Create Cycle Reader Test

Create or update test file in `Testing/UnitTests/Vecto UnitTests/TestCases/ComponentReader/`:

```csharp
using NUnit.Framework;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentReader
{
    [TestFixture]
    public class NewCycleReaderTests
    {
        [Test]
        public void ReadCycle_ValidFormat_ReturnsCorrectCycleType()
        {
            // Arrange
            string cycleContent = "<s>,<v>,<grad>,<stop>\n0,0,0,0\n100,10,0.01,0";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(cycleContent));
            
            // Act
            var cycle = DrivingCycleDataReader.ReadFromStream(
                stream, CycleType.DistanceBased, "TestCycle", false);
            
            // Assert
            Assert.AreEqual(CycleType.DistanceBased, cycle.CycleType);
            Assert.AreEqual("TestCycle", cycle.Name);
            Assert.AreEqual(2, cycle.Entries.Count);
        }
        
        [Test]
        public void ReadCycle_MonotonicDistance_NoException()
        {
            // Arrange
            string cycleContent = "<s>,<v>,<grad>,<stop>\n0,0,0,0\n100,10,0,0\n200,20,0,0";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(cycleContent));
            
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var cycle = DrivingCycleDataReader.ReadFromStream(
                    stream, CycleType.DistanceBased, "TestCycle", false);
                cycle.ValidateCycleData(cycle, null);
            });
        }
        
        [Test]
        public void ReadCycle_NonMonotonicDistance_ThrowsException()
        {
            // Arrange
            string cycleContent = "<s>,<v>,<grad>,<stop>\n100,10,0,0\n50,20,0,0";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(cycleContent));
            
            // Act & Assert
            Assert.Throws<Exception>(() => {
                var cycle = DrivingCycleDataReader.ReadFromStream(
                    stream, CycleType.DistanceBased, "TestCycle", false);
                cycle.ValidateCycleData(cycle, null);
            });
        }
    }
}
```

#### 5.2 Create Declaration Cycle Factory Test

Test that the factory correctly retrieves your new cycle:

```csharp
using NUnit.Framework;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCommon.Models;

namespace TUGraz.Vecto.UnitTests.TestCases.Declaration
{
    [TestFixture]
    public class DeclarationCycleFactoryTests
    {
        private DeclarationCycleFactory _factory;
        
        [SetUp]
        public void Setup()
        {
            _factory = new DeclarationCycleFactory();
        }
        
        [Test]
        public void GetDeclarationCycle_LongHaul_ReturnsCycle()
        {
            // Arrange
            var mission = new Mission { MissionType = MissionType.LongHaul };
            
            // Act
            var cycle = _factory.GetDeclarationCycle(mission);
            
            // Assert
            Assert.IsNotNull(cycle);
            Assert.AreEqual(CycleType.DistanceBased, cycle.CycleType);
            Assert.Greater(cycle.Entries.Count, 0);
        }
        
        [Test]
        public void GetDeclarationCycle_NewMissionType_ReturnsCycle()
        {
            // Arrange
            var mission = new Mission { MissionType = MissionType.NewMissionType };
            
            // Act
            var cycle = _factory.GetDeclarationCycle(mission);
            
            // Assert
            Assert.IsNotNull(cycle);
            Assert.AreEqual(CycleType.DistanceBased, cycle.CycleType);
            Assert.IsNotEmpty(cycle.Entries);
        }
        
        [Test]
        public void GetDeclarationCycle_SameMissionTwice_ReturnsSameInstance()
        {
            // Arrange
            var mission = new Mission { MissionType = MissionType.LongHaul };
            
            // Act
            var cycle1 = _factory.GetDeclarationCycle(mission);
            var cycle2 = _factory.GetDeclarationCycle(mission);
            
            // Assert
            // Should return cached instance (if caching is enabled)
            Assert.IsNotNull(cycle1);
            Assert.IsNotNull(cycle2);
        }
    }
}
```

#### 5.3 Integration Test

Create integration test in `Testing/IntegrationTests/` or `VectoCore/VectoCoreTest/Integration/`:

```csharp
using NUnit.Framework;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.VectoCoreTest.Integration.Declaration
{
    [TestFixture]
    public class NewCycleIntegrationTest
    {
        [Test]
        public void FullSimulation_WithNewCycle_CompletesSuccessfully()
        {
            // Arrange
            // Set up complete vehicle configuration with new cycle
            var inputData = CreateTestInputData();
            var simulator = CreateSimulator(inputData);
            
            // Act
            var result = simulator.Run();
            
            // Assert
            Assert.IsNotNull(result);
            Assert.Greater(result.TotalDistance, 0);
            Assert.Greater(result.FuelConsumption, 0);
            // Add more specific assertions based on expected results
        }
        
        private VectoInputData CreateTestInputData()
        {
            // Implementation: Create test vehicle with new cycle
            return null; // Placeholder
        }
        
        private ISimulator CreateSimulator(VectoInputData inputData)
        {
            // Implementation: Create simulator instance
            return null; // Placeholder
        }
    }
}
```

---

## Testing Your Changes

### Build and Compile

1. **Build the solution:**
   ```
   Visual Studio: Build → Build Solution (Ctrl+Shift+B)
   Or via command line: dotnet build VECTO.sln
   ```

2. **Check for compilation errors:**
   - Fix any namespace issues
   - Resolve missing references
   - Ensure embedded resources are properly included

### Run Unit Tests

1. **Run all cycle-related tests:**
   ```
   Test Explorer → Run All Tests in ComponentReader folder
   ```

2. **Run specific test:**
   ```
   Test Explorer → Right-click test → Run
   ```

3. **Verify test results:**
   - All tests should pass
   - Pay attention to any warnings
   - Check test coverage

### Integration Testing

1. **Test with generic vehicle:**
   - Use an existing generic vehicle configuration
   - Replace its cycle with your new cycle
   - Run a complete simulation
   - Verify outputs (fuel consumption, CO₂, distance, time)

2. **Test with different load conditions:**
   - Test with minimum payload
   - Test with maximum payload
   - Test with average payload

3. **Validate against expected results:**
   - Compare with reference data (if available)
   - Check that results are physically realistic
   - Verify compliance with regulatory requirements

### Manual Testing

1. **Use VECTO GUI:**
   - Launch VECTO application
   - Load a vehicle configuration
   - Select your new mission/cycle
   - Run simulation and review results

2. **Check output files:**
   - Review .vres (result) files
   - Check .vsum (summary) files
   - Verify .vmod (model) outputs

3. **Validate cycle visualization:**
   - Check speed profile graphs
   - Verify gradient profile
   - Ensure stopping phases are correct

---

## Deployment

### Pre-Deployment Checklist

- [ ] All unit tests pass
- [ ] Integration tests pass
- [ ] Manual testing completed
- [ ] Code reviewed by peer
- [ ] Documentation updated
- [ ] Release notes prepared
- [ ] Backwards compatibility verified

### Versioning

1. **Update version numbers:**
   - Assembly version in project properties
   - Application version in constants
   - Documentation version references

2. **Tag release:**
   ```bash
   git tag -a v3.4.0 -m "Added NewMissionType cycle"
   git push origin v3.4.0
   ```

### Build Release

1. **Clean build:**
   ```
   Build → Clean Solution
   Build → Rebuild Solution (Release configuration)
   ```

2. **Create deployment package:**
   - Include all necessary assemblies
   - Include generic vehicle files with new cycles
   - Include updated documentation
   - Create installation instructions

### Deployment Steps

1. **Update embedded resources:**
   - Ensure all new .vdri files are embedded
   - Verify resource names match expected patterns

2. **Deploy to staging environment:**
   - Test in staging before production
   - Run full regression test suite
   - Validate with sample vehicles

3. **Deploy to production:**
   - Follow organizational deployment procedures
   - Notify stakeholders of changes
   - Monitor for issues post-deployment

4. **Update documentation:**
   - User manual updates
   - Release notes
   - Technical documentation
   - Training materials

---

## Troubleshooting

### Common Issues and Solutions

#### Issue 1: Cycle File Not Found

**Symptoms:**
- Exception: "Resource not found"
- Cycle fails to load

**Solutions:**
- Verify embedded resource name matches expected pattern
- Check Build Action is set to "Embedded Resource"
- Rebuild project to include resource
- Check resource naming convention:
  ```
  TUGraz.VectoCore.Models.Declaration.MissionCycles.{MissionType}.vdri
  ```

#### Issue 2: Cycle Type Auto-Detection Fails

**Symptoms:**
- Wrong cycle type detected
- Parsing errors

**Solutions:**
- Verify header format matches expected patterns
- Ensure headers are enclosed in angle brackets `<>`
- Check for typos in header names
- Verify column order matches expected format
- Consult `DrivingCycleReaderTests.cs` for valid header examples

#### Issue 3: Validation Errors

**Symptoms:**
- "Cycle is not monotonous" error
- Data validation failures

**Solutions:**
- Check that distance/time values are strictly increasing
- Remove or fix duplicate distance/time values
- Ensure no missing data points
- Validate data types (numbers, not text)
- Check for proper decimal separators (use . not ,)

#### Issue 4: Simulation Fails to Complete

**Symptoms:**
- Simulation hangs or crashes
- Unexpected results

**Solutions:**
- Verify cycle has realistic speed and gradient values
- Check stopping time values are appropriate
- Ensure cycle distance/duration is reasonable
- Validate that vehicle can achieve requested speeds
- Check for division by zero conditions (speed = 0)

#### Issue 5: Factory Cannot Find New Mission Type

**Symptoms:**
- Exception when requesting cycle for new mission
- Factory returns null or default cycle

**Solutions:**
- Ensure `MissionType` enum includes new type
- Verify `MissionTypeHelper` has cases for new type
- Check embedded resource naming matches mission name
- Rebuild solution after enum changes
- Clear and rebuild to ensure updated assemblies

### Debugging Tips

1. **Enable detailed logging:**
   ```csharp
   // Add logging in DeclarationCycleFactory
   Logger.Debug($"Loading cycle for mission: {missionType}");
   ```

2. **Use debugger:**
   - Set breakpoints in `DeclarationCycleFactory.ReadDeclarationCycle()`
   - Step through `DrivingCycleDataReader.ReadFromStream()`
   - Inspect cycle entries after parsing

3. **Validate resource availability:**
   ```csharp
   var assembly = Assembly.GetExecutingAssembly();
   var resourceNames = assembly.GetManifestResourceNames();
   // Check if your .vdri resource is in the list
   ```

4. **Test cycle file directly:**
   ```csharp
   using var fileStream = File.OpenRead("path/to/cycle.vdri");
   var cycle = DrivingCycleDataReader.ReadFromStream(
       fileStream, CycleType.DistanceBased, "Test", false);
   ```

---

## Appendix

### A. Cycle File Format Reference

#### Distance-Based Cycle Headers

| Header Combination | Use Case | Declaration | Engineering |
|-------------------|----------|-------------|-------------|
| `<s>,<v>,<grad>,<stop>` | Standard declaration | ✓ | ✗ |
| `<s>,<v>,<grad>,<stop>,<Padd>` | With additional power | ✗ | ✓ |
| `<s>,<v>,<grad>,<stop>,<Padd>,<vair_res>,<vair_beta>` | With crosswind | ✗ | ✓ |
| `<s>,<v>,<grad>,<stop>,<Padd>,<Aux_XXX>` | With auxiliaries | ✗ | ✓ |

#### Time-Based Cycle Headers

| Header Combination | Use Case | Cycle Type |
|-------------------|----------|------------|
| `<t>,<n>,<Me>` | Engine torque | EngineOnly |
| `<t>,<n>,<Pe>` | Engine power | EngineOnly |
| `<t>,<Pwheel>,<gear>,<n>` | Wheel power | PWheel |
| `<t>,<v>,<grad>` | Measured speed | MeasuredSpeed |
| `<t>,<v>,<grad>,<n>,<gear>` | Speed with gear | MeasuredSpeedGear |

### B. Example Cycles

#### Example 1: Simple Long Haul Cycle
```csv
<s>,<v>,<grad>,<stop>
0,0,0,30
1000,40,0,0
5000,60,0.005,0
10000,80,0.01,0
20000,85,0.005,0
30000,85,0,10
40000,80,-0.005,0
45000,60,-0.01,0
48000,40,-0.005,0
50000,0,0,30
```

#### Example 2: Urban Delivery Cycle
```csv
<s>,<v>,<grad>,<stop>
0,0,0,15
200,20,0,5
400,30,0.01,0
600,25,0,10
800,30,0,0
1000,20,-0.01,5
1200,0,0,20
```

#### Example 3: Engine-Only Cycle
```csv
<t>,<n>,<Me>
0,800,0
10,1000,500
20,1200,800
30,1400,1000
40,1600,1200
50,1400,1000
60,1200,800
70,1000,500
80,800,0
```

### C. Glossary

| Term | Definition |
|------|------------|
| **Declaration Mode** | Certification mode using standardized cycles and parameters |
| **Engineering Mode** | Development mode with custom cycles and detailed parameters |
| **Mission Profile** | Operational scenario (e.g., Long Haul, Urban Delivery) |
| **Driving Cycle** | Speed/distance/time profile representing vehicle operation |
| **Factory Pattern** | Design pattern for creating objects based on type |
| **Embedded Resource** | File compiled into assembly as binary resource |
| **VTP** | Verification Test Procedure for regulatory compliance |
| **PTO** | Power Take-Off for auxiliary equipment |
| **HDV** | Heavy Duty Vehicle |
| **CO₂** | Carbon Dioxide emissions |

### D. Related Files and Locations

| Component | File Path | Purpose |
|-----------|-----------|---------|
| Cycle Factory | `VectoCore/VectoCore/Models/Declaration/IDeclarationCycleFactory.cs` | Cycle retrieval logic |
| Mission Types | `VectoCommon/VectoCommon/Models/MissionType.cs` | Mission enum definitions |
| Cycle Data | `VectoCore/VectoCore/Models/SimulationComponent/Data/DrivingCycleData.cs` | Cycle data structure |
| Reader | `VectoCore/VectoCore/InputData/Reader/ComponentData/DrivingCycleDataReader.cs` | Cycle parsing |
| Factory Factory | `VectoCore/VectoCore/InputData/Reader/VectoRunDataFactoryFactory.cs` | Factory routing |
| Tests | `Testing/UnitTests/Vecto UnitTests/TestCases/ComponentReader/` | Unit tests |
| Generic Cycles | `Generic Vehicles/Declaration Mode/` | Sample cycle files |

### E. Regulatory References

- **EU Regulation 2017/2400**: CO₂ emission certification for HDVs
- **WLTC**: Worldwide harmonized Light vehicles Test Cycle
- **VECTO Official Documentation**: [EC VECTO Page](https://climate.ec.europa.eu/eu-action/transport-emissions/road-transport-reducing-co2-emissions-vehicles/vehicle-energy-consumption-calculation-tool-vecto_en)

### F. Change History Template

When updating cycles, document changes:

```markdown
## Cycle Update History

### Version 3.4.0 - 2024-XX-XX
- **Change**: Added NewMissionType cycle
- **Reason**: Regulatory requirement for new vehicle category
- **Impact**: New mission profile available in declaration mode
- **Testing**: Full regression suite passed
- **Validated By**: [Name]
- **Approved By**: [Name]

### Version 3.3.9 - 2024-XX-XX
- **Change**: Updated LongHaul cycle gradient profile
- **Reason**: Improved real-world accuracy
- **Impact**: 2% change in fuel consumption for long haul missions
- **Testing**: Validated against field data
- **Validated By**: [Name]
- **Approved By**: [Name]
```

### G. Support and Contact

For questions or issues related to cycles:

- **Email**: JRC-VECTO@ec.europa.eu
- **Repository**: https://code.europa.eu/vecto/vecto
- **User Manual**: `Documentation/User Manual/help.html`
- **Developer Guide**: `Documentation/Developer Guide/VECTO Developer Guide.docx`

---

## Conclusion

This guide provides a complete process for adding or updating default cycles in the BharatVECTO repository. By following these steps, you can:

1. ✓ Understand the VECTO cycle architecture
2. ✓ Create properly formatted cycle files
3. ✓ Integrate cycles into the factory system
4. ✓ Test thoroughly at unit and integration levels
5. ✓ Deploy safely with proper validation

Remember to:
- Always test thoroughly before deployment
- Document all changes
- Follow the established patterns and conventions
- Validate results against expected behavior
- Maintain backwards compatibility where possible

For additional assistance, refer to the existing codebase examples, unit tests, and the VECTO developer documentation.

---

**Document Version**: 1.0  
**Last Updated**: 2024  
**Author**: VECTO Development Team  
**Status**: Active
