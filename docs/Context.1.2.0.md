# SMP (Simple Media Player) - Context

## 1. 프로젝트 목적

Windows 환경에서 동작하는 미디어 플레이어로,
YouTube 기반 재생 + 로컬 플레이리스트 관리 + Media Key 제어를 통합한 구조를 목표로 한다.

---

## 2. 아키텍처 개요

클린 아키텍처 기반으로 구성:

### Layer 구조

```
UI (WinForms)
├── UseCases (Application Layer)
│ ├── PlayUseCase
│ ├── NextTrackUseCase
│ ├── PrevTrackUseCase
│ ├── StopUseCase
│ ├── PlayPauseUseCase
│ └── SetVolumeUseCase 등
│
├── Domain
│ ├── PlayerState
│ ├── PlaylistItem
│ └── LoopMode
│
├── Infrastructure
│ ├── MediaKeyListener
│ ├── MediaKeyPatternDetector
│ └── External Services (YouTube, Tray 등)
```

---

## 3. 핵심 기능

### 3.1 플레이어 기능
- 재생 (Play)
- 일시정지 (Pause)
- 정지 (Stop)
- 다음곡 / 이전곡 이동
- 플레이리스트 기반 재생
- 루프 모드 지원
  - None
  - All
  - Single

---

### 3.2 볼륨 제어
- UI Slider 기반 볼륨 조절
- UseCase를 통한 볼륨 변경
- Media Key Volume Up/Down 이벤트 대응

---

### 3.3 Media Key 제어

#### 지원 키
- Play/Pause
- Next
- Previous
- Volume Up
- Volume Down

#### 처리 구조

```
MediaKeyListener → MediaKeyPatternDetector → UI(MainForm) → UseCase
```

---

## 4. MediaKeyPatternDetector 역할

### 목적
연속 입력을 기반으로 패턴을 감지하여 다음 기능 수행:

| 입력 패턴 | 동작 |
|----------|------|
| Volume Up 1회 | 볼륨 증가 |
| Volume Down 1회 | 볼륨 감소 |
| Volume Up 연속 (≥2) | Next Track |
| Volume Down 연속 (≥2) | Previous Track |

### 동작 방식
- 일정 시간(Threshold: 300ms) 내 입력을 누적
- 타이머 종료 시 패턴 판별
- 이벤트 발생 방식으로 UI에 전달

---

## 5. 이벤트 흐름

### 일반 흐름

```
Keyboard / Media Input
↓
MediaKeyListener (Hook)
↓
MediaKeyPatternDetector
↓
MainForm 이벤트 바인딩
↓
UseCase 실행
↓
Domain 상태 변경 + Player 제어
```

---

## 6. UI(MainForm) 역할

- UseCase 실행 트리거
- MediaKey 이벤트 구독 및 핸들링
- Playlist UI 반영
- Volume UI 반영
- Tray UI 관리

UI는 비즈니스 로직을 포함하지 않으며 UseCase 호출만 담당

---

## 7. 주요 구현 포인트

### 7.1 이벤트 바인딩 구조

- MediaKey 이벤트는 Handler 메서드를 통해 구독
- 중복 구독 방지를 위해 Unsubscribe 후 Subscribe 처리

---

### 7.2 PlayPause UseCase

- 현재 상태 기반 토글 처리
- Playing → Pause
- Paused → Play

---

### 7.3 Playlist 관리

- Repository 기반 로드/저장
- UI ListBox와 동기화
- UseCase를 통해 Domain 상태 유지

---

## 8. 확인된 동작 특성 및 제약

### 8.1 AirPods 입력 특성

- Volume 제스처 → Windows 마스터 볼륨 직접 제어 (OS 레벨)
- Play/Pause → 정상 이벤트 전달
- Double Tap → Keyboard Hook으로 전달되지 않을 수 있음

👉 결과:
- MediaKeyPatternDetector는 일부 입력을 감지하지 못할 수 있음

---

## 9. 주요 이슈 및 원인

### Issue 1: PatternDetector가 더블탭을 감지하지 못함

원인:
- AirPods 더블탭은 Keyboard Hook 이벤트로 전달되지 않음
- OS 내부 Media Session에서 처리됨

결과:
- PatternDetector는 Keyboard 입력 기반이므로 감지 불가

---

### Issue 2: PlayPause UseCase DI 오류

원인:
- DI 컨테이너에 PlayPauseUseCase 등록 누락

결과:
- 런타임에서 Service Resolution 실패 발생

---

### Issue 3: ExecuteAsync 오버로드 오류

원인:
- UseCase 메서드 시그니처 불일치

결과:
- 인자 전달 mismatch로 컴파일 에러 발생

---

## 10. 설계 결정 사항

### 10.1 PatternDetector 유지 이유

- 키 입력 기반 패턴 확장 가능성 확보
- 일반 키보드 입력 처리 가능
- 추후 커스터마이징 확장 대비

---

### 10.2 Media Input 처리 전략

- Keyboard Hook 기반 입력만 PatternDetector에서 처리
- AirPods/OS Media Control은 별도 흐름으로 취급

---

### 10.3 UseCase 중심 구조 유지

- UI는 UseCase만 호출
- 비즈니스 로직은 Application Layer에 집중
- Domain 상태는 PlayerState에서 관리

---

## 11. 현재 상태 요약

- MediaKey 기반 재생 제어 구현 완료
- Playlist 관리 완료
- Volume 제어 완료
- PatternDetector 구조 적용 완료
- AirPods 더블탭은 OS 레벨 처리로 인해 Detector에서 감지되지 않는 구조적 한계 확인

---

## 12. 향후 확장 가능 영역

- Windows Media Session 연동
- 글로벌 Media Control API 통합
- Input Source abstraction 추가
- Hotkey / Global Shortcut 시스템 확장
- Plugin 형태의 Input Handler 구조 도입

---

# End of Context