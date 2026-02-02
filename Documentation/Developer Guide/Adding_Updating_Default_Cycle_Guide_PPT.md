# Adding/Updating Default Cycles in BharatVECTO
## Presentation Guide for PowerPoint Conversion

---

**Document Purpose**: This file is structured for easy conversion to PowerPoint  
**Conversion Tool**: Use Pandoc, PowerPoint import, or manual slide creation  
**Target Audience**: VECTO Developers, Technical Teams, Regulatory Staff

---

## Slide 1: Title Slide

**Title:**  
Adding or Updating Default Cycle for Vehicle Types in BharatVECTO

**Subtitle:**  
A Comprehensive Process Guide for Testing and Deployment

**Footer:**  
VECTO Development Team | Version 1.0

---

## Slide 2: Agenda

**What We'll Cover:**

1. Introduction to VECTO Cycles
2. Understanding the Architecture
3. Process Overview
4. Step-by-Step Implementation
5. Testing Strategy
6. Deployment Process
7. Best Practices & Troubleshooting

**Duration:** ~45 minutes

---

## Slide 3: What is VECTO?

**VECTO: Vehicle Energy Consumption Calculation TOol**

- Official EU vehicle simulator for Heavy Duty Vehicles (HDVs)
- Certifies CO₂ emissions and fuel consumption
- Mandatory for trucks since January 2019
- Uses standardized driving cycles for simulations

**Key Facts:**
- Developed by European Commission
- Written in C# / .NET Framework
- Based on EU Regulation 2017/2400

---

## Slide 4: What is a Driving Cycle?

**Definition:**  
A standardized speed-time or speed-distance profile representing typical vehicle operation

**Purpose:**
- Simulate real-world driving conditions
- Ensure consistent, reproducible testing
- Support regulatory compliance

**Components:**
- Speed profile over time/distance
- Road gradient information
- Stopping phases
- Optional auxiliary loads

---

## Slide 5: Cycle Types in VECTO

| Cycle Type | Description | Primary Use |
|------------|-------------|-------------|
| **DistanceBased** | Speed/gradient over distance | Declaration mode |
| **EngineOnly** | Engine speed and torque | Engine testing |
| **PWheel** | Power at wheel | Power-based analysis |
| **MeasuredSpeed** | Time-based speed | Engineering mode |
| **MeasuredSpeedGear** | Speed + gear info | Detailed engineering |
| **VTP** | Verification testing | Regulatory compliance |
| **PTO/EPTO** | Power take-off | Auxiliary equipment |

---

## Slide 6: Mission Types

**Truck Missions:**
- Long Haul - Long-distance freight
- Regional Delivery - Regional distribution  
- Urban Delivery - City operations
- Municipal Utility - Municipal services
- Construction - Construction sites

**Bus/Coach Missions:**
- Heavy Urban - Dense city routes
- Urban - City bus operations
- Suburban - Suburban routes
- Interurban - Inter-city connections
- Coach - Long-distance services

---

## Slide 7: System Architecture Overview

```
┌─────────────────────────────────┐
│    VECTO Application            │
└────────────┬────────────────────┘
             │
             ▼
┌─────────────────────────────────┐
│ VectoRunDataFactoryFactory      │
│ (Routes by vehicle type)        │
└────────────┬────────────────────┘
             │
             ▼
┌─────────────────────────────────┐
│ Declaration/Engineering Factory │
│ (Mode-specific factories)       │
└────────────┬────────────────────┘
             │
             ▼
┌─────────────────────────────────┐
│ DeclarationCycleFactory         │
│ (Retrieves default cycles)      │
└────────────┬────────────────────┘
             │
             ▼
┌─────────────────────────────────┐
│ Embedded Resource Cycles        │
│ (.vdri files in resources)      │
└─────────────────────────────────┘
```

---

## Slide 8: Key Components

**1. DeclarationCycleFactory**
- Retrieves default cycles for declaration mode
- Caches cycles for performance
- Reads from embedded resources

**2. DrivingCycleDataReader**
- Parses cycle files (.vdri format)
- Auto-detects cycle type
- Validates cycle data

