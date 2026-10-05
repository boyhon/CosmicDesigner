# 현재 개발 산출물 Freeze 및 Release Candidate 생성 작업

현재 시점까지 개발된 프로젝트 전체 산출물을 하나의 안정된 기준점(Baseline)으로 Freeze한다.

이번 작업의 목적은 현재 상태를 손실 없이 Git에 보존하고, 이 Freeze된 소스를 기준으로 실행 파일 및 설치 프로그램을 생성하여 Pull Request를 요청할 수 있는 Release Candidate 상태를 만드는 것이다.

작업 대상은 프로그램 소스코드만이 아니다.

현재 프로젝트에서 향후 개발, 유지보수, 테스트, 문서화, 빌드, 설치에 필요한 모든 관리 대상 파일을 확인하여 Freeze 범위에 포함한다.

---

# 1. 기본 원칙

이번 작업에서는 새로운 기능을 추가하지 않는다.

이번 작업의 목적은 다음과 같다.

```text
현재 개발 상태 확인
↓
Freeze 범위 결정
↓
불필요 파일 제외
↓
현재 산출물 검증
↓
실행 파일 Build
↓
Installer 생성
↓
설치 검증
↓
Git Commit
↓
Git Push
↓
Pull Request 요청
```

Freeze 이후에는 현재 기준점이 명확하게 재현될 수 있어야 한다.

즉 Git에서 해당 Commit을 Checkout하면 다음이 가능해야 한다.

```text
1. 소스 확인
2. 도움말 확인
3. 필요한 Resource 확인
4. Build 수행
5. 실행 파일 생성
6. Installer 생성
7. 설치
8. 프로그램 실행
9. 도움말 사용
```

---

# 2. Freeze 대상 정의

먼저 프로젝트 전체를 조사하여 Freeze 대상 파일을 분류한다.

최소한 다음 항목을 확인한다.

## A. Source Code

- 프로그램 소스코드
- 프로젝트 파일
- 솔루션 파일
- 빌드 설정
- 공통 라이브러리
- 내부 모듈
- 스크립트

## B. User Documentation

- `/help` 디렉토리
- HTML 사용자 매뉴얼
- CSS
- JavaScript
- 도움말 이미지
- troubleshooting 문서
- terminology 문서
- manual-traceability 문서
- DOCUMENTATION_GAPS 문서
- Help 관련 README

## C. Resources

향후 개발 및 실행에 필요한 Resource를 조사한다.

예:

- 프로그램 아이콘
- 이미지
- 템플릿
- 샘플 데이터
- 샘플 DXF
- 기본 설정 파일
- localization resource
- manifest
- installer resource
- 기타 runtime resource

프로그램 빌드나 향후 개발에 필요한 파일은 Freeze 대상에 포함한다.

## D. Tests

- Unit Test
- Integration Test
- Regression Test Suite
- Manual Test
- Documentation Test
- Test Data
- Test Scripts

## E. Project Management Artifacts

- AGENTS.md
- CR 관리 파일
- CHANGELOG
- Release Note
- README
- Build 문서
- 개발 지침
- 환경 구성 문서

## F. Build / Installer Related Files

- Build script
- Installer script
- Setup project
- packaging script
- version resource
- manifest
- installer image/resource

---

# 3. Freeze 제외 대상

Git에 포함하면 안 되는 파일을 식별한다.

예:

```text
bin/
obj/
.vs/
*.user
*.suo
*.tmp
*.log
cache/
temporary files
local build output
user-specific settings
IDE state files
```

다만 이미 생성된 실행 파일이나 설치 파일을 Git으로 관리하는 현재 프로젝트 정책이 있다면 기존 정책을 따른다.

기존 정책이 불명확한 경우 다음 원칙을 적용한다.

```text
Source와 재현 가능한 Build Output은 분리한다.
```

즉 일반적인 경우 다음은 Git Commit 대상에서 제외한다.

```text
*.exe
*.dll
*.msi
*.zip
bin/
obj/
dist/
release-output/
```

그러나 Release Artifact로 별도 보관해야 한다면 Git Repository에 직접 Commit하지 말고 Release Artifact 또는 별도 output 디렉토리로 관리한다.

현재 Repository의 기존 관리방식을 먼저 확인하고 일관성을 유지한다.

---

# 4. .gitignore 점검

