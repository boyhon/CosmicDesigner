# CR-053 Test Cases

## TC-053-001

- Execution Type: AUTO
- Status: ACTIVE
- Purpose: non-Bent Section 구간 합계가 Flat W/H 총 길이가 되며 두께 보정이 중복 차감되지 않는지 검증한다.
- Preconditions: Windows, .NET 10, Release build.
- Procedure: `--tests TC-053-001`로 `FlatLengthFromSectionSegments`를 실행한다.
- Expected: 두께 2의 두 절곡에서 `199 + 1998 + 199 = 2396`이 H와 W 모두 유지되고, 두께를 4로 바꿔도 총 길이는 유지되며 Bent 외곽 치수만 보정된다.
- Implementation: `CosmicDesigner.Verification/Program.cs::FlatLengthFromSectionSegments`

## TC-053-002

- Execution Type: MANUAL
- Status: ACTIVE
- Result: PENDING_MANUAL
- Purpose: 실제 Section Designer 입력과 Flat Material 표시를 확인한다.
- Preconditions: CosmicDesigner 1.20.18, 두께 2, H 절곡 두 개가 있는 문서.
- Procedure: H Section Designer non-Bent Mode에서 세 구간을 차례로 `199`, `1998`, `199`로 입력한다. 좌측 Material의 Flat H, Flat Designer 외곽, H Section 표시를 확인한다. W 방향에서도 같은 값을 입력한다. Undo/Redo 후 값을 다시 확인한다.
- Expected: Flat H와 Flat W가 각각 `2396`이며 외곽과 Section 표시가 갱신된다. Bent Mode 외곽 치수에는 두께 보정이 유지된다.
- Evidence: 사용자 또는 QA 확인자, 확인 일시와 관측 결과가 필요하며 현재 사람 판정은 기록되지 않았다.
