# Changelog

## [1.0.3] - 2026-03-23

### Bug Fixes
- 

### Improvements
- 


## [1.0.2] - 2026-03-23

### Bug Fixes
- 네임스페이스 구조 오류 및 충돌 문제 수정 (`SMP.Application` → `SMP.App` 구조 정리)
- `IUpdateService`, `UpdateInfo` namespace 누락 및 경로 불일치 문제 수정
- DI 주입 구조 안정화 및 optional 서비스 처리 개선
- namespace ↔ 폴더 구조 불일치로 인한 IDE0130 경고 수정
- 트레이 → 메인 화면 복원 시 상태 처리 안정성 개선

### Improvements
- Clean Architecture 기준으로 프로젝트 구조 재정리 (App / Domain / Infrastructure / UI)
- 리사이즈 제한 유지 및 UI 고정 동작 안정화
- 트레이 기반 실행 흐름 안정성 개선
- 업데이트 처리 흐름 예외 안전성 강화
- 코드 품질 개선 및 경고 대응 (static 검토, 구조 정리)

---

## [1.0.1] - 2026-03-23

### Bug Fixes
- 트레이 "열기" 클릭 시 창이 표시되지 않던 문제 수정
- 트레이 아이콘 더블 클릭 시 GUI 창이 열리도록 개선

### Improvements
- 전체 창(Maximize) 버튼 제거
- 트레이 UX 안정성 개선

---

## [1.0.0] - 2026-03-22

### Initial Release
- 기본 음원 플레이어 기능
- 플레이리스트 관리
- 트레이 기능
- 유튜브 연동
