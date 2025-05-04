using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.Sound;
using Verse.AI.Group;
using UnityEngine;
using System.Reflection.Emit;
using static CompressedRaid.StaticVariables_ModCompatibility;

namespace CompressedRaid
{
    [StaticConstructorOnStartup]
    public class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new HarmonyLib.Harmony("miyamiya.compressedraid.latest");
            Patcher(harmony);

            BionicsDataStore.DataRestore();
            DrugHediffDataStore.DataRestore();
        }

        struct TargetMethod
        {
            public Type type;
            public MethodInfo method;
            public TargetMethod(Type type, MethodInfo method)
            {
                this.type = type;
                this.method = method;
            }
            public override string ToString()
            {
                return String.Format("{0}.{1}", type.FullName, method.Name);
            }
            public override bool Equals(object obj)
            {
                MethodInfo m = obj as MethodInfo;
                if (m == null)
                {
                    return false;
                }
                return this.GetHashCode() == m.GetHashCode();
            }
            public override int GetHashCode()
            {
                return this.ToString().GetHashCode();
            }
        }

        private static void Patcher(Harmony harmony)
        {
            CompatibilityPatches.Patcher(harmony, out bool mser, out bool ppai);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            ManualSafePatch(harmony);
        }

        private static void ManualSafePatch(Harmony harmony)
        {
            HashSet<TargetMethod> targetMethods = new HashSet<TargetMethod>();
            TranspilerTest(harmony, targetMethods);

            //GeneratePawns
            Type orgType = typeof(PawnGroupMakerUtility);
            string orgName = nameof(PawnGroupMakerUtility.ChoosePawnGenOptionsByPoints);
            MethodInfo method = null;
            if (!General.m_CanTranspilerGeneratePawns)
            {
                method = AccessTools.Method(orgType, orgName, new Type[] { typeof(float), typeof(List<PawnGenOption>), typeof(PawnGroupMakerParms) });
                try
                {
                    harmony.Patch(method,
                        null,
                        null,
                        null,
                        new HarmonyMethod(typeof(PawnGroupMakerUtility_Patch), nameof(PawnGroupMakerUtility_Patch.ChoosePawnGenOptionsByPoints_Finalizer), new Type[] { typeof(Exception), typeof(float), typeof(List<PawnGenOption>), typeof(PawnGroupMakerParms), typeof(IEnumerable<PawnGenOption>).MakeByRefType() }) { methodType = MethodType.Normal });
                    General.SendLog_Debug(General.MessageTypes.Debug, String.Format("[{0}.{1}] Finalizer patched!!", orgType.FullName, orgName));
                }
                catch (Exception ex)
                {
                    General.SendLog_Debug(General.MessageTypes.DebugError, String.Format("[{0}.{1}] Patch Failed!! reason:{2}{3}", orgType.FullName, orgName, Environment.NewLine, ex.ToString()));
                }
            }

            foreach (TargetMethod targetMethod in targetMethods)
            {
                Type workerType = targetMethod.type;
                method = targetMethod.method;

                try
                {
                    harmony.Patch(method,
                        null,
                        null,
                        null,
                        new HarmonyMethod(typeof(PawnGroupKindWorker_Patch), nameof(PawnGroupKindWorker_Patch.GeneratePawns_Finalizer), new Type[] { typeof(Exception), typeof(PawnGroupMakerParms), typeof(List<Pawn>) }) { methodType = MethodType.Normal });

                    if (General.m_CanTranspilerGeneratePawns)
                    {
                        harmony.Patch(method,
                            null,
                            null,
                            new HarmonyMethod(typeof(PawnGroupKindWorker_Patch), nameof(PawnGroupKindWorker_Patch.GeneratePawns_Transpiler), new Type[] { typeof(IEnumerable<CodeInstruction>), typeof(MethodBase) }) { methodType = MethodType.Normal });
                        General.SendLog_Debug(General.MessageTypes.Debug, String.Format("[{0}] Transpiler patched!!", targetMethod));
                    }
                    General.m_AllowPawnGroupKindWorkerTypes.Add(workerType);
                }
                catch (Exception ex)
                {
                    General.SendLog_Debug(General.MessageTypes.DebugError, String.Format("[{0}] Patch Failed!! reason:{1}{2}", targetMethod, Environment.NewLine, ex.ToString()));
                }
            }

            //GenerateAnimals
            orgType = typeof(ManhunterPackIncidentUtility);
            orgName = nameof(ManhunterPackIncidentUtility.GenerateAnimals_NewTmp);
            method = AccessTools.Method(orgType, orgName, new Type[] { typeof(PawnKindDef), typeof(int), typeof(float), typeof(int) });
            if (General.m_CanTranspilerGenerateAnimals)
            {
                try
                {
                    harmony.Patch(method,
                        null,
                        null,
                        new HarmonyMethod(typeof(ManhunterPackIncidentUtility_Patch), nameof(ManhunterPackIncidentUtility_Patch.GenerateAnimals_NewTmp_Transpiler), new Type[] { typeof(IEnumerable<CodeInstruction>) }) { methodType = MethodType.Normal });

                    harmony.Patch(method,
                        null,
                        null,
                        null,
                        new HarmonyMethod(typeof(ManhunterPackIncidentUtility_Patch), nameof(ManhunterPackIncidentUtility_Patch.GenerateAnimals_NewTmp_Finalizer), new Type[] { typeof(Exception), typeof(List<Pawn>).MakeByRefType(), typeof(PawnKindDef) }) { methodType = MethodType.Normal });
                    General.SendLog_Debug(General.MessageTypes.Debug, String.Format("[{0}.{1}] Transpiler patched!!", orgType.FullName, orgName));
                }
                catch (Exception ex)
                {
                    General.SendLog_Debug(General.MessageTypes.DebugError, String.Format("[{0}.{1}] Patch Failed!! reason:{2}{3}", orgType.FullName, orgName, Environment.NewLine, ex.ToString()));
                }
            }
            else
            {
                try
                {
                    harmony.Patch(method,
                        new HarmonyMethod(typeof(ManhunterPackIncidentUtility_Patch), nameof(ManhunterPackIncidentUtility_Patch.GenerateAnimals_NewTmp_Prefix), new Type[] { typeof(List<Pawn>).MakeByRefType(), typeof(PawnKindDef), typeof(int), typeof(float), typeof(int) }) { methodType = MethodType.Normal });

                    General.SendLog_Debug(General.MessageTypes.Debug, String.Format("[{0}.{1}] Prefix patched!!", orgType.FullName, orgName));
                }
                catch (Exception ex)
                {
                    General.SendLog_Debug(General.MessageTypes.DebugError, String.Format("[{0}.{1}] Patch Failed!! reason:{2}{3}", orgType.FullName, orgName, Environment.NewLine, ex.ToString()));
                }
            }
        }

        private static void TranspilerTest(Harmony harmony, HashSet<TargetMethod> targetMethods)
        {
            //GeneratePawns
            MethodInfo methodGeneratePawns_TestTramspiler = AccessTools.Method(typeof(PawnGroupKindWorker_Patch), nameof(PawnGroupKindWorker_Patch.GeneratePawns_Test_Transpiler), new Type[] { typeof(IEnumerable<CodeInstruction>) });
            MethodInfo orgMethod;
            foreach (AllowPawnGroupKindWorkerTypeDef allowedWorker in DefDatabase<AllowPawnGroupKindWorkerTypeDef>.AllDefs)
            {
                if (allowedWorker.workerTypeNames != null)
                {
                    foreach (string workerTypeName in allowedWorker.workerTypeNames.Distinct())
                    {
                        Type workerType = AccessTools.TypeByName(workerTypeName);
                        if ((workerType?.IsSubclassOf(typeof(PawnGroupKindWorker)) ?? false))
                        {
                            orgMethod = AccessTools.Method(workerType, nameof(PawnGroupKindWorker.GeneratePawns), new Type[] { typeof(PawnGroupMakerParms), typeof(PawnGroupMaker), typeof(List<Pawn>), typeof(bool) });
                            TargetMethod targetMethod = new TargetMethod(workerType, orgMethod);
                            if (orgMethod?.DeclaringType == workerType && !targetMethods.Contains(targetMethod))
                            {
                                targetMethods.Add(targetMethod);
                                if (General.m_CanTranspilerGeneratePawns)
                                {
                                    try
                                    {
                                        harmony.Patch(orgMethod,
                                            null,
                                            null,
                                            new HarmonyMethod(methodGeneratePawns_TestTramspiler) { methodType = MethodType.Normal });
                                        harmony.Unpatch(orgMethod, methodGeneratePawns_TestTramspiler);
                                    }
                                    catch
                                    {
                                        General.m_CanTranspilerGeneratePawns = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //GenerateAnimals
            MethodInfo methodGenerateAnimals_TestTramspiler = AccessTools.Method(typeof(ManhunterPackIncidentUtility_Patch), nameof(ManhunterPackIncidentUtility_Patch.GenerateAnimals_Test_Transpiler), new Type[] { typeof(IEnumerable<CodeInstruction>) });
            try
            {
                orgMethod = AccessTools.Method(typeof(ManhunterPackIncidentUtility), nameof(ManhunterPackIncidentUtility.GenerateAnimals_NewTmp), new Type[] { typeof(PawnKindDef), typeof(int), typeof(float), typeof(int) });
                harmony.Patch(orgMethod,
                    null,
                    null,
                    new HarmonyMethod(methodGenerateAnimals_TestTramspiler) { methodType = MethodType.Normal });
                harmony.Unpatch(orgMethod, methodGenerateAnimals_TestTramspiler);
            }
            catch
            {
                General.m_CanTranspilerGenerateAnimals = false;
            }
        }

    }

    #region Patch Manually

    #region PawnGroupKindWorker Patches
    [StaticConstructorOnStartup]
    class PawnGroupKindWorker_Patch
    {
        internal static IEnumerable<CodeInstruction> GeneratePawns_Test_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int index = 0;
            CodeInstruction pre = null;
            bool matched = false;
            foreach (CodeInstruction ci in instructions)
            {
                if (!matched && ci.opcode == OpCodes.Callvirt && pre?.opcode == OpCodes.Call)
                {
                    MemberInfo member = pre.operand as MemberInfo;
                    matched = member?.Name == nameof(PawnGroupMakerUtility.ChoosePawnGenOptionsByPoints);
                }
                pre = ci;
                index++;
            }
            General.m_CanTranspilerGeneratePawns &= matched;
            return instructions;
        }

        internal static IEnumerable<CodeInstruction> GeneratePawns_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
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
                        yield return new CodeInstruction(OpCodes.Ldarg_1); //parms(PawnGroupMakerParms parms)
                        yield return CodeInstruction.Call(typeof(PatchContinuityHelper), nameof(PatchContinuityHelper.SetCompressWork_GeneratePawns), new Type[] { typeof(IEnumerable<PawnGenOption>), typeof(PawnGroupMakerParms) });
                    }
                }
                yield return ci;
                pre = ci;
                index++;
            }
        }

        internal static Exception GeneratePawns_Finalizer(Exception __exception, PawnGroupMakerParms parms, List<Pawn> outPawns)
        {
            if (__exception == null)
            {
                General.GeneratePawns_Impl(parms, outPawns);
            }
            return __exception;
        }
    }
#endregion

#region PawnGroupMakerUtility Patch
    [StaticConstructorOnStartup]
    class PawnGroupMakerUtility_Patch
    {
        internal static Exception ChoosePawnGenOptionsByPoints_Finalizer(Exception __exception, float pointsTotal, List<PawnGenOption> options, PawnGroupMakerParms groupParms, ref IEnumerable<PawnGenOption> __result)
        {
            if (__exception == null)
            {
                PatchContinuityHelper.SetCompressWork_GeneratePawns(groupParms, ref __result);
            }
            return __exception;
        }

    }
#endregion

#region ManhunterPackIncidentUtility Patches
    [StaticConstructorOnStartup]
    class ManhunterPackIncidentUtility_Patch
    {
        internal static IEnumerable<CodeInstruction> GenerateAnimals_Test_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeInstruction pre1 = null;
            CodeInstruction pre2 = null;
            bool matched = false;
            foreach (CodeInstruction ci in instructions)
            {
                if (!matched)
                {
                    matched = ci.opcode == OpCodes.Ldc_I4_0 && pre1?.opcode == OpCodes.Stloc_1 && pre2?.opcode == OpCodes.Ldarg_3;
                }
                pre2 = pre1;
                pre1 = ci;
            }
            General.m_CanTranspilerGenerateAnimals &= matched;
            return instructions;
        }

        internal static IEnumerable<CodeInstruction> GenerateAnimals_NewTmp_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeInstruction pre1 = null;
            CodeInstruction pre2 = null;
            bool matched = false;
            foreach (CodeInstruction ci in instructions)
            {
                if (!matched && ci.opcode == OpCodes.Ldc_I4_0 && pre1?.opcode == OpCodes.Stloc_1 && pre2?.opcode == OpCodes.Ldarg_3)
                {
                    matched = true;
                    yield return new CodeInstruction(OpCodes.Ldarg_0); //animalKind(PawnKindDef animalKind)
                    yield return new CodeInstruction(OpCodes.Ldloc_1); //baseNum(int num)
                    yield return CodeInstruction.Call(typeof(PatchContinuityHelper), nameof(PatchContinuityHelper.SetCompressWork_GenerateAnimals), new Type[] { typeof(PawnKindDef), typeof(int) });
                    yield return new CodeInstruction(OpCodes.Stloc_1); //store value (int num)
                }
                yield return ci;
                pre2 = pre1;
                pre1 = ci;
            }
            if (!matched)
            {
                Log.Error(String.Format("[Compressed Raid] Error: [{0}] Compress Patch Failed!! reason: {1}", IncidentDefOf.ManhunterPack.LabelCap, "transpiler patch failed."));
            }
        }

        internal static Exception GenerateAnimals_NewTmp_Finalizer(Exception __exception, ref List<Pawn> __result, PawnKindDef animalKind)
        {
            if (__exception == null)
            {
                General.GenerateAnimals_Impl(animalKind, __result);
            }
            return __exception;
        }

        internal static bool GenerateAnimals_NewTmp_Prefix(ref List<Pawn> __result, PawnKindDef animalKind, int tile, float points, int animalCount = 0)
        {
            if (CompressedRaidMod.ModDisabled() || !CompressedRaidMod.allowManhuntersValue)
            {
                return true;
            }

            List<Pawn> list = new List<Pawn>();
            int num = (animalCount > 0) ? animalCount : ManhunterPackIncidentUtility.GetAnimalsCount(animalKind, points);
            int baseNum = num;
            int maxPawnNum = CompressedRaidMod.maxRaidPawnsCountValue;
            if (maxPawnNum >= baseNum)
            {
                return true;
            }

            bool allowedCompress = true;
            if (!CompressedRaidMod.allowMechanoidsValue && (animalKind?.RaceProps?.IsMechanoid ?? false))
            {
                allowedCompress = false;
            }
            if (!CompressedRaidMod.allowInsectoidsValue && animalKind?.RaceProps?.FleshType == FleshTypeDefOf.Insectoid)
            {
                allowedCompress = false;
            }

            int enhancePawnNumber = PowerupUtility.GetEnhancePawnNumber(maxPawnNum);
            float gainStatValue = PowerupUtility.GetGainStatValue(baseNum, maxPawnNum, enhancePawnNumber);  //Math.Min(num / maxPawnNum, float.MaxValue);
            num = allowedCompress ? Math.Min(baseNum, maxPawnNum) : baseNum;
            int order = PowerupUtility.GetNewOrder();
            int enhancedCount = 0;
            bool disableFactors = PowerupUtility.DisableFactors();
            for (int i = 0; i < num; i++)
            {
                Pawn item = PawnGenerator.GeneratePawn(new PawnGenerationRequest(animalKind, null, PawnGenerationContext.NonPlayer, tile, false, false, false, false, true, false, 1f, false, true, true, true, false, false, false, false, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null));

                if (allowedCompress && PowerupUtility.EnableEnhancePawn(i, enhancePawnNumber))
                {
                    //DummyForCompatibility付与ここから
                    if (MOD_MSER_Active)
                    {
                        item.health.AddHediff(CR_DummyForCompatibilityDefOf.CR_DummyForCompatibility);
                    }
                    //DummyForCompatibility付与ここまで

                    //Hediff仕込みここから
                    if (CompressedRaidMod.CompressedEnabled())
                    {
                        if (CompressedRaidMod.AllowCompress(item) && gainStatValue > 0f && !disableFactors)
                        {
                            Hediff powerup = PowerupUtility.RemoveAndSetPowerupHediff(item, order);
                            if (powerup != null)
                            {
                                bool powerupEnable = PowerupUtility.TrySetStatModifierToHediff(powerup, gainStatValue);
                                if (powerupEnable)
                                {
                                    enhancedCount++;
                                }
                            }

                        }
                    }
                    //Hediff仕込みここまで
                }
                list.Add(item);
            }

            if (allowedCompress)
            {
                //Optionここから
                if (CompressedRaidMod.OptionEnabled() && gainStatValue > 0f)
                {
                    //GearRefine追加ここから
                    if (CompressedRaidMod.enableRefineGearOptionValue)
                    {
                        enhancedCount += GearRefiner.RefineGear(list, gainStatValue, enhancePawnNumber);
                    }
                    //Bionics追加ここから
                    if (CompressedRaidMod.enableAddBionicsOptionValue)
                    {
                        enhancedCount += BionicsDataStore.AddBionics(list, gainStatValue, enhancePawnNumber);
                    }
                    //Drug追加ここから
                    if (CompressedRaidMod.enableAddDrugOptionValue)
                    {
                        enhancedCount += DrugHediffDataStore.AddDrugHediffs(list, gainStatValue, enhancePawnNumber);
                    }
                }
                //Optionここまで

                //DummyForCompatibility除去ここから
                if (MOD_MSER_Active)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        Pawn p = list[i];
                        CR_DummyForCompatibility dummyHediff = p.health.hediffSet.GetHediffs<CR_DummyForCompatibility>()?.FirstOrDefault();
                        if (dummyHediff != null)
                        {
                            p.health.RemoveHediff(dummyHediff);
                        }
                    }
                }
                //DummyForCompatibility除去ここまで
            }
            __result = list;
            if (CompressedRaidMod.displayMessageValue && allowedCompress)
            {
                if (gainStatValue > 0f && !disableFactors && enhancedCount > 0)
                {
                    Messages.Message(String.Format("CR_RaidCompressedMassageEnhanced".Translate(), baseNum, maxPawnNum, gainStatValue + 1f, enhancePawnNumber), MessageTypeDefOf.NeutralEvent, true);
                }
                else
                {
                    Messages.Message(String.Format("CR_RaidCompressedMassageNotEnhanced".Translate(), baseNum, maxPawnNum), MessageTypeDefOf.NeutralEvent, true);
                }
            }
            return false;
        }
    }
