# Cycle Addition and Update Documentation

This directory contains comprehensive documentation for adding or updating default driving cycles for vehicle types in the BharatVECTO repository.

## Available Documentation

### 1. Comprehensive Guide (Markdown)
**File:** `Adding_Updating_Default_Cycle_Guide.md`

**Description:** Complete step-by-step guide covering:
- Understanding VECTO cycle architecture
- Creating and formatting cycle files
- Embedding cycles as resources
- Updating factory classes
- Writing comprehensive unit tests
- Integration testing procedures
- Deployment and validation
- Troubleshooting common issues

**Use This For:**
- Detailed technical implementation
- Reference during development
- Understanding the complete process
- Finding specific technical details

**Format:** Markdown (can be viewed in any text editor or GitHub)

---

### 2. Unit Test Templates
**File:** `CycleAddition_UnitTest_Template.cs`

**Description:** Production-ready unit test templates including:
- Basic cycle reading tests
- Validation tests
- Edge case handling
- Factory retrieval tests
- Integration test templates
- Performance tests

**Use This For:**
- Starting point for writing tests
- Understanding test patterns
- Ensuring comprehensive test coverage
- Copy-paste test examples

**Format:** C# source code (copy to test project and adapt)

---

### 3. PowerPoint Presentation (Ready to Use) ⭐ NEW
**File:** `Cycle_Addition_Process_Guide.pptx`

**Description:** Professional PowerPoint presentation (20 slides):
- System architecture diagrams
- 8-step implementation process
- Component relationships and integration
- Cascading effects analysis
- Testing strategies and validation
- Best practices and troubleshooting
- File locations reference
- Deployment checklist

**Use This For:**
- Team training sessions (45-60 minutes)
- Executive overviews (15-20 minutes)
- QA team briefings (30 minutes)
- Developer onboarding
- Stakeholder presentations

**Compatible With:**
- Microsoft PowerPoint 2007+
- PowerPoint Online
- LibreOffice Impress
- Google Slides
- Apple Keynote

**Supporting Documentation:**
- `README_Cycle_Addition_Presentation.md` - Usage guide for different audiences
- `DELIVERY_SUMMARY_Cycle_Presentation.md` - Complete delivery documentation

---

### 4. PowerPoint Presentation Guide (Markdown Source)
**File:** `Adding_Updating_Default_Cycle_Guide_PPT.md`

**Description:** Markdown source for presentation slides:
- 33 slides covering the complete process
- Visual flow diagrams
- Step-by-step workflows
- Best practices and troubleshooting
- Can be converted to PowerPoint if customization needed

**Use This For:**
- Converting to custom PowerPoint format
- Creating organization-specific presentations
- Reference material in markdown format

**How to Convert to PowerPoint:**

#### Option 1: Using Pandoc (Recommended)
```bash
pandoc -s Adding_Updating_Default_Cycle_Guide_PPT.md \
       -o Cycle_Addition_Guide.pptx \
       --slide-level=2
```

#### Option 2: Manual Creation
1. Open PowerPoint
2. Create new presentation
3. Copy each slide section as a new slide
4. Format using your organization's template

#### Option 3: Google Slides
1. Upload the Markdown file to Google Drive
2. Open with Google Slides
3. Adjust formatting
4. Export as PowerPoint

---

## Quick Start

### For First-Time Users:
1. ⭐ Start with the **PowerPoint Presentation** (`Cycle_Addition_Process_Guide.pptx`) for a visual overview
2. Read the **Comprehensive Guide** (`Adding_Updating_Default_Cycle_Guide.md`) for detailed steps
3. Use **Unit Test Templates** (`CycleAddition_UnitTest_Template.cs`) when implementing
4. Refer to **Quick Reference** (`Cycle_Addition_Quick_Reference.md`) as a cheat sheet

### For Experienced Developers:
1. Refer to the **Quick Reference** for rapid lookup
2. Copy **Unit Test Templates** directly
3. Use **Comprehensive Guide** as needed for details
4. Present with **PowerPoint Presentation** for team alignment

### For Managers/Reviewers:
1. ⭐ Review the **PowerPoint Presentation** for complete process overview and impact analysis
2. Check the **Comprehensive Guide** for quality standards and validation criteria
3. Reference **Unit Test Templates** for quality gates
4. Use **DELIVERY_SUMMARY** for detailed documentation validation

---

## File Organization

```
Documentation/Developer Guide/
├── README_Cycle_Documentation.md                     ← This file (start here)
│
├── Cycle_Addition_Process_Guide.pptx                 ← ⭐ NEW: PowerPoint presentation
├── README_Cycle_Addition_Presentation.md             ← ⭐ NEW: Presentation usage guide
├── DELIVERY_SUMMARY_Cycle_Presentation.md            ← ⭐ NEW: Delivery documentation
│
├── Adding_Updating_Default_Cycle_Guide.md            ← Main technical guide
├── Cycle_Addition_Quick_Reference.md                 ← One-page cheat sheet
├── CycleAddition_UnitTest_Template.cs                ← Test templates
└── Adding_Updating_Default_Cycle_Guide_PPT.md        ← Presentation markdown source
```

---

## Related Documentation

### In This Repository:

