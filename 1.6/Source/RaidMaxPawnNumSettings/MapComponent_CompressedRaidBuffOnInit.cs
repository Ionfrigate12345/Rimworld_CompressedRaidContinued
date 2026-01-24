using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompressedRaid.Global;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace CompressedRaid
{
    public class MapComponent_CompressedRaidBuffOnInit : MapComponent
    {
        private bool _done;
        private int _delayTicks = 60;    
        private const int TicksBetweenChecks = 15;

        private static HashSet<int> _loadedAtStartup;
        private static bool _initialized;

        public MapComponent_CompressedRaidBuffOnInit(Map map) : base(map)
        {
            if(map.Parent is Settlement && map.ParentFaction == Faction.OfPlayer)
            {
                return;
            }
            if (!_initialized)
            {
                // record all maps that exist right now
                _loadedAtStartup = new HashSet<int>(Find.Maps.Select(m => m.uniqueID));
                _initialized = true;
            }
        }

        public override void MapComponentTick()
        {
            if (_done) return;

            if (map.Parent is Settlement && map.ParentFaction == Faction.OfPlayer)
            {
                return;
            }

            // only buff maps that were NOT in the startup snapshot
            if (_loadedAtStartup.Contains(map.uniqueID))
                return;

            // Wait a couple of frames so mapPawns is guaranteed ready
            if (_delayTicks > 0)
            {
                _delayTicks--;
                return;
            }

            // every 30 ticks, try to buff
            if (Find.TickManager.TicksGame % TicksBetweenChecks != 0)
                return;

            var pawns = map.mapPawns?.AllPawnsSpawned;
            if (pawns == null)
                return;

            _done = true;  // we’re going to do it now

            // — everything below is your existing buff logic —
            var settings = LoadedModManager.GetMod<CompressedRaidMod>()?.CRModSettings;
            if (settings == null || !settings.mapGeneratedEnemyEnhanced)
                return;

            List<Pawn> hostile = pawns
                .Where(p => p.Faction != null
                         && p.Faction.HostileTo(Faction.OfPlayer)
                         && !p.Dead && !p.Downed)
                .ToList();

            float threat = Utils.GetThreatPointsByWealth(1.0f);
            if (threat <= settings.mapGeneratedEnemyMainColonyThreatMinimum)
                return;

            float gain = (threat - settings.mapGeneratedEnemyMainColonyThreatMinimum)
                         / settings.mapGeneratedEnemyMainColonyThreatPerStatPercentage
                         / 100f;

            int order = PowerupUtility.GetNewOrder();
            int enhanceNum = PowerupUtility.GetEnhancePawnNumber(hostile.Count);
            bool disable = PowerupUtility.DisableFactors();

            int count = 0;
            for (int i = 0; i < hostile.Count; i++)
            {
                var pawn = hostile[i];
                if (CompressedRaidMod.AllowCompress(pawn)
                 && gain > 0f
                 && !disable
                 && PowerupUtility.EnableEnhancePawn(i, enhanceNum))
                {
                    var hd = PowerupUtility.SetPowerupHediff(pawn, order, false);
                    if (hd != null && PowerupUtility.TrySetStatModifierToHediff(hd, gain))
                        count++;
                }   
            }

            if (CompressedRaidMod.displayMessageValue && count > 0)
                Messages.Message(
                $"[CR] {count} preexisting enemies buffed on “{map.Parent?.Label ?? map.Tile.ToString()}”",
                  MessageTypeDefOf.NeutralEvent, false);
        }
    }
}
