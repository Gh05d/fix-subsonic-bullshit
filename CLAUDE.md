# CLAUDE.md

## Project Overview

Fix Subsonic Bullshit is a UMM mod for Pathfinder: Wrath of the Righteous that fixes the inflated DC calculation on Carnivorous Crystal Subsonic Hum ability. Distributed via [Nexus Mods](https://www.nexusmods.com/pathfinderwrathoftherighteous/mods/949) and [GitHub](https://github.com/Gh05d/fix-subsonic-bullshit).

## Build

```bash
~/.dotnet/dotnet build FixSubsonicBullshit/FixSubsonicBullshit.csproj -p:SolutionDir=$(pwd)/
```

**First-time setup:** `GameInstall/` (symlink to `../wrath-epic-buffing/GameInstall` works) + `GamePath.props` with `<WrathInstallDir>$(SolutionDir)GameInstall</WrathInstallDir>` — see parent `wrath-mods/CLAUDE.md` §Common Build Setup.

## Deploy

```bash
./deploy.sh
```

Builds and deploys DLL + Info.json to Steam Deck via SCP. Requires `deck-direct` SSH alias.

## Gotchas

- `GameInstall/` and `GamePath.props` are machine-specific — gitignored, never commit
- `Assembly-CSharp.dll` and `Owlcat*.dll` are publicized (private field access). If you get CS0122 on other DLLs, add `Publicize="true"` to the csproj reference.

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

- Shared style (K&R, 4-space, `var`): parent `wrath-mods/CLAUDE.md` §Code Style
