# Documentation Gaps — 2026-10-06

아래 5개 Gap은 문서 누락/검증 한계를 분리한 것이다. 기본 기능 설명은 현재 소스로 작성했으며 UI 사람 PASS를 의미하지 않는다. 전용 사용자 절차를 쓰지 않은 기능 1개: Micro Joint 신규 생성. 일부 기존 표시의 단위 라벨은 실제 구현 제약으로 도움말에도 안내한다.

## GAP-001
Feature: 현재 전체 화면 및 대화상자 Screenshot
Reason: 현재 버전의 안전한 전체 화면 캡처 미확보. 기존 자동 테스트 이미지가 전체 사용자 UI를 대신하지 않음.
Missing Information: 현재 main-window/settings/open-dialog 캡처와 사람이 대조한 설명.
Suggested Action: TC-064-002에서 실제 프로그램을 확인하며 개인정보 없는 이미지를 images에 저장하고 alt/caption/버전 기록.
Related CR: CR-064, CR-055, CR-057
Related Source: VCutting/MainWindow.xaml; SettingsWindows.cs; images/README.md

## GAP-002
Feature: 설치 및 업그레이드의 실제 사용자 절차
Reason: 게시 목록/Help 복사/재귀 설치 정의는 확인했지만 새 패키지의 설치·제거를 사람 환경에서 실행하지 않음.
Missing Information: 새/사용자 지정 경로 및 업그레이드 설치 결과, 시작 메뉴, 제거 결과.
Suggested Action: TC-064-003 실행 기록 후 실제 패키지 버전·화면을 보완. 기존 installer 1.1.0 패키지가 VCutting를 포함했다고 주장하지 않음.
Related CR: CR-064
Related Source: installer/Build-Installer.ps1; installer/VCutting.Explorer.iss

## GAP-003
Feature: Micro Joint 신규 생성 업무
Reason: 기존 객체의 속성 편집과 모델 지원은 확인했으나 MainWindow에 새 Micro Joint 생성 명령이 없음.
Missing Information: 승인된 사용자 생성 명령/선택 경로.
Suggested Action: 별도 기능 요청으로 생성 UX를 확정한 후 전용 도움말 추가. 현재 용어집과 화면에 기존 객체 편집 범위만 설명.
Related CR: CR-064 (조사), BASE-MicroJoints
Related Source: VCutting/MainWindow.xaml.cs::ShowProperties; Models.cs

## GAP-004
Feature: 비-cm 문서의 모든 속성/치수 라벨 해석
Reason: 현재 Material/일부 도형은 현재 단위 라벨이지만 기존 Bend/Section/Cut 속성 및 툴팁에 cm가 남음. 이번 작업에서 기능을 임의 변경하지 않음.
Missing Information: 모든 입력란의 mm/cm/m UI 검증과 라벨 정합성에 대한 승인된 수정 범위.
Suggested Action: TC-023 수동 검증 후 별도 CR로 표시 정합성 검토. 도움말은 Document Units/Material과 함께 확인하도록 안내.
Related CR: CR-023, CR-047, CR-048, CR-051, CR-052
Related Source: VCutting/MainWindow.xaml.cs::ShowProperties; DesignViews.cs::AddDimensionEditor

## GAP-005
Feature: 복합 교차 절곡과 일반 DXF import의 고급 사례
Reason: 일직선이 아닌 힌지 fallback 및 cm 외 입력 mm 해석·일반 열린 L 객체 import가 제한됨. 제조 승인/모든 CAD 파일 호환 자료가 없음.
Missing Information: 대표 실무 도면의 제작 방향·순서 비교, 일반 DXF의 열린 경로/미터/곡선 외곽 입력 승인 자료.
Suggested Action: 해당 TC-061/BASE-GeneralDxfImport 및 사람 시나리오를 보완하고 근거가 확보된 범위만 고급 예제 추가.
Related CR: CR-061, CR-054, CR-023
Related Source: VCutting/GeneralDxfImportEngine.cs; Engines.cs; tests/regression/cr/CR-061/test-cases.md

- CR-069: 이름 변경 후 실제 UI/브라우저/설치 검증과 현재 화면 캡처는 PENDING_MANUAL. HTML 텍스트는 갱신했으며 사람 PASS를 주장하지 않는다. 배포 화면 캡처는 현재 없다(images/README.md).

- CR-070: Development About/Help visual comparison and future explicitly approved candidate installer/installed-app/Help version consistency require human review. No newly issued Freeze or actual new installer exists. JS-disabled HTML shows Development fallback.

CR-071 update: 설치 마법사 6장 확보 및 두 설치 안내 반영. 위 전체 캡처 미확보 기록은 설치 화면에 대해서 대체된다. 앱 화면 캡처, 실제 설치/업데이트 및 브라우저/인쇄 사람 검증은 여전히 미확인. 첨부 화면 제공을 사람 테스트 PASS로 해석하지 않는다.