현재 `.gitignore`를 확인한다.

다음 항목이 적절하게 제외되고 있는지 확인한다.

- IDE 임시파일
- 빌드 중간파일
- 사용자별 환경설정
- cache
- log
- temporary files
- local secret
- API key
- credential
- 개인 경로 정보

필요한 경우 `.gitignore`를 보완한다.

단, 프로젝트에 필요한 Resource가 잘못 Ignore되지 않도록 주의한다.

---

# 5. Secret 및 민감정보 검사

Commit 전에 다음을 반드시 확인한다.

```text
API Key
Access Token
GitHub Token
Password
DB Password
Private Certificate
Private Key
Credential
Local User Path
Machine-specific Secret
```

이러한 정보가 소스나 설정 파일에 포함되어 있다면 Commit하지 않는다.

필요한 경우 예제 설정 파일로 변경한다.

예:

```text
config.example.json
.env.example
```

실제 Secret 값은 제거한다.

---

# 6. 현재 상태 Freeze

현재 작업 트리를 확인한다.

다음 명령에 준하는 상태를 확인한다.

```text
git status
git branch
git log
```

현재 Branch가 무엇인지 확인한다.

현재 Branch가 개발 작업 Branch라면 그 Branch에서 Freeze 작업을 수행한다.

임의로 main/master에 직접 Commit하지 않는다.

현재 프로젝트의 Branch 운영 규칙을 확인한다.

---

# 7. Freeze Baseline 식별

현재 Freeze 상태를 명확히 식별할 수 있도록 기준 정보를 남긴다.

가능하면 다음 정보를 문서화한다.

```text
Freeze Date
Freeze Branch
Freeze Commit
Application Version
Documentation Version
Build Configuration
Installer Version
```

예:

```text
Freeze Baseline
Version: 0.9.0-rc1
Status: Release Candidate
```

기존 Versioning 정책이 있으면 그것을 따른다.

정책이 없으면 임의로 큰 버전 변경을 하지 말고 현재 버전을 유지하면서 Release Candidate 식별자만 사용하는 것을 우선 검토한다.

---

# 8. Release Note 작성 또는 갱신

현재 Freeze 시점의 Release Note 또는 CHANGELOG를 확인한다.

필요하면 다음 내용을 추가한다.

```text
Version
Date
Included CRs
Implemented Features
Known Limitations
Documentation Status
Regression Test Status
Build Status
Installer Status
```

특히 이번 Freeze에 포함된 주요 CR을 식별한다.

---

# 9. Build 전 검증

Build 전에 현재 소스가 일관된 상태인지 확인한다.

다음을 확인한다.

```text
Broken Project Reference 없음
Missing Resource 없음
Missing Help File 없음
Missing Config Template 없음
Missing Dependency 없음
```

가능하면 기존 Automated Test를 실행한다.

```text
Unit Test
Integration Test
Regression Test
Documentation Test
```

실패가 있으면 원인을 확인한다.

이번 작업의 목적은 기능 개발이 아니므로 큰 기능 수정은 하지 않는다.

Freeze를 막는 명확한 Build 오류, Resource 누락, 문서 링크 오류 등만 최소 범위에서 수정한다.

---

# 10. 실행 파일 생성

Freeze 대상이 확정되면 Freeze된 소스를 기준으로 실행 파일을 생성한다.

기존 Build 방식과 Build Configuration을 먼저 확인한다.

예:

```text
Release
x64
Any CPU
```

프로젝트에 정의된 정상 Build 절차를 따른다.

---

# 11. 기존 실행 파일 존재 여부 확인

실행 파일을 무조건 다시 만들지 않는다.

다음 조건을 확인한다.

기존 실행 파일이 존재하고:

```text
1. 현재 Freeze 대상 소스와 대응되는 Build 결과이며
2. 최신 소스보다 오래되지 않았고
3. Version이 일치하며
4. 필요한 Resource가 포함되어 있고
5. 정상 실행 가능한 것이 확인되면
```

Build를 Skip할 수 있다.

반대로 다음 중 하나라도 해당되면 다시 Build한다.

```text
Source가 변경됨
Help가 변경됨
Resource가 변경됨
Version이 다름
Build 시점 불명확
실행 파일 손상 여부 불명확
```

