# CLAUDE.md

## Project Overview

Fix Subsonic Bullshit is a UMM mod for Pathfinder: Wrath of the Righteous that fixes the inflated DC calculation on Carnivorous Crystal Subsonic Hum ability.

## Build

```bash
~/.dotnet/dotnet build FixSubsonicBullshit/FixSubsonicBullshit.csproj -p:SolutionDir=$(pwd)/
```

- `dotnet` is not on PATH — always use `~/.dotnet/dotnet`
- `-p:SolutionDir` is required on Linux — without it, GamePath.props import fails silently

## Deploy

```bash
./deploy.sh
```

Builds and deploys DLL + Info.json to Steam Deck via SCP. Requires `deck-direct` SSH alias.

## Gotchas

- `GameInstall/` is a symlink to `~/Code/wrath-epic-buffing/GameInstall` — do not commit it
- `GamePath.props` is machine-specific — excluded by .gitignore, each developer creates their own
- `Assembly-CSharp.dll` and `Owlcat*.dll` are publicized (private field access). If you get CS0122 on other DLLs, add `Publicize="true"` to the csproj reference.
- Pre-existing `findstr` warnings from build are normal on Linux — ignore them.
