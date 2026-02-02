# Quick Reference: Cycle Addition in BharatVECTO

## One-Page Cheat Sheet

---

## File Format Quick Reference

### Distance-Based Cycle (Declaration Mode)
```csv
<s>,<v>,<grad>,<stop>
0,0,0,10
100,30,0,0
500,50,0.005,0
1000,0,0,15
```

### Time-Based Cycle (Engineering Mode)
```csv
<t>,<v>,<grad>,<Padd>
0,0,0,0
1,5,0.01,1000
2,10,0.02,1500
```

### Engine-Only Cycle
```csv
<t>,<n>,<Me>
0,800,0
10,1000,500
20,1200,800
```

---

## Header Reference

| Header | Type | Unit | Description |
|--------|------|------|-------------|
| `<s>` | Distance | m | Distance traveled |
| `<t>` | Time | s | Time elapsed |
| `<v>` | Speed | km/h | Vehicle speed |
| `<grad>` | Gradient | rad | Road gradient |
| `<stop>` | Time | s | Stopping time |
| `<n>` | Speed | rpm | Engine speed |
| `<gear>` | Number | - | Gear position |
| `<Padd>` | Power | W | Additional power |

---

## 8-Step Process

```
1. Define Parameters    → Choose type, gather data
2. Create File          → Format .vdri correctly
3. Embed Resource       → Build action: Embedded Resource
4. Update Factory       → Add to MissionType enum (if new)
5. Write Tests          → Use templates provided
6. Integration Test     → Full simulation run
7. Validate Results     → Check against specs
8. Deploy               → Stage → Production
```

---

## Key File Locations

```
Cycle Files:        VectoCore/Models/Declaration/MissionCycles/
Factory:            VectoCore/Models/Declaration/IDeclarationCycleFactory.cs
Mission Enum:       VectoCommon/Models/MissionType.cs
Reader:             VectoCore/.../ComponentData/DrivingCycleDataReader.cs
Unit Tests:         Testing/UnitTests/Vecto UnitTests/TestCases/
```

---

## Code Snippets

### Add New Mission Type
```csharp
// 1. VectoCommon/Models/MissionType.cs
public enum MissionType {
    Coach,
    NewMission,  // Add here
    VerificationTest
}

// 2. VectoCore/Models/Declaration/MissionType.cs
case MissionType.NewMission:
    return "New Mission";
```

### Read Cycle File
```csharp
using var stream = File.OpenRead("cycle.vdri");
var cycle = DrivingCycleDataReader.ReadFromStream(
    stream, 
    CycleType.DistanceBased, 
    "MyCycle", 
    false
);
```

### Basic Unit Test
```csharp
[Test]
public void ReadCycle_ValidFormat_ParsesCorrectly()
{
    var content = "<s>,<v>,<grad>,<stop>\n0,0,0,0";
    using var stream = new MemoryStream(
        Encoding.UTF8.GetBytes(content));
    
    var cycle = DrivingCycleDataReader.ReadFromStream(
        stream, CycleType.DistanceBased, "Test", false);
    
    Assert.IsNotNull(cycle);
    Assert.AreEqual(CycleType.DistanceBased, cycle.CycleType);
}
```

---

## Common Commands

### Build Solution
```bash
# Visual Studio
Ctrl+Shift+B

# Command line
dotnet build VECTO.sln
```

### Run Tests
```bash
# All tests
dotnet test

# Specific test
dotnet test --filter "FullyQualifiedName~ReadCycle"
```

### Embed Resource
```xml
<!-- In .csproj file -->
<EmbeddedResource Include="Models\Declaration\MissionCycles\*.vdri" />
```

---

## Validation Rules

✓ Distance/time must be monotonically increasing  
✓ Speed values: 0-90 km/h (typical)  
✓ Gradient: -10% to +10% (typical)  
✓ File encoding: UTF-8  
✓ Extension: .vdri  
✓ Headers: Case-insensitive, in angle brackets  

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Resource not found | Check Build Action = "Embedded Resource" |
| Wrong type detected | Verify header format with `<>` |
| Validation fails | Check monotonic distance/time |
| Factory returns null | Update MissionType enum |

---

## Testing Checklist

- [ ] Unit tests pass
- [ ] Cycle file validated
- [ ] Factory retrieves cycle
- [ ] Integration test runs
- [ ] Results within specs
- [ ] Manual GUI test
- [ ] Documentation updated
- [ ] Code reviewed

---

## Mission Types Reference

**Trucks:**
- LongHaul
- RegionalDelivery
- UrbanDelivery
- MunicipalUtility
- Construction

**Buses:**
- HeavyUrban
- Urban
- Suburban
- Interurban
- Coach

---

## Cycle Types

| Type | Use Case |
|------|----------|
| DistanceBased | Declaration mode |
| EngineOnly | Engine testing |
| PWheel | Power-based analysis |
| MeasuredSpeed | Engineering mode |
| MeasuredSpeedGear | Detailed engineering |
| VTP | Regulatory verification |
| PTO/EPTO | Auxiliary operations |

---

## Example Values

### Realistic Speed Profile
```
Stop:        0 km/h
Urban:      30 km/h
Suburban:   50 km/h
Highway:    80 km/h
Maximum:    90 km/h
```

### Gradient Ranges
```
Flat:        0%
Slight:     ±2%
Moderate:   ±5%
Steep:      ±8%
Maximum:   ±10%
```

---

## Build & Deploy

```bash
# Clean build
Build → Clean Solution
Build → Rebuild Solution (Release)

# Run tests
Test → Run All Tests

# Git workflow
git add .
git commit -m "Add NewCycle"
git push
```

---

## Resources

**Documentation:**
- Full Guide: `Adding_Updating_Default_Cycle_Guide.md`
- Test Templates: `CycleAddition_UnitTest_Template.cs`
- PPT Guide: `Adding_Updating_Default_Cycle_Guide_PPT.md`

**Support:**
- Email: JRC-VECTO@ec.europa.eu
- Repository: https://code.europa.eu/vecto/vecto

---

## Quality Gates

| Stage | Gate |
|-------|------|
| Design | Requirements documented ✓ |
| Code | Compiles ✓ |
| Unit Test | All pass ✓ |
| Integration | Simulation completes ✓ |
| Validation | Results correct ✓ |
| Deploy | Staging tests pass ✓ |

---

## Estimated Timelines

| Task | Time |
|------|------|
| Update existing cycle | 1-2 days |
| Add new mission type | 3-5 days |
| Custom engineering cycle | 1 day |
| Bug fix | 0.5-1 day |

---

## Best Practices

**DO:**
- ✓ Test thoroughly
- ✓ Document changes
- ✓ Follow patterns
- ✓ Request review

**DON'T:**
- ✗ Skip validation
- ✗ Break compatibility
- ✗ Use unrealistic values
- ✗ Deploy untested

---

**Version:** 1.0  
**Last Updated:** 2024  
**For:** VECTO Developers  
**Print:** Single-sided for desk reference
