# ADR-002 — CosmicDesigner external display resources
## Status
Accepted — CR-072, 2026-10-07 user-approved design.
## Context
UI text mixed languages; external user customization and future language additions required. Internal IDs/schema/culture must remain unchanged.
## Decision
Bundled Languages/en.json and ko.json UTF-8 objects {code,name,strings}. Stable ui.NNNN keys plus semantic supplemental keys; never regenerate/renumber issued keys. Composite format {0}, {1:0.###}; same indices required. English embedded copy is immutable bootstrap recovery only, shipped external files remain authoritative.
Layers: embedded English → bundled English → user English → selected bundled language → selected user language. Missing selected keys fall back to effective English. Valid user JSON named language-code.json under LocalApplicationData/CosmicDesigner/Languages permits new language metadata/discovery. Resource file is data only; traversal codes, oversized files, invalid encoding, duplicate properties, invalid formats/slots and unknown keys rejected safely. Diagnostics shown through Settings. No arbitrary executable plugins.
XAML DynamicResource keys update Application resources. Code-created static labels retain presentation tokens and repaint on language change; view drawing text reads resources each paint. Editable numeric/path fields are never translated; readonly shape labels are presentation copies. No Thread Culture/CurrentCulture change. OS native dialogs/errors use OS locale; unavailable external error messages are preserved as diagnostic facts.
## Alternatives Considered
Compiled-only resources reject user customization. Changing model identifiers would risk DXF/selection compatibility. Changing global culture would change numeric interpretation.
## Compatibility and Migration
Missing DisplayLanguage → en; user settings location unchanged. Upgrade doesn't manage/delete user overrides. App source directory/assembly currently VCutting remains prior unresolved identity implementation; displayed application name CosmicDesigner and package VCutting maintained. CosmicExplorer is future plan only.
## Consequences
External resource catalogs and integrity tests must ship together. WPF/native dialog buttons and application UI visual verification need human review. Already open native dialogs keep their OS locale; application language applies immediately on Settings selection, restored on Cancel, persisted on OK; re-open Settings after adding/removing language files.

English catalog additionally normalizes the formerly mixed Korean dialog/settings wording into English; existing English commands/shortcuts remain unchanged. Code tokens retain dynamic kind/title arguments for live repaint. Unit name bindings update their text target without refreshing/resetting the selected enum. Auto interval input accepts the localized display token and invariant Auto, while storing zero metres.

English catalog additionally normalizes the formerly mixed Korean dialog/settings wording into English; existing English commands/shortcuts remain unchanged. Code tokens retain dynamic kind/title arguments for live repaint. Unit name bindings update their text target without refreshing/resetting the selected enum. Auto interval input accepts the localized display token and invariant Auto, while storing zero metres.
