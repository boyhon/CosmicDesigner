# CR 기반 Regression Test Suite 관리 — 2026-10-05

사용자 지침을 프로젝트의 필수 절차로 적용한다.

## Execution Type — 2026-10-05 추가 지침
모든 Test Case에 `Execution Type`을 지정한다. manifest의 필드는 `executionType`이며 기존 `automation`은 호환용 정보다. 분류의 기준은 executionType이다.

| Execution Type | 기준 |
|---|---|
| AUTO | Codex 또는 CI가 사람의 개입 없이 실행하고 결과를 판정한다. |
| MANUAL | 사용자 또는 QA가 직접 실행하고 결과를 판단한다. |
| SEMI_AUTO | Codex가 준비/일부 검증을 수행하지만 최종 판정에는 사람의 확인이 필요하다. |
| NOT_AUTOMATED | 기술적으로 자동화 가능하지만 자동 테스트가 아직 구현되지 않았다. |

TC의 유지 상태(ACTIVE/SUPERSEDED/DEPRECATED), Execution Type, 실행 결과를 별도로 관리한다. 실행하지 못한 테스트는 `PENDING_MANUAL`, `NOT_RUN`, `BLOCKED` 중 실제 상황에 맞게 기록한다. BLOCKED에는 원인을 기록한다. 기존 NOT RUN 표기는 앞으로 NOT_RUN으로 정규화한다.

MANUAL과 사람의 판정이 필요한 SEMI_AUTO를 Codex가 임의로 PASS 처리하지 않는다. 사람의 확인자, 확인 일시 및 관측 결과/근거가 있어야 최종 PASS/FAIL을 기록한다. SEMI_AUTO의 자동 부분 PASS는 전체 PASS가 아니다. NOT_AUTOMATED는 AUTO로 합산하지 않으며 미구현 상태와 사유를 별도 보고한다.

자동 회귀(AUTO)와 수동 회귀(MANUAL 및 사람의 판정이 필요한 SEMI_AUTO)의 PASS/FAIL/미실행 수를 각각 보고한다. NOT_AUTOMATED도 별도 집계한다. 전체 자동 PASS만으로 CR 전체 완료를 주장하지 않는다.

필수 MANUAL/SEMI_AUTO 확인이 남은 CR은 `Verification Status: WAITING FOR USER VERIFICATION`으로 유지한다. 기존 Implemented lifecycle과 병행하며 `CODE COMPLETE`, `AUTOMATED TESTS PASSED`, `USER VERIFICATION REQUIRED`를 근거에 맞게 보고할 수 있다. 필수 확인이 끝나기 전 Verified/Closed로 승격하지 않는다. 과거 승인 기록은 소급 삭제하지 않고 새로 등록한 테스트의 미실행 사실을 별도 기록한다.

보고 예:
```text
Automated Regression: PASS / FAIL / NOT_RUN 수
Manual Regression: PASS / FAIL / PENDING_MANUAL / NOT_RUN / BLOCKED 수
Not Automated: 미구현 수와 사유
CR Status: CODE COMPLETE / AUTOMATED TESTS PASSED / USER VERIFICATION REQUIRED
Verification Status: WAITING FOR USER VERIFICATION
```

## CR 산출물
각 CR은 요구사항, 영향받는 기능/관련 CR, 구현 변경, Test Case, 누적 Suite 반영 여부와 실행 결과를 남긴다. 승인된 기존 기능은 새 CR에서 명시적으로 변경하지 않는 한 유지한다.

## Test Case
ID는 TC-NNN-XXX 형식으로 CR 번호에 연결하며 발급 후 변경/재사용하지 않는다. 목적, 사전 조건, 입력/조작, 기대 결과, 자동화 여부, 구현 위치를 기록한다. 자동화 가능한 것은 자동화하고 GUI/시각 검증은 명확한 수동 절차를 둔다.
상태는 ACTIVE / SUPERSEDED / DEPRECATED이며 변경 CR과 이유를 남기고 삭제하지 않는다.

## 구현 전 영향도
모듈, 데이터 구조, 계산, UI, 기존 CR 및 TC 영향을 분석한다. CR에 Impact Analysis 및 Affected Regression Tests를 기록한다. 과거 CR의 이관은 사후 매핑임을 표시하고 구현 전 분석이었다고 소급 주장하지 않는다.

## 실행 및 Merge 게이트
1. 신규 CR 테스트
2. 영향받는 기존 테스트
3. 전체 자동 Regression Suite (장시간인 경우 Smoke/Affected/Full 구분)

신규/영향/기존 자동 테스트 PASS, 실패 원인 분석 완료 및 의도하지 않은 기존 기능 변경 없음이 확인되기 전 Merge하지 않는다. 수동 테스트를 자동 PASS로 간주하지 않는다. 미실행은 NOT RUN이다.

기존 테스트 실패는 먼저 회귀인지 승인된 요구 변경인지 판단한다. 회귀는 코드를 수정한다. 요구 변경은 근거 CR로 기대 결과 수정/SUPERSEDED 이유를 기록한다. 테스트 통과만을 위해 삭제하거나 기대값을 바꾸지 않는다.

## CR 필수 섹션
- Impact Analysis
- Test Cases
- Affected Regression Tests
- Regression Result: New CR Tests / Affected Tests / Full Regression / Result

기존 CR lifecycle을 유지하고 READY FOR MERGE / NEEDS FIX는 별도 Merge readiness로 기록한다. 테스트/수동 검증이 미완료면 READY FOR MERGE를 주장하지 않는다.

## 종료 보고
CR:
구현 상태:
Changed Files:
New Tests: TC ID 목록
Affected Existing Tests: TC ID 목록
Test Results:
- New CR Tests: PASS/FAIL/NOT RUN
- Affected Regression: PASS/FAIL/NOT RUN
- Full Regression: PASS/FAIL/NOT RUN
Regression Issues:
CR Status: READY FOR MERGE / NEEDS FIX (필요한 미검증은 설명)

## 유지
tests/regression/manifest.json과 cr/CR-NNN/test-cases.md에 누적 관리한다. 기존 검증 executable을 유지하며 ID 선택 실행과 전체 실행을 제공한다. 자동 TC는 기존 함수의 실제 검사 범위만 나타내며 CR 전체 UI 승인을 대신하지 않는다.
