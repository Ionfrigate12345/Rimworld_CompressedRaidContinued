using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static CompressedRaid.StaticVariables_ModCompatibility;

namespace CompressedRaid
{
    [StaticConstructorOnStartup]
    internal class CompatibilityPatches
    {
        public static void Patcher(Harmony harmony, out bool mser, out bool ppai)
        {
            //MSER
            mser = PatchMSER(harmony);

            //Powerful Psycast AI
            ppai = PatchPowerfulPsycastAI(harmony);
        }

        #region MSE
        private static bool PatchMSER(Harmony harmony)
        {
            if (!MOD_MSER_PatchAllowed)
            {
                return false;
            }
            ModMetaData mod = ModsConfig.ActiveModsInLoadOrder.Where(x => x.PackageId.StartsWith(MOD_MSER_ID.ToLower())).FirstOrDefault();
            if (mod == null)
            {
                return false;
            }
            Type orgType1 = AccessTools.TypeByName(MOD_MSER_Patch1_TypeName);
            MethodInfo orgMethod1 = null;
            if (orgType1 != null)
            {
                orgMethod1 = AccessTools.Method(orgType1, MOD_MSER_Patch1_MethodName, MOD_MSER_Patch1_ArgumentsTypes);
            }
            MOD_MSER_Active = mod.Active && orgMethod1 != null;
            if (MOD_MSER_Active)
            {
                try
                {
                    harmony.Patch(orgMethod1,
                        new HarmonyMethod(typeof(CompatibilityPatches), nameof(CompatibilityPatches.OrenoMSE_MSE_VanillaExtender_HediffApplyHediffs_Prefix), null) { methodType = MethodType.Normal, before = new string[] { MOD_MSER_HarmonyID } });
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error(String.Format("[Compressed Raid] Error: [Medical System Expansion - Revived] Compatibility Patch Failed!! reason:{0}{1}", Environment.NewLine, ex.ToString()));
                }
            }
            return false;
        }
        public static bool OrenoMSE_MSE_VanillaExtender_HediffApplyHediffs_Prefix(Hediff hediff, Pawn pawn, BodyPartRecord record)
        {
            //bool forward = !pawn?.health?.hediffSet?.GetHediffs<CR_DummyForCompatibility>().Any() ?? true;
            //if (forward)
            //{
            //    Log.Message(String.Format("@@@Forward to MSE.HediffApplyHediffs.:{0}, {1}", hediff.LabelCap, pawn.LabelShort));
            //}
            //else
            //{
            //    Log.Message(String.Format("@@@Canceled MSE.HediffApplyHediffs.:{0}, {1}", hediff.LabelCap, pawn.LabelShort));
            //}
            //return forward;
            return !(pawn?.health?.hediffSet?.GetHediffs<CR_DummyForCompatibility>()?.Any() ?? true);
        }
        #endregion

        #region Powerful Psycast AI
        private static bool PatchPowerfulPsycastAI(Harmony harmony)
        {
            if (!MOD_PowerfulPsycastAI_PatchAllowed)
            {
                return false;
            }
            ModMetaData mod = ModsConfig.ActiveModsInLoadOrder.Where(x => x.PackageId.StartsWith(MOD_PowerfulPsycastAI_ID.ToLower())).FirstOrDefault();
            if (mod == null)
            {
                return false;
            }
            MOD_PowerfulPsycastAI_Active = mod.Active;
            return MOD_PowerfulPsycastAI_Active;
        }
        #endregion
    }

}