## Package/application identity correction — 2026-10-06
사용자 확정 명칭: 패키지 VCutting, 프로그램 CosmicDesigner. 매뉴얼 제목/실행 안내 정정. 현재 VCutting/VCutting.csproj AssemblyName=VCutting과 설치 실행 대상 VCutting.exe는 사용자 정정과 불일치한다. 이번 기억/매뉴얼 정정 요청에서 바이너리/설치 변경을 수행하지 않았다. CosmicExplorer는 향후 계획이며 미구현. 사용자 제공 설치 화면의 VCutting 실행 표시는 당시 원본 그대로 보존. 새 패키지 화면 및 실행 파일 정합성 검증은 후속 구현 작업 필요.

2026-10-07 CR-072/073: Korean wording, all window layouts/DPI/access keys, actual installed catalogs, initial fresh profile mm and retained old profile require human verification. Native file dialog/buttons and third-party/OS exception messages follow OS locale. Existing source/installer VCutting.exe naming mismatch remains a separate recorded identity follow-up. No actual new installer or Freeze issued.

CR-074: 실제 타원 preview/icon/기울기/한글 tooltip/DPI/3D 대조 TC-074-002 PENDING_MANUAL; 사람 PASS 기록 없음.

CR-075: 타원 축/회전/중심 property UI/언어/잘림/실제 drag/history/3D TC-075-002 PENDING_MANUAL. native DXF ELLIPSE writer 및 shared parser 미구현; 이번 속성 개선은 기존 LINE 저장을 유지한다.
CR-076: 코드 산출물/installer 참조 CosmicDesigner.exe 정합성 구현. 이전 명칭 gap의 executable 부분 해결; 새 installer actual clean install/upgrade 및 old shortcut handling은 TC-076-002 PENDING_MANUAL. 역사적 설치 캡처는 보존한다.

CR-077: TC-077-002 PENDING_MANUAL. 주황색 선택, context menu, 복합 천공/고리/곡선 편집의 실제 화면·Korean/DPI/Flat/3D 확인 대기. 첨부 원본은 요구 증거이며 구현 결과 screenshot이 아니다. 다중 루프 compound Fillet은 안전하게 제한한다. 과거 앱의 다중 루프 JSON 해석은 보장하지 않는다.

CR-078: 정다각형 아이콘/두 클릭 Preview/속성/선택/Undo/DPI/Korean 및 실제 AutoCAD closed LWPOLYLINE 확인 TC-078-012 PENDING_MANUAL. 자동 startup/geometry/parser PASS는 사람 시각 승인과 별도. Bulge 또는 nondefault OCS LWPOLYLINE 지원은 이번 범위 밖이며 parser는 unsupported로 보고한다. 기존 일반 Hole은 L layer 고정 정책 유지.

2026-10-07 CR-079: actual star UI screenshots, Korean/English layout and external CAD review pending TC-079-004. No human PASS.

CR-079 solid interior correction: actual UI/3D screenshot review pending TC-079-006; no human PASS.

CR-079 optimized-profile screenshots and external CAD/CAM review pending TC009; no human PASS.

CR-080 actual toolbar order/English-Korean layout screenshots and About/report identity human match pending TC-080-001.

CR081/082 real property controls/English-Korean layout/Flat-3D/CAD and screenshots pending MANUAL081002/082002; no human PASS.

CR-083: actual Copy/Paste menu/text-focus/two-process clipboard/English-Korean layout screenshots and human CAD review pending TC-083-002; no human PASS inferred.

CR-084 actual SVG dialog/layout/screenshots and representative source SVG/external CAD visual comparison pending TC-084-004; no human PASS.

CR-085: CosmicConvert GUI/native dialogs/layout/preview and actual CAD visual acceptance/screenshots PENDING_MANUAL (TC-085-005). Automated parser proves entity readability, not single-Cut mixed-loop assembly; current importer creates per-entity Cuts. No UI performance measurement. Intersecting/touching filled rings currently reject; richer Boolean conversion remains a limitation.

CR-086: actual GUI/CAD10mm margin/frame/status screenshots and measurements pending TC-086-002. Automated geometry/quiet/readback evidence kept separately.
CR-087 supersedes CR-085 entity-only limitation for native metadata output. Actual screen/GUI/external CAD acceptance remains TC-087-003 PENDING_MANUAL; no measured UI performance. Historical850LINE fixture NOT_RUN.
CR-088 actual provided outlined-text SVG preview/Designer/CAD human review pending TC088002. Nested text remains unsupported even if parent fill-opacity0; no font outlining implementation.
