using UnityEngine;
using Verse;

namespace ShakeAndSwayAsRepressMusic
{
    public class SasrmSettings : ModSettings
    {
        public bool EnablePrisonBreak = true;
        public bool EnableSlaveRebellion = true;
        public bool EnableEntityEscape = true;
        public override void ExposeData()
        {
            Scribe_Values.Look(ref EnablePrisonBreak, "EnablePrisonBreakTracker", true);
            Scribe_Values.Look(ref EnableSlaveRebellion, "EnableSlaveRebellionTracker", true);
            Scribe_Values.Look(ref EnableEntityEscape, "EnableAnomalyEscapeTracker", true);
        }
    }
    public class SasrmMod : Mod
    {
        public static SasrmSettings settings;

        public SasrmMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<SasrmSettings>();
        }
        public override string SettingsCategory() => "一秒六棍の小曲";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.CheckboxLabeled("是否在囚犯越狱时播放？", ref settings.EnablePrisonBreak);
            listing.CheckboxLabeled("是否在奴隶叛乱时播放？（需要文化DLC）", ref settings.EnableSlaveRebellion);
            listing.CheckboxLabeled("是否在异常逃脱时播放？（需要异象DLC）", ref settings.EnableEntityEscape);
            listing.End();
        }
    }
}
