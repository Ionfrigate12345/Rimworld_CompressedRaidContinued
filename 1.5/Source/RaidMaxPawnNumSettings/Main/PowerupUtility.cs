using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace CompressedRaid
{
    public class PowerupUtility
    {

        public class PowerupStat
        {
            public StatDef stat { get; set; }
            public float value { get; set; }
        }

        public static bool DisableFactors()
        {
            return (CompressedRaidMod.gainFactorMultPainValue <= 0f &&
                    CompressedRaidMod.gainFactorMultArmorRating_BluntValue <= 0f &&
                    CompressedRaidMod.gainFactorMultArmorRating_SharpValue <= 0f &&
                    CompressedRaidMod.gainFactorMultArmorRating_HeatValue <= 0f &&
                    CompressedRaidMod.gainFactorMultMeleeDodgeChanceValue <= 0f &&
                    CompressedRaidMod.gainFactorMultMeleeHitChanceValue <= 0f &&
                    CompressedRaidMod.gainFactorMultMoveSpeedValue <= 0f &&
                    CompressedRaidMod.gainFactorMultShootingAccuracyPawnValue <= 0f &&
                    CompressedRaidMod.gainFactorMultPawnTrapSpringChanceValue <= 0f &&
                    CompressedRaidMod.gainFactorMultCapacitySightValue <= 0 &&
                    CompressedRaidMod.gainFactorMultCapacityMovingValue <= 0 &&
                    CompressedRaidMod.gainFactorMultCapacityHearingValue <= 0 &&
                    CompressedRaidMod.gainFactorMultCapacityManipulationValue <= 0 &&
                    CompressedRaidMod.gainFactorMultCapacityMetabolismValue <= 0 &&
                    CompressedRaidMod.gainFactorMultCapacityConsciousnessValue <= 0 &&
                    CompressedRaidMod.gainFactorMultCapacityBloodFiltrationValue <= 0 &&
                    CompressedRaidMod.gainFactorMultCapacityBloodPumpingValue <= 0 &&
                    CompressedRaidMod.gainFactorMultCapacityBreathingValue <= 0)
                    ||
                    (CompressedRaidMod.gainFactorMaxPainValue <= 0f &&
                    CompressedRaidMod.gainFactorMaxArmorRating_BluntValue <= 0f &&
                    CompressedRaidMod.gainFactorMaxArmorRating_SharpValue <= 0f &&
                    CompressedRaidMod.gainFactorMaxArmorRating_HeatValue <= 0f &&
                    CompressedRaidMod.gainFactorMaxMeleeDodgeChanceValue <= 0f &&
                    CompressedRaidMod.gainFactorMaxMeleeHitChanceValue <= 0f &&
                    CompressedRaidMod.gainFactorMaxMoveSpeedValue <= 0f &&
                    CompressedRaidMod.gainFactorMaxShootingAccuracyPawnValue <= 0f &&
                    CompressedRaidMod.gainFactorMaxPawnTrapSpringChanceValue <= 0f &&
                    CompressedRaidMod.gainFactorMaxCapacitySightValue <= 0 &&
                    CompressedRaidMod.gainFactorMaxCapacityMovingValue <= 0 &&
                    CompressedRaidMod.gainFactorMaxCapacityHearingValue <= 0 &&
                    CompressedRaidMod.gainFactorMaxCapacityManipulationValue <= 0 &&
                    CompressedRaidMod.gainFactorMaxCapacityMetabolismValue <= 0 &&
                    CompressedRaidMod.gainFactorMaxCapacityConsciousnessValue <= 0 &&
                    CompressedRaidMod.gainFactorMaxCapacityBloodFiltrationValue <= 0 &&
                    CompressedRaidMod.gainFactorMaxCapacityBloodPumpingValue <= 0 &&
                    CompressedRaidMod.gainFactorMaxCapacityBreathingValue <= 0);
        }

        private static IEnumerable<StatModifier> GetSMList(float gainStatValue)
        {
            if (CompressedRaidMod.gainFactorMultArmorRating_BluntValue > 0f && CompressedRaidMod.gainFactorMaxArmorRating_BluntValue > 0f)
            {
                yield return new StatModifier() { stat = StatDefOf.ArmorRating_Blunt, value = Math.Min(gainStatValue * CompressedRaidMod.gainFactorMultArmorRating_BluntValue, CompressedRaidMod.gainFactorMaxArmorRating_BluntValue) };
            }
            if (CompressedRaidMod.gainFactorMultArmorRating_SharpValue > 0f && CompressedRaidMod.gainFactorMaxArmorRating_SharpValue > 0f)
            {
                yield return new StatModifier() { stat = StatDefOf.ArmorRating_Sharp, value = Math.Min(gainStatValue * CompressedRaidMod.gainFactorMultArmorRating_SharpValue, CompressedRaidMod.gainFactorMaxArmorRating_SharpValue) };
            }
            if (CompressedRaidMod.gainFactorMultArmorRating_HeatValue > 0f && CompressedRaidMod.gainFactorMaxArmorRating_HeatValue > 0f)
            {
                yield return new StatModifier() { stat = StatDefOf.ArmorRating_Heat, value = Math.Min(gainStatValue * CompressedRaidMod.gainFactorMultArmorRating_HeatValue, CompressedRaidMod.gainFactorMaxArmorRating_HeatValue) };
            }
            if (CompressedRaidMod.gainFactorMultMeleeDodgeChanceValue > 0f && CompressedRaidMod.gainFactorMaxMeleeDodgeChanceValue > 0f)
            {
                yield return new StatModifier() { stat = StatDefOf.MeleeDodgeChance, value = Math.Min(gainStatValue * CompressedRaidMod.gainFactorMultMeleeDodgeChanceValue, CompressedRaidMod.gainFactorMaxMeleeDodgeChanceValue) };
            }
            if (CompressedRaidMod.gainFactorMultMeleeHitChanceValue > 0f && CompressedRaidMod.gainFactorMaxMeleeHitChanceValue > 0f)
            {
                yield return new StatModifier() { stat = StatDefOf.MeleeHitChance, value = Math.Min(gainStatValue * CompressedRaidMod.gainFactorMultMeleeHitChanceValue, CompressedRaidMod.gainFactorMaxMeleeHitChanceValue) };
            }
            if (CompressedRaidMod.gainFactorMultMoveSpeedValue > 0f && CompressedRaidMod.gainFactorMaxMoveSpeedValue > 0f)
            {
                yield return new StatModifier() { stat = StatDefOf.MoveSpeed, value = Math.Min(gainStatValue * CompressedRaidMod.gainFactorMultMoveSpeedValue, CompressedRaidMod.gainFactorMaxMoveSpeedValue) };
            }
            if (CompressedRaidMod.gainFactorMultShootingAccuracyPawnValue > 0f && CompressedRaidMod.gainFactorMaxShootingAccuracyPawnValue > 0f)
            {
                yield return new StatModifier() { stat = StatDefOf.ShootingAccuracyPawn, value = Math.Min(gainStatValue * CompressedRaidMod.gainFactorMultShootingAccuracyPawnValue, CompressedRaidMod.gainFactorMaxShootingAccuracyPawnValue) };
            }
            if (CompressedRaidMod.gainFactorMultPawnTrapSpringChanceValue > 0f && CompressedRaidMod.gainFactorMaxPawnTrapSpringChanceValue > 0f)
            {
                yield return new StatModifier() { stat = StatDefOf.PawnTrapSpringChance, value = Math.Min(gainStatValue * CompressedRaidMod.gainFactorMultPawnTrapSpringChanceValue, CompressedRaidMod.gainFactorMaxPawnTrapSpringChanceValue) * -1f };
            }

            yield break;
        }

        private static IEnumerable<PawnCapacityModifier> GetPCMList(float gainStatValue)
        {

            if (CompressedRaidMod.gainFactorMultCapacitySightValue > 0f && CompressedRaidMod.gainFactorMaxCapacitySightValue > 0f)
            {
                yield return new PawnCapacityModifier() { capacity = PawnCapacityDefOf.Sight, offset = Math.Min(gainStatValue * CompressedRaidMod.gainFactorMultCapacitySightValue, CompressedRaidMod.gainFactorMaxCapacitySightValue) };
            }
            if (CompressedRaidMod.gainFactorMultCapacityMovingValue > 0f && CompressedRaidMod.gainFactorMaxCapacityMovingValue > 0f)
            {
                yield return new PawnCapacityModifier() { capacity = PawnCapacityDefOf.Moving, offset = Math.Min(gainStatValue * CompressedRaidMod.gainFactorMultCapacityMovingValue, CompressedRaidMod.gainFactorMaxCapacityMovingValue) };
            }
            if (CompressedRaidMod.gainFactorMultCapacityHearingValue > 0f && CompressedRaidMod.gainFactorMaxCapacityHearingValue > 0f)
            {
                yield return new PawnCapacityModifier() { capacity = PawnCapacityDefOf.Hearing, offset = Math.Min(gainStatValue * CompressedRaidMod.gainFactorMultCapacityHearingValue, CompressedRaidMod.gainFactorMaxCapacityHearingValue) };
            }
            if (CompressedRaidMod.gainFactorMultCapacityManipulationValue > 0f && CompressedRaidMod.gainFactorMaxCapacityManipulationValue > 0f)
            {
                yield return new PawnCapacityModifier() { capacity = PawnCapacityDefOf.Manipulation, offset = Math.Min(gainStatValue * CompressedRaidMod.gainFactorMultCapacityManipulationValue, CompressedRaidMod.gainFactorMaxCapacityManipulationValue) };
            }
            if (CompressedRaidMod.gainFactorMultCapacityConsciousnessValue > 0f && CompressedRaidMod.gainFactorMaxCapacityConsciousnessValue > 0f)
            {
                yield return new PawnCapacityModifier() { capacity = PawnCapacityDefOf.Consciousness, offset = Math.Min(gainStatValue * CompressedRaidMod.gainFactorMultCapacityConsciousnessValue, CompressedRaidMod.gainFactorMaxCapacityConsciousnessValue) };
            }
            if (CompressedRaidMod.gainFactorMultCapacityBloodFiltrationValue > 0f && CompressedRaidMod.gainFactorMaxCapacityBloodFiltrationValue > 0f)
            {
                yield return new PawnCapacityModifier() { capacity = PawnCapacityDefOf.BloodFiltration, offset = Math.Min(gainStatValue * CompressedRaidMod.gainFactorMultCapacityBloodFiltrationValue, CompressedRaidMod.gainFactorMaxCapacityBloodFiltrationValue) };
            }
            if (CompressedRaidMod.gainFactorMultCapacityBloodPumpingValue > 0f && CompressedRaidMod.gainFactorMaxCapacityBloodPumpingValue > 0f)
            {
                yield return new PawnCapacityModifier() { capacity = PawnCapacityDefOf.BloodPumping, offset = Math.Min(gainStatValue * CompressedRaidMod.gainFactorMultCapacityBloodPumpingValue, CompressedRaidMod.gainFactorMaxCapacityBloodPumpingValue) };
            }
            if (CompressedRaidMod.gainFactorMultCapacityBreathingValue > 0f && CompressedRaidMod.gainFactorMaxCapacityBreathingValue > 0f)
            {
                yield return new PawnCapacityModifier() { capacity = PawnCapacityDefOf.Breathing, offset = Math.Min(gainStatValue * CompressedRaidMod.gainFactorMultCapacityBreathingValue, CompressedRaidMod.gainFactorMaxCapacityBreathingValue) };
            }
            yield break;
        }

        private static float GetGainMult(bool raidFriendly)
        {
            return raidFriendly? CompressedRaidMod.gainFactorMultRaidFriendlyValue : CompressedRaidMod.gainFactorMultValue;
        }

        public static bool TrySetStatModifierToHediff(Hediff hediff, float gainStatValue)
        {
            if (gainStatValue <= 0f)
            {
                return false;
            }
            hediff.CurStage.painFactor = 1f;
            if (CompressedRaidMod.gainFactorMultPainValue > 0f && CompressedRaidMod.gainFactorMaxPainValue > 0f)
            {
                float painFactor = 1 - Math.Min((gainStatValue * CompressedRaidMod.gainFactorMultPainValue) , CompressedRaidMod.gainFactorMaxPainValue);
                hediff.CurStage.painFactor = Math.Max(painFactor, 0);
            }
            hediff.CurStage.statOffsets = new List<StatModifier>();
            foreach (StatModifier sm in GetSMList(gainStatValue))
            {
                hediff.CurStage.statOffsets.Add(sm);
            }
            hediff.CurStage.capMods = new List<PawnCapacityModifier>();
            foreach (PawnCapacityModifier pcm in GetPCMList(gainStatValue))
            {
                hediff.CurStage.capMods.Add(pcm);
            }

            CR_Powerup crh = hediff as CR_Powerup;
            if (crh != null)
            {
                crh.CreateSaveData();
            }

            return true;
        }

        public static float GetGainStatValue(int basePawnNum, int modifiedPawnNum, int enhancePawnNumber, bool raidFriendly = false)
        {
            int calcUsesPawnNum = CompressedRaidMod.calcBaseMultByEnhancedValue ? enhancePawnNumber : modifiedPawnNum;
            if (calcUsesPawnNum == 0)
            {
                return 0f;
            }
            float gainStatValue = Math.Min((basePawnNum * 1f) / (calcUsesPawnNum * 1f), float.MaxValue) - 1f;
            if (gainStatValue > 0f)
            {
                gainStatValue *= GetGainMult(raidFriendly);
            }
            else
            {
                gainStatValue = 0f;
            }
            return gainStatValue;
        }

        private static string GetPowerupDefNameStartWith()
        {
            return "CR_Powerup";
        }

        public static Hediff RemoveAndSetPowerupHediff(Pawn pawn, int order)
        {
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs.Where(x=>x is CR_Powerup).ToList();
            for (int i = 0; i < hediffs.Count; i++)
            {
                Hediff hediff = hediffs[i];
                pawn.health.RemoveHediff(hediff);
            }
            HediffDef crDef = HediffDef.Named(String.Format("{0}{1}", GetPowerupDefNameStartWith(), order));
            pawn.health.AddHediff(crDef);
            Hediff ret = pawn.health.hediffSet.hediffs.Where(x => x is CR_Powerup).FirstOrDefault();
            return ret;
        }

        public static int GetNewOrder()
        {
            List<CR_Powerup> crList = new List<CR_Powerup>();

            foreach ( Pawn pawn in Current.Game.World.worldPawns.AllPawnsAlive.Where(pawn=> pawn.health.hediffSet.hediffs.Any(hediff => hediff is CR_Powerup)))
            {
                if (pawn.Map == null || !(pawn.Map.IsPlayerHome || pawn.Map.mapPawns.AnyColonistSpawned))
                {
                    List<Hediff> hediffs = pawn.health.hediffSet.hediffs.Where(x => x is CR_Powerup).ToList();

                    for (int i = 0; i < hediffs.Count; i++)
                    {
                        Hediff hediff = hediffs[i];
                        pawn.health.RemoveHediff(hediff);
                    }
                    continue;
                }
                CR_Powerup cr = pawn.health.hediffSet.hediffs.Where(h => h is CR_Powerup).FirstOrDefault() as CR_Powerup;
                if (cr != null && !crList.Any(x => x.Order == cr.Order))
                {
                    crList.Add(cr);
                }
                if (crList.Count >= 20)
                {
                    break;
                }
            }

            foreach (Map map in Current.Game.Maps.Where(map => map.IsPlayerHome || map.mapPawns.AnyColonistSpawned))
            {
                foreach (Pawn pawn in map.mapPawns.AllPawns.Where(pawn => pawn.health.hediffSet.hediffs.Any(hediff => hediff is CR_Powerup)))
                {
                    CR_Powerup cr = pawn.health.hediffSet.hediffs.Where(h => h is CR_Powerup).FirstOrDefault() as CR_Powerup;
                    if (cr != null && !crList.Any(x => x.Order == cr.Order))
                    {
                        crList.Add(cr);
                    }
                    if (crList.Count >= 20)
                    {
                        break;
                    }
                }
                if (crList.Count >= 20)
                {
                    break;
                }
            }

            crList.Sort((a,b)=>a.Order - b.Order);
            int order = 1;
            bool isNewOrder = false;
            foreach(CR_Powerup cr in crList)
            {
                if (order < cr.Order)
                {
                    isNewOrder = true;
                    break;
                }
                order++;
            }
            if (!isNewOrder)
            {
                if (crList.Any() && crList.Max(x=>x.Order) == 20)
                {
                    order = 1;
                }
            }
            return order;
        }

        private HediffDef GetPowerupDef(int order)
        {
            return HediffDef.Named(GetPowerupDefNameStartWith() + order);
        }

        public List<BionicHediffDefHolder> ListupBionicHediff()
        {
            List<BionicHediffDefHolder> ret = new List<BionicHediffDefHolder>();
            foreach (HediffDef def in DefDatabase<HediffDef>.AllDefs.Where(x=>!x.isBad && x.spawnThingOnRemoved != null))
            {
                BionicHediffDefHolder holder = new BionicHediffDefHolder(def);
                ret.Add(holder);
            }
            return ret;
        }

        public struct BionicHediffDefHolder
        {
            private HediffDef def;

            public BionicHediffDefHolder(HediffDef def)
            {
                this.def = def;
            }
        }

        public static int GetEnhancePawnNumber(int compressPawnNumber)
        {
            if (compressPawnNumber == 0)
            {
                return 0;
            }
            else if (CompressedRaidMod.chanceOfEnhancementValue <= 0f)
            {
                return 0;
            }
            else if (CompressedRaidMod.chanceOfEnhancementValue >= 1f)
            {
                return compressPawnNumber;
            }
            int enhancePawnNumber = compressPawnNumber;
            for (int i = 0; i < compressPawnNumber; i++)
            {
                if (!Rand.Chance(CompressedRaidMod.chanceOfEnhancementValue))
                {
                    enhancePawnNumber--;
                }
            }
            return Math.Max(enhancePawnNumber, 0);
        }

        public static bool EnableEnhancePawn(int pawnIndex, int enhancePawnNumber)
        {
            return pawnIndex < enhancePawnNumber;
        }
    }


}
