# CR Regression Test Suite

정책: ../../docs/project/REGRESSION_POLICY.md
목록: manifest.json
명세: cr/CR-NNN/test-cases.md
실행 기록: results/

기존 VCutting.Verification 프로젝트와 모든 기존 함수/assertion을 유지한다. 여러 CR에 관련된 동일 함수는 테스트 선택 내에서 한 번 실행하고 연결된 각 ID에 결과를 보고한다. 미연결 기존 기본 검증은 BASE-*로 전체 Suite에 포함한다. 자동화 ID는 함수 검사 범위만 나타내고 수동 acceptance 검증을 대신하지 않는다.

## 실행
Windows + .NET 10 환경에서 기존 검증 프로젝트 Release 빌드 후 아래처럼 실행한다.

- 전체: dotnet <build-directory>/VCutting.Verification.dll
- 목록: dotnet <build-directory>/VCutting.Verification.dll --list
- 신규 CR 선택: dotnet <build-directory>/VCutting.Verification.dll --tests TC-046-001
- 영향 테스트 선택: dotnet <build-directory>/VCutting.Verification.dll --tests TC-017-001,TC-042-001,TC-045-001
- 등록되지 않은 ID/빈 선택은 오류로 종료한다. 수동 TC는 자동 runner에서 실행할 수 없다.

각 자동 TC 결과는 PASS/FAIL과 ID로 출력한다. 자동 assertion 실패 시 나머지 테스트도 계속 실행하여 실패 목록을 모으고 exit code 1을 반환한다. 자동 결과에 수동 PASS를 합산하지 않는다. 실행 output/환경/source 상태를 results/에 보관한다.

## 변경
모든 TC의 `executionType`은 AUTO / MANUAL / SEMI_AUTO / NOT_AUTOMATED 중 하나다. `automation`은 이전 형식 호환용이며 실행 분류는 executionType을 따른다. 기본 BASE 검사에도 AUTO를 지정한다. 현재 등록은 AUTO 52개, MANUAL 49개, SEMI_AUTO/NOT_AUTOMATED 0개다.

MANUAL/사람 확인이 필요한 SEMI_AUTO는 확인자·일시·관측 근거 없이 PASS로 기록하지 않는다. 자동 준비 또는 부분 검증 성공과 최종 판정을 분리한다. 미실행 결과는 PENDING_MANUAL / NOT_RUN / BLOCKED이며 BLOCKED에는 이유를 기록한다. NOT_AUTOMATED를 수동 판정 사례와 혼동하거나 자동 PASS로 합산하지 않는다.

실행 결과는 Automated Regression / Manual Regression / Not Automated로 각각 집계한다. 필수 사람 확인이 남은 CR은 WAITING FOR USER VERIFICATION으로 두고 Verified/Closed로 승격하지 않는다.

새 TC는 CR-NNN의 마지막 번호 다음에 추가한다. 기존 ID를 재사용/재번호화하지 않는다.

CR-064 adds TC-064-001 AUTO for shipped help references/navigation/encoding/path and publication integrity; TC-064-002/003 MANUAL for browser/content and actual installation. Current manifest: AUTO 67, MANUAL 67; runner Full additionally includes 6 existing BASE AUTO checks (73 total). Older introductory migration counts above describe that earlier snapshot.
ACTIVE 테스트는 누적 보존한다. 변경은 근거 CR, 목적 유지/요구 폐기 사유와 상태(SUPERSEDED/DEPRECATED)를 manifest와 명세에 기록하고 원본 assertion 변경 내역도 검토한다.
자동 TC 등록/상태를 변경할 때 runner 등록도 함께 갱신하며 --list와 manifest를 대조한다.

## 이관 한계
2026-10-05 기록은 기존 함수/CR의 사후 매핑이다. 과거 구현 전 영향 분석이나 신규 UI 실행 결과를 꾸며 기록하지 않는다. 수동 테스트는 아직 NOT RUN이다. 기존 lifecycle과 수동 승인 기록을 유지하며 READY FOR MERGE로 일괄 승격하지 않는다.

CR-065: AUTO TC-065-001 is registered in the application runner and checks the compiled installer. Compile it first; default artifact is artifacts/release/1.20.29-rc1-cr065/output/VCuttingSetup.exe. Set COSMIC_INSTALLER_PATH for another location. Build-Installer.ps1 also runs the independent PowerShell verifier. Current AUTO 68 + BASE 6 = 74; MANUAL 68.

CR-066 adds TC-066-001 installer branding/shortcut/upgrade contract to the runner. Set COSMIC_INSTALLER_PATH to the cr066 package when validating the delivered installer. Current AUTO 69 + BASE 6 = 75; MANUAL 69.

