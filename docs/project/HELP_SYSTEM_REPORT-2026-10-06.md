# CR-064 Help System Baseline Report

## Help System Location
공식 HTML 원본: help-content/help/index.html. 기존 공통 배포 위치를 재사용하여 실제 실행 파일 옆 help/index.html이 된다. 프로젝트별 별도 help를 만들면 공통 Installer 병합에서 충돌하므로 기존 help-content를 확장했다. CosmicDesigner_help.html은 호환 진입점이다. 기존 DXF 앱의 도움말은 유지한다.

## Created Files
- 사용자 HTML 22개: index.html, getting-started.html, screen-layout.html, basic-operations.html; features/{files,holes,slits,bends,sections,contours,boundary-merge,fillet,material-units,settings,preview}.html; workflows/{new-design,import-design,notch-fold,edit-save}.html; troubleshooting/common-errors.html; reference/{terminology,shortcuts}.html.
- 내부 관리: help-content/help/{README.md,manual-traceability.md,manual-sections.json,DOCUMENTATION_GAPS.md,SOURCE_INVENTORY.md,images/README.md}.
- 구현: CosmicDesigner/UserHelp.cs.
- 요청/관리: docs/project/USER_HELP_REQUEST-2026-10-06.md (사용자 원문 보존), CR-064.md, 본 보고서.
- 테스트: tests/regression/cr/CR-064/test-cases.md, results/CR-064-2026-10-06.md, results/CR-064-new.txt.

## Updated Files
- MainWindow.xaml 및 MainWindow.xaml.cs: Help 메뉴 4개와 공통 launcher.
- Directory.Build.props: 기존 재귀 HTML/asset 복사 유지, 내부 Markdown/JSON 배포 제외.
- help-content/CosmicDesigner_help.html, 공통 css/help.css와 js/help.js; 설치 대표/상세 HTML의 새 패키지 구성.
- installer/Build-Installer.ps1, DXFExplorer.iss, Verify-Help.ps1, README.md: CosmicDesigner 게시/바로가기/배포 검증 및 내부 문서 제외.
- AGENTS.md, DEVELOPMENT_POLICY.md, PROJECT_STATUS.md, CR index, CR-001~063 Documentation Impact backfill, current-requirements.md, CHANGELOG.md.
- CosmicDesigner.Verification/Program.cs 및 regression manifest/README.

## Current Manual Coverage
프로그램 소개·실행·메뉴/화면·파일/저장 확인·DXF 자동 가져오기·Recent·Select/Pan/Zoom/Undo/Delete·지원 7종 Hole·3종 Slit·V/V1·외곽 LINE/ARC·경계 홈·Fillet·H/W 단면/지역 치수/빈 구간·두께·mm/cm/m·환경 설정·3D·4개 업무·오류·용어·단축키를 현재 소스로 설명했다. JavaScript 없이도 링크 사용 가능, 목차 검색/본문 Ctrl+F, 반응형/인쇄 CSS를 제공한다. 개발 클래스/함수/CR 번호는 사용자 메뉴에 노출하지 않는다.

| 지표 | 수치 |
|---|---:|
| 조사한 과거 CR | 63 |
| 이번 CR 포함 확인한 CR | 64 |
| 과거 Manual Impact Yes | 62 |
| 이번 CR 포함 Manual Impact Yes | 63 |
| 현재 도움말에 반영한 과거 CR | 62 |
| 새 CosmicDesigner 사용자 HTML | 22 |
| 기존 대표/타 제품 포함 검증 HTML | 35 |
| 전용 사용자 절차 미문서화 기능 | 1 (Micro Joint 신규 생성 UI 없음) |
| Documentation Gap | 5 |

## CRs Reflected in Manual
CR-002~063의 사용자 영향을 모두 현재 도움말 위치에 매핑했다. CR-001은 CDF 승인 정책만 있으므로 User Visible Change/Manual Impact No; 실제 CDF 기능을 설명하지 않는다. CR-064 Help 자체도 추적한다. 상세 제목/기능/섹션/테스트/상태/갱신일은 manual-traceability.md와 각 CR Documentation Impact에 기록한다. 과거 자동 경계 통합·절곡 검정선·지원 도구 목록 등의 설명은 현재 명시적 통합·V/V1 색상·현재 7종 도구 동작으로 대체한다. 기존 CR의 승인 이력은 보존한다.

