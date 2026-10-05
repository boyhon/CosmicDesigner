# 사용자 매뉴얼 및 도움말 시스템 구축

현재 프로젝트에 사용자용 프로그램 매뉴얼과 도움말 시스템을 구축한다.

이 작업의 목적은 단순히 현재 기능을 설명하는 문서를 한 번 작성하는 것이 아니라, 앞으로 프로그램의 기능이 CR(Change Request)에 의해 변경될 때마다 함께 유지·보수될 수 있는 **Living Documentation 구조**를 만드는 것이다.

프로그램 소스코드, 현재 구현 상태, 기존 CR 기록, 테스트 케이스, Regression Test Suite, Manual Test 자료 및 프로젝트 내에서 확인 가능한 모든 기존 문서를 조사하여 현재 시점에서 작성 가능한 사용자 도움말을 최대한 완성한다.

---

# 1. 기본 원칙

사용자 도움말은 HTML을 공식 원본(Source of Truth)으로 관리한다.

Word, PDF 등의 별도 파일을 원본으로 관리하지 않는다.

필요한 경우 향후 HTML로부터 PDF 등의 배포용 문서를 생성할 수 있지만, 실제 유지·보수 대상은 HTML이다.

도움말은 개발자용 내부 설계 문서가 아니라 실제 프로그램 사용자를 위한 문서여야 한다.

따라서 다음과 같은 내부 구현 상세는 특별한 필요가 없는 한 사용자 도움말에 포함하지 않는다.

- 클래스 구조
- 내부 함수명
- DB 테이블 구조
- 내부 API 구현
- 리팩터링 내역
- 내부 알고리즘 상세
- 개발 도구 사용법

사용자가 프로그램을 설치하고 실행하고 실제 업무를 수행하는 데 필요한 내용을 중심으로 작성한다.

---

# 2. Help 디렉토리 생성

프로그램 프로젝트 루트 또는 실제 실행 프로그램과 논리적으로 가장 가까운 위치를 확인하여 다음 구조를 기본으로 하는 Help 디렉토리를 생성한다.

예:

```text
/help
    index.html

    getting-started.html
    screen-layout.html
    basic-operations.html

    /features
        [기능별 HTML]

    /workflows
        [업무 시나리오별 HTML]

    /troubleshooting
        common-errors.html

    /reference
        terminology.html
        shortcuts.html

    /images
        [도움말용 이미지]

    /css
        help.css

    /js
        help.js
```

기존 프로젝트 구조상 `/help`보다 더 적절한 위치가 있다면 그 구조를 사용할 수 있다.

단, 최종 위치와 선택 이유를 작업 결과에 기록한다.

---

# 3. 기존 프로젝트 조사

도움말을 작성하기 전에 프로젝트 전체를 조사한다.

최소한 다음 내용을 확인한다.

1. 실행 프로그램의 이름과 목적
2. 메인 화면 구성
3. 메뉴 구조
4. ToolBar 또는 주요 버튼
5. Dialog 및 설정 화면
6. 지원하는 파일 형식
7. 파일 Open / Save / Import / Export 기능
8. 주요 사용자 작업 흐름
9. 최근 구현된 기능
10. 아직 구현되지 않은 기능
11. 오류 처리 및 사용자 메시지
12. 사용자 입력값과 옵션
13. 기존 README 및 설계 문서
14. CR 기록
15. Regression Test Suite
16. Manual Test Case
17. 일반 Test Case
18. Release 관련 자료
19. 기존 도움말 또는 문서
20. 프로그램 설치 및 실행 방법

추측으로 기능을 만들어내지 않는다.

소스코드, CR, 테스트, 기존 문서 등에서 확인 가능한 사실만 사용자 매뉴얼에 반영한다.

확실하지 않은 기능은 다음처럼 임의로 설명하지 않는다.

```text
이 기능은 아마도 ...
```

대신 작업 보고서에서 다음과 같이 분리한다.

```text
Documentation Gap:
확인 가능한 자료가 부족하여 현재 도움말에 포함하지 않음.
```

---

# 4. CR 활용

현재 프로젝트에서 관리되고 있는 CR(Change Request)을 조사한다.

각 CR에 대해 다음을 판단한다.

```text
CR-ID
Title
User Visible Change: Yes / No
Manual Impact: Yes / No
Affected Feature
Affected Manual Section
Related Test
Related Regression Test
Manual Status
```

