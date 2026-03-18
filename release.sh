#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")"

# 1. Read version from csproj
VERSION=$(grep -oP '<Version>\K[^<]+' FixSubsonicBullshit/FixSubsonicBullshit.csproj)
TAG="v${VERSION}"
ZIP="FixSubsonicBullshit/bin/FixSubsonicBullshit-${VERSION}.zip"

echo "Preparing release: Fix Subsonic Bullshit ${TAG}"

# 2. Check if tag already exists
if git rev-parse "$TAG" >/dev/null 2>&1; then
    echo "ERROR: Tag ${TAG} already exists. Bump the version first."
    exit 1
fi

# 3. Check for uncommitted changes
if ! git diff --quiet || ! git diff --cached --quiet; then
    echo "ERROR: Working tree is dirty. Commit or stash changes first."
    exit 1
fi

# 4. Push current branch
echo "Pushing to origin..."
git push origin master

# 5. Build Release configuration
echo "Building Release..."
~/.dotnet/dotnet build FixSubsonicBullshit/FixSubsonicBullshit.csproj \
    -c Release \
    -p:SolutionDir="$(pwd)/" \
    --nologo

# 6. Verify zip exists
if [ ! -f "$ZIP" ]; then
    echo "ERROR: Expected zip not found at ${ZIP}"
    exit 1
fi

echo "Release artifact: ${ZIP} ($(du -h "$ZIP" | cut -f1))"

# 7. Create and push tag
git tag -a "$TAG" -m "Release ${TAG}"
git push origin "$TAG"

# 8. Create GitHub release
echo "Creating GitHub release..."
gh release create "$TAG" "$ZIP" \
    --repo Gh05d/fix-subsonic-bullshit \
    --title "Fix Subsonic Bullshit ${TAG}" \
    --notes "$(cat <<NOTES
## Fix Subsonic Bullshit ${TAG}

Fixes the inflated DC on Carnivorous Crystal Subsonic Hum ability.

### Installation
1. Download \`FixSubsonicBullshit-${VERSION}.zip\`
2. Extract into \`{GameDir}/Mods/FixSubsonicBullshit/\`
3. Enable in Unity Mod Manager

### Requirements
- [Unity Mod Manager](https://www.nexusmods.com/site/mods/21) 0.23.0+
- Pathfinder: Wrath of the Righteous 1.4+
NOTES
)"

# 9. Update Repository.json with DownloadUrl for UMM auto-download
DOWNLOAD_URL="https://github.com/Gh05d/fix-subsonic-bullshit/releases/download/${TAG}/FixSubsonicBullshit-${VERSION}.zip"
cat > Repository.json <<REPO
{
    "Releases": [
        {
            "Id": "FixSubsonicBullshit",
            "Version": "${VERSION}",
            "DownloadUrl": "${DOWNLOAD_URL}"
        }
    ]
}
REPO

git add Repository.json
git commit -m "chore: update Repository.json for ${TAG}"
git push origin master

echo ""
echo "Release ${TAG} published!"
echo "  GitHub: https://github.com/Gh05d/fix-subsonic-bullshit/releases/tag/${TAG}"
echo "  Repository.json updated with DownloadUrl"
echo ""
echo "Nexus Mods upload will be triggered automatically via GitHub Actions."
