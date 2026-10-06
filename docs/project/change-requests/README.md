# Change Request Index

Change Requests after the `2026-10-01` baseline are stored in this directory.

Users submit requirements in natural language; they are not expected to assign CR numbers or follow the template. Codex checks this index and existing CR files, updates overlaps or issues the next unused sequential number, preserves the source, and updates `PROJECT_STATUS.md` according to the [Change Request Intake policy](../DEVELOPMENT_POLICY.md#4-change-request-intake).

## Next available number

`CR-067`

The project-management setup and baseline documentation do not consume a CR because they do not change application behavior.

## Index

| CR | Title | Status | Release | Commit |
|---|---|---|---|---|
| [CR-001](CR-001.md) | CDF 파일 포맷 제안 및 승인 정책 | Verified | 1.2.0 | — |
| [CR-002](CR-002.md) | 최근 파일 10개 열기 및 지속성 | Verified | 1.2.0 | — |
| [CR-003](CR-003.md) | View 메뉴와 전역 표시 상태 지속성 | Verified | 1.2.0 | — |
| [CR-004](CR-004.md) | Flat Designer 기본 포인터, Pan, Zoom, 눈금자와 격자 | Implemented | 1.3.0 | — |
| [CR-005](CR-005.md) | 모든 Hole 객체의 선택, 이동과 Resize 직접 조작 | Implemented | 1.3.0 | — |
| [CR-006](CR-006.md) | Section Designer 치수 편집기의 투명 표시와 외곽 치수 기준 | Implemented | 1.3.1 | — |
| [CR-007](CR-007.md) | W/H Section Designer 보기 상태의 파일별 저장 | Implemented | 1.3.0 | — |
| [CR-008](CR-008.md) | W/H Section Designer 마우스 휠 확대·축소 | Implemented | 1.3.0 | — |
| [CR-009](CR-009.md) | Section Designer 쐐기 생성 클릭 범위 제한 | Implemented | 1.3.0 | — |
| [CR-010](CR-010.md) | 3D Preview 단순 투명 아크릴 골격 표현 | Implemented | 1.3.3 | — |
| [CR-011](CR-011.md) | Section Designer CAD 외곽 치수 기준 및 판재 두께 표시 | Implemented | 1.3.4 | `f1c55a0` |
| [CR-012](CR-012.md) | Flat Designer H/W 단면 선택선 및 연동 | Implemented | 1.4.0 | `f1c55a0` |
| [CR-013](CR-013.md) | W Section Bent 적응형 화면 배율 교정 | Implemented | 1.4.1 | `f1c55a0` |
| [CR-014](CR-014.md) | Section Designer 중앙 고정 Zoom | Implemented | 1.4.2 | `f1c55a0` |
| [CR-015](CR-015.md) | Outer Contour 경계 천공 병합 | Implemented | 1.5.1 | `f1c55a0`, `ade8b61` |
| [CR-016](CR-016.md) | 선택 위치의 실제 Outer Contour 단면 표시 | Implemented | 1.6.0 | `daad64f` |
| [CR-017](CR-017.md) | Flat Designer Outer Contour LINE 직접 편집 | Implemented | 1.7.0 | `2b1a9d3` |
| [CR-018](CR-018.md) | 잘린 Section 치수 편집과 Outer Contour 연쇄 갱신 | Implemented | 1.8.0 | `037bce9` |
| [CR-019](CR-019.md) | 3D Preview Outer Contour 컷 형상 반영 | Implemented | 1.9.0 | `0ecd6cd` |
| [CR-020](CR-020.md) | File 메뉴 New/Open/Save 단축키 | Implemented | 1.10.0 | `24762b7` |
| [CR-021](CR-021.md) | 미저장 변경 사항 저장 확인 | Implemented | 1.11.0 | `882f035` |
| [CR-022](CR-022.md) | Edit Settings 사용자 환경 설정창 | Implemented | 1.12.0 | `c72934b` |
| [CR-023](CR-023.md) | 문서 단위와 두 가지 단위 변경 방식 | Implemented | 1.12.0 | `c72934b` |
| [CR-024](CR-024.md) | 문서명 우선 Window Title | Implemented | 1.12.1 | `41151ca` |
| [CR-025](CR-025.md) | Ruler 및 Grid 간격 설정 | Implemented | 1.13.0 | `f1486b2` |
| [CR-026](CR-026.md) | Flat Designer 절삭 영역 흰색 표시 | Implemented | 1.13.1 | `cf205bf` |
| [CR-027](CR-027.md) | 3D Preview 내부 Cut 투명 표시 | Implemented | 1.13.2 | `d178aff` |
| [CR-028](CR-028.md) | 3D Preview 내부 절곡 Edge 제거 | Implemented | 1.13.3, 1.13.4 | `0592574`, `94d9fcd` |
| [CR-029](CR-029.md) | 3D Preview V/V1 절곡선 색상 표시 | Implemented | 1.14.0 | `e33bb68` |
| [CR-030](CR-030.md) | 삼각형 Cut 꼭짓점 직접 편집 | Implemented | 1.15.0 | `62bc5b6` |
| [CR-031](CR-031.md) | 원·삼각형·사각형 Cut 드래그 생성 | Implemented | 1.16.0 | `6fbf06b` |
| [CR-032](CR-032.md) | Triangle Cut Outer Contour 명시적 통합 | Implemented | 1.17.0 | `e244747` |
| [CR-033](CR-033.md) | Outer Contour 직각 모서리 Fillet | Implemented | 1.18.0 | `09c2abb` |
| [CR-034](CR-034.md) | 다중 모서리 연속 Fillet 교정 | Implemented | 1.18.1 | `41e53f3` |
| [CR-035](CR-035.md) | Fillet ARC 선택 표시 및 반지름 편집 | Implemented | 1.19.0 | `d57839a` |
| [CR-036](CR-036.md) | 임의 각도 및 내부 Hole Fillet | Implemented | 1.20.0 | `45557d9` |
| [CR-037](CR-037.md) | 천공 도구 정리 및 도형 아이콘 버튼 | Implemented | 1.20.1 | — |
| [CR-038](CR-038.md) | 다이아몬드·평행사변형 드래그 생성 | Implemented | 1.20.2 | — |
| [CR-039](CR-039.md) | 3D 천공 면과 윤곽선 정합성 교정 | Implemented | 1.20.3 | — |
| [CR-040](CR-040.md) | 평행사변형 Cut Outer Contour 통합 | Implemented | 1.20.4 | — |
| [CR-041](CR-041.md) | 삼각형 천공 드래그 방향 반영 | Implemented | 1.20.5 | — |
| [CR-042](CR-042.md) | Outer Contour LINE 삭제 후 폐합 및 재료 영역 정합성 | Implemented | 1.20.6 | — |
| [CR-043](CR-043.md) | 사선 Outer Contour의 사각형 통합 복구 | Implemented | 1.20.7 | — |
| [CR-044](CR-044.md) | 3D Preview Fillet 곡선 표시 | Implemented | 1.20.8 | — |
| [CR-045](CR-045.md) | Fillet ARC 외곽의 천공 통합 | Implemented | 1.20.9 | — |
| [CR-046](CR-046.md) | Outer Contour 편집 후 틈 자동 LINE 연결 | Implemented | 1.20.10 | — |

| [CR-047](CR-047.md) | Triangle Cut 좌하단 위치 속성 | Implemented | 1.20.12 | — |

| [CR-048](CR-048.md) | Circle 반지름 속성 | Implemented | 1.20.13 | — |

## File naming

```text
CR-001.md
CR-002.md
...
```

## Source-document decomposition

The initial source bundle is preserved unchanged at [COSMIC_DESIGNER_CHANGE_REQUIREMENTS.md](../../COSMIC_DESIGNER_CHANGE_REQUIREMENTS.md). Its requirement IDs map to CRs as follows:

| Source requirement | Registered CR |
|---|---|
| `POL-001` | `CR-001` |
| `MENU-001`, `VIEW-002` | `CR-002` |
| `MENU-002`, `VIEW-001` | `CR-003` |
| `FLAT-001`–`FLAT-005`, coordinate-transform validation | `CR-004` |
| `FLAT-006`–`FLAT-010`, Undo/Redo and direct-manipulation validation | `CR-005` |
| `SECT-001` | `CR-006` |
| `SECT-002`, file-state persistence validation | `CR-007` |
| `TEST-004` common regression requirement | `CR-002`–`CR-007` |

Never reuse an issued number. Use the required structure in [DEVELOPMENT_POLICY.md](../DEVELOPMENT_POLICY.md). When a CR changes state, update this index and [PROJECT_STATUS.md](../PROJECT_STATUS.md).

- [CR-049](CR-049.md) — Implemented — 드래그 방향별 반원 천공; 다음 번호 CR-050.

- [CR-050](CR-050.md) — Implemented — 반원 지름 Outer Contour 통합.

- [CR-051](CR-051.md) — Implemented — 반원 실제 중심 및 반지름 속성.

- [CR-052](CR-052.md) — Implemented — 사분면 방향 4분원 천공. 다음 번호 CR-053.

- [CR-053](CR-053.md) — Implemented — Section 구간 합계와 Flat W/H 계산 일치. 다음 번호 CR-054.

- [CR-054](CR-054.md) — Implemented (WAITING FOR USER VERIFICATION) — 직선·원호·연결선 절개 도구. 다음 번호 CR-055.

- [CR-055](CR-055.md) — Implemented (WAITING FOR USER VERIFICATION) — 객체 속성 영역 OBJECTS / PROPERTIES 순서.

- [CR-056](CR-056.md) — Section 기본 사각 외곽 동기화; Implemented; 1.20.21. AUTO 신규 1/1, 영향 7/7, 전체 65/65 PASS. MANUAL PENDING_MANUAL; WAITING FOR USER VERIFICATION.

- CR-057 — Hole/Slit 아이콘 굵기 및 크기; Implemented; 1.20.22. 영향 AUTO 3/3, 전체 65/65 PASS; MANUAL PENDING_MANUAL; WAITING FOR USER VERIFICATION.

- CR-058 — Bent Section 실제 두께 축척 정합성; Implemented; 1.20.23. New AUTO 1/1, affected 7/7, full 66/66 PASS. TC-058-002 PENDING_MANUAL; WAITING FOR USER VERIFICATION.

- CR-059 — Section 치수 중앙 및 겹침 배치; Implemented; 1.20.24. New AUTO 1/1, affected 6/6, full 67/67 PASS. TC-059-002 PENDING_MANUAL; WAITING FOR USER VERIFICATION.

- CR-060 — Flat V/V1 절곡선 생성 및 이동; Implemented; 1.20.25. New AUTO 1/1, affected 9/9, full 68/68 PASS; TC-060-002 PENDING_MANUAL; WAITING FOR USER VERIFICATION.

- CR-054 follow-up — Implemented (WAITING FOR USER VERIFICATION) — 직선/원호/연결선 직접 클릭 선택 보완. 신규 CR 번호 미소비.

- [CR-061](CR-061.md) — Implemented — 홈 사이 실제 힌지 기준 3D 날개 회전; 1.20.27. New AUTO 1/1, affected 9/9, full 70/70 PASS; TC-061-002 PENDING_MANUAL.

- [CR-062](CR-062.md) — Implemented: Section의 모든 재료 구간과 빈 공간 표시; 1.20.28; New AUTO 1/1, affected 10/10, full 71/71 PASS. TC-062-002 PENDING_MANUAL / WAITING FOR USER VERIFICATION.

- CR-063 — Implemented: Cut 이동 중앙 안내선; 1.20.29. New AUTO 1/1, affected 9/9, full 72/72 PASS; TC-063-002 PENDING_MANUAL / WAITING FOR USER VERIFICATION.

- [CR-064](CR-064.md) — Implemented (WAITING FOR USER VERIFICATION) — 사용자 HTML 매뉴얼 및 Living Documentation 도움말 시스템.

- [CR-065](CR-065.md) — Implemented / WAITING FOR USER VERIFICATION — 설치 파일명 및 Freeze 버전.

- [CR-066](CR-066.md) — Implemented / WAITING FOR USER VERIFICATION — 설치 화면 CosmicDesigner.
