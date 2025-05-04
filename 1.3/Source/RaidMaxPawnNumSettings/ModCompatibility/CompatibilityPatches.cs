using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static CompressedRaid.StaticVariables_ModCompatibility;

namespace CompressedRaid
{
    [StaticConstructorOnStartup]
    internal class CompatibilityPatches
    {
        public static void Patcher(Harmony harmony, out bool mser, out bool ppai, out bool nmr)
        {
            //MSER
            mser = PatchMSER(harmony);

            //Powerful Psycast AI
            ppai = PatchPowerfulPsycastAI(harmony);

            //NoMoreRelative
            nmr = PatchNoMoreRelative(harmony);
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

        #region NoMoreRelative
        private static bool PatchNoMoreRelative(Harmony harmony)
        {
            if (!MOD_NoMoreRelationShip_PatchAllowed)
            {
                return false;
            }
            ModMetaData mod = ModsConfig.ActiveModsInLoadOrder.Where(x => x.PackageId.StartsWith(MOD_NoMoreRelationShip_ID.ToLower())).FirstOrDefault();
            if (mod == null)
            {
                return false;
            }
            Type orgType1 = AccessTools.TypeByName(MOD_NoMoreRelationShip_Patch1_TypeName);
            MethodInfo orgMethod1 = null;
            if (orgType1 != null)
            {
                orgMethod1 = AccessTools.Method(orgType1, MOD_NoMoreRelationShip_Patch1_MethodName, MOD_NoMoreRelationShip_Patch1_ArgumentsTypes);
            }
            MOD_NoMoreRelationShip_Active = mod.Active && orgMethod1 != null;
            if (MOD_NoMoreRelationShip_Active)
            {
                try
                {
                    harmony.Patch(orgMethod1,
                        null,
                        null,
                        new HarmonyMethod(typeof(CompatibilityPatches), nameof(CompatibilityPatches.PawnGroupKindWorker_Normal_Patch_Prefix_Transpiler), null) { methodType = MethodType.Normal });

                    harmony.Patch(orgMethod1,
                        null,
                        null,
                        null,
                        new HarmonyMethod(typeof(CompatibilityPatches), nameof(CompatibilityPatches.PawnGroupKindWorker_Normal_Patch_Prefix_Finalizer), null) { methodType = MethodType.Normal });
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error(String.Format("[Compressed Raid] Error: [NoMoreRelationShip禁止袭击生成亲戚] Compatibility Patch Failed!! reason:{0}{1}", Environment.NewLine, ex.ToString()));
                }
            }
            return false;

        }
        #region NoMoreRelative_PawnGroupKindWorker_Normal_Patch Patches
        internal static IEnumerable<CodeInstruction> PawnGroupKindWorker_Normal_Patch_Prefix_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            int index = 0;
            CodeInstruction pre = null;
            bool matched = false;
            foreach (CodeInstruction ci in instructions)
            {
                if (!matched && ci.opcode == OpCodes.Callvirt && pre?.opcode == OpCodes.Call)
                {
                    MemberInfo member = pre.operand as MemberInfo;
                    if (member?.Name == nameof(PawnGroupMakerUtility.ChoosePawnGenOptionsByPoints))
                    {
#if DEBUG
                        Log.Message(String.Format("@@@ target={0}.{1} index={2} HIT!!!", original.DeclaringType, original.Name, index));
#endif
                        matched = true;
                        yield return new CodeInstruction(OpCodes.Ldarg_0); //parms(PawnGroupMakerParms parms)
                        yield return CodeInstruction.Call(typeof(PatchContinuityHelper), nameof(PatchContinuityHelper.SetCompressWork_GeneratePawns), new Type[] { typeof(IEnumerable<PawnGenOption>), typeof(PawnGroupMakerParms) });
                    }
                }
                yield return ci;
                pre = ci;
                index++;
            }
        }

        internal static Exception PawnGroupKindWorker_Normal_Patch_Prefix_Finalizer(Exception __exception, PawnGroupMakerParms parms, List<Pawn> outPawns)
        {
            if (__exception == null)
            {
#if DEBUG
                Log.Message($"Prefix_Finalizer IN!! outPawns={outPawns?.Count}");
#endif
                General.GeneratePawns_Impl(parms, outPawns);
            }
            return __exception;
        }
#endregion
#endregion
    }

}
