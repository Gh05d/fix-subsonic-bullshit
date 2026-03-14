# Fix Subsonic Bullshit

A mod for **Pathfinder: Wrath of the Righteous** that fixes the broken DC calculation on the Carnivorous Crystal's Subsonic Hum ability.

## The Problem

Carnivorous Crystals use Subsonic Hum, an aura that forces a Fortitude save every round or be stunned. The tooltip shows DC 22 (the correct tabletop value), but the actual DC is massively inflated:

| Difficulty | Tooltip DC | Actual DC |
|---|---|---|
| Normal | 22 | ~32 |
| Core | 22 | ~38-40 |
| Unfair | 22 | ~52 |

This happens because the game inflates creature stats (Constitution, Hit Dice) for difficulty, and the DC formula (`10 + HD/2 + Con modifier`) operates on these inflated values. The result: multiple crystals permanently stunlock your entire party with an essentially unbeatable save.

**References:**
- [Steam: DC 32 RtwP / DC 40 Turn-Based](https://steamcommunity.com/app/1184370/discussions/0/4932019356822263381/)
- [Steam: Immunity not working as intended](https://steamcommunity.com/app/1184370/discussions/0/5086242673972209412/)

## The Fix

This mod recalculates the Subsonic Hum DC using the correct tabletop formula (`10 + HD/2 + Con modifier`) but **caps Constitution at 26** to prevent difficulty stat inflation from producing absurd DCs.

- Con cap of 26 = base 18 + Advanced template (+4) + Giant template (+4)
- The DC still scales with creature variants (Standard, Advanced, Giant, Enhanced)
- Only Subsonic Hum is affected — other abilities remain unchanged

## Installation

1. Install [Unity Mod Manager](https://www.nexusmods.com/site/mods/21) (0.23.0+)
2. Download the latest release zip
3. Extract into `{GameDir}/Mods/FixSubsonicBullshit/`
4. Enable in Unity Mod Manager

## Compatibility

- Pathfinder: Wrath of the Righteous 1.4+
- Compatible with all other mods (no conflicts)
- Works in both Real-Time with Pause and Turn-Based mode

## Building from Source

Requires .NET SDK and the game's managed DLLs.

1. Clone this repo
2. Create a `GameInstall/` directory (or symlink) containing the game's `Wrath_Data/Managed/` folder
3. Create `GamePath.props`:
   ```xml
   <Project xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
       <PropertyGroup>
           <WrathInstallDir>$(SolutionDir)GameInstall</WrathInstallDir>
       </PropertyGroup>
   </Project>
   ```
4. Build: `dotnet build FixSubsonicBullshit/FixSubsonicBullshit.csproj -p:SolutionDir="$(pwd)/"`

## License

MIT
