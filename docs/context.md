# SMP Project Context

## 📌 Overview
- Name: SMP (Simple Music Player)
- Platform: Windows (WinForms)
- Runtime: .NET 10
- Architecture: Clean Architecture
- Github : https://github.com/minias/smp
- @tag V1.1.0
---

## 🧱 Architecture

```sh
├─App
│  ├─Interfaces
│  ├─Ports
│  └─Service
├─Domain
│  └─Entities
├─Infrastructure
│  ├─Audio
│  ├─Serialization
│  ├─Storage
│  ├─Tray
│  ├─Update
│  └─Youtube
└─UI
```

### Domain
- PlaylistItem
- Core business rules

### Applications
- PlayerService
- UseCase 중심 로직

### Infrastructure
- Audio (재생)
- Youtube (yt-dlp 연동)
- Storage (파일 저장)
- Tray (시스템 트레이)
- Update (GitHub 릴리즈 기반 업데이트)

### UI
- MainForm
- WinForms 기반

### Github 브랜치 구조

```sh
main        → 운영(배포)
dev         → 통합 개발 브랜치
feature/*   → 기능 개발
release/*   → 배포 준비
hotfix/*    → 긴급 수정
```

### Github PR 규칙

dev로만 merge
squash merge 권장

### 커밋 메시지

```sh
feature: 재생목록 순서 조정
fix: 재생목록 널포인트 이슈
hotfix: 1.0.3 실행오류 수정
```

### 기능개발 절차

```sh
dev → feature/xxx → dev (PR & Merge)
```

### 배포 절차

```sh
# 버그 패치
dev → release/v1.1.1 → main
                     → dev (동기화)
# 가능 추가
dev → release/v1.2.0 → main
                     → dev (동기화)
```

### 긴급 수정 (운영 장애)

```sh
main → hotfix/xxx → main
                  → dev (반영 필수)
```
## 📦 빌드 및 릴리즈 자동화

```sh
# 1. 버전 릴리즈(버그) 1.0.0 -> 1.0.1
.\release.ps1 -Type patch
# 1.1 버전 릴리즈(마이너) 1.0.3 -> 1.1.0
.\release.ps1 -Type minor
# 1.2 버전 릴리즈(메이저) 1.0.0 -> 2.0.0
.\release.ps1 -Type major

# 2. 빌드/퍼블리시
.\publish.ps1

# 3. smp.iss 실행 및 빌드
.\smp.iss 
```

## 📦 배포 (Installer)

InnoSetup 6사용 <https://jrsoftware.org/isinfo.php>
- smp.iss 파일을 실행하면 이노셋업으로 진행됩니다.
- 설치 파일은 Output/ 폴더에 생성됩니다.

```
Output/SMP_Setup.exe
```

---

## 요구사항

- DotNet8.0 명세 기준 네이밍
- 코드의 첫줄은 항상 주석으로 경로와 파일명을 기재
- 기존 코드 절대 삭제하지 않음 (명시 없는 한)
- 모든 수정은 전체파일 기준
- 기존 코드 수정은 Diff 기반 변경
- 추가된 코드만 명확히 표시
- 추측 기반 수정 금지
- 기능이나 함수가 추가되면 반드시 주석코드가 있어야 함
- 수정이 완료 되면 Context.MD 를 최신화 해서 보여줄 것

## ⚙️ Core Features

- 유튜브 URL 기반 음악 재생
- yt-dlp 사용
- 플레이리스트 캐시
- 플레이리스트 저장/로드
- 트레이 앱 동작
- 볼륨 제어
- 자동 업데이트

---

## 📦 External Dependencies

- yt-dlp.exe (런타임 다운로드)
- GitHub Release API (업데이트 체크)

---

## 🐞 Known Issues

---

## 🚧 Current Work

- 스피커, 이어폰의 << | >> 버튼 클릭 시 다음 재생 목록으로 가능 기능 추가
- 실행파일의 리소스 최적화 및 메모리 누수 방지 코드 추가
- 리스트박스의 아이템 (플레이리스트목록) 순서를 마우스 드래그로 수정가능, 또는 shift+위,아래키로 이동
- DI 구조 개선
  - IAudioPlayer 인터페이스 기반 주입
  - IWavePlayer DI 등록 추가
  - PlayerService → IAudioPlayer 의존으로 변경
  - StreamingAudioPlayer DI 매핑 적용
  - YtDlpService 중복 등록 제거
  - PlayerService 중복 등록 제거
- Infrastructure 의존성 분리
  - PlayerService → Interface 의존
- IAudioPlayer 인터페이스 확장
  - Pause / Volume / Dispose / Event 정의 추가
  - PlayerService 요구사항 기준으로 Port 재정의
- Port 설계 개선
  - Application 중심 인터페이스 설계 적용
- PlaybackState 충돌 해결
  - Domain alias 적용
- StreamingAudioPlayer 구조 개선
  - IDisposable 중복 상속 제거
  - IAudioPlayer 중심 의존 구조 유지
- Port 기반 설계 정합성 강화 
- Clean Architecture 정합성 확보 
- TrayService → ITrayService 구현 추가
- DI resolve 오류 해결
- UI → Infrastructure 의존 제거 완료

---

## 🎯 Next Goals

---

## v1.2.0 변경사항

### 변경
- App/Ports → App/Interfaces로 통합
- IAudioPlayer 이동 및 namespace 변경
- PlayerService 완전 제거
- UseCase 기반 구조 전환 완료
- PlayerState 중심 상태 관리 적용
- PlayerService 제거

### 검증
- Debug 빌드 실행 테스트 통과

### 다음 단계
- 이벤트 메모리 관리 강화
- CancellationToken 적용
- AudioPlayer 리소스 검증

---

## 🧠 Notes

- 모든 서비스는 DI 기반
- UI는 최소 로직만 유지
- Infrastructure는 외부 의존성 담당