사용자에게 보이는 기능 변경은 원칙적으로 사용자 매뉴얼 반영 여부를 검토한다.

다음 유형은 특히 Manual Impact가 있는 것으로 본다.

- 신규 사용자 기능
- 메뉴 추가/삭제
- 버튼 및 화면 변경
- 사용자 입력 방법 변경
- 업무 절차 변경
- 설정 옵션 변경
- Import / Export 변경
- 파일 형식 변경
- 오류 메시지 또는 오류 처리 변경
- 사용자 결과값 변경
- 기존 기능의 동작 방식 변경

다음 항목은 일반적으로 사용자 매뉴얼 변경 대상이 아니다.

- 내부 리팩터링
- DB 내부 구조 변경
- 클래스 이름 변경
- 내부 라이브러리 변경
- 사용자 동작에 영향을 미치지 않는 성능 개선

단, 사용자 경험이나 결과에 영향을 주면 문서 반영 여부를 다시 판단한다.

---

# 5. Regression Test 및 Manual Test 활용

Regression Test Suite와 Manual Test Case를 적극적으로 참고한다.

특히 Manual Test Case는 사용자가 실제로 기능을 사용하는 절차를 설명하는 중요한 자료로 활용한다.

예를 들어 테스트가 다음과 같다면:

```text
1. File 메뉴에서 Open을 선택한다.
2. DXF 파일을 선택한다.
3. 도면이 화면에 표시되는지 확인한다.
4. Layer 표시가 정상인지 확인한다.
```

도움말에는 이를 사용자 관점으로 변환한다.

예:

```text
DXF 파일 열기

1. File > Open을 선택합니다.
2. 열고자 하는 DXF 파일을 선택합니다.
3. 파일이 로딩되면 작업 영역에 도면이 표시됩니다.
4. 필요한 경우 Layer 패널에서 표시할 Layer를 선택합니다.
```

테스트 문장을 그대로 복사하지 말고 사용자 문서 형식으로 다시 작성한다.

---

# 6. 사용자 매뉴얼 기본 구성

최상위 `index.html`에서는 다음 구조로 전체 도움말에 접근할 수 있게 한다.

## 1. 프로그램 소개

다음을 설명한다.

- 프로그램의 목적
- 주요 사용 대상
- 프로그램으로 할 수 있는 주요 작업
- 지원하는 주요 파일 형식
- 전체적인 작업 흐름

---

## 2. 설치 및 시작

확인 가능한 경우 다음 내용을 작성한다.

- 설치
- 실행
- 프로그램 시작
- 초기 설정
- 작업 파일 위치
- 기본 환경 설정

확인할 수 없는 설치 방법을 추측해서 작성하지 않는다.

---

## 3. 화면 구성

실제 UI를 조사하여 작성한다.

예:

- Main Menu
- Toolbar
- Drawing Area
- Properties Panel
- Status Bar
- Entity List
- Simulation Panel
- 기타 실제 존재하는 UI

각 화면 요소에 대해 다음을 설명한다.

```text
이름
위치
목적
주요 기능
사용 시점
```

---

# 7. 기능별 도움말

`/features` 디렉토리 아래에서 주요 기능마다 별도의 HTML을 작성한다.

각 기능 페이지는 가능하면 다음 형식을 따른다.

```text
기능명

1. 기능 목적
2. 사용 위치
3. 사용 조건
4. 사용 방법
5. 입력값 및 옵션
6. 실행 결과
7. 주의사항
8. 예제
9. 관련 기능
```

모든 항목을 억지로 채울 필요는 없다.

관련 정보가 없는 항목은 생략할 수 있다.

---

# 8. 업무 시나리오 중심 도움말

단순한 메뉴 설명 외에 사용자가 실제로 하려는 업무를 기준으로 `/workflows` 문서를 만든다.

예:

```text
새 작업 만들기
기존 파일 열기
도형 생성하기
도형 편집하기
설정 변경하기
결과 저장하기
DXF Export 하기
기존 설계 수정하기
오류 수정하기
```

실제 프로그램 기능에 맞게 내용을 결정한다.

업무 시나리오는 다음 구조를 권장한다.

```text
목적

사전 조건

절차
1.
2.
3.

결과

주의사항

관련 도움말
```

---

# 9. 문제 해결

`/troubleshooting/common-errors.html`을 생성한다.

소스코드, CR, 테스트, 기존 이슈 등에서 사용자에게 실제 발생할 수 있는 문제를 조사한다.

