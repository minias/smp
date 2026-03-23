# SMP Project Context

## 📌 Overview
- Name: SMP (Simple Music Player)
- Platform: Windows (WinForms)
- Runtime: .NET 10
- Architecture: Clean Architecture
- Github : https://github.com/minias/smp
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
 ├── Serialization
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

- 플레이리스트 저장 후 실행시 비어있는값으로 변경됨
- 플레이리스트 구조 변경

---

## 🚧 Current Work


---

## 🎯 Next Goals

---

## 🧠 Notes

- 모든 서비스는 DI 기반
- UI는 최소 로직만 유지
- Infrastructure는 외부 의존성 담당

