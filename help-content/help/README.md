# CosmicDesigner Help — Living Documentation

## 原本과 배포
공식 사용자 원본은 이 디렉터리의 HTML이다. Word/PDF/생성 스크립트를 원본으로 사용하지 않는다. 위치는 기존 help-content/help 배포 구조를 재사용하여 실행 파일 옆 help/index.html이 되도록 선택했다. 기존 다른 DXF 앱의 페이지와 대표 HTML 진입점은 유지한다. CosmicDesigner_help.html은 호환 진입점이다.

Directory.Build.props는 WinExe 빌드/게시마다 HTML/CSS/JS/images를 복사하며 내부 .md/.json은 제외한다. installer는 5개 앱 게시 및 충돌 검증 후 재귀 배포한다. 내부 관리 파일은 사용자 메뉴에서 노출하지 않는다.

## 페이지 추가와 탐색 변경
1. 기능/업무에 맞는 features 또는 workflows HTML을 직접 작성한다. lang=ko, UTF-8, viewport, 제목과 h1, common css/help.css 및 js/help.js를 사용한다.
2. 모든 사용자 페이지의 manual-links 목차, 이전/다음, Home, 관련 도움말과 index 카드를 함께 갱신한다. JS 없이도 탐색되어야 한다. 목차 검색은 로컬 DOM 필터이며 본문 검색은 Ctrl+F다.
3. manual-sections.json의 페이지/제목/CR/테스트 연결 및 manual-traceability.md를 갱신한다. JSON은 검증/유지보수용이며 HTML 내용을 생성하는 원본이 아니다.
4. 신규 이미지에는 의미 있는 영문 파일명, alt, 설명, 캡처 버전·조건을 기록한다. 실제 프로그램 화면만 사용하고 개인정보/불필요한 개발 정보를 제거한다. 이미지가 없으면 참조를 넣지 않는다.
5. 소스·현재 요구·CR·AUTO와 MANUAL 명세를 비교해 최신 구현을 기술한다. 테스트 문장을 그대로 복사하지 않고 사용 목적·절차·결과·주의사항으로 쓴다.
6. 미확인 기능은 DOCUMENTATION_GAPS.md에 기록하고 해결 시 근거와 함께 갱신한다. 제안 기능과 실제 사용법을 혼합하지 않는다.

## 매 CR 갱신
User Visible Change와 Manual Impact를 Yes/No로 평가한다. Yes이면 영향 페이지·워크플로·오류·이미지·추적을 갱신하고 CR Documentation Impact에 연결한다. 내부 변경으로 사용자 동작이 같다면 이유를 기록하고 사용자 문서는 불필요하게 수정하지 않는다. 과거 CR의 승인 상태와 새 문서의 사람 검증을 구분한다.

## 검증
Release 검증 프로젝트를 빌드하여 --tests TC-064-001을 신규 검사로 실행한다. 영향 테스트 및 전체 suite를 뒤이어 실행한다. installer/Verify-Help.ps1 -Root help-content로 기존 제품 문서 링크도 확인한다. publish 출력에서 HTML 원본 동등성도 검사한다. TC-064-002(메뉴/브라우저/한글/모바일·인쇄·실제 화면 대조)와 TC-064-003(실설치/업데이트/제거)는 사람 이름·일시·관측 근거가 있어야 PASS다.

CR 완료 = 구현 + 관련 자동 테스트 + 필요한 사람 검증 + 회귀 통과 + Manual Impact Yes 문서 갱신. 자동 검사 통과만으로 Verified/Closed로 승격하지 않는다.
