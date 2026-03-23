#!/usr/bin/env bash

# =========================
# Git Release Automation
# =========================

set -e

# 현재 브랜치 확인
BRANCH=$(git rev-parse --abbrev-ref HEAD)

echo "현재 브랜치: $BRANCH"

# 버전 입력
read -p "릴리즈 버전 입력 (예: 1.0.2): " VERSION

if [ -z "$VERSION" ]; then
  echo "버전이 입력되지 않았습니다."
  exit 1
fi

TAG="v$VERSION"

echo "릴리즈 버전: $TAG"

# 변경사항 커밋
echo "변경사항 커밋 중..."
git add .
git commit -m "chore(release): $TAG"

# 태그 생성
echo "태그 생성 중..."
git tag -a "$TAG" -m "Release $TAG"

# 브랜치 푸시
echo "브랜치 푸시..."
git push origin "$BRANCH"

# 태그 푸시
echo "태그 푸시..."
git push origin "$TAG"

echo "릴리즈 완료: $TAG"