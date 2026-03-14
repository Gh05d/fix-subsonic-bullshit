Fix Subsonic Bullshit v1.0.0
============================

A mod for Pathfinder: Wrath of the Righteous that fixes the broken DC calculation
on the Carnivorous Crystal's Subsonic Hum ability.


THE PROBLEM
-----------

Carnivorous Crystals use Subsonic Hum, an aura that forces a Fortitude save every
round or be stunned. The tooltip shows DC 22 (the correct tabletop value), but the
actual save DC is massively inflated:

  - Normal:  DC ~32  (tooltip says 22)
  - Core:    DC ~38-40  (tooltip says 22)
  - Unfair:  DC ~52  (tooltip says 22)

This happens because the game inflates creature stats (Constitution, Hit Dice) for
difficulty, and the DC formula (10 + HD/2 + Con modifier) operates on these inflated
values. Multiple crystals can permanently stunlock your entire party.


THE FIX
-------

This mod recalculates the Subsonic Hum DC using the correct Pathfinder tabletop
formula (10 + HD/2 + Con modifier) but caps Constitution at 26 to prevent difficulty
stat inflation from producing absurd DCs.

  - Con cap of 26 = base 18 + Advanced template (+4) + Giant template (+4)
  - The DC still scales dynamically with each creature's actual stats
  - Only Subsonic Hum is affected — all other abilities remain unchanged
  - Works in both Real-Time with Pause and Turn-Based mode


INSTALLATION
------------

1. Install Unity Mod Manager (0.23.0+)
   https://www.nexusmods.com/site/mods/21

2. Download the mod zip from the Files tab

3. Extract into your game's Mods directory:
   {GameDir}/Mods/FixSubsonicBullshit/

4. Enable the mod in Unity Mod Manager


REQUIREMENTS
------------

  - Unity Mod Manager 0.23.0+
  - Pathfinder: Wrath of the Righteous 1.4+


COMPATIBILITY
-------------

  - Compatible with all other mods (no known conflicts)
  - Works alongside Buff It 2 The Limit, TabletopTweaks, ToyBox, etc.


UNINSTALLATION
--------------

Delete the FixSubsonicBullshit folder from your Mods directory.


SOURCE CODE & BUG REPORTS
-------------------------

GitHub: https://github.com/Gh05d/fix-subsonic-bullshit

Report issues on the GitHub Issues page or in the Nexus Mods comments.


LICENSE
-------

MIT License. See LICENSE file for details.