## Tests Used as Documentation Sources
tests/regression/manifest.json 및 CR-001~063 test-cases.md의 AUTO/MANUAL 명세, CosmicDesigner.Verification/Program.cs의 실제 assertion/렌더·DXF round-trip을 참고했다. 주요 예: TC-020/021 파일·저장 확인, TC-023 단위, TC-031/038/041/047~052 Hole, TC-033~036/044 Fillet, TC-054 절개, TC-060 절곡, TC-061 3D, TC-062 단면, TC-063 중앙 안내. 각 페이지의 정확한 TC 연결은 manual-sections.json 및 manual-traceability.md에 있다. 명세를 사용자 절차로 재작성했으며 수동 시험 실행을 주장하지 않는다. README/기존 설치 문서/현재 요구·릴리스·소스 목록은 SOURCE_INVENTORY.md에 기록한다.

## Help Integration Status
User Manual / Getting Started / Keyboard Shortcuts / Troubleshooting → EXE 기준 help 내부 허용 경로. 설치 경로·현재 작업 디렉터리에 독립적이다. 누락 및 브라우저 연결 실패 안내를 구현했다. About 동작은 유지했다. 경로·메뉴 연결 AUTO PASS. 실제 클릭/브라우저 가독성은 TC-064-002 PENDING_MANUAL.

## Installer Integration Status
기존 Installer의 4개 앱 목록에 CosmicDesigner를 추가하고 시작 메뉴 항목과 재귀 Help 배포를 연결했다. 새 설정과 framework-dependent 게시 결과 검증 PASS. 기존 Installer 제품 이름/AppId/버전 1.1.0을 임의 변경하지 않았다. 이번 게시물은 설치 프로그램 및 self-contained 배포물이 아니며 .NET 10 Desktop Runtime이 필요하다. 실제 Installer 재빌드/설치·업그레이드·제거는 NOT_RUN / TC-064-003 PENDING_MANUAL.

## Documentation Gaps
GAP-001 현재 전체 UI 스크린샷; GAP-002 실제 설치 라이프사이클; GAP-003 Micro Joint 신규 생성 UI; GAP-004 일부 기존 cm 라벨의 비-cm 문서 UI 검증; GAP-005 복합 교차 절곡/고급 일반 DXF 사례. 이유·누락정보·권장조치·관련 CR·소스는 DOCUMENTATION_GAPS.md에 기록했다. 임의 Screenshot을 생성하지 않았다. 프로그램 About에 남은 과거 하드코딩 버전도 현재 버전 근거로 사용하지 않았다.

## Added Documentation Tests
TC-064-001 AUTO: HTML/UTF-8/링크/앵커/이미지/전체 탐색/실행 출력 동일성/안전 경로/메뉴·배포 정의/내부 정보 제외.
TC-064-002 MANUAL: 실제 메뉴·브라우저·오프라인·키보드·반응형·인쇄·내용 대조.
TC-064-003 MANUAL: 새/사용자 지정 경로 설치·업데이트·제거. 모두 immutable TC ID/Execution Type/사전조건/절차/기대결과/위치를 누적 명세에 기록했다.

## Regression Test Result
Release build 0 warning/error; New AUTO 1/1; Affected AUTO 5/5; Full AUTO 73/73 PASS (기존 72개 보존). FAIL 0/NOT_RUN 0. MANUAL 신규 2건 PENDING_MANUAL; 이번 실행에서 Suite MANUAL 67건 NOT_RUN, 과거 승인 상태 별도 보존. SEMI_AUTO/NOT_AUTOMATED 0. HTML source/publish 35개 링크 검사 PASS. 전체 결과는 tests/regression/results/CR-064-2026-10-06.md.

## Recommended Next Documentation Work
TC-064-002/003 사람 확인과 안전한 현재 화면 캡처를 우선한다. 그 뒤 실제 업무 도면 예제, Micro Joint 생성 UX와 기존 단위 라벨 정합성을 별도 CR로 검토한다. 앞으로 모든 CR에 문서 영향을 평가하고 Yes이면 HTML/추적/워크플로/오류/필요 이미지를 함께 갱신한다.

CR-064: Implemented, CODE COMPLETE / AUTOMATED TESTS PASSED / USER VERIFICATION REQUIRED. Verification Status: WAITING FOR USER VERIFICATION. 필수 사람 검증 전 Verified/Closed 아님. 커밋/푸시 없음; 기존 미커밋 변경 보존.
