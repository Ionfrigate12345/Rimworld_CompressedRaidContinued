using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using HarmonyLib;
using System.Reflection;

namespace CompressedRaid
{
    [StaticConstructorOnStartup]
    public class DrugHediffDataStore
    {
        private const float CALCULATE_WEIGHT_MAX = 20000f;
        private const float CALCULATE_WEIGHT_MIN = 1000f;
        private const float PAIN_WEIGHT_SCALE = 5000f;
        private const float NATURAL_HEALING_WEIGHT_SCALE = 2500f;
        private const float PERMENENT_WEIGHT_FIX_OFFSET = -10000f;
        private static readonly Type HEDIFF_VOLATILITY_TYPE = typeof(HediffComp_SeverityPerDay);
        private static readonly Type HEDIFF_GIVER_TYPE = typeof(IngestionOutcomeDoer_GiveHediff);
        private static readonly Dictionary<PawnCapacityDef, float> PAWN_CAPACITIE_WEIGHT_SCALE = new Dictionary<PawnCapacityDef, float>();
        private static readonly Dictionary<StatDef, float> STAT_WEIGHT_SCALE = new Dictionary<StatDef, float>();
        private static readonly Type[] METHOD_PARAM = new Type[] { typeof(Pawn) };
        private static readonly MethodInfo METHOD_PRE_POST_INGESTED = AccessTools.Method(typeof(Thing), "PrePostIngested", METHOD_PARAM);
        private static readonly MethodInfo METHOD_POST_INGESTED = AccessTools.Method(typeof(Thing), "PostIngested", METHOD_PARAM);

        public static List<AllowDrugHediffsHolder> m_ListupAllowDrugHediffs;
        private static HashSet<ThingDef> m_MustNegativeEffectDrugThingDefs;
        private static HashSet<ThingDef> m_ExcludeDrugThingDefs;
        private static int m_DrugOverdoseFinalStageIndex;

        static DrugHediffDataStore()
        {
            m_ListupAllowDrugHediffs = new List<AllowDrugHediffsHolder>();

            //combat-related capacities
            PAWN_CAPACITIE_WEIGHT_SCALE[PawnCapacityDefOf.Moving] = 4000f;
            PAWN_CAPACITIE_WEIGHT_SCALE[PawnCapacityDefOf.Consciousness] = 2500f;
            PAWN_CAPACITIE_WEIGHT_SCALE[PawnCapacityDefOf.Manipulation] = 2500f;
            PAWN_CAPACITIE_WEIGHT_SCALE[PawnCapacityDefOf.Sight] = 2500f;

            //combat-related stats
            STAT_WEIGHT_SCALE[StatDefOf.ArmorRating_Blunt] = 5000f;
            STAT_WEIGHT_SCALE[StatDefOf.ArmorRating_Sharp] = 5000f;
            STAT_WEIGHT_SCALE[StatDefOf.ArmorRating_Heat] = 5000f;
            STAT_WEIGHT_SCALE[StatDefOf.MeleeDodgeChance] = 5000f;
            STAT_WEIGHT_SCALE[StatDefOf.MeleeHitChance] = 5000f;
            STAT_WEIGHT_SCALE[StatDefOf.MoveSpeed] = 4000f;
            STAT_WEIGHT_SCALE[StatDefOf.ShootingAccuracyPawn] = 5000f;
            STAT_WEIGHT_SCALE[StatDefOf.PawnTrapSpringChance] = -5000f;
        }

        public class AllowDrugHediffsHolder
        {
            public float weight;
            public ThingDef drug;
            public HediffDef[] hediffDefs;
            public bool permenent;

            public AllowDrugHediffsHolder(float weight, ThingDef drug, params HediffDef[] hediffDefs)
            {
                this.drug = drug;
                this.hediffDefs = hediffDefs;
                this.weight = weight;
                this.permenent = true;
                foreach (HediffDef hediffDef in hediffDefs)
                {
                    if (!hediffDef.isBad)
                    {
#if DEBUG
                        //test
                        for (int i = 0; i < hediffDef.comps.Count; i++)
                        {
                            HediffCompProperties comp = hediffDef.comps[i];
                            Log.Message(String.Format("CR_MESSAGE {0}, tendable={1}, comp[{2}].type={3}", hediffDef.LabelCap, hediffDef.tendable, i, comp.compClass));
                        }
                        //test
#endif
                        this.permenent &= !hediffDef.tendable && !(hediffDef.comps?.Any(x => x.compClass == HEDIFF_VOLATILITY_TYPE) ?? false);
                        if (!this.permenent)
                        {
                            break;
                        }
                    }
                }
                if (this.permenent)
                {
                    this.weight = Math.Max(this.weight + PERMENENT_WEIGHT_FIX_OFFSET, CALCULATE_WEIGHT_MIN);
                }
            }
        }


