using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace CompressedRaid.Global
{
    internal class Utils
    {
        public static int GetThreatPointsByWealth(float factorPercentage)
        {
            var settings = LoadedModManager.GetMod<CompressedRaidMod>()?.CRModSettings;
            if (settings.mapGeneratedEnemyMainColonyUseAllMaps)
            {
                var playerMaps = Find.Maps.ToList();
                int threatSum = 0;
                foreach (var map in playerMaps)
                {
                    threatSum += GetThreatPointsByPlayerWealth(map, factorPercentage);
                }
                return threatSum;
            }
            else
            {
                var playerHomeMap = GetPlayerMainColonyMap();
                if (playerHomeMap == null)
                {
                    return 0;
                }
                return GetThreatPointsByPlayerWealth(playerHomeMap, factorPercentage);
            }
        }

        public static int GetThreatPointsByPlayerWealth(
            Map map,
            float factorPercentage //Percentage of threat from this player colony
            )
        {
            float threatAvg = StorytellerUtility.DefaultThreatPointsNow(map);
            return (int)(threatAvg * factorPercentage);
        }

        public static Map GetPlayerMainColonyMap(bool excludeSOS2Rimnauts2SpaceMaps = true, bool requirePlayerHome = true)
        {
            var playerHomes = (from map in Find.Maps
                               where (requirePlayerHome == false || map.IsPlayerHome)
                               && (excludeSOS2Rimnauts2SpaceMaps == false || !IsSOS2OrRimNauts2SpaceMap(map))
                               select map).OrderByDescending(map => map.PlayerWealthForStoryteller).ToList();

            return playerHomes.Count > 0 ? playerHomes.First() : null;
        }

        //判断该地图是否为SOS2的太空地图。
        public static bool IsSOS2SpaceMap(Map map)
        {
            if (map.Biome.defName.Contains("OuterSpace"))
            {
                return true;
            }
            return false;
        }

        public static bool IsRimNauts2SpaceMap(Map map)
        {
            return map.Biome.defName.StartsWith("RimNauts2_");
        }

        public static bool IsSOS2OrRimNauts2SpaceMap(Map map)
        {
            return IsSOS2SpaceMap(map) || IsRimNauts2SpaceMap(map);
        }

        public static bool IsUndergroundMaps(Map map)
        {
            return (ModsConfig.AnomalyActive && map.Biome == BiomeDefOf.Undercave) //Undercave of Anomaly
                || (ModsConfig.IsActive("Mlie.DeepRim") && map.Biome.defName == "Underground") //Underground map from DeepRim
                ;
        }

        public static bool RunIncident(IncidentDef incidentDef, Map map = null, float points = 0)
        {
            IIncidentTarget target = Find.World;
            if (map != null)
            {
                target = map;
            }
            var incidentParms = StorytellerUtility.DefaultParmsNow(incidentDef.category, target);
            if (incidentDef.pointsScaleable)
            {
                var storytellerComp = Find.Storyteller.storytellerComps.First(comp =>
                    comp is StorytellerComp_OnOffCycle || comp is StorytellerComp_RandomMain);
                incidentParms = storytellerComp.GenerateParms(incidentDef.category, incidentParms.target);
            }
            if (points > 0)
            {
                incidentParms.points = points;
            }

            return incidentDef.Worker.TryExecute(incidentParms);
        }

        public static IncidentDef RandomIncidentDefForAnomalyRaid()
        {
            var anomalyManager = Current.Game.GetComponent<GameComponent_Anomaly>();
            int anomalyThreatLevel = anomalyManager.Level;

            var eligibleAnomalyThreatIncidents = new List<IncidentDef>();
            if (anomalyThreatLevel >= 0)
            {
                eligibleAnomalyThreatIncidents.Add(IncidentDef.Named("PsychicRitualSiege"));
            }
            if (anomalyThreatLevel >= 1)
            {
                eligibleAnomalyThreatIncidents.Add(IncidentDef.Named("SightstealerSwarm"));
                eligibleAnomalyThreatIncidents.Add(IncidentDef.Named("HateChanters"));
                eligibleAnomalyThreatIncidents.Add(IncidentDef.Named("FleshbeastAttack"));
                eligibleAnomalyThreatIncidents.Add(IncidentDef.Named("GorehulkAssault"));
                eligibleAnomalyThreatIncidents.Add(IncidentDef.Named("Revenant"));
            }
            if (anomalyThreatLevel >= 2)
            {
                eligibleAnomalyThreatIncidents.Add(IncidentDef.Named("DevourerAssault"));
                eligibleAnomalyThreatIncidents.Add(IncidentDef.Named("ChimeraAssault"));
            }
            return eligibleAnomalyThreatIncidents.RandomElement();
        }

        public static Faction FindRandomHostileHumanFactionOnMap(Map map)
        {
            var hostilePawn = map.mapPawns.AllPawnsSpawned.Where(pawn =>
                pawn.RaceProps.Humanlike
                && pawn.Faction.HostileTo(Faction.OfPlayer)
                && !pawn.IsPrisoner
                && !pawn.IsSlave
                && !pawn.Dead
            ).RandomElement();
            if (hostilePawn == null)
            {
                return null;
            }
            return hostilePawn.Faction;
        }
    }
}