- **User Manual**: `Documentation/User Manual/help.html`
- **VECTO Developer Guide**: `Documentation/Developer Guide/VECTO Developer Guide.docx`
- **Existing Test Examples**: `Testing/UnitTests/Vecto UnitTests/TestCases/ComponentReader/`
- **Sample Cycles**: `Generic Vehicles/Declaration Mode/`

### External Resources:

- **VECTO Official Site**: https://climate.ec.europa.eu/eu-action/transport-emissions/road-transport-reducing-co2-emissions-vehicles/vehicle-energy-consumption-calculation-tool-vecto_en
- **EU Regulation 2017/2400**: Official HDV CO₂ certification regulation
- **Repository**: https://code.europa.eu/vecto/vecto

---

## Process Overview

The complete cycle addition process follows these 8 steps:

```
1. Define Cycle Parameters
   ↓
2. Create Cycle File (.vdri)
   ↓
3. Embed as Resource or Add to File System
   ↓
4. Update Factory Classes (if adding new mission type)
   ↓
5. Write Unit Tests
   ↓
6. Run Integration Tests
   ↓
7. Validate Results
   ↓
8. Deploy and Document
```

**Estimated Timeline:** 2-5 days depending on complexity

---

## Key Components Modified

When adding or updating cycles, you typically interact with:

| Component | Location | Purpose |
|-----------|----------|---------|
| Cycle Files | `VectoCore/Models/Declaration/MissionCycles/` | Cycle data |
| Factory | `VectoCore/Models/Declaration/IDeclarationCycleFactory.cs` | Cycle loading |
| Mission Types | `VectoCommon/Models/MissionType.cs` | Mission definitions |
| Tests | `Testing/UnitTests/Vecto UnitTests/` | Validation |

---

## Common Use Cases

### Use Case 1: Update Existing Cycle
**Example:** Modify the LongHaul cycle for improved accuracy

**Steps:**
1. Update the `.vdri` file in embedded resources
2. Write validation tests
3. Run regression tests
4. Document changes
5. Deploy

**Time Required:** 1-2 days

---

### Use Case 2: Add New Mission Type
**Example:** Add a new "Express Delivery" mission

**Steps:**
1. Add to `MissionType` enum
2. Create new cycle `.vdri` file
3. Update `MissionTypeHelper`
4. Embed resource
5. Write comprehensive tests
6. Integration testing
7. Documentation
8. Deploy

**Time Required:** 3-5 days

---

### Use Case 3: Custom Engineering Mode Cycle
**Example:** Create specialized test cycle for R&D

**Steps:**
1. Create `.vdri` file
2. Place in appropriate directory
3. Reference in vehicle configuration
4. Test with specific vehicle
5. Document usage

**Time Required:** 1 day

---

## Quality Checklist

Before deploying, ensure:

- [ ] Cycle file format is correct
- [ ] Headers match expected patterns
- [ ] Data is monotonically increasing
- [ ] Values are physically realistic
- [ ] All unit tests pass
- [ ] Integration tests pass
- [ ] Manual testing completed
- [ ] Code reviewed
- [ ] Documentation updated
- [ ] Release notes prepared
- [ ] Backwards compatibility verified
- [ ] Regulatory compliance confirmed

---

## Support

### Questions or Issues?

1. **Check the documentation** - Most questions are answered in the guides
2. **Review existing code** - Look at similar implementations
3. **Run the tests** - Test templates show expected behavior
4. **Ask the team** - Email: JRC-VECTO@ec.europa.eu

### Contributing

If you find errors or have suggestions for improving this documentation:

1. Create an issue in the repository
2. Submit a merge request with improvements
3. Contact the development team

---

## Version History

### Version 1.1 (2024) - CURRENT ⭐
- Added professional PowerPoint presentation (20 slides)
- Added presentation usage guide for different audiences
- Added comprehensive delivery documentation
- Enhanced quick start guide with presentation-first approach
- Updated file organization documentation

### Version 1.0 (2024)
- Initial release
- Comprehensive guide created
- Unit test templates added
- PowerPoint presentation guide added (markdown source)
- Documentation structure established

### Future Updates
- Add video tutorials
- Include more example cycles
- Expand troubleshooting section
- Add automated testing examples

---

## Best Practices Summary

**DO:**
- ✓ Read all documentation before starting
- ✓ Follow the 8-step process
- ✓ Write tests for all changes
- ✓ Validate results thoroughly
- ✓ Document everything
- ✓ Use version control
- ✓ Request code review

**DON'T:**
- ✗ Skip testing steps
- ✗ Modify production without staging tests
- ✗ Break backwards compatibility
- ✗ Use unrealistic cycle values
- ✗ Deploy without documentation
- ✗ Forget regulatory compliance

---

## License

This documentation is part of the VECTO project and is subject to the same license:

**License:** EUPL 1.2+  
**Copyright:** 2012-2022 European Commission, DG_CLIMA

See the repository LICENSE.txt file for full details.

---

## Contact

**Email:** JRC-VECTO@ec.europa.eu  
**Repository:** https://code.europa.eu/vecto/vecto  
**Issues:** Use the repository issue tracker

---

**Last Updated:** 2024  
**Maintained By:** VECTO Development Team  
**Status:** Active Documentation
