# CLAUDE.md

## Project Overview

Fix Subsonic Bullshit is a UMM mod for Pathfinder: Wrath of the Righteous that fixes the inflated DC calculation on Carnivorous Crystal Subsonic Hum ability. Distributed via [Nexus Mods](https://www.nexusmods.com/pathfinderwrathoftherighteous/mods/949) and [GitHub](https://github.com/Gh05d/fix-subsonic-bullshit).

Shared build/deploy/Nexus/release rules: → parent `wrath-mods/CLAUDE.md` (§Common Build Setup, §Steam Deck Deployment, §Nexus Mods, §Release Process).

## Build

```bash
~/.dotnet/dotnet build FixSubsonicBullshit/FixSubsonicBullshit.csproj -p:SolutionDir=$(pwd)/
```

First-time setup (`GameInstall/` symlink + `GamePath.props` with `<WrathInstallDir>$(SolutionDir)GameInstall</WrathInstallDir>`, publicized `Assembly-CSharp*.dll`/`Owlcat*.dll`): → parent §Common Build Setup.

## Deploy

```bash
./deploy.sh
```

Ships DLL + Info.json only. Rules: → parent §Steam Deck Deployment.

## Release & Distribution

```bash
./release.sh
```

Reads version from csproj, builds Release config, tags, creates GitHub release via `gh`, updates `Repository.json`. Requires `gh` CLI authenticated. (This repo has no `/release` command — `release.sh` replaces it.)

- **Version bump workflow**: Bump `<Version>` in `FixSubsonicBullshit/FixSubsonicBullshit.csproj` + `Version` in `FixSubsonicBullshit/Info.json` → commit → run `release.sh` → Nexus upload runs automatically via GitHub Actions (→ parent §Nexus Mods).
- `release.sh` aborts on dirty tree / existing tag, pushes `master` BEFORE the Release build, and commits `Repository.json` itself — don't pre-edit Repository.json.
- Nexus mod-page description BBCode in `docs/nexus-description.bbcode`, readme in `docs/nexus-readme.txt`.

## Architecture

Single-file mod (`Main.cs`). Patches `ContextActionSavingThrow.RunAction()` with a Harmony Prefix that:
1. Identifies Subsonic Hum by blueprint GUID (`a89a5b1edba9c614b92a7ba7ab3f5a1d`)
2. Reads the caster's actual Constitution and HD
3. Caps Constitution at 26 (base 18 + two templates)
4. Recalculates DC using tabletop formula: `10 + HD/2 + ConMod(capped)`
5. Sets `Context.Params.DC` before the `RuleSavingThrow` is constructed

## Code Style

- Shared style (K&R, 4-space, `var`): → parent §Code Style
