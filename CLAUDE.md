# CLAUDE.md

## Project Overview

Fix Subsonic Bullshit is a UMM mod for Pathfinder: Wrath of the Righteous that fixes the inflated DC calculation on Carnivorous Crystal Subsonic Hum ability. Distributed via [Nexus Mods](https://www.nexusmods.com/pathfinderwrathoftherighteous/mods/949) and [GitHub](https://github.com/Gh05d/fix-subsonic-bullshit).

## Build

```bash
~/.dotnet/dotnet build FixSubsonicBullshit/FixSubsonicBullshit.csproj -p:SolutionDir=$(pwd)/
```

- `dotnet` is not on PATH — always use `~/.dotnet/dotnet`
- `-p:SolutionDir` is required on Linux — without it, GamePath.props import fails silently

**First-time setup:** Create `GameInstall/` with the game's `Wrath_Data/Managed/` DLLs, then create `GamePath.props`:
```xml
<Project xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
  <PropertyGroup>
    <WrathInstallDir>$(SolutionDir)GameInstall</WrathInstallDir>
  </PropertyGroup>
</Project>
```

## Deploy

```bash
./deploy.sh
```

Builds and deploys DLL + Info.json to Steam Deck via SCP. Requires `deck-direct` SSH alias.

## Gotchas

- `GameInstall/` is a symlink to the game's managed DLLs — do not commit it. For dev setup, symlink to a local copy or to `../wrath-epic-buffing/GameInstall`
- `GamePath.props` is machine-specific — excluded by .gitignore, each developer creates their own
- `Assembly-CSharp.dll` and `Owlcat*.dll` are publicized (private field access). If you get CS0122 on other DLLs, add `Publicize="true"` to the csproj reference.
- Pre-existing `findstr` warnings from build are normal on Linux — ignore them.

## Release & Distribution

```bash
./release.sh
```

Reads version from csproj, builds Release config, tags, creates GitHub release via `gh`, updates `Repository.json`. Requires `gh` CLI authenticated.

- **Version bump workflow**: Bump `<Version>` in csproj + `Version` in `Info.json` → commit → run `release.sh` → upload zip to Nexus.
- **Nexus upload**: Automatic via GitHub Actions on release publish (`.github/workflows/nexus-upload.yml`). Description BBCode in `docs/nexus-description.bbcode`, readme in `docs/nexus-readme.txt`.

## Architecture

Single-file mod (`Main.cs`). Patches `ContextActionSavingThrow.RunAction()` with a Harmony Prefix that:
1. Identifies Subsonic Hum by blueprint GUID (`a89a5b1edba9c614b92a7ba7ab3f5a1d`)
2. Reads the caster's actual Constitution and HD
3. Caps Constitution at 26 (base 18 + two templates)
4. Recalculates DC using tabletop formula: `10 + HD/2 + ConMod(capped)`
5. Sets `Context.Params.DC` before the `RuleSavingThrow` is constructed

## Code Style

- K&R brace style (opening brace on same line)
- 4-space indentation
- `var` when type is apparent
