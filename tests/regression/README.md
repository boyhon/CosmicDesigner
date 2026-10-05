# CR Regression Test Suite

정책: ../../docs/project/REGRESSION_POLICY.md
목록: manifest.json
명세: cr/CR-NNN/test-cases.md
실행 기록: results/

기존 CosmicDesigner.Verification 프로젝트와 모든 기존 함수/assertion을 유지한다. 여러 CR에 관련된 동일 함수는 테스트 선택 내에서 한 번 실행하고 연결된 각 ID에 결과를 보고한다. 미연결 기존 기본 검증은 BASE-*로 전체 Suite에 포함한다. 자동화 ID는 함수 검사 범위만 나타내고 수동 acceptance 검증을 대신하지 않는다.

## 실행
Windows + .NET 10 환경에서 기존 검증 프로젝트 Release 빌드 후 아래처럼 실행한다.

- 전체: dotnet <build-directory>/CosmicDesigner.Verification.dll
- 목록: dotnet <build-directory>/CosmicDesigner.Verification.dll --list
- 신규 CR 선택: dotnet <build-directory>/CosmicDesigner.Verification.dll --tests TC-046-001
- 영향 테스트 선택: dotnet <build-directory>/CosmicDesigner.Verification.dll --tests TC-017-001,TC-042-001,TC-045-001
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
