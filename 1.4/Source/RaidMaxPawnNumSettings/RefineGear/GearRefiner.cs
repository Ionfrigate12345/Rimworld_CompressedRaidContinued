using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace CompressedRaid
{
    public class GearRefiner
    {
        //private static StringBuilder sb = new StringBuilder();
        public static int RefineGear(List<Pawn> pawns, float gainStatValue, int enhancePawnNumber)
        {
            //sb.Clear();
            //sb.AppendLine("====RefineGear処理ここから====");
            int count = 0;
            for (int i = 0; i < pawns.Count; i++)
            {
                if (!PowerupUtility.EnableEnhancePawn(i, enhancePawnNumber))
                {
                    continue;
                }
                Pawn pawn = pawns[i];
                if (!CompressedRaidMod.AllowCompress(pawn))
                {
                    continue;
                }
                if (RefineGear(pawn, gainStatValue))
                {
                    count++;
                }
            }
            return count;
            //sb.AppendLine("====RefineGear処理ここまで====");

            //using (System.IO.StreamWriter sw = new System.IO.StreamWriter("./DEBUG_REFINE_EQUIPMENT.txt", false, Encoding.Default))
            //{
            //    sw.WriteLine(sb.ToString());
            //    sw.Close();
            //    sw.Dispose();
            //}
        }

        private static bool RefineGear(Pawn pawn, float gainStatValue)
        {
            //sb.AppendLine("-@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@-");
            //sb.AppendLine(String.Format("--RefineGear:{0}の処理ここから--", pawn.LabelCap));

            HediffDef hediffDeathAcidifier = DefDatabase<HediffDef>.GetNamed("DeathAcidifier");
            //sb.AppendLine(String.Format("-{0}には{1}{2}-", pawn.LabelCap, hediffDeathAcidifier.LabelCap, pawn.health.hediffSet.HasHediff(hediffDeathAcidifier) ? "が埋め込まれています。" : "は埋め込まれていません。"));

            bool refinedFlg = false;

            List<ThingWithComps> weapons;
            if (TryGetQualitableWeapons(pawn, out weapons))
            {
                for (int i = 0; i < weapons.Count; i++)
                {
                    ThingWithComps weapon = weapons[i];

                    CompQuality qualityComp = weapon.GetComp<CompQuality>();
                    //sb.AppendLine(String.Format("-リファイン前 : 品質持ち武器:{0}, 品質:{1}({2}), バイオコード={3}-", weapon.LabelCap, qualityComp.Quality, (byte)qualityComp.Quality, (weapon.GetComp<CompBiocodable>()?.Biocoded ?? false) ? "yes" : "no"));

                    bool optionSetBiocode = CompressedRaidMod.optionSetBiocodeValue;
                    bool refined = TryRefineGear(pawn, weapon, qualityComp, gainStatValue);
                    refinedFlg |= refined;
                    if (!refined || !optionSetBiocode)
                    {
                        continue;
                    }

                    //Set biocode when the weapon quality increase.
                    CompBiocodable biocodeWeaponComp = weapon.GetComp<CompBiocodable>();
                    if (biocodeWeaponComp != null && biocodeWeaponComp.CodedPawn == null)
                    {
                        if (pawn.Name?.ToStringFull != null)
                        {
                            biocodeWeaponComp.CodeFor(pawn);
                        }
                    }
                }
            }

            List<ThingWithComps> apparels;
            if (TryGetQualitableApparels(pawn, out apparels))
            {
                bool refinedApparels = false;
                for (int i = 0; i < apparels.Count; i++)
                {
                    ThingWithComps apparel = apparels[i];

                    CompQuality qualityComp = apparel.GetComp<CompQuality>();
                    //sb.AppendLine(String.Format("-リファイン前 : 品質持ち防具:{0}, 品質:{1}({2})-", apparel.LabelCap, qualityComp.Quality, (byte)qualityComp.Quality));
                    refinedApparels |= TryRefineGear(pawn, apparel, qualityComp, gainStatValue);
                }
                bool optionAddDeathAcidifier = CompressedRaidMod.optionAddDeathAcidifierValue;
                if (optionAddDeathAcidifier && refinedApparels)
                {
                    //Add DeathAcidifier hediff when the apparels quality increase.
                    BionicsDataStore.TryAddDeathAcidifier(pawn);
                }
                refinedFlg |= refinedApparels;
            }

            return refinedFlg;
            ////sb.AppendLine("-@@@@@@@ 装備のリファイン中... @@@@@@@-");
            ////sb.AppendLine(String.Format("-{0}には{1}{2}-", pawn.LabelCap, hediffDeathAcidifier.LabelCap, pawn.health.hediffSet.HasHediff(hediffDeathAcidifier) ? "が埋め込まれています。" : "は埋め込まれていません。"));
            //if (TryGetQualitableWeapons(pawn, out weapons))
            //{
            //    for (int i = 0; i < weapons.Count; i++)
            //    {
            //        ThingWithComps weapon = weapons[i];
            //        CompQuality qualityComp = weapon.GetComp<CompQuality>();
            //        //sb.AppendLine(String.Format("-リファイン後 : 品質持ち武器:{0}, 品質:{1}({2}), バイオコード={3}-", weapon.LabelCap, qualityComp.Quality, (byte)qualityComp.Quality, (weapon.GetComp<CompBiocodable>()?.Biocoded ?? false) ? "yes" : "no"));
            //    }
            //}
            //if (TryGetQualitableApparels(pawn, out apparels))
            //{
            //    for (int i = 0; i < apparels.Count; i++)
            //    {
            //        ThingWithComps apparel = apparels[i];
            //        CompQuality qualityComp = apparel.GetComp<CompQuality>();
            //        //sb.AppendLine(String.Format("-リファイン後 : 品質持ち防具:{0}, 品質:{1}({2})-", apparel.LabelCap, qualityComp.Quality, (byte)qualityComp.Quality));
            //    }
            //}
            ////sb.AppendLine(String.Format("--RefineGear:{0}の処理ここまで--", pawn.LabelCap));
            ////sb.AppendLine("-@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@-");
            ////sb.AppendLine();
        }

        private static bool TryGetQualitableWeapons(Pawn pawn, out List<ThingWithComps> weapons)
        {
            IEnumerable<ThingWithComps> work = pawn.equipment?.AllEquipmentListForReading?.Where(x => x.GetComp<CompQuality>() != null);
            if (work == null || work.EnumerableNullOrEmpty())
            {
                weapons = null;
                return false;
            }
            weapons = work.ToList();
            return true;
        }

        private static bool TryGetQualitableApparels(Pawn pawn, out List<ThingWithComps> apparels)
        {
            IEnumerable<ThingWithComps> work = pawn.apparel?.WornApparel?.Where(x => x.GetComp<CompQuality>() != null);
            if (work == null || work.EnumerableNullOrEmpty())
            {
                apparels = null;
                return false;
            }
            apparels = work.ToList();
            return true;
        }

        private static bool TryRefineGear(Pawn pawn, ThingWithComps equipment, CompQuality qualityComp, float gainStatValue)
        {
            if (qualityComp.Quality == QualityCategory.Legendary)
            {
                return false;
            }

            if (!TryGetIncreaseQuality(gainStatValue, out byte increaseQuality))
            {
                return false;
            }

            QualityCategory refinedQuality = GetRefinedQuality(qualityComp.Quality, increaseQuality);
            qualityComp.SetQuality(refinedQuality, pawn.IsColonist ? ArtGenerationContext.Colony : ArtGenerationContext.Outsider);
            return true;
        }

        private static bool TryGetIncreaseQuality(float gainStatValue, out byte increaseQuality)
        {
            increaseQuality = 0x00;
            float refineGearChanceFactorValue = CompressedRaidMod.refineGearChanceFactorValue;
            float refineGearChanceNegativeCurveValue = CompressedRaidMod.refineGearChanceNegativeCurveValue;
            float refineGearChanceMaxValue = CompressedRaidMod.refineGearChanceMaxValue;
            byte qualityUpMaxNum = CompressedRaidMod.qualityUpMaxNumValue;
            byte increaseNum = 0;
            float refineGearChance = gainStatValue * refineGearChanceFactorValue;
            if (refineGearChance <= 0f)
            {
                return false;
            }
            while (increaseNum < qualityUpMaxNum)
            {
                if (!Rand.Chance(Math.Min(refineGearChance, refineGearChanceMaxValue)))
                {
                    break;
                }
                increaseQuality++;
                increaseNum++;
                refineGearChance *= refineGearChanceNegativeCurveValue;
                if (refineGearChance <= 0f)
                {
                    break;
                }
            }
            return increaseQuality > 0x00;
        }

        private static QualityCategory GetRefinedQuality(QualityCategory currentQuality, byte increaseQuality)
        {
            byte currentQualityByte = (byte)currentQuality;
            byte refinedQualityByte = (byte)Math.Min(currentQualityByte + increaseQuality, (byte)QualityCategory.Legendary);
            QualityCategory refinedQuality = (QualityCategory)Enum.ToObject(typeof(QualityCategory), refinedQualityByte);
            return refinedQuality;

        }
    }
}
