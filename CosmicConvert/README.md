# CosmicConvert (CR-085)

Separate Windows WPF SVG → DXF executable. .NET 10 Windows Desktop Runtime required; no reference to CosmicDesigner at runtime. Common repository development version/build metadata apply.

Build from repository root (supply a distinct `VCuttingBuildIdentity`):

```powershell
dotnet build CosmicConvert/CosmicConvert.csproj -c Release -m:1 -p:BaseOutputPath=C:/MyDisk/Projects/DXF-explorer/artifacts/my-converter-build/ -p:VCuttingBuildIdentity=my-distinct-development-id
```

Run `CosmicConvert.exe "C:\Drawings\Sample - Apple"` to read the literal argument plus `.svg`, write the argument plus `.dxf`, then exit quietly. No args starts the Open/Save/Exit window. Help entry next to executable: CosmicConvert_help.html; source: help-content/help/cosmicconvert.html. Output is mm / L layer / LINE-ARC-CIRCLE; 10mm margin rectangle around actual shape bounds, translated so frame origin is(0,0); no viewport-derived rectangle. Existing COSMIC_DESIGNER_JSON metadata restores element-level Compound Cuts and open Slits; plain entities remain unchanged.

Reader produces ordered source curves in physical mm; Optimizer emits grouped certified approximations (.05mm); DxfWriter validates round-trip before same-directory atomic replacement. GUI Open only reads and previews. Save converts on a worker; Exit during processing cancels/defer-closes safely. The window title displays actual build identity.

```powershell
dotnet build CosmicConvert.Verification/CosmicConvert.Verification.csproj -c Release -m:1 -p:BaseOutputPath=C:/MyDisk/Projects/DXF-explorer/artifacts/my-converter-build/ -p:VCuttingBuildIdentity=my-distinct-development-id
dotnet artifacts/my-converter-build/Release/net10.0-windows/CosmicConvert.Verification.dll
```

Verification must run from repository root. `--list` and `--tests TC-085-001,...` supported. TC-085-004 currently needs the user-supplied Apple files at their recorded external paths; it does not overwrite them. Reports/artifacts are written under repository artifacts. Run accumulated VCutting.Verification plus this runner for full regression. Required human case TC-085-005 is distinct from automated PASS.

Limitations: intersecting/touching filled rings fail explicitly; CSS/text/images/clipping etc unsupported. Preview shows cutting paths, not SVG painted appearance. CosmicDesigner native Open restores mixed contours through its existing metadata contract; removing metadata loses that object grouping. No Designer source/behavior changed. GUI/CAD visuals and UI performance remain human verification items.

CR-088 skips explicitly invisible leaf text labels; visible/nested text still requires outlining. Fill topology uses bounded bounding-box sweep to handle complex disjoint lettering without changing curve tolerance.