**3. VectoRunDataFactoryFactory**
- Factory of factories pattern
- Routes to vehicle-specific factory
- Handles mode selection

---

## Slide 9: Process Overview

**8-Step Process:**

1. **Define** cycle parameters
2. **Create** cycle file (.vdri)
3. **Embed** resource or add file
4. **Update** factory (if needed)
5. **Write** unit tests
6. **Run** integration tests
7. **Validate** results
8. **Deploy** and document

**Timeline:** 2-5 days depending on complexity

---

## Slide 10: Step 1 - Define Cycle Parameters

**Key Decisions:**

✓ Cycle Type (DistanceBased, EngineOnly, etc.)  
✓ Mission Type (LongHaul, Urban, etc.)  
✓ Data Requirements (speed, gradient, distance)  
✓ Regulatory Requirements

**Data to Gather:**
- Distance points (meters)
- Speed values (km/h or m/s)
- Road gradient (radians or %)
- Stopping times (seconds)
- Optional: auxiliary loads, wind data

---

## Slide 11: Step 2 - Create Cycle File

**File Format: CSV with specific headers**

**Example - Distance-Based Cycle:**
```csv
<s>,<v>,<grad>,<stop>
0,0,0,10
100,30,0,0
500,50,0.005,0
1000,70,0.01,0
1500,85,0.008,5
2000,0,0,15
```

**Important:**
- Headers in angle brackets: `<header>`
- Case-insensitive
- UTF-8 encoding
- .vdri file extension

---

## Slide 12: Header Reference

| Header | Description | Unit | Required? |
|--------|-------------|------|-----------|
| `<s>` | Distance | meters | Distance-based |
| `<t>` | Time | seconds | Time-based |
| `<v>` | Vehicle speed | km/h | Most types |
| `<grad>` | Road gradient | radians | Optional |
| `<stop>` | Stopping time | seconds | Distance-based |
| `<n>` | Engine speed | rpm | Engine modes |
| `<gear>` | Gear number | - | Gear modes |
| `<Padd>` | Additional power | Watts | Optional |

---

## Slide 13: Step 3 - Embed as Resource

**Two Options:**

**Option A: Embed Resource (Declaration Mode)**
1. Add file to: `VectoCore/Models/Declaration/MissionCycles/`
2. Set Build Action: "Embedded Resource"
3. Resource name: `TUGraz.VectoCore.Models.Declaration.MissionCycles.{MissionType}.vdri`

**Option B: File System (Engineering Mode)**
1. Place in: `Generic Vehicles/Declaration Mode/{Type}/`
2. Reference in vehicle configuration
3. Loaded at runtime

**Recommendation:** Use Option A for default cycles

---

## Slide 14: Step 4 - Update Factory Classes

**When Adding NEW Mission Type:**

**1. Update MissionType Enum**
```csharp
public enum MissionType {
    // ... existing ...
    Coach,
    NewMissionType,  // Add here
    VerificationTest
}
```

**2. Update MissionTypeHelper**
```csharp
case MissionType.NewMissionType:
    return "New Mission Type";
```

**3. DeclarationCycleFactory** (usually automatic)

---

## Slide 15: Step 5 - Write Unit Tests

**Test Categories:**

**1. Cycle Reading Tests**
- Valid format parsing
- Header detection
- Case insensitivity

**2. Validation Tests**
- Monotonic distance/time
- Realistic values
- Edge cases

**3. Factory Tests**
- Cycle retrieval
- Caching behavior
- Multiple missions

**4. Integration Tests**
- Full simulation runs
- Result validation

---

## Slide 16: Example Unit Test

```csharp
[Test]
public void ReadCycle_ValidDistanceBasedFormat_ParsesCorrectly()
{
    // Arrange
    string cycleContent = 
        "<s>,<v>,<grad>,<stop>\n" +
        "0,0,0,10\n" +
        "100,30,0,0\n";
    
    using var stream = new MemoryStream(
        Encoding.UTF8.GetBytes(cycleContent));
    
    // Act
    var cycle = DrivingCycleDataReader.ReadFromStream(
        stream, CycleType.DistanceBased, "Test", false);
    
    // Assert
    Assert.AreEqual(2, cycle.Entries.Count);
    Assert.AreEqual(CycleType.DistanceBased, cycle.CycleType);
}
```

