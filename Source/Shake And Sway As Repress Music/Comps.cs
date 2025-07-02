using RimWorld;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;

namespace ShakeAndSwayAsRepressMusic
{
    public class RepressTrackerComp : MapComponent
    {
        private readonly List<TrackerBase> trackers = new List<TrackerBase>();

        public RepressTrackerComp(Map map) : base(map)
        {
        }
        public void AddTracker(TrackerBase tracker)
        {
            if (!trackers.Any(x => x.GetType() == tracker.GetType()))
            {
                //Log.Message($"Adding tracker: {tracker.GetType().Name}");
                trackers.Add(tracker);
            }
            else
            {
                //Log.Message($"Tracker of type {tracker.GetType().Name} already exists, not adding again.");
                return;
            }
        }
        public override void MapComponentTick()
        {
            base.MapComponentTick();
            var removingTrackers = new List<TrackerBase>();
            foreach (var tracker in trackers)
            {
                if (tracker.Tick(map))
                {
                    if (trackers.Count == 1)
                    {
                        Find.MusicManagerPlay.StartNewSong();
                    }
                    removingTrackers.Add(tracker);
                }
            }
            if (removingTrackers.Any())
            {
                foreach (var tracker in removingTrackers)
                {
                    //Log.Message($"Removing tracker: {tracker.GetType().Name}, current track count: {trackers.Count}");
                    trackers.Remove(tracker);
                }
            }
        }
    }
    public abstract class TrackerBase
    {
        public abstract bool Tick(Map map);
    }
    public class PrisonBreakTracker : TrackerBase
    {
        public override bool Tick(Map map)
        {
            var breakingPrisoners = map.mapPawns.PrisonersOfColonySpawned.Where(
                x => !x.DeadOrDowned &&
                PrisonBreakUtility.IsPrisonBreaking(x)
                );
            //var breakingPrisoners = prisoners.Where(x => PrisonBreakUtility.IsPrisonBreaking(x));
            return !breakingPrisoners.Any();
        }
    }
    public class SlaveRebellionTracker : TrackerBase
    {
        public override bool Tick(Map map)
        {
            var slaves = map.mapPawns.SlavesOfColonySpawned.Where(x => !x.DeadOrDowned);
            var rebellingSlaves = slaves.Where(x => SlaveRebellionUtility.IsRebelling(x));
            return !rebellingSlaves.Any();
        }
    }
    public class EntityEscapeTracker : TrackerBase
    {
        public override bool Tick(Map map)
        {
            var escapingEntities = map.mapPawns.AllPawnsSpawned.Where(
                x => x.HasComp<CompHoldingPlatformTarget>() &&
                x.TryGetComp<CompHoldingPlatformTarget>().isEscaping
                );
            return !escapingEntities.Any();
        }
    }
}
