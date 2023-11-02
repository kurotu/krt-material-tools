#!/bin/bash
set -eu

VERSION=$1
PACKAGE_DIR=Packages/com.github.kurotu.krt-material-tools
PACKAGE_JSON="$PACKAGE_DIR/package.json"

sed -i -b -e "s/\"version\": \".*\"/\"version\": \"${VERSION}\"/g" "$PACKAGE_JSON"
sed -i -b -e "s/string Version = \".*\"/string Version = \"${VERSION}\"/g" "$PACKAGE_DIR/Editor/Common/UpdateChecker.cs"

git add "$PACKAGE_JSON"
git add "$PACKAGE_DIR/Editor/Common/UpdateChecker.cs"
git commit -m "Version $VERSION"