단순히 EXE 파일이 존재한다는 이유만으로 Build를 Skip하지 않는다.

---

# 12. Build 결과 검증

생성 또는 재사용한 실행 파일을 확인한다.

가능하면 다음을 검증한다.

```text
프로그램 실행
Main Window 표시
주요 Menu 표시
Help 메뉴 표시
기본 파일 Open
기본 Save
주요 핵심 기능
정상 종료
```

기존 Regression Test Suite가 있다면 실행한다.

---

# 13. Installer 생성

현재 Freeze된 프로그램을 설치할 수 있는 Installer를 생성한다.

기존 Installer 프로젝트 또는 Setup Script가 있으면 그것을 우선 사용한다.

없다면 현재 프로젝트 기술 스택에 적합한 기존 프로젝트 표준 설치 방식으로 구성한다.

Installer는 다음 항목을 설치해야 한다.

```text
Program Executable
Required DLLs
Runtime Resources
Configuration Templates
Icons
Help Directory
Help HTML
Help CSS
Help JavaScript
Help Images
기타 Runtime 필수 파일
```

---

# 14. Help 파일 설치 포함

도움말 시스템 전체를 반드시 Installer에 포함한다.

설치 결과가 예를 들어 다음과 같아야 한다.

```text
<Application Install Directory>
    Program.exe

    /help
        index.html
        getting-started.html
        screen-layout.html

        /features
        /workflows
        /troubleshooting
        /reference
        /images
        /css
        /js
```

프로그램 내부 Help 메뉴가 다음 파일을 정상적으로 열 수 있어야 한다.

```text
help/index.html
```

설치 경로가 바뀌어도 동작하도록 절대 경로를 사용하지 않는다.

프로그램 실행 파일 기준 상대경로 또는 설치 디렉토리 기준 경로를 사용한다.

---

# 15. Installer 설치 후 즉시 사용 가능 조건

설치 완료 후 추가적인 수동 파일 복사 없이 프로그램이 즉시 사용 가능해야 한다.

다음을 검증한다.

```text
Installer 실행
↓
설치 완료
↓
프로그램 실행
↓
필수 Resource 정상 Load
↓
Help 메뉴 실행
↓
사용자 매뉴얼 표시
```

사용자가 소스 디렉토리나 개발 환경의 파일에 접근해야 동작하는 상태는 허용하지 않는다.

---

# 16. Installer Version 정보

Installer와 프로그램 Version을 가능한 범위에서 일치시킨다.

예:

```text
Application Version: 0.9.0-rc1
Installer Version: 0.9.0-rc1
Documentation Baseline: 0.9.0-rc1
```

기존 Versioning 정책이 있으면 그 정책을 따른다.

---

# 17. Clean Install Test

가능하면 Clean Install Test를 수행한다.

테스트 기준:

```text
1. 기존 설치 제거 또는 별도 Clean Test 위치 준비
2. Installer 실행
3. 설치 성공
4. 프로그램 실행
5. 주요 화면 정상 표시
6. 필수 Resource Load 확인
7. Help 메뉴 실행
8. help/index.html 정상 표시
9. 주요 Help Link 정상 동작
10. 프로그램 종료
```

실제 OS 환경 제약으로 설치 테스트가 불가능하면 그 사실을 명확히 보고한다.

---

# 18. 설치 프로그램 산출물 관리

Installer Output 디렉토리를 명확하게 한다.

예:

```text
/release
    /0.9.0-rc1
        ProgramSetup.exe
        checksums.txt
        release-notes.md
```

또는 현재 프로젝트 구조에 맞는 Release Output 디렉토리를 사용한다.

Source Tree와 Build Output을 혼합하지 않는다.

---

# 19. Checksum 생성

가능하면 최종 배포 실행 파일 및 설치 파일에 대해 SHA-256 checksum을 생성한다.

예:

```text
Program.exe
ProgramSetup.exe
```

결과는 예를 들어 다음 파일에 저장한다.

```text
checksums.txt
```

---

# 20. Git Commit 전 최종 검증

Commit 전에 다시 다음을 확인한다.

```text
git status
```

의도하지 않은 파일이 포함되어 있지 않은지 확인한다.

특히 다음을 점검한다.

```text
temporary file
log
local path
secret
token
unnecessary binary
cache
IDE user setting
```

---

# 21. Git Commit

Freeze 대상 전체를 하나의 논리적인 Baseline Commit으로 Commit한다.

Commit Message는 프로젝트 기존 규칙을 따른다.

규칙이 없다면 다음과 같은 형식을 사용할 수 있다.

```text
chore(release): freeze current project baseline for release candidate
```

또는:

```text
release: freeze current source, help, resources and installer baseline
```

Commit에는 다음이 포함되어야 한다.

```text
Source Code
Help
Resources
Tests
CR-related files
Build Scripts
Installer Source
Documentation
Project Configuration
```

단, Git 관리 대상이 아닌 Build Artifact는 포함하지 않는다.

---

# 22. Commit 검증

Commit 후 다음을 확인한다.

```text
git status
```

Working Tree가 의도한 상태인지 확인한다.

또한:

```text
git show --stat HEAD
```

등을 통해 Freeze Commit에 포함된 파일을 검토한다.

---

# 23. Git Push

현재 작업 Branch를 Remote에 Push한다.

예:

```text
git push origin <current-branch>
```

강제 Push는 하지 않는다.

```text
--force
--force-with-lease
```

는 명확한 필요와 프로젝트 정책이 없는 한 사용하지 않는다.

---

# 24. Pull Request 요청

Push 후 현재 Branch에서 대상 Branch로 Pull Request를 생성하거나 생성 가능한 상태를 준비한다.

대상 Branch는 기존 프로젝트 정책을 확인한다.

예:

```text
feature/release-freeze
→
main
```

또는:

```text
codex
→
master
```

기존 Branch 전략을 따른다.

---

# 25. PR 제목

PR 제목은 Freeze 목적이 명확하게 드러나도록 한다.

예:

```text
Freeze current development baseline and prepare installer
```

또는:

```text
Release candidate: source, help, resources, build and installer baseline
```

---

# 26. PR 본문

PR 본문에는 최소한 다음 내용을 포함한다.

```text
## Purpose

Freeze current development baseline and prepare a reproducible
release candidate.

## Included

- Application source
- Help / user manual
- Runtime and development resources
- Tests
- CR-related artifacts
- Build configuration
- Installer source

## Build

- Build status:
- Executables:
- Rebuilt / reused:

## Installer

- Installer:
- Help included:
- Clean install result:

## Tests

- Automated tests:
- Regression tests:
- Manual tests:
- Documentation tests:

## Documentation

- Help baseline:
- Documentation gaps:

## Known Issues

- ...

## Freeze Baseline

- Branch:
- Commit:
- Version:
```

---

# 27. PR 생성 권한이 없는 경우

GitHub 인증 또는 권한 문제로 PR을 직접 생성할 수 없다면 작업을 중단하지 않는다.

다음까지 완료한다.

```text
Commit 완료
Push 완료
PR 제목 작성
PR 본문 작성
Target Branch 확인
```

그리고 사용자가 바로 PR을 생성할 수 있도록 정확한 정보를 제공한다.

---

# 28. 기존 파일을 임의 삭제하지 말 것

Freeze 과정에서 불필요해 보이는 파일을 발견하더라도 바로 삭제하지 않는다.

특히 다음 파일은 향후 개발 Resource일 가능성을 확인한다.

```text
Sample
Reference Drawing
Test Data
Template
Prototype
Icon
Design Resource
Migration Script
Historic Config
```

명확한 임시파일이 아니라면 보존을 우선한다.

삭제가 필요한 경우 이유가 명확해야 한다.

---

# 29. Freeze 이후 변경 금지 원칙

Freeze Commit이 만들어진 이후에는 해당 Commit 자체를 수정하지 않는다.

추가 수정이 필요하면 새로운 Commit으로 처리한다.

Amend나 History Rewrite는 특별한 이유가 없는 한 하지 않는다.

즉 Freeze Commit은 현재 개발 상태의 역사적 기준점으로 유지한다.

---

# 30. Freeze와 Release Artifact 구분

다음을 명확하게 구분한다.

```text
Git Freeze Baseline
=
Source + Documentation + Resource + Test + Build Definition

Release Artifact
=
Executable + Installer + Checksum + Release Note
```

Git Repository는 재현 가능한 개발 기준점을 보존하는 것이 목적이다.

실행 파일과 Installer는 Freeze된 소스로부터 생성된 배포 산출물이다.