---

## Slide 17: Step 6 - Integration Testing

**Testing Checklist:**

□ Build solution successfully  
□ All unit tests pass  
□ Test with generic vehicle  
□ Test different load conditions  
□ Validate output files (.vres, .vsum)  
□ Manual testing with GUI  
□ Compare with reference data  
□ Check regulatory compliance  

**Tools:**
- NUnit Test Explorer
- VECTO GUI
- Result file validators

---

## Slide 18: Step 7 - Validate Results

**Validation Points:**

**1. Physical Realism**
- Speed values achievable
- Gradient values reasonable
- Energy balance correct

**2. Regulatory Compliance**
- Meets certification requirements
- Follows EU regulations
- Proper documentation

**3. Consistency**
- Results reproducible
- Backwards compatible
- Aligned with similar missions

---

## Slide 19: Step 8 - Deployment

**Pre-Deployment:**
- [ ] All tests passing
- [ ] Code review complete
- [ ] Documentation updated
- [ ] Release notes prepared

**Deployment Steps:**
1. Version update
2. Clean rebuild (Release mode)
3. Staging environment testing
4. Production deployment
5. User communication

**Post-Deployment:**
- Monitor for issues
- Collect user feedback
- Update training materials

---

## Slide 20: Best Practices

**DO:**
✓ Always validate cycle data  
✓ Write comprehensive tests  
✓ Document changes thoroughly  
✓ Follow naming conventions  
✓ Use version control  
✓ Test with multiple vehicles  

**DON'T:**
✗ Skip validation steps  
✗ Modify without testing  
✗ Break backwards compatibility  
✗ Use unrealistic values  
✗ Forget to document  

---

## Slide 21: Common Issues & Solutions

| Issue | Solution |
|-------|----------|
| **Resource not found** | Check embedded resource name and build action |
| **Type detection fails** | Verify header format with angle brackets |
| **Validation errors** | Ensure monotonic distance/time values |
| **Simulation fails** | Check realistic speed/gradient ranges |
| **Factory returns null** | Verify MissionType enum and helpers updated |

**Debugging Tip:** Enable detailed logging in DeclarationCycleFactory

---

## Slide 22: File Locations Reference

| Component | Path |
|-----------|------|
| Cycle Factory | `VectoCore/.../Declaration/IDeclarationCycleFactory.cs` |
| Mission Types | `VectoCommon/Models/MissionType.cs` |
| Cycle Data | `VectoCore/.../SimulationComponent/Data/DrivingCycleData.cs` |
| Reader | `VectoCore/.../Reader/ComponentData/DrivingCycleDataReader.cs` |
| Unit Tests | `Testing/UnitTests/Vecto UnitTests/TestCases/ComponentReader/` |
| Generic Cycles | `Generic Vehicles/Declaration Mode/` |

---

## Slide 23: Tools & Requirements

**Development Tools:**
- Visual Studio 2019+
- .NET Framework 4.5+
- Git version control
- NUnit test runner

**Required Knowledge:**
- C# programming
- VECTO architecture
- Vehicle simulation concepts
- Regulatory requirements

**Access Required:**
- Repository read/write
- Cycle data sources
- Regulatory documentation

---

## Slide 24: Example Workflow Timeline

**Day 1: Planning & Setup**
- Define requirements (2h)
- Gather cycle data (2h)
- Review existing cycles (1h)

**Day 2: Implementation**
- Create cycle file (1h)
- Embed resource (30m)
- Update factory code (2h)
- Initial testing (2h)

**Day 3: Testing**
- Write unit tests (3h)
- Integration testing (3h)

**Day 4: Validation & Documentation**
- Result validation (2h)
- Documentation (3h)

**Day 5: Review & Deploy**
- Code review (2h)
- Deployment prep (2h)
- Deploy & monitor (2h)

---

## Slide 25: Quality Gates

**Before Proceeding to Next Step:**

**Design Phase** → Requirements documented ✓

