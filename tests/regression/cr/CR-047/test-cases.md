# CR-047 Regression Cases

## TC-047-001
Execution Type: AUTO / ACTIVE
Purpose: 기존 Triangle 윤곽을 평행 이동하여 좌하 좌표 입력을 적용한다.
Preconditions: Windows/.NET 10 Release verification build.
Procedure: --tests TC-047-001
Expected: 임의 삼각형 모든 endpoint의 동일 delta, width/height, Inner Contour, 경계 clamp, 유효하지 않은 입력 거부, mm 단위, Undo/Redo, DXF 왕복 및 Fillet ARC 보존.
Implementation: CosmicDesigner.Verification/Program.cs::TriangleLowerLeft

## TC-047-002
Execution Type: MANUAL / ACTIVE
Result: PENDING_MANUAL
Preconditions: 1.20.11 실행, cm 문서에서 Triangle 생성 후 꼭짓점 편집.
Procedure:
1. Object Tree 또는 Flat 몸체로 전체 선택한다. Center 대신 Lower-left X/Y가 표시되는지 확인한다.
2. 외접 사각형의 최솟값 X/Y와 속성을 비교한다. X만 변경하고 다른 축/세 변의 모양을 확인한다. Y도 변경한다.
3. Width/Height를 수정하지 않고 focus를 거친 뒤 좌표를 변경한다. 모양과 상하 방향이 유지되는지 확인한다.
4. 음수/경계 초과 위치 입력 시 재료 안으로 제한되고 속성 표시가 실제 위치와 일치하는지 확인한다.
5. Undo/Redo와 저장/재열기를 확인한다. mm/m 문서의 위치 단위 표시와 입력을 확인한다.
6. Rectangle/Circle의 Center 속성과 Triangle 개별 LINE 선택 속성이 유지되는지 확인한다.
Expected: 전체 Triangle 위치만 좌하 기준으로 표시/편집되며 실제 윤곽과 일치한다.
Reviewer/date/evidence: 사용자 검증 대기.

## TC-047-003
Execution Type: AUTO / ACTIVE
Purpose: Width/Height 변경 시 좌하 위치와 반대 치수가 바뀌지 않는다.
Preconditions: Windows/.NET 10 Release.
Procedure: --tests TC-047-003
Expected: 임의/역삼각형의 폭/높이 독립 변경, 꼭짓점 축별 비례 유지, Inner Contour, Undo/Redo, DXF, mm 단위. 재료 범위 초과/비유한/최소 미만 및 ARC 윤곽 변경 시 무변경 거부.
Implementation: CosmicDesigner.Verification/Program.cs::TriangleAnchoredSize

## TC-047-004
Execution Type: MANUAL / ACTIVE
Result: PENDING_MANUAL
Preconditions: 1.20.12에서 Triangle 전체 선택; 꼭짓점 편집 후 좌하 X/Y와 Width/Height 기록.
Procedure: Width만 늘린 뒤 Tab을 눌러 X/Y/Height가 유지되는지 확인한다. Height만 줄인 뒤 X/Y/Width를 비교한다. 반복 입력, 다른 속성으로 focus 이동, Undo/Redo, 저장/재열기에서 윤곽과 숫자를 비교한다. 경계 초과 크기는 위치를 옮기지 않고 원래 입력으로 복원되고 거부 사유가 표시되는지 확인한다. mm/m 단위에서도 확인한다. Fillet Triangle 크기 입력은 기존 ARC를 훼손하지 않고 거부되는지 확인한다.
Expected: 입력한 치수만 변경; 두 위치 및 반대 치수 유지; 임의 삼각형의 비율 위치와 방향 유지.
Reviewer/date/evidence: 사용자 검증 대기.
