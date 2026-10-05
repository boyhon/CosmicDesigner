# CR-054 Regression Cases

## TC-054-001
- Status: ACTIVE
- Execution Type: AUTO
- Purpose: 열린 절개 모델 생성, ARC 양방향/큰 sweep, 퇴화 거부, 재료 범위, 면적 보존, 저장/단위/삭제/Undo/Redo/ID/3D.
- Preconditions: Windows .NET 10 WPF Release verification build.
- Procedure: CosmicDesigner.Verification.exe --tests TC-054-001
- Expected: assertion 모두 통과하고 exit 0. LINE/ARC가 그대로 보존되고 기존 Outer/Cut 및 재료 면적 변경 없음.
- Implementation: CosmicDesigner.Verification/Program.cs::SlitOperations

## TC-054-002
- Status: ACTIVE
- Execution Type: MANUAL
- Purpose: 제공한 이미지 및 실제 UI 그리기/취소/선택/저장 수용.
- Preconditions: artifacts/CosmicDesigner-1.20.19/CosmicDesigner.exe, 새 문서.
- Procedure:
  1. Slit의 직선/원호/연결선 세 이미지가 첨부와 일치하고 tooltip이 이름을 표시하는지 확인한다.
  2. 직선 버튼, 재료 내부 두 점 클릭. 점선 미리보기와 완료된 검은 선 및 Select 복귀를 확인한다. 수평/수직/대각선을 반복한다.
  3. 원호 버튼, 시작점/통과점/끝점 클릭. 정방향/역방향/180도 이상 원호를 확인한다. 일직선 세 점은 생성되지 않고 끝점 재선택 가능해야 한다.
  4. 연결선 버튼, 세 점 이상 연속 클릭 후 Enter. 미리보기와 열린 연결선 일치, 마지막에서 처음으로 폐합하는 선 없음, Select 복귀를 확인한다.
  5. 각 도구의 미완성 상태에서 Esc. 전체 미완성 경로가 사라지고 문서/Undo에 생성이 없어야 한다. Select/Hole/Fillet 전환 및 New/Open/Undo 후 미완성 경로가 잔류하지 않아야 한다.
  6. 세 경로를 직접/트리 선택하고 금색 강조/속성 확인, Delete, Undo/Redo. 모든 경로 복구 및 천공/외곽 유지 확인.
  7. DXF 저장 후 재열기 및 mm/cm 변환. 원호/연결선 위치와 형태가 같고 절개 내부 재료색/3D 면적은 유지되어야 한다. 3D에서 검은 절개선과 절곡을 가로지른 선을 확인한다.
- Expected: 모든 관측 결과 일치. 자동 PASS는 UI PASS를 대신하지 않는다.
- Result: PENDING_MANUAL. Human reviewer/time/evidence pending.

## TC-054-003
- Status: ACTIVE
- Execution Type: AUTO
- Purpose: 정확한 직선/원호/연결선 선택, 픽셀 tolerance/zoom/pan/정역 sweep/겹침 및 WPF 선택 이벤트/모드 검사.
- Preconditions: Windows/.NET 10 WPF Release 1.20.26.
- Procedure: CosmicDesigner.Verification.exe --tests TC-054-003
- Expected: SlitClickSelection assertion 모두 통과; curve 선택 및 객체 이벤트 동기화, 그리기 모드/눈금자 입력 유지.
- Implementation: CosmicDesigner.Verification/Program.cs::SlitClickSelection

## TC-054-004
- Status: ACTIVE
- Execution Type: MANUAL
- Purpose: Flat 실제 클릭 선택과 OBJECTS/속성 동기화.
- Preconditions: 1.20.26에서 직선/원호/연결선 및 교차 절곡 생성.
- Procedure: Select에서 각 선 몸체/원호의 실제 곡선/연결선 각 구간 클릭. 확대·축소/Pan 후 반복. 절곡과 겹친 절개 클릭, 빈 공간 클릭, OBJECTS 트리 선택을 반복. Delete 후 Undo/Redo. Hole/Slit/V/Fillet 도구 생성 중 선택이 생성 입력을 가로채지 않는지 확인.
- Expected: 클릭한 절개 전체가 금색이고 OBJECTS/Selected object가 일치. 겹친 절곡보다 절개 선택 우선. 연결선의 모든 구간에서 같은 경로 선택. Delete/Undo/Redo와 도구 생성 정상.
- Result: PENDING_MANUAL; reviewer/time/evidence pending.
