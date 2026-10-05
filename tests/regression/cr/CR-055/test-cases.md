# TC-055-001 — 객체 속성 영역 순서
- Status: ACTIVE
- Execution Type: MANUAL
- Purpose: OBJECTS 상단 / PROPERTIES 하단 배치와 실제 선택·속성 동작.
- Preconditions: CosmicDesigner 1.20.20 새 문서 및 객체가 있는 문서.
- Procedure: 좌측 상단 OBJECTS 제목/트리, 그 아래 PROPERTIES 제목/Material/Selected object 순서 확인. 트리에서 Material/Cut/ARC를 선택하여 속성 갱신과 Flat 강조를 확인. 속성 편집 후 연동 확인. 최소 창 크기와 항목이 많은 문서에서 스크롤로 하단 속성에 접근한다.
- Expected: OBJECTS가 위에 있고 PROPERTIES가 아래에 있으며 선택·속성 편집·스크롤이 정상 작동한다.
- Implementation: MainWindow.xaml; human acceptance.
- Result: PENDING_MANUAL; reviewer/time/evidence pending.
