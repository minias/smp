# SMP Release Automation Context

이 문서는 SMP 프로젝트의 릴리즈 자동화 과정에서 정리된 설정, 문제 해결 내용, 스크립트 구조를 기록한 문서이다.

---

# 1. 개요

SMP 프로젝트는 PowerShell 기반 릴리즈 스크립트를 통해 다음을 자동화한다:

- Semantic Versioning (patch / minor / major)
- CHANGELOG 자동 갱신
- Git commit / tag 생성
- Git push 수행
- 릴리즈 프로세스 표준화

---

# 2. 실행 환경

- PowerShell 5.1 (Windows 기본) 및 PowerShell 7 호환
- Git CLI 사용
- UTF-8 기반 출력 처리 필요

문제:
- PowerShell 5.1에서는 기본 인코딩이 CP949라 한글 깨짐 발생

해결:
- 스크립트 내 UTF-8 강제 설정
- 콘솔 코드페이지 65001 설정

---

# 3. 주요 문제 해결 내역

## 3.1 PowerShell Execution Policy

스크립트 실행 차단 문제 해결:

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
```

또는 사용자 범위:

```powershell
Set-ExecutionPolicy -Scope CurrentUser RemoteSigned
```

---

## 3.2 Git 명령 인자 처리 문제

문제:
- "git add ." 형태 문자열 전달 시 명령 오류 발생

해결:
- 배열 기반 호출로 변경

```powershell
Invoke-Git @("add", ".")
```

---

## 3.3 예약 변수 충돌

문제:
- $Args 사용 시 PowerShell 자동 변수와 충돌

해결:
- 변수명 변경 (GitArgs)

---

## 3.4 PowerShell Analyzer 경고

적용된 규칙:
- PSAvoidAssignmentToAutomaticVariable
- PSUseApprovedVerbs

해결:
- 함수 이름을 Approved Verb 기반으로 변경
  - Parse-Version → Get-VersionInfo
  - Bump-Version → Update-Version

---

## 3.5 CHANGELOG 중복 삽입 문제

해결:
- 버전 존재 여부 검사 추가

```powershell
if ($content -match "\[$newVersion\]") {
    throw "CHANGELOG already contains version"
}
```

---

## 3.6 Git 실패 처리

해결:
- Invoke-Git wrapper 함수에서 실패 시 throw 처리

---

## 3.7 한글 깨짐 문제

원인:
- CP949 vs UTF-8 인코딩 충돌

해결:

```powershell
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8
cmd /c chcp 65001 | Out-Null
```

---

# 4. 릴리즈 스크립트 구조

## 입력 파라미터

- Type: patch | minor | major
- DryRun: 실제 변경 없이 테스트
- NoPush: git push 생략
- SkipChangelog: CHANGELOG 업데이트 생략

---

## 동작 흐름

1. Git 저장소 확인
2. 최신 태그 조회
3. 버전 파싱
4. 버전 증가 (semantic versioning)
5. CHANGELOG 업데이트
6. git add
7. git commit
8. git tag 생성
9. git push (옵션)

---

# 5. 버전 정책

- patch: 버그 수정
- minor: 기능 추가
- major: breaking change

예시:

- 1.0.0 → patch → 1.0.1
- 1.0.1 → minor → 1.1.0
- 1.1.0 → major → 2.0.0

---

# 6. CHANGELOG 구조

```markdown
# Changelog

## [1.0.2] - YYYY-MM-DD

### Bug Fixes
-

### Improvements
-
```

릴리즈 시 자동으로 상단에 삽입됨

---

# 7. 운영 가이드

## 릴리즈 실행

```powershell
.\release.ps1 -Type patch
```

## 테스트 실행

```powershell
.\release.ps1 -Type patch -DryRun
```

## push 제외

```powershell
.\release.ps1 -Type patch -NoPush
```

## CHANGELOG 제외

```powershell
.\release.ps1 -Type patch -SkipChangelog
```

---

# 8. 향후 확장 가능 항목

- GitHub Actions 자동 릴리즈
- CHANGELOG 자동 생성 (commit 기반)
- 빌드 자동화 (EXE 패키징)
- 배포 자동화
- 릴리즈 노트 자동 생성

---

# 9. 참고 사항

- PowerShell 5.1 환경에서는 UTF-8 설정 필수
- Git CLI는 배열 기반 인자 전달 권장
- 스크립트는 CI/CD 환경에서도 동일하게 동작하도록 설계됨

---

이 문서는 릴리즈 자동화 구조 변경 시 지속적으로 업데이트한다.