        private static bool TryCalculateWeight(ThingDef drug, HediffDef hediffDef, ref float weight)
        {
            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine(String.Format("-----{0}------", drug.LabelCap));
            float weightBk = weight;
            bool combatEnhancement = false;
            HediffStage effect = hediffDef?.stages?.FirstOrDefault();
            if (effect == null)
            {
                return false;
            }
            float work;
            //painFactor
            work = (1f - effect.painFactor) * PAIN_WEIGHT_SCALE;
            if (work > 0f)
            {
                combatEnhancement = true;
                //sb.AppendLine(String.Format("+point:{0}={1}", "painFactor",  work));
            }
            //if (sb != null && work < 0f)
            //{
            //    sb.AppendLine(String.Format("-point:{0}={1}", "painFactor", work));
            //}
            weight -= work;

            //painOffset
            work = effect.painOffset * -PAIN_WEIGHT_SCALE;
            if (work > 0f)
            {
                combatEnhancement = true;
                //sb.AppendLine(String.Format("+point:{0}={1}", "painOffset", work));
            }
            //if (sb != null && work < 0f)
            //{
            //    sb.AppendLine(String.Format("-point:{0}={1}", "painOffset", work));
            //}
            weight -= work;
            //natural healing
            if (effect.naturalHealingFactor >= 0f)
            {
                weight += (effect.naturalHealingFactor - 1f) * NATURAL_HEALING_WEIGHT_SCALE;
            }
            if (effect.capMods != null)
            {
                foreach (PawnCapacityModifier cap in effect.capMods)
                {
                    if (PAWN_CAPACITIE_WEIGHT_SCALE.TryGetValue(cap.capacity, out float value))
                    {
                        work = cap.offset * cap.postFactor * value;
                        if (work > 0f)
                        {
                            combatEnhancement = true;
                            //sb.AppendLine(String.Format("+point:{0}={1}", cap.capacity.LabelCap, work));
                        }
                        //if (sb != null && work < 0f)
                        //{
                        //    sb.AppendLine(String.Format("-point:{0}={1}", cap.capacity.LabelCap, work));
                        //}
                        weight -= work;
                    }
                }
            }
            if (effect.statOffsets != null)
            {
                foreach (StatModifier stat in effect.statOffsets)
                {
                    if (STAT_WEIGHT_SCALE.TryGetValue(stat.stat, out float value))
                    {
                        work = stat.value * value;
                        if (work > 0f)
                        {
                            combatEnhancement = true;
                            //sb.AppendLine(String.Format("+point:{0}={1}", stat.stat.LabelCap, work));
                        }
                        //if (sb != null && work < 0f)
                        //{
                        //    sb.AppendLine(String.Format("-point:{0}={1}", stat.stat.LabelCap, work));
                        //}
                        weight -= work;
                    }
                }
            }
            if (effect.statFactors != null)
            {
                foreach (StatModifier stat in effect.statFactors)
                {
                    if (STAT_WEIGHT_SCALE.TryGetValue(stat.stat, out float value))
                    {
                        work = (stat.value - 1f) * value;
                        if (work > 0f)
                        {
                            combatEnhancement = true;
                            //sb.AppendLine(String.Format("+point:{0}={1}", stat.stat.LabelCap, work));
                        }
                        //if (sb != null && work < 0f)
                        //{
                        //    sb.AppendLine(String.Format("-point:{0}={1}", stat.stat.LabelCap, work));
                        //}
                        weight -= work;
                    }
                }
            }

            if (!combatEnhancement || weight > CALCULATE_WEIGHT_MAX)
            {
                weight = weightBk;
                return false;
            }

            //if (drug == ThingDefOf.Luciferium)
            //{
            //    weight = CALCULATE_WEIGHT_MIN;
            //}

            if (weight < CALCULATE_WEIGHT_MIN)
            {
                weight = CALCULATE_WEIGHT_MIN;
            }

            //Log.Message(sb.ToString());

            return true;
        }