각 문제는 가능하면 다음 형식을 사용한다.

```text
증상

가능한 원인

해결 방법

관련 기능
```

개발자용 Exception Stack Trace 설명은 포함하지 않는다.

---

# 10. 용어집

`/reference/terminology.html`을 작성한다.

프로그램에서 사용하는 특수한 용어나 도메인 용어를 설명한다.

예를 들어 CAD/DXF 또는 제조 프로그램이라면 실제 프로그램에서 사용하는 용어를 확인하여 다음처럼 정리한다.

```text
Outer Contour
Inner Contour
Layer
Entity
Bending
V-Cutting
Grooving
DXF
```

단, 프로젝트에 실제 존재하는 용어를 우선한다.

서로 동의어로 사용하는 용어가 있다면 함께 표시한다.

예:

```text
V-Cutting / Grooving
본 프로그램에서는 동일한 의미로 사용한다.
```

---

# 11. 문서 Navigation

모든 도움말 HTML에서 사용자가 쉽게 이동할 수 있어야 한다.

최소한 다음 Navigation을 제공한다.

```text
Home
이전 페이지
다음 페이지
관련 기능
```

가능하면 좌측 Navigation 또는 공통 Menu를 사용한다.

예:

```text
프로그램 소개
시작하기
화면 구성
기본 사용법
기능별 도움말
업무별 도움말
문제 해결
용어집
```

---

# 12. HTML 설계 원칙

도움말은 프로그램과 함께 로컬 파일로 배포되어도 정상적으로 동작해야 한다.

따라서 불필요한 외부 의존성을 만들지 않는다.

가능하면 다음 조건을 만족한다.

- HTML5
- UTF-8
- Responsive Layout
- 외부 서버 없이 동작
- 상대 경로 사용
- JavaScript 최소화
- 별도의 Web Server가 없어도 열람 가능
- Chromium/Edge 등 일반 브라우저에서 정상 표시
- Windows 환경에서 정상 동작

CDN에 반드시 의존하지 않는다.

CSS는 `/help/css/help.css`에서 공통 관리한다.

JavaScript가 필요한 경우 `/help/js/help.js`로 관리한다.

---

# 13. 디자인

화려한 웹사이트를 만드는 것이 목적이 아니다.

기술 매뉴얼로서 다음을 우선한다.

- 가독성
- 일관성
- 빠른 검색
- 명확한 제목 구조
- 코드/메뉴/버튼의 명확한 구분
- 충분한 여백
- Print 가능한 Layout

다음 요소를 적절히 사용한다.

```text
Note
Tip
Warning
Important
Example
```

예:

```text
주의:
저장되지 않은 변경사항은 파일을 닫으면 손실될 수 있습니다.
```

---

# 14. Screenshot 및 이미지

현재 프로젝트에서 프로그램 실행 또는 기존 Screenshot 확인이 가능한 경우 도움말에 필요한 이미지를 준비한다.

이미지는 `/help/images`에 저장한다.

파일 이름은 의미 있는 영문 이름을 사용한다.

예:

```text
main-window.png
file-open-dialog.png
material-settings.png
bend-properties.png
```

이미지에 개인 정보나 개발 환경의 불필요한 정보가 포함되지 않도록 한다.

프로그램을 실행할 수 없거나 Screenshot을 확보할 수 없는 경우 이미지 파일을 임의로 만들지 않는다.

대신 문서 구조만 준비한다.

---

# 15. Help Menu와 연결

프로그램에 기존 Help 메뉴가 있는지 확인한다.

존재한다면 현재 구조를 분석하여 가능한 범위에서 다음 연결을 구현한다.

```text
Help
 ├─ User Manual
 ├─ Getting Started
 ├─ Keyboard Shortcuts
 ├─ Troubleshooting
 └─ About
```

User Manual을 선택하면 로컬의 다음 파일을 열도록 한다.

```text
help/index.html
```

기존 Help 메뉴가 없다면 프로그램 UI 구조와의 영향도를 조사한다.

단순하고 안전하게 추가할 수 있으며 기존 기능을 해치지 않는 경우 구현할 수 있다.

큰 UI 변경이 필요한 경우에는 임의 구현하지 말고 별도 제안사항으로 기록한다.

---

# 16. 설치 패키지 반영

프로젝트에 Installer 또는 Setup 프로젝트가 있다면 이를 조사한다.

Help 디렉토리가 프로그램 설치 시 함께 배포되도록 한다.