CR-068/069: VCutting.sln and VCutting.Verification; TC-068-001 AUTO installer default folder, TC-069-001 AUTO rename/legacy compatibility. Full AUTO 77 (71 mappings + BASE 6), MANUAL 71, SEMI_AUTO 1. Legacy COSMIC_INSTALLER_PATH override preserved; default installer artifacts/release/cr069/output/VCuttingSetup.exe. Stable TC-024/064/065/066 branding expectations updated under CR-069 approval and default folder under CR-068; original reports retained. Human checks pending.

CR-070: set VCUTTING_RELEASE_PYTHON to Python 3; TC-070-001 runs isolated Git/.NET frozen-build fixtures (runtime packs must already be available; repository installer cache is reused when present). Full AUTO 78 (72 + BASE 6); MANUAL 72, SEMI_AUTO 1, NOT_AUTOMATED 0. TC-065-001 consumes selected approved installer/build.json or verifies absence-of-Freeze gates and reports actual new compile NOT_RUN. Production cannot be inferred from AUTO PASS.

CR-071: TC-071-001 original installer screenshots/order/deployed integrity AUTO; TC-071-002 browser/print MANUAL pending. Full AUTO 79 (73 mappings + 6 BASE); MANUAL 73, SEMI_AUTO 1.

CR-072/073: AUTO TC-072-001/TC-073-001 in LocalizationTests.cs; MANUAL TC-072-002/TC-073-002 pending. Final full AUTO 81 (75 mapped + 6 BASE), MANUAL 75, SEMI_AUTO 1, NOT_AUTOMATED 0. Set VCUTTING_RELEASE_PYTHON for the accumulated version fixture; language fixtures write only disposable repository artifacts. See results/CR-072-073-2026-10-07.md.

CR-075/076: new AUTO 2/2, affected 14/14, full AUTO 84/84 PASS (78 mapped + 6 BASE); MANUAL 78, SEMI_AUTO 1, NOT_AUTOMATED 0. TC-075-002/TC-076-002 pending. CR-076 user approval changes main output expectations to CosmicDesigner.exe/assembly without changing project/package/namespace/AppId. Accumulated version fixture requires VCUTTING_RELEASE_PYTHON. Results: results/CR-075-076-2026-10-07.md.

CR-077: internal Cut union AUTO TC-077-001 / MANUAL TC-077-002. New 1/1, affected 17/17, full AUTO 85/85 PASS (79 mappings + 6 BASE). MANUAL79, SEMI_AUTO1, NOT_AUTOMATED0; required human verification pending. Result: results/CR-077-2026-10-07.md.

CR-078: AUTO TC-078-001..011, MANUAL TC-078-012. New11/11, affected20/20, full96/96 AUTO PASS (90 mapped+6 BASE). MANUAL80/SEMI_AUTO1/NOT_AUTOMATED0 registered; current human UI/AutoCAD verification pending. Actual app startup AUTO PASS. Result results/CR-078-2026-10-07.md.

CR-085: separate CosmicConvert.Verification runner has AUTO TC-085-001..004 and MANUAL005. Execute both accumulated VCutting.Verification and this new runner for full suite. New converter runtime has no reference to Designer; verification uses existing parser/importer. Human results recorded separately.

CR-086 adds AUTO TC-086-001 and MANUAL002. Current external850LINE Apple baseline for TC-085-004 is unavailable (replaced by167entity result); runner reports NOT_RUN and nonzero incomplete status. Historical expectations/evidence retained. New margin case verifies Apple independently. Run both runners; do not count missing fixture as PASS.
CR-087 adds AUTO001/002 and MANUAL003. Standalone converter emits existing native metadata; actual Designer serializer/hit/history/2D mask/3D mesh integration. Full112PASS/1historicalNOT_RUN across113ACTIVE AUTO; human review pending. Evidence results/CR-087-2026-10-08.md.
CR-088 adds AUTO001/MANUAL002: invisible auxiliary text and complex lettering broad phase; full113PASS/0FAIL/1historicalNOT_RUN across114ACTIVE AUTO. Result results/CR-088-2026-10-08.md; human pending.

CR-089 adds AUTO TC-089-001 actual WPF Section Fit/Reset and MANUAL TC-089-002 visual/button acceptance. Designer107/107 AUTO PASS; accumulated with converter114PASS/1historicalNOT_RUN. Human pending. Results results/CR-089-2026-10-08.md.
CR-090 adds AUTO TC-090-001 package contract and MANUAL TC-090-002 actual install/shortcuts. Designer108/108PASS; accumulated115PASS/1historicalNOT_RUN. Human pending; release ledger records actual RC builds.
