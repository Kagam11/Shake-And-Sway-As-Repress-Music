using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace ShakeAndSwayAsRepressMusic
{
    [StaticConstructorOnStartup]
    internal static class InitiallizeHarmony
    {
        static InitiallizeHarmony()
        {
            var harmony = new Harmony("Kagami.ShakeAndSwayAsRepressMusic");
            var settings = SasrmMod.settings;

            if (settings.EnablePrisonBreak)
            {
                //Log.Message("ShakeAndSwayAsRepressMusic: Initializing Harmony patches.");
                harmony.Patch(
                    AccessTools.Method(
                        typeof(PrisonBreakUtility),
                        nameof(PrisonBreakUtility.StartPrisonBreak),
                        new Type[] { typeof(Pawn) }
                        ),
                    postfix: new HarmonyMethod(
                        typeof(PatchPrisonBreak).GetMethod(nameof(PatchPrisonBreak.Postfix))
                        )
                    );
            }
            if (ModsConfig.IdeologyActive && settings.EnableSlaveRebellion)
            {
                //Log.Message("ShakeAndSwayAsRepressMusic: Patching SlaveRebellionUtility.StartSlaveRebellion for Ideology mod compatibility.");
                harmony.Patch(
                    AccessTools.Method(
                        typeof(SlaveRebellionUtility),
                        nameof(SlaveRebellionUtility.StartSlaveRebellion),
                        new Type[] { typeof(Pawn), typeof(bool) }
                        ),
                    postfix: new HarmonyMethod(
                        typeof(PatchSlaveRebellion).GetMethod(nameof(PatchSlaveRebellion.Postfix))
                        )
                    );
            }
            if (ModsConfig.AnomalyActive && settings.EnableEntityEscape)
            {
                //Log.Message("ShakeAndSwayAsRepressMusic: Patching CompHoldingPlatformTarget.Escape for Anomaly mod compatibility.");
                harmony.Patch(
                    AccessTools.Method(
                        typeof(CompHoldingPlatformTarget),
                        nameof(CompHoldingPlatformTarget.Escape),
                        new Type[] { typeof(bool) }
                        ),
                    postfix: new HarmonyMethod(
                        typeof(PatchEntityEscape).GetMethod(nameof(PatchEntityEscape.Postfix)))
                    );
            }
        }
    }

    //[HarmonyPatch(typeof(PrisonBreakUtility), nameof(PrisonBreakUtility.StartPrisonBreak), new Type[] { typeof(Pawn) })]
    public class PatchPrisonBreak
    {
        public static void Postfix(Pawn initiator)
        {
            //Log.Message($"ShakeAndSwayAsRepressMusic: Prison break started by {initiator.Name}.");
            Find.MusicManagerPlay.ForcePlaySong(SasrmSongDef.ShakeAndSway, false);
            initiator.Map.GetComponent<RepressTrackerComp>()?.AddTracker(new PrisonBreakTracker());
        }
    }
    public class PatchSlaveRebellion
    {
        public static void Postfix(Pawn initiator, bool __result)
        {
            //Log.Message($"ShakeAndSwayAsRepressMusic: Slave rebellion started by {initiator.Name}. Result: {__result}");
            if (!__result) return; // If the rebellion didn't start, do nothing.
            Find.MusicManagerPlay.ForcePlaySong(SasrmSongDef.ShakeAndSway, false);
            initiator.Map.GetComponent<RepressTrackerComp>()?.AddTracker(new SlaveRebellionTracker());
        }
    }
    public class PatchEntityEscape
    {
        public static void Postfix(CompHoldingPlatformTarget __instance)
        {
            //Log.Message($"ShakeAndSwayAsRepressMusic: Entity escape initiated by {__instance.HeldPlatform}.");
            Find.MusicManagerPlay.ForcePlaySong(SasrmSongDef.ShakeAndSway, false);
            //Log.Message($"ShakeAndSwayAsRepressMusic: Finding map");
            //Log.Message($"ShakeAndSwayAsRepressMusic: instance: {__instance}");
            var map = __instance.parent.Map;
            //Log.Message($"ShakeAndSwayAsRepressMusic: Adding Tracker");
            map.GetComponent<RepressTrackerComp>()?.AddTracker(new EntityEscapeTracker());
            //Log.Message($"ShakeAndSwayAsRepressMusic: Tracker added successfully.");
        }
    }
}