설치 결과가 가능하면 다음처럼 되도록 한다.

```text
Program.exe

/help
    index.html
    ...
```

프로그램 실행 파일에서 Help를 호출할 때 설치 위치가 달라도 정상적으로 작동하도록 상대 경로 또는 실행 파일 기준 경로를 사용한다.

절대 경로를 사용하지 않는다.

---

# 17. 문서와 CR Traceability

HTML 사용자 화면에는 CR 번호를 노출할 필요가 없다.

그러나 유지보수를 위해 HTML source에는 필요한 경우 다음과 같은 metadata 또는 comment를 둘 수 있다.

예:

```html
<!--
Related CR:
CR-0047
CR-0051

Related Tests:
RT-0047-01
MT-0051-02
-->
```

또는 별도 관리 파일을 만들 수 있다.

예:

```text
/help/manual-traceability.md
```

권장 구조:

```text
Manual Section
Related CR
Related Test
Last Updated
Status
```

사용자가 보는 HTML과 내부 개발 추적정보를 분리한다.

---

# 18. Documentation Impact 관리 규칙 추가

프로젝트의 AGENTS.md 또는 이에 해당하는 Codex 작업 지침 파일을 확인한다.

기존 규칙을 훼손하지 않는 범위에서 다음 원칙을 추가한다.

```text
## User Documentation Management

Every CR must be evaluated for user documentation impact.

For each CR:

1. Determine whether the CR changes user-visible behavior.
2. Set Manual Impact = Yes or No.
3. If Manual Impact = Yes:
   - identify affected help/manual sections;
   - update the corresponding HTML help pages;
   - update screenshots when required;
   - update troubleshooting information when applicable;
   - update related workflow documentation when applicable.
4. Link the CR to the affected manual sections and related tests.
5. Internal implementation changes that do not affect user behavior
   should not unnecessarily modify the user manual.
6. A CR with Manual Impact = Yes must not be considered fully complete
   until its documentation is updated.
```

다음 Definition of Done 원칙도 반영한다.

```text
CR Definition of Done

Implementation Complete
+
Relevant Automated Tests Passed
+
Relevant Manual Tests Completed
+
Regression Tests Passed
+
User Documentation Updated (when Manual Impact = Yes)
=
CR Complete
```

---

# 19. 기존 CR의 문서화 Backfill

이번 작업은 새로운 CR부터 적용하는 것에 그치지 않는다.

현재 프로젝트의 과거 CR도 조사한다.

가능한 범위에서:

```text
Closed CR
Completed CR
Implemented CR
```

중 사용자 기능에 영향을 준 CR을 찾아 현재 프로그램 사용법에 반영한다.

오래된 CR의 내용과 현재 소스코드가 충돌하는 경우 **현재 구현된 프로그램 동작을 기준으로 한다.**

CR은 기능 변화의 근거 자료로 사용하지만 최종 도움말은 현재 프로그램 상태를 설명해야 한다.

---

# 20. 문서 작성 시 우선순위

정보가 많을 경우 다음 순서로 작성한다.

Priority 1

```text
프로그램 소개
실행 방법
메인 화면
파일 열기
파일 저장
핵심 기능
```

Priority 2

```text
주요 편집 기능
설정
Import / Export
주요 업무 Workflow
```

Priority 3

```text
문제 해결
용어집
Shortcut
세부 기능
```

Priority 4

```text
고급 기능
Rare Case
추가 예제
```

---

# 21. Documentation Gap 관리

현재 자료만으로 설명하기 어려운 기능은 추측하지 않는다.

별도의 파일을 만든다.

```text
/help/DOCUMENTATION_GAPS.md
```

다음 형식을 사용한다.

```text
Feature:
Reason:
Missing Information:
Suggested Action:
Related CR:
Related Source:
```

예:

```text
Feature:
Advanced DXF Export Options

Reason:
UI는 존재하지만 옵션별 동작을 확인할 충분한 테스트 자료가 없음.

Suggested Action:
Manual Test 후 도움말 보완 필요.
```

---

# 22. 도움말 유지보수 파일

다음 내부 관리 파일을 생성하는 것을 권장한다.

```text
/help/README.md
/help/manual-traceability.md
/help/DOCUMENTATION_GAPS.md
```

`README.md`에는 다음을 설명한다.

```text
Help 시스템 구조
새 도움말 페이지 추가 방법
Navigation 변경 방법
Screenshot 저장 규칙
CR과 도움말 연결 방법
문서 갱신 방법
```

