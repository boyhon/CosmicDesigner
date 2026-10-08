# CR-058
## TC-058-001
Execution Type: AUTO. Status: ACTIVE.
Precondition: Release verification executable.
Procedure: --tests TC-058-001.
Expected: actual perpendicular thickness equals thickness*scale for all legs/corners at .2/2 thickness and .5/1.5/3 scale, H/W and rotation; folded centers match 3D vertices; narrow opening preserved.
Implementation: Program.cs::SectionTrueThickness.
## TC-058-002
Execution Type: MANUAL. Status: ACTIVE.
Precondition: supplied design with narrow opening, thickness 2.
Procedure: compare H Bent and 3D; inspect gap/outer geometry, dimension witness lines; rotate four times, wheel zoom; repeat W and thin material .2.
Expected: no inflated material thickness; gap and proportions follow actual dimensions; dimension witnesses remain aligned, editing retained.
Result: PENDING_MANUAL.