        private static int OrderKey(ModContentPack mod)
        {
            if (mod.IsCoreMod)
            {
                return -1000;
            }
            else if (mod.PackageId == ModContentPack.RoyaltyModPackageId)
            {
                return -900;
            }
            else
            {
                return mod.loadOrder;
            }
        }

        public static void DataRestore()
        {
            m_DrugOverdoseFinalStageIndex = HediffDefOf.DrugOverdose.stages.Count() - 1;
#if DEBUG
            Log.Message(String.Format("CR_MESSAGE {0}の最終ステージのインデックス={1}", HediffDefOf.DrugOverdose.LabelCap, m_DrugOverdoseFinalStageIndex));
#endif

            StringBuilder sb = new StringBuilder();
            m_ExcludeDrugThingDefs = new HashSet<ThingDef>();
            foreach (ExcludeDrugDef excludeDrugDef in DefDatabase<ExcludeDrugDef>.AllDefs.OrderBy(x => OrderKey(x.modContentPack)))
            {
                if (excludeDrugDef.drugs != null)
                {
                    foreach (string drugDefName in excludeDrugDef.drugs)
                    {
                        ThingDef drugThingDef = DefDatabase<ThingDef>.GetNamed(drugDefName, false);
                        if (drugThingDef != null && !m_ExcludeDrugThingDefs.Contains(drugThingDef))
                        {
                            m_ExcludeDrugThingDefs.Add(drugThingDef);
                            sb.AppendLine(String.Format("{0} ({1}.{2})", drugThingDef.LabelCap, drugThingDef.modContentPack.Name, drugThingDef.defName));
                        }
                    }
                }
            }
            if (sb.Length > 0)
            {
                sb.Insert(0, "CR_InfoExcludedDrugs".Translate());
                Log.Message(sb.ToString());
            }

            m_ListupAllowDrugHediffs = new List<AllowDrugHediffsHolder>();
            foreach (ThingDef drug in DefDatabase<ThingDef>.AllDefs.Where(x=> x.ingestible != null && x.ingestible.drugCategory > DrugCategory.None && x.ingestible.outcomeDoers != null && !m_ExcludeDrugThingDefs.Contains(x)))
            {
                List<HediffDef> drugHediffs = new List<HediffDef>();
                float weight = CALCULATE_WEIGHT_MAX;
                foreach (IngestionOutcomeDoer_GiveHediff doer in drug.ingestible.outcomeDoers.Where(x=>x.GetType() == HEDIFF_GIVER_TYPE))
                {
                    if (doer.hediffDef?.isBad ?? true)
                    {
                        continue;
                    }
                    if (!TryCalculateWeight(drug, doer.hediffDef, ref weight))
                    {
                        continue;
                    }
                    drugHediffs.Add(doer.hediffDef);
                }
                if (drugHediffs.Any())
                {
                    m_ListupAllowDrugHediffs.Add(new AllowDrugHediffsHolder(weight, drug, drugHediffs.ToArray()));
                }
            }

            sb.Clear();
            m_MustNegativeEffectDrugThingDefs = new HashSet<ThingDef>();
            foreach (MustNegativeEffectDef mustNegativeEffectDef in DefDatabase<MustNegativeEffectDef>.AllDefs.OrderBy(x => OrderKey(x.modContentPack)))
            {
                if (mustNegativeEffectDef.drugs != null)
                {
                    foreach (string drugDefName in mustNegativeEffectDef.drugs)
                    {
                        ThingDef drugThingDef = DefDatabase<ThingDef>.GetNamed(drugDefName, false);
                        if (drugThingDef != null && !m_MustNegativeEffectDrugThingDefs.Contains(drugThingDef))
                        {
                            m_MustNegativeEffectDrugThingDefs.Add(drugThingDef);
                            sb.AppendLine(String.Format("{0} ({1}.{2})", drugThingDef.LabelCap, drugThingDef.modContentPack.Name, drugThingDef.defName));
                        }
                    }
                }
            }
            if (Prefs.DevMode)
            {
                if (sb.Length > 0)
                {
                    sb.Insert(0, "CR_InfoMustNegativeEffectDrugs".Translate());
                    Log.Message(sb.ToString());
                }
            }
#if DEBUG
            //test start
            m_ListupAllowDrugHediffs.Sort((x, y) => (int)(x.weight - y.weight));
            Log.Message("==Compressed Raid.DrugDataStore_DataRestore DebugMessage START==");

            Log.Message(String.Format("適用可能なドラッグリスト件数:{0}", m_ListupAllowDrugHediffs.Count));
            foreach (AllowDrugHediffsHolder drug in m_ListupAllowDrugHediffs)
            {
                Log.Message(String.Format("drug={0}({1}), permenent={2}, category={3}, weight={4}, hediffs={5}", drug.drug.LabelCap, drug.drug.defName, drug.permenent, drug.drug.ingestible.drugCategory, drug.weight, string.Join(",", from x in drug.hediffDefs select x.LabelCap)));
            }

            Log.Message("==Compressed Raid.DrugDataStore_DataRestore DebugMessage END  ==");
            //test end
#endif
        }

