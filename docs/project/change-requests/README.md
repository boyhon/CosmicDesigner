# Change Request Index

Change Requests after the `2026-10-01` baseline are stored in this directory.

Users submit requirements in natural language; they are not expected to assign CR numbers or follow the template. Codex checks this index and existing CR files, updates overlaps or issues the next unused sequential number, preserves the source, and updates `PROJECT_STATUS.md` according to the [Change Request Intake policy](../DEVELOPMENT_POLICY.md#4-change-request-intake).

## Next available number

`CR-024`

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
| [CR-022](CR-022.md) | Edit Settings 사용자 환경 설정창 | Implemented | 1.12.0 | Pending |
| [CR-023](CR-023.md) | 문서 단위와 두 가지 단위 변경 방식 | Implemented | 1.12.0 | Pending |

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
