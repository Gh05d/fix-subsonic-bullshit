using HarmonyLib;
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
}
