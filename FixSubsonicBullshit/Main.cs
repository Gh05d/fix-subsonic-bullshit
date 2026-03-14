using HarmonyLib;
using Kingmaker.RuleSystem.Rules;
using UnityModManagerNet;

namespace FixSubsonicBullshit {
    static class Main {
        static Harmony harmony;
        static UnityModManager.ModEntry.ModLogger logger;

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

    [HarmonyPatch(typeof(RuleSavingThrow), nameof(RuleSavingThrow.OnTrigger))]
    static class SubsonicHumDiagnosticPatch {
        static void Postfix(RuleSavingThrow __instance) {
            var reason = __instance.Reason;
            if (reason?.Ability == null) return;

            var blueprintName = reason.Ability.Blueprint?.name ?? "unknown";

            // Log all saving throws in DEBUG builds
            Main.Verbose($"[FSB] SavingThrow: ability={blueprintName}, " +
                         $"DC={__instance.DifficultyClass}, " +
                         $"type={__instance.Type}, " +
                         $"passed={__instance.IsPassed}, " +
                         $"roller={__instance.Initiator?.CharacterName ?? "?"}, " +
                         $"caster={reason.Caster?.CharacterName ?? "?"}");

            // Broad match for Subsonic Hum until GUID is known
            if (!blueprintName.Contains("Subsonic") && !blueprintName.Contains("Hum"))
                return;

            Main.Log($"[FSB] === SUBSONIC HUM DETECTED ===");
            Main.Log($"[FSB] Blueprint: {blueprintName} ({reason.Ability.Blueprint?.AssetGuid})");
            Main.Log($"[FSB] Final DC: {__instance.DifficultyClass}");
            Main.Log($"[FSB] Save Type: {__instance.Type}");
            Main.Log($"[FSB] Passed: {__instance.IsPassed}");
            Main.Log($"[FSB] D20 Roll: {__instance.D20}");
            Main.Log($"[FSB] Roll Result (with mods): {__instance.RollResult}");
            Main.Log($"[FSB] Roller: {__instance.Initiator?.CharacterName ?? "?"}");

            // Context-level DC info
            var ctx = reason.Context;
            if (ctx != null) {
                Main.Log($"[FSB] Context.Params.DC: {ctx.Params?.DC}");
                Main.Log($"[FSB] Context.Params.CasterLevel: {ctx.Params?.CasterLevel}");
                Main.Log($"[FSB] SpellLevel: {ctx.SpellLevel}");
                Main.Log($"[FSB] SpellDescriptor: {ctx.SpellDescriptor}");
            }

            // Caster stats for DC calculation analysis
            var caster = reason.Caster;
            if (caster != null) {
                Main.Log($"[FSB] Caster: {caster.CharacterName}");
                var stats = caster.Stats;
                Main.Log($"[FSB] Caster Constitution: {stats?.Constitution?.ModifiedValue}");
                Main.Log($"[FSB] Caster Con Base: {stats?.Constitution?.BaseValue}");
                Main.Log($"[FSB] Caster HD: {caster.Progression?.CharacterLevel}");
                Main.Log($"[FSB] Caster CR: {caster.Progression?.MythicLevel}");
            }
        }
    }
}