#endregion

#endregion

#region Patch Harmony Automatic

#region RaidStrategyWorker Patches
    [StaticConstructorOnStartup]
    [HarmonyPatch(typeof(RaidStrategyWorker))]
    class RaidStrategyWorker_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(RaidStrategyWorker.SpawnThreats))]
        [HarmonyPatch(new Type[] { typeof(IncidentParms) })]
        static bool SpawnThreats_Prefix(RaidStrategyWorker __instance, ref List<Pawn> __result, IncidentParms parms)
        {
            if (CompressedRaidMod.ModDisabled())
            {
                return true;
            }
            if (parms?.pawnKind == null)
            {
                return true;
            }
            if (parms?.faction == null)
            {
                return true;
            }
            if (parms?.raidArrivalMode?.Worker == null)
            {
                return true;
            }
            if (!CompressedRaidMod.AllowCompress(parms, out bool raidFriendly))
            {
                return true;
            }
            //bool hostileRaid = FactionUtility.HostileTo(Faction.OfPlayer, parms.faction);
            //if (!hostileRaid)
            //{
            //    return true;
            //}

            bool allowedCompress = true;
            if (!CompressedRaidMod.allowMechanoidsValue && (parms?.pawnKind?.RaceProps?.IsMechanoid ?? false))
            {
                allowedCompress = false;
            }
            if (!CompressedRaidMod.allowInsectoidsValue && parms?.pawnKind?.RaceProps?.FleshType == FleshTypeDefOf.Insectoid)
            {
                allowedCompress = false;
            }

            int maxPawnNum = CompressedRaidMod.maxRaidPawnsCountValue;
            int baseNum = parms.pawnCount;
            if (maxPawnNum >= baseNum)
            {
                return true;
            }
            if (allowedCompress)
            {
                parms.pawnCount = Math.Min(maxPawnNum, parms.pawnCount);
            }

            int enhancePawnNumber = PowerupUtility.GetEnhancePawnNumber(parms.pawnCount);

            int enhancedCount = 0;
            float gainStatValue = PowerupUtility.GetGainStatValue(baseNum, maxPawnNum, enhancePawnNumber, raidFriendly);
            int order = PowerupUtility.GetNewOrder();

            bool disableFactors = PowerupUtility.DisableFactors();

            List<Pawn> list = new List<Pawn>();


            for (int i = 0; i < parms.pawnCount; i++)
            {
                PawnKindDef pawnKind = parms.pawnKind;
                Faction faction = parms.faction;
                PawnGenerationContext context = PawnGenerationContext.NonPlayer;
                int tile = -1;
                bool forceGenerateNewPawn = false;
                bool newborn = false;
                bool allowDead = false;
                bool allowDowned = false;
                bool canGeneratePawnRelations = true;
                bool mustBeCapableOfViolence = true;
                float colonistRelationChanceFactor = 1f;
                bool forceAddFreeWarmLayerIfNeeded = false;
                bool allowGay = true;
                float biocodeWeaponsChance = parms.biocodeWeaponsChance;
                Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnKind, faction, context, tile, forceGenerateNewPawn, newborn, allowDead, allowDowned, canGeneratePawnRelations, mustBeCapableOfViolence, colonistRelationChanceFactor, forceAddFreeWarmLayerIfNeeded, allowGay, __instance.def.pawnsCanBringFood, true, false, false, false, false, biocodeWeaponsChance, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null)
                {
                    BiocodeApparelChance = 1f
                });

                if (pawn != null)
                {
                    if (allowedCompress && PowerupUtility.EnableEnhancePawn(i, enhancePawnNumber))
                    {
                        //DummyForCompatibility付与ここから
                        if (MOD_MSER_Active)
                        {
                            pawn.health.AddHediff(CR_DummyForCompatibilityDefOf.CR_DummyForCompatibility);
                        }
                        //DummyForCompatibility付与ここまで

                        //Hediff仕込みここから
                        if (CompressedRaidMod.CompressedEnabled())
                        {
                            if (CompressedRaidMod.AllowCompress(pawn) && gainStatValue > 0f && !disableFactors)
                            {
                                Hediff powerup = PowerupUtility.RemoveAndSetPowerupHediff(pawn, order);
                                if (powerup != null)
                                {
                                    bool powerupEnable = PowerupUtility.TrySetStatModifierToHediff(powerup, gainStatValue);
                                    if (powerupEnable)
                                    {
                                        enhancedCount++;
                                    }
                                }
                            }
                        }
                        //Hediff仕込みここまで
                    }
                    list.Add(pawn);
                }
            }
            if (list.Any<Pawn>())
            {
                if (allowedCompress)
                {
                    //Optionここから
                    if (CompressedRaidMod.OptionEnabled() && gainStatValue > 0f)
                    {
                        //GearRefine追加ここから
                        if (CompressedRaidMod.enableRefineGearOptionValue)
                        {
                            enhancedCount += GearRefiner.RefineGear(list, gainStatValue, enhancePawnNumber);
                        }
                        //Bionics追加ここから
                        if (CompressedRaidMod.enableAddBionicsOptionValue)
                        {
                            enhancedCount += BionicsDataStore.AddBionics(list, gainStatValue, enhancePawnNumber);
                        }
                        //Drug追加ここから
                        if (CompressedRaidMod.enableAddDrugOptionValue)
                        {
                            enhancedCount += DrugHediffDataStore.AddDrugHediffs(list, gainStatValue, enhancePawnNumber);
                        }
                    }
                    //Optionここまで

                    //DummyForCompatibility除去ここから
                    if (MOD_MSER_Active)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            Pawn p = list[i];
                            CR_DummyForCompatibility dummyHediff = p.health.hediffSet.GetHediffs<CR_DummyForCompatibility>()?.FirstOrDefault();
                            if (dummyHediff != null)
                            {
                                p.health.RemoveHediff(dummyHediff);
                            }
                        }
                    }
                    //DummyForCompatibility除去ここまで
                }

                parms.raidArrivalMode.Worker.Arrive(list, parms);
                __result = list;
            }
            __result = null;

            if (CompressedRaidMod.displayMessageValue && allowedCompress)
            {
                if (gainStatValue > 0f && !disableFactors && enhancedCount > 0)
                {
                    Messages.Message(String.Format("CR_RaidCompressedMassageEnhanced".Translate(), baseNum, maxPawnNum, gainStatValue + 1f, enhancePawnNumber), MessageTypeDefOf.NeutralEvent, true);
                }
                else
                {
                    Messages.Message(String.Format("CR_RaidCompressedMassageNotEnhanced".Translate(), baseNum, maxPawnNum), MessageTypeDefOf.NeutralEvent, true);
                }
            }

            return false;
        }
    }