**Implementation** → Code compiles ✓

**Unit Testing** → All tests pass ✓

**Integration** → Simulation completes ✓

**Validation** → Results within specs ✓

**Deployment** → Staging tests pass ✓

---

## Slide 26: Regulatory Considerations

**EU Regulation 2017/2400:**
- Mandatory CO₂ certification for new trucks
- Standardized testing procedures
- Public reporting requirements

**VECTO's Role:**
- Official certification tool
- Ensures consistency
- Supports monitoring

**When Adding Cycles:**
- Verify regulatory approval
- Document compliance
- Maintain audit trail

---

## Slide 27: Documentation Requirements

**Required Documents:**

1. **Technical Specification**
   - Cycle parameters
   - Data sources
   - Validation criteria

2. **Implementation Guide**
   - Code changes
   - Configuration updates
   - Test procedures

3. **User Documentation**
   - Usage instructions
   - Examples
   - Troubleshooting

4. **Release Notes**
   - Changes summary
   - Impact analysis
   - Migration guide

---

## Slide 28: Success Metrics

**How to Measure Success:**

✓ **Technical:**
- All tests pass (100%)
- Build succeeds
- No regression errors
- Performance maintained

✓ **Functional:**
- Meets requirements
- Regulatory compliant
- User acceptance

✓ **Quality:**
- Code reviewed
- Documented
- Maintainable

---

## Slide 29: Resources & Support

**Documentation:**
- Full guide: `Developer Guide/Adding_Updating_Default_Cycle_Guide.md`
- Test templates: `Developer Guide/CycleAddition_UnitTest_Template.cs`
- User manual: `Documentation/User Manual/help.html`

**Support Channels:**
- Email: JRC-VECTO@ec.europa.eu
- Repository: https://code.europa.eu/vecto/vecto
- Issue tracker: Repository issues

**Training:**
- Developer guide workshops
- Code review sessions
- Pair programming opportunities

---

## Slide 30: Summary & Key Takeaways

**Key Points:**

1. **Understand** the VECTO architecture before starting
2. **Follow** the 8-step process systematically
3. **Test** thoroughly at every stage
4. **Document** all changes comprehensively
5. **Validate** against regulatory requirements
6. **Deploy** carefully with proper staging

**Remember:**
- Quality over speed
- Testing is not optional
- Documentation saves time
- Ask for help when needed

---

## Slide 31: Q&A

**Questions?**

**Common Topics:**
- Specific cycle requirements
- Testing strategies
- Deployment procedures
- Regulatory compliance
- Troubleshooting assistance

**Contact for Follow-up:**
- Development team
- Technical leads
- Regulatory experts

---

## Slide 32: Next Steps

**After This Presentation:**

1. Review full documentation guide
2. Set up development environment
3. Identify your first cycle to implement
4. Schedule code review session
5. Plan testing timeline

**Additional Resources:**
- Schedule 1-on-1 mentoring
- Join developer community
- Access sample code repository

---

## Slide 33: Appendix - Conversion Instructions

**How to Convert This Document to PowerPoint:**

**Method 1: Using Pandoc (Recommended)**
```bash
pandoc -s Adding_Updating_Default_Cycle_Guide_PPT.md \
       -o Cycle_Addition_Guide.pptx \
       --slide-level=2
```

**Method 2: Manual Creation**
1. Open PowerPoint
2. Create slides based on sections
3. Copy content from each slide section
4. Format using organizational template

**Method 3: Import to Google Slides**
1. Upload to Google Drive
2. Open with Google Slides
3. Adjust formatting as needed
4. Export to PowerPoint

**Slide Design Tips:**
- Use organizational template
- Add visuals for architecture diagrams
- Include code snippets as images
- Use animations for process flows
- Add footer with version/date

---

## Document Metadata

**Version:** 1.0  
**Created:** 2024  
**Format:** Markdown for PowerPoint Conversion  
**Sections:** 33 slides  
**Estimated Presentation Time:** 45-60 minutes  
**Target Audience:** Technical developers, project managers, QA teams  
**Maintenance:** Update when process changes or new features added

---

## End of Presentation Guide