---

# 31. Traceability 유지

가능하면 현재 Release Candidate와 CR의 관계를 남긴다.

예:

```text
Release Candidate
    ├─ CR-xxxx
    ├─ CR-xxxx
    ├─ CR-xxxx
    └─ Documentation updates
```

Release Note 또는 별도 문서에서 이번 Freeze에 포함된 주요 CR을 기록한다.

---

# 32. Regression Test

Installer 및 Help 추가로 기존 프로그램이 손상되지 않았는지 확인한다.

기존 Regression Test Suite를 실행한다.

특히 다음을 포함한다.

```text
Program startup
File open
File save
Main editing features
Existing calculations
Export
Help invocation
Resource loading
```

기존 테스트가 존재하지 않는 기능은 임의로 새 기능 테스트를 대량 추가하지 않는다.

Freeze 검증에 필요한 최소 테스트만 보완한다.

---

# 33. Documentation Test

다음을 확인한다.

```text
help/index.html 존재
Broken Link 없음
Missing Image 없음
잘못된 상대경로 없음
Installer에 help 포함
Installed Help 정상 표시
```

가능하면 자동화한다.

---

# 34. 최종 산출물

이번 작업 종료 시 다음 산출물이 존재해야 한다.

## Git

```text
Freeze Commit
Remote Push
Pull Request 또는 PR 생성 준비
```

## Build

```text
검증된 실행 파일
```

## Installer

```text
설치 프로그램
```

## Documentation

```text
Help Directory
Release Note
필요한 Build / Installer 설명
```

## Verification

```text
Test Result
Regression Result
Clean Install Result
Documentation Test Result
```

---

# 35. 작업 완료 보고

최종 보고는 다음 형식을 사용한다.

```text
# Freeze Result

## 1. Freeze Baseline

Branch:
Commit:
Version:
Date:

## 2. Freeze Scope

Source:
Help:
Resources:
Tests:
Project Files:
Installer Source:

## 3. Excluded Files

...

## 4. Build Result

Executable:
Build Configuration:
Build Status:
Rebuilt or Reused:
Reason:

## 5. Installer Result

Installer:
Version:
Help Included:
Runtime Resources Included:

## 6. Installation Test

Install:
Program Start:
Help:
Resource Load:
Result:

## 7. Test Result

Unit:
Integration:
Regression:
Manual:
Documentation:

## 8. Git Result

Commit:
Push:
Remote Branch:

## 9. Pull Request

Title:
Target Branch:
PR URL:
Status:

## 10. Release Artifacts

Executable:
Installer:
Checksum:
Release Note:

## 11. Known Issues

...

## 12. Documentation Gaps

...

## 13. Remaining Risks

...
```

PR URL을 직접 생성할 수 없는 경우:

```text
PR URL: Not created
Reason:
Required Action:
```

를 작성한다.

---

# 36. 매우 중요한 작업 제한

이번 작업에서 다음을 하지 않는다.

```text
새 기능 추가
대규모 Refactoring
UI 재설계
알고리즘 변경
기존 정상 기능 수정
불필요한 파일명 변경
불필요한 디렉토리 이동
Git History Rewrite
Force Push
```

Freeze를 위해 꼭 필요한 Build 오류, Resource 누락, Installer 오류, Help 경로 오류 등의 최소 수정만 허용한다.

---

# 최종 목표

이번 작업의 완료 조건은 다음과 같다.

```text
현재 프로젝트의 모든 관리 대상 산출물이
하나의 Git Commit으로 Freeze되어 있다.

+

해당 Freeze Commit이 Remote Repository에 Push되어 있다.

+

Freeze된 소스를 이용하여 실행 파일을 만들 수 있다.

+

실행 파일이 이미 Freeze 소스와 정확히 일치하는 경우에는
검증 후 재사용할 수 있다.

+

Freeze된 프로그램을 설치할 수 있는 Installer가 존재한다.

+

Installer에는 User Help 전체가 포함된다.

+

설치 직후 프로그램과 Help를 바로 사용할 수 있다.

+

Regression Test와 설치 검증 결과가 기록되어 있다.

+

Pull Request가 생성되었거나,
즉시 생성할 수 있는 상태까지 준비되어 있다.
```

이 상태를 현재 프로젝트의 Release Candidate Baseline으로 만든다.