#endregion

#region TunnelHiveSpawner Patches
    [StaticConstructorOnStartup]
    [HarmonyPatch(typeof(TunnelHiveSpawner))]
    class TunnelHiveSpawner_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(TunnelHiveSpawner.Tick))]
        static bool Prefix(TunnelHiveSpawner __instance, Sustainer ___sustainer, float ___FilthSpawnMTB, float ___FilthSpawnRadius, List<ThingDef> ___filthTypes, float ___DustMoteSpawnMTB, int ___secondarySpawnTick)
        {
            if (!CompressedRaidMod.allowInsectoidsValue || CompressedRaidMod.ModDisabled())
            {
                return true;
            }

            if (__instance.Spawned)
            {
                ___sustainer.Maintain();
                Vector3 vector = __instance.Position.ToVector3Shifted();
                IntVec3 c;
                if (Rand.MTBEventOccurs(___FilthSpawnMTB, 1f, 1.TicksToSeconds()) && CellFinder.TryFindRandomReachableCellNear(__instance.Position, __instance.Map, ___FilthSpawnRadius, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), null, null, out c, 999999))
                {
                    FilthMaker.TryMakeFilth(c, __instance.Map, ___filthTypes.RandomElement<ThingDef>(), 1, FilthSourceFlags.None);
                }
                if (Rand.MTBEventOccurs(___DustMoteSpawnMTB, 1f, 1.TicksToSeconds()))
                {
                    MoteMaker.ThrowDustPuffThick(new Vector3(vector.x, 0f, vector.z)
                    {
                        y = AltitudeLayer.MoteOverhead.AltitudeFor()
                    }, __instance.Map, Rand.Range(1.5f, 3f), new Color(1f, 1f, 1f, 2.5f));
                }
                if (___secondarySpawnTick <= Find.TickManager.TicksGame)
                {
                    ___sustainer.End();
                    Map map = __instance.Map;
                    IntVec3 position = __instance.Position;
                    __instance.Destroy(DestroyMode.Vanish);
                    if (__instance.spawnHive)
                    {
                        Hive hive = (Hive)GenSpawn.Spawn(ThingMaker.MakeThing(ThingDefOf.Hive, null), position, map, WipeMode.Vanish);
                        hive.SetFaction(Faction.OfInsects, null);
                        hive.questTags = __instance.questTags;
                        foreach (CompSpawner compSpawner in hive.GetComps<CompSpawner>())
                        {
                            if (compSpawner.PropsSpawner.thingToSpawn == ThingDefOf.InsectJelly)
                            {
                                compSpawner.TryDoSpawn();
                                break;
                            }
                        }
                    }
                    if (__instance.insectsPoints > 0f)
                    {
                        __instance.insectsPoints = Mathf.Max(__instance.insectsPoints, Hive.spawnablePawnKinds.Min((PawnKindDef x) => x.combatPower));
                        float pointsLeft = __instance.insectsPoints;
                        List<Pawn> list = new List<Pawn>();
                        int num = 0;

                        float gainStatValue = 0f;
                        bool disableFactors = true;
                        int maxPawnNum = CompressedRaidMod.maxRaidPawnsCountValue;
                        float preCompressedPointsMax = pointsLeft;
                        bool compressed = false;
                        float preCompressedPointLeft = 0;

                        int enhancedCount = 0;

                        Func<PawnKindDef, bool> func = null;
                        while (pointsLeft > 0f)
                        {
                            num++;
                            if (num > 1000)
                            {
                                Log.Error("Too many iterations.", false);
                                break;
                            }
                            IEnumerable<PawnKindDef> spawnablePawnKinds = Hive.spawnablePawnKinds;
                            Func<PawnKindDef, bool> predicate;
                            if ((predicate = func) == null)
                            {
                                predicate = func = ((PawnKindDef x) => x.combatPower <= pointsLeft);
                            }
                            PawnKindDef pawnKindDef;
                            if (!spawnablePawnKinds.Where(predicate).TryRandomElement(out pawnKindDef))
                            {
                                num--;
                                break;
                            }
                            if (num > maxPawnNum)
                            {
                                compressed = true;
                                pointsLeft -= pawnKindDef.combatPower;
                                continue;
                            }
                            Pawn pawn = PawnGenerator.GeneratePawn(pawnKindDef, Faction.OfInsects);
                            list.Add(pawn);
                            pointsLeft -= pawnKindDef.combatPower;
                            preCompressedPointLeft = pointsLeft;
                        }
                        if (list.Any<Pawn>())
                        {
                            int enhancePawnNumber = PowerupUtility.GetEnhancePawnNumber(maxPawnNum);
                            if (compressed)
                            {
                                gainStatValue = PowerupUtility.GetGainStatValue(num, maxPawnNum, enhancePawnNumber) * (preCompressedPointsMax / Math.Max((preCompressedPointsMax - preCompressedPointLeft), 1f));
                                int order = PowerupUtility.GetNewOrder();
                                disableFactors = PowerupUtility.DisableFactors();
                                if (gainStatValue > 0f && !disableFactors)
                                {
                                    for (int i = 0; i < list.Count; i++)
                                    {
                                        if (!PowerupUtility.EnableEnhancePawn(i, enhancePawnNumber))
                                        {
                                            continue;
                                        }
                                        Pawn item = list[i];

                                        //DummyForCompatibility付与ここから
                                        if (MOD_MSER_Active)
                                        {
                                            item.health.AddHediff(CR_DummyForCompatibilityDefOf.CR_DummyForCompatibility);
                                        }
                                        //DummyForCompatibility付与ここまで

                                        //Hediff仕込みここから
                                        if (CompressedRaidMod.AllowCompress(item) && CompressedRaidMod.CompressedEnabled())
                                        {
                                            Hediff powerup = PowerupUtility.RemoveAndSetPowerupHediff(item, order);
                                            if (powerup != null)
                                            {
                                                bool powerupEnable = PowerupUtility.TrySetStatModifierToHediff(powerup, gainStatValue);
                                                if (powerupEnable)
                                                {
                                                    enhancedCount++;
                                                }
                                            }
                                        }
                                        //Hediff仕込みここまで

                                    }
                                }
                            }

                            //Optionここから
                            if (CompressedRaidMod.OptionEnabled() && gainStatValue > 0f)
                            {
                                //GearRefine追加ここから
                                if (CompressedRaidMod.enableRefineGearOptionValue)
                                {
                                    enhancedCount += GearRefiner.RefineGear(list, gainStatValue, enhancePawnNumber);
                                }
                                //Bionics追加ここから
                                if (CompressedRaidMod.enableAddBionicsOptionValue)
                                {
                                    enhancedCount += BionicsDataStore.AddBionics(list, gainStatValue, enhancePawnNumber);
                                }
                                //Drug追加ここから
                                if (CompressedRaidMod.enableAddDrugOptionValue)
                                {
                                    enhancedCount += DrugHediffDataStore.AddDrugHediffs(list, gainStatValue, enhancePawnNumber);
                                }
                            }
                            //Optionここまで

                            //DummyForCompatibility除去ここから
                            if (MOD_MSER_Active)
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    Pawn p = list[i];
                                    CR_DummyForCompatibility dummyHediff = p.health.hediffSet.GetHediffs<CR_DummyForCompatibility>()?.FirstOrDefault();
                                    if (dummyHediff != null)
                                    {
                                        p.health.RemoveHediff(dummyHediff);
                                    }
                                }
                            }
                            //DummyForCompatibility除去ここまで

                            foreach (Pawn pawn in list)
                            {
                                GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(position, map, 2, null), map, WipeMode.Vanish);
                                pawn.mindState.spawnedByInfestationThingComp = __instance.spawnedByInfestationThingComp;
                            }
                            LordMaker.MakeNewLord(Faction.OfInsects, new LordJob_AssaultColony(Faction.OfInsects, true, false, false, false, true), map, list);

                            if (CompressedRaidMod.displayMessageValue && compressed)
                            {
                                if (gainStatValue > 0f && !disableFactors && enhancedCount > 0)
                                {
                                    Messages.Message(String.Format("CR_RaidCompressedMassageEnhanced".Translate(), num, maxPawnNum, gainStatValue + 1f, enhancePawnNumber), MessageTypeDefOf.NeutralEvent, true);
                                }
                                else
                                {
                                    Messages.Message(String.Format("CR_RaidCompressedMassageNotEnhanced".Translate(), num, maxPawnNum), MessageTypeDefOf.NeutralEvent, true);
                                }
                            }

                        }
                    }
                }
            }

            return false;
        }
    }
#endregion

#endregion
}