이 파일들은 개발자 및 Codex용이며 일반 사용자 메뉴에서는 노출하지 않아도 된다.

---

# 23. 현재 시점의 도움말 작성

구조만 만들고 끝내지 않는다.

현재 프로젝트에서 실제로 확인할 수 있는 내용을 사용하여 가능한 범위에서 매뉴얼 내용을 작성한다.

즉 다음 작업을 실제로 수행한다.

```text
1. 프로젝트 분석
2. 기능 목록 작성
3. CR 분석
4. 테스트 분석
5. Manual Test 분석
6. 사용자 Workflow 추출
7. Help 구조 생성
8. HTML 도움말 작성
9. Navigation 구현
10. 필요한 CSS 작성
11. 프로그램 Help 연결 검토 및 가능한 경우 구현
12. Installer 포함 여부 확인 및 가능한 경우 반영
13. Documentation Gap 작성
14. Traceability 작성
```

단순히 Template만 만든 상태로 완료하지 않는다.

---

# 24. 검증

작업 후 다음을 검증한다.

## HTML 검증

- 깨진 Link가 없는가
- 잘못된 상대경로가 없는가
- 존재하지 않는 이미지가 참조되지 않는가
- UTF-8 한글이 정상 표시되는가
- `index.html`에서 모든 주요 문서로 이동 가능한가
- 문서 사이 Navigation이 정상인가

## 내용 검증

- 실제 프로그램과 설명이 일치하는가
- 존재하지 않는 메뉴를 설명하고 있지 않은가
- 오래된 CR 내용을 현재 기능처럼 설명하고 있지 않은가
- 개발자용 내용을 사용자용 매뉴얼에 과도하게 노출하지 않았는가

## Regression

Help 시스템 추가로 기존 프로그램 기능이 변경되거나 손상되지 않았는지 기존 Regression Test Suite를 실행한다.

자동 테스트가 불가능한 Help 메뉴 동작은 Manual Test Case를 추가한다.

---

# 25. 도움말 관련 Regression Test 추가

가능하면 다음 테스트를 Regression Test Suite에 추가한다.

예:

```text
DOC-RT-001
help/index.html이 존재한다.

DOC-RT-002
index.html의 주요 Navigation 링크가 모두 유효하다.

DOC-RT-003
Help > User Manual 선택 시 help/index.html이 열린다.

DOC-RT-004
설치 결과 Help 디렉토리가 존재한다.

DOC-RT-005
도움말에서 참조하는 이미지 파일이 모두 존재한다.
```

자동화 가능한 테스트와 Manual Test를 구분한다.

예:

```text
AUTO
HTML 파일 존재 확인
Broken Link 확인
Image Reference 확인

MANUAL
Help 메뉴 클릭
Browser 표시 상태
화면 가독성
Screenshot 내용 확인
```

---

# 26. 작업 완료 보고

작업 완료 후 다음 형식으로 보고한다.

```text
## Help System Location

## Created Files

## Updated Files

## Current Manual Coverage

## CRs Reflected in Manual

## Tests Used as Documentation Sources

## Help Integration Status

## Installer Integration Status

## Documentation Gaps

## Added Documentation Tests

## Regression Test Result

## Recommended Next Documentation Work
```

특히 다음 수치를 가능하면 제공한다.

```text
확인한 CR 수
Manual Impact = Yes CR 수
도움말에 반영한 CR 수
도움말 페이지 수
미문서화 기능 수
Documentation Gap 수
```

---

# 핵심 원칙

이 작업에서 가장 중요한 원칙은 다음과 같다.

```text
CR
↓
Implementation
↓
Test / Manual Test
↓
Regression Test
↓
User Manual / Help
↓
Release
```

CR을 단순한 개발 기록으로만 취급하지 말고, 사용자에게 영향을 주는 기능 변경을 문서화하기 위한 추적 기준으로 사용한다.

소스코드가 변경되었지만 사용자 매뉴얼이 갱신되지 않는 상황을 방지한다.

최종 목표는 다음과 같다.

```text
Code와 User Manual이 동일한 프로그램 상태를 설명한다.
```

이번 작업에서는 현재 프로젝트에서 확보 가능한 모든 정보를 조사하여 그 상태에 맞는 사용자 도움말의 첫 번째 Baseline을 작성한다.

기존 기능을 임의로 변경하지 말고, Help 시스템 도입으로 인한 영향 범위를 최소화하면서 작업한다.