        private static List<AllowDrugHediffsHolder> GetDrugHediffs(Pawn pawn)
        {
            List<AllowDrugHediffsHolder> drugs = new List<AllowDrugHediffsHolder>();
            foreach (AllowDrugHediffsHolder drug in m_ListupAllowDrugHediffs.Where((AllowDrugHediffsHolder x) => {
                if (CompressedRaidMod.withoutSocialDrugValue && (x.drug.ingestible?.drugCategory == DrugCategory.Social))
                {
                    return false;
                }
                if (CompressedRaidMod.withoutHardDrugValue && (x.drug.ingestible?.drugCategory == DrugCategory.Hard))
                {
                    return false;
                }
                if (CompressedRaidMod.withoutMedicalDrugValue && (x.drug.ingestible?.drugCategory == DrugCategory.Medical))
                {
                    return false;
                }
                if (CompressedRaidMod.withoutNonVolatilityDrugValue && x.permenent)
                {
                    return false;
                }
                if (CompressedRaidMod.limitationsByTechLevelValue)
                {
                    int pawnTechLevelInt = (int)(pawn.Faction?.def.techLevel ?? TechLevel.Animal);
                    int rangeMax = pawnTechLevelInt + CompressedRaidMod.techLevelUpperRangeValue;
                    int rangeMin = pawnTechLevelInt - CompressedRaidMod.techLevelLowerRangeValue;
                    int drugTechLevelInt = (int)x.drug.techLevel;
                    if (drugTechLevelInt > rangeMax || drugTechLevelInt < rangeMin)
                    {
                        return false;
                    }
                }
                return true;
            }))
            {
                bool hasHediff = false;
                foreach(HediffDef hediffDef in drug.hediffDefs)
                {
                    hasHediff = pawn.health.hediffSet.HasHediff(hediffDef);
                    if (hasHediff)
                    {
                        break;
                    }
                }
                if (!hasHediff)
                {
                    drugs.Add(drug);
                }
            }
            return drugs;
        }

        private static bool DisallowDoseMore(Pawn pawn)
        {
            if (pawn == null || pawn.health?.hediffSet == null || pawn.Downed || pawn.Dead)
            {
#if DEBUG
                Log.Message(String.Format("CR_MESSAGE {0}はもうダメになってます。これ以上薬物を付与できません。", pawn.LabelShortCap));
#endif
                return true;
            }
            //HediffDefOf.DrugOverdose
            if (pawn.health.hediffSet.hediffs.Any(x => x.def == HediffDefOf.DrugOverdose && x.CurStageIndex == m_DrugOverdoseFinalStageIndex))
            {
#if DEBUG
                Log.Message(String.Format("CR_MESSAGE {0}はオーバードーズの最終段階です。これ以上薬物を付与できません。", pawn.LabelShortCap));
#endif
                return true;
            }
            return false;
        }

