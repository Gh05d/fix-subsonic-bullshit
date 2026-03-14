using System;
using HarmonyLib;
using Kingmaker.UnitLogic.Mechanics.Actions;
using UnityModManagerNet;

namespace FixSubsonicBullshit {
    static class Main {
        static Harmony harmony;
        static UnityModManager.ModEntry.ModLogger logger;

        // Cap Constitution at 26 (base 18 + Advanced template +4 + Giant template +4)
        // This prevents difficulty stat inflation from blowing up the DC
        public const int ConstitutionCap = 26;

        // CarnivorousCrystal_AreaEffect_SubsonicHum
        public const string SubsonicHumAreaEffectGuid = "a89a5b1edba9c614b92a7ba7ab3f5a1d";

        static bool Load(UnityModManager.ModEntry modEntry) {
            logger = modEntry.Logger;
            modEntry.OnUnload = OnUnload;
            harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll();
            Log("Fix Subsonic Bullshit loaded.");
            return true;
        }

        static bool OnUnload(UnityModManager.ModEntry modEntry) {
            harmony.UnpatchAll(modEntry.Info.Id);
            return true;
        }

        public static void Log(string msg) => logger.Log(msg);

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Verbose(string msg) => logger.Log(msg);
    }

    [HarmonyPatch(typeof(ContextActionSavingThrow), nameof(ContextActionSavingThrow.RunAction))]
    static class SubsonicHumDCFix {
        static void Prefix(ContextActionSavingThrow __instance) {
            var context = __instance.Context;
            if (context == null) return;

            // Match Subsonic Hum by blueprint GUID (check context and parent context)
            string guid = context.AssociatedBlueprint?.AssetGuid.ToString();
            string parentGuid = context.ParentContext?.AssociatedBlueprint?.AssetGuid.ToString();

            if (guid != Main.SubsonicHumAreaEffectGuid && parentGuid != Main.SubsonicHumAreaEffectGuid) {
                // Fallback: match by name
                string name = context.AssociatedBlueprint?.name
                           ?? context.ParentContext?.AssociatedBlueprint?.name
                           ?? "";
                if (!name.Contains("SubsonicHum")) return;
            }

            var caster = context.MaybeCaster;
            if (caster == null) return;

            int originalDC = context.Params.DC;

            // Tabletop formula: DC = 10 + HD/2 + Con modifier
            // Cap Constitution to prevent difficulty stat inflation
            int con = caster.Stats.Constitution.ModifiedValue;
            int cappedCon = Math.Min(con, Main.ConstitutionCap);
            int conMod = (cappedCon - 10) / 2;
            int hd = caster.Progression.CharacterLevel;
            int fixedDC = 10 + (hd / 2) + conMod;

            context.Params.DC = fixedDC;

            Main.Log($"[FSB] Fixed Subsonic Hum DC: {originalDC} -> {fixedDC} " +
                     $"(Con {con} capped to {cappedCon}, HD {hd}, " +
                     $"caster: {caster.CharacterName})");
        }
    }
}
