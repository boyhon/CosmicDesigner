# Documentation Gaps — 2026-10-06

아래 5개 Gap은 문서 누락/검증 한계를 분리한 것이다. 기본 기능 설명은 현재 소스로 작성했으며 UI 사람 PASS를 의미하지 않는다. 전용 사용자 절차를 쓰지 않은 기능 1개: Micro Joint 신규 생성. 일부 기존 표시의 단위 라벨은 실제 구현 제약으로 도움말에도 안내한다.

## GAP-001
Feature: 현재 전체 화면 및 대화상자 Screenshot
Reason: 현재 버전의 안전한 전체 화면 캡처 미확보. 기존 자동 테스트 이미지가 전체 사용자 UI를 대신하지 않음.
Missing Information: 현재 main-window/settings/open-dialog 캡처와 사람이 대조한 설명.
Suggested Action: TC-064-002에서 실제 프로그램을 확인하며 개인정보 없는 이미지를 images에 저장하고 alt/caption/버전 기록.
Related CR: CR-064, CR-055, CR-057
Related Source: CosmicDesigner/MainWindow.xaml; SettingsWindows.cs; images/README.md

## GAP-002
Feature: 설치 및 업그레이드의 실제 사용자 절차
Reason: 게시 목록/Help 복사/재귀 설치 정의는 확인했지만 새 패키지의 설치·제거를 사람 환경에서 실행하지 않음.
Missing Information: 새/사용자 지정 경로 및 업그레이드 설치 결과, 시작 메뉴, 제거 결과.
Suggested Action: TC-064-003 실행 기록 후 실제 패키지 버전·화면을 보완. 기존 installer 1.1.0 패키지가 CosmicDesigner를 포함했다고 주장하지 않음.
Related CR: CR-064
Related Source: installer/Build-Installer.ps1; installer/DXFExplorer.iss

## GAP-003
Feature: Micro Joint 신규 생성 업무
Reason: 기존 객체의 속성 편집과 모델 지원은 확인했으나 MainWindow에 새 Micro Joint 생성 명령이 없음.
Missing Information: 승인된 사용자 생성 명령/선택 경로.
Suggested Action: 별도 기능 요청으로 생성 UX를 확정한 후 전용 도움말 추가. 현재 용어집과 화면에 기존 객체 편집 범위만 설명.
Related CR: CR-064 (조사), BASE-MicroJoints
Related Source: CosmicDesigner/MainWindow.xaml.cs::ShowProperties; Models.cs

## GAP-004
Feature: 비-cm 문서의 모든 속성/치수 라벨 해석
Reason: 현재 Material/일부 도형은 현재 단위 라벨이지만 기존 Bend/Section/Cut 속성 및 툴팁에 cm가 남음. 이번 작업에서 기능을 임의 변경하지 않음.
Missing Information: 모든 입력란의 mm/cm/m UI 검증과 라벨 정합성에 대한 승인된 수정 범위.
Suggested Action: TC-023 수동 검증 후 별도 CR로 표시 정합성 검토. 도움말은 Document Units/Material과 함께 확인하도록 안내.
Related CR: CR-023, CR-047, CR-048, CR-051, CR-052
Related Source: CosmicDesigner/MainWindow.xaml.cs::ShowProperties; DesignViews.cs::AddDimensionEditor

## GAP-005
Feature: 복합 교차 절곡과 일반 DXF import의 고급 사례
Reason: 일직선이 아닌 힌지 fallback 및 cm 외 입력 mm 해석·일반 열린 L 객체 import가 제한됨. 제조 승인/모든 CAD 파일 호환 자료가 없음.
Missing Information: 대표 실무 도면의 제작 방향·순서 비교, 일반 DXF의 열린 경로/미터/곡선 외곽 입력 승인 자료.
Suggested Action: 해당 TC-061/BASE-GeneralDxfImport 및 사람 시나리오를 보완하고 근거가 확보된 범위만 고급 예제 추가.
Related CR: CR-061, CR-054, CR-023
Related Source: CosmicDesigner/GeneralDxfImportEngine.cs; Engines.cs; tests/regression/cr/CR-061/test-cases.md