        private static bool AddDrugHediffs(Pawn pawn, float gainStatValue)
        {
            List<AllowDrugHediffsHolder> drugs = GetDrugHediffs(pawn);
            if (drugs.EnumerableNullOrEmpty())
            {
                return false;
            }
            float addDrugChanceFactorValue = CompressedRaidMod.addDrugChanceFactorValue;
            float addDrugChanceNegativeCurveValue = CompressedRaidMod.addDrugChanceNegativeCurveValue;
            float addDrugChanceMaxValue = CompressedRaidMod.addDrugChanceMaxValue;
            int addDrugMaxNumberValue = (int)CompressedRaidMod.addDrugMaxNumberValue;
            bool giveOnlyActiveIngredientsValue = CompressedRaidMod.giveOnlyActiveIngredientsValue;
            int addDrugNum = 0;
            int loopCount = 0;
            int loopMax = Math.Max(100, addDrugMaxNumberValue);
            bool forceEnd = false;

            while (loopMax > loopCount && addDrugMaxNumberValue > addDrugNum && drugs.Any() && !forceEnd)
            {
                float chance = gainStatValue * addDrugChanceFactorValue;
                if (addDrugNum > 0)
                {
                    for (int i = 0; i < addDrugNum; i++, chance *= addDrugChanceNegativeCurveValue) ;
                }
                chance = Math.Min(chance, addDrugChanceMaxValue);
                if (!Rand.Chance(chance))
                {
                    break;
                }
                AllowDrugHediffsHolder drug = drugs.RandomElementByWeight(x => x.weight);
                drugs.Remove(drug);
                bool hasHediff = false;
                foreach (HediffDef hediffDef in drug.hediffDefs)
                {
                    hasHediff = pawn.health.hediffSet.HasHediff(hediffDef);
                    if (hasHediff)
                    {
                        break;
                    }
                }
                if (!hasHediff)
                {
                    bool mustNegativeEffect = m_MustNegativeEffectDrugThingDefs?.Contains(drug.drug) ?? false;
                    if (!mustNegativeEffect && CompressedRaidMod.ignoreNonVolatilityDrugsValue && drug.permenent)
                    {
                        mustNegativeEffect = true;
                    }
                    if (giveOnlyActiveIngredientsValue && !mustNegativeEffect)
                    {
#if DEBUG
                        //test
                        if (drug.drug == ThingDefOf.Luciferium)
                        {
                            Log.Message(String.Format("CR_MESSAGE: {0}がここにきたらおかしい！バグ！", drug.drug.LabelCap));
                        }
                        else
                        {
                            Log.Message(String.Format("CR_MESSAGE: {0}は設定に従い有効成分のみ付与！", drug.drug.LabelCap));
                        }
                        //test
#endif
                        foreach (HediffDef hediffDef in drug.hediffDefs)
                        {
                            if (DisallowDoseMore(pawn))
                            {
                                forceEnd = true;
                                break;
                            }
                            pawn.health.AddHediff(hediffDef);
                        }
                    }
                    else
                    {
                        if (DisallowDoseMore(pawn))
                        {
                            forceEnd = true;
                        }
                        if (!forceEnd)
                        {
                            Thing drugThing = ThingMaker.MakeThing(drug.drug);
                            METHOD_PRE_POST_INGESTED.Invoke(drugThing, new object[] { pawn });
                            if (drugThing.def.ingestible.outcomeDoers != null)
                            {
                                for (int j = 0; j < drugThing.def.ingestible.outcomeDoers.Count; j++)
                                {
                                    if (DisallowDoseMore(pawn))
                                    {
                                        forceEnd = true;
                                        break;
                                    }
                                    drugThing.def.ingestible.outcomeDoers[j].DoIngestionOutcome(pawn, drugThing);
                                }
                            }
                            drugThing.Destroy();
                            METHOD_POST_INGESTED.Invoke(drugThing, new object[] { pawn });
                        }
                    }
                    addDrugNum++;
                }
                loopCount++;
            }
            return addDrugNum > 0;
        }

        public static int AddDrugHediffs(List<Pawn> pawns, float gainStatValue, int enhancePawnNumber)
        {
            if (pawns == null || pawns.EnumerableNullOrEmpty() || enhancePawnNumber == 0)
            {
                return 0;
            }
            int count = 0;
            for (int i = 0; i < pawns.Count(); i++)
            {
                if(!PowerupUtility.EnableEnhancePawn(i, enhancePawnNumber))
                {
                    continue;
                }
                Pawn pawn = pawns.ElementAt(i);
                if (!CompressedRaidMod.AllowCompress(pawn))
                {
                    continue;
                }
                if (!pawn.RaceProps.IsFlesh)
                {
                    continue;
                }
                if (AddDrugHediffs(pawn, gainStatValue))
                {
                    count++;
                }
            }
            return count;
        }

    }
}
