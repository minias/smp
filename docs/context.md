# SMP Project Context

## 📌 Overview
- Name: SMP (Simple Music Player)
- Platform: Windows (WinForms)
- Runtime: .NET 10
- Architecture: Clean Architecture

---

## 🧱 Architecture

```sh
Domain
 └── Entities

Application
 ├── UseCases
 ├── Ports (Interfaces)
 └── DTO

Infrastructure
 ├── Youtube (yt-dlp)
 ├── Audio
 ├── Storage
 ├── Update
 ├── Tray
 └── Config

UI
 └── WinForms (Presenter 역할만)
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

---

## 요구사항

- DotNet8.0 명세 기준 네이밍
- 코드의 첫줄은 항상 주석으로 경로와 파일명을 기재
- 모든 수정은 전체파일 기준
- 기능이나 함수가 추가되면 반드시 주석코드가 있어야 함
- 수정이 완료 되면 Context.MD 를 최신화 해서 보여줄 것

## ⚙️ Core Features

- 유튜브 URL 기반 음악 재생
- yt-dlp 사용
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

- 트레이 아이콘 Dispose 문제 (해결됨)
- UI Resize 깨짐 (고정 사이즈 적용)
- 긴 대화 시 ChatGPT 성능 저하

---

## 🚧 Current Work

- UpdateService 안정화
- TrayService lifecycle 개선
- GitHub Release 자동 배포

---

## 🎯 Next Goals

- 자동 업데이트 무중단 적용
- 다운로드 진행 UI 추가
- 설정 파일 관리 분리

---

## 🧠 Notes

- 모든 서비스는 DI 기반
- UI는 최소 로직만 유지
- Infrastructure는 외부 의존성 담당

---

# 🔥 변경 요약

## 추가된 구조
```sh
Infrastructure/Serialization