using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static CompressedRaid.StaticVariables_ModCompatibility;

namespace CompressedRaid
{
    internal static class General
    {
        internal enum MessageTypes : byte
        {
            Infomation = 0x00,
            Warning = 0x01,
            Error = 0x03,
            Debug = 0x04,
            DebugWarning = 0x05,
            DebugError = 0x06,
        }

        internal static void SendLog_Debug(MessageTypes type, string temp, params object[] args)
        {
#if DEBUG
            if (!Prefs.DevMode && (byte)type >= 0x04)
            {
                return;
            }
            if (args?.Any() ?? false)
            {
                if (type == MessageTypes.Error || type == MessageTypes.DebugError)
                {
                    Log.Error(String.Format("[Compressed Raid] {0}: {1}", type, String.Format(temp, args)));
                }
                else
                {
                    Log.Message(String.Format("[Compressed Raid] {0}: {1}", type, String.Format(temp, args)));
                }
            }
            else
            {
                if (type == MessageTypes.Error || type == MessageTypes.DebugError)
                {
                    Log.Error(String.Format("[Compressed Raid] {0}: {1}", type, temp));
                }
                else
                {
                    Log.Message(String.Format("[Compressed Raid] {0}: {1}", type, temp));
                }
            }
#endif
        }
        internal static void SendLog(MessageTypes type, string temp, params object[] args)
        {
            if (!Prefs.DevMode && (byte)type >= 0x04)
            {
                return;
            }
            if (args?.Any() ?? false)
            {
                if (type == MessageTypes.Error || type == MessageTypes.DebugError)
                {
                    Log.Error(String.Format("[Compressed Raid] {0}: {1}", type, String.Format(temp, args)));
                }
                else
                {
                    Log.Message(String.Format("[Compressed Raid] {0}: {1}", type, String.Format(temp, args)));
                }
            }
            else
            {
                if (type == MessageTypes.Error || type == MessageTypes.DebugError)
                {
                    Log.Error(String.Format("[Compressed Raid] {0}: {1}", type, temp));
                }
                else
                {
                    Log.Message(String.Format("[Compressed Raid] {0}: {1}", type, temp));
                }
            }
        }

        public static bool m_CanTranspilerGeneratePawns = true;
        public static bool m_CanTranspilerGenerateAnimals = true;
        public static HashSet<Type> m_AllowPawnGroupKindWorkerTypes = new HashSet<Type>();
        private static void GenerateAnything_Impl(List<Pawn> pawns, int baseNum, int maxPawnNum, bool raidFriendly)
        {
            int enhancePawnNumber = PowerupUtility.GetEnhancePawnNumber(pawns.Count);
            float gainStatValue = PowerupUtility.GetGainStatValue(baseNum, maxPawnNum, enhancePawnNumber, raidFriendly);
            int order = PowerupUtility.GetNewOrder();
            int enhancedCount = 0;

            bool disableFactors = PowerupUtility.DisableFactors();

            for (int i = 0; i < pawns.Count; i++)
            {
                Pawn pawn = pawns[i];
                if (PowerupUtility.EnableEnhancePawn(i, enhancePawnNumber))
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
            }

            //Optionここから
            if (CompressedRaidMod.OptionEnabled() && gainStatValue > 0f)
            {
                //GearRefine追加ここから
                if (CompressedRaidMod.enableRefineGearOptionValue)
                {
                    enhancedCount += GearRefiner.RefineGear(pawns, gainStatValue, enhancePawnNumber);
                }
                //Bionics追加ここから
                if (CompressedRaidMod.enableAddBionicsOptionValue)
                {
                    enhancedCount += BionicsDataStore.AddBionics(pawns, gainStatValue, enhancePawnNumber);
                }
                //Drug追加ここから
                if (CompressedRaidMod.enableAddDrugOptionValue)
                {
                    enhancedCount += DrugHediffDataStore.AddDrugHediffs(pawns, gainStatValue, enhancePawnNumber);
                }
            }
            //Optionここまで

            //DummyForCompatibility除去ここから
            if (MOD_MSER_Active)
            {
                for (int i = 0; i < pawns.Count; i++)
                {
                    Pawn p = pawns[i];
                    CR_DummyForCompatibility dummyHediff = p.health.hediffSet.hediffs.Where(x => x is CR_DummyForCompatibility).Cast<CR_DummyForCompatibility>().FirstOrDefault();
                    if (dummyHediff != null)
                    {
                        p.health.RemoveHediff(dummyHediff);
                    }
                }
            }
            //DummyForCompatibility除去ここまで
            if (CompressedRaidMod.displayMessageValue)
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
        }
        internal static void GeneratePawns_Impl(PawnGroupMakerParms parms, List<Pawn> pawns)
        {
            if ((pawns?.EnumerableNullOrEmpty() ?? true) || parms?.groupKind == null)
            {
                return;
            }
            bool allowedCompress = PatchContinuityHelper.TryGetCompressWork_GeneratePawnsValues(parms.GetHashCode(), out Type pawnGroupWorker, out int baseNum, out int maxPawnNum, out bool raidFriendly) && parms.groupKind.workerClass == pawnGroupWorker;
            if (!allowedCompress)
            {
                return;
            }
            GenerateAnything_Impl(pawns, baseNum, maxPawnNum, raidFriendly);
        }
        internal static void GenerateAnimals_Impl(PawnKindDef animalKind, List<Pawn> pawns)
        {
            if ((pawns?.EnumerableNullOrEmpty() ?? true) || animalKind == null)
            {
                return;
            }
            if (!PatchContinuityHelper.TryGetCompressWork_GenerateAnimalsValues(animalKind.GetHashCode(), out int baseNum, out int maxPawnNum, out bool raidFriendly))
            {
                return;
            }
            GenerateAnything_Impl(pawns, baseNum, maxPawnNum, raidFriendly);
        }
    }
}
