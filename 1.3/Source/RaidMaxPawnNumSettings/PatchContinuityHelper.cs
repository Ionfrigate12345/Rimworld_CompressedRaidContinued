using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CompressedRaid
{
    internal static class PatchContinuityHelper
    {
        struct CompressWork
        {
            public bool allowedCompress;
            public int hashKey;
            public Type pawnGroupWorker;
            public int baseNum;
            public int maxPawnNum;
            public bool raidFriendly;
            public CompressWork(int hashKey, int baseNum, int maxPawnNum, bool raidFriendly = false)
            {
                this.allowedCompress = true;
                this.hashKey = hashKey;
                this.baseNum = baseNum;
                this.maxPawnNum = maxPawnNum;
                this.raidFriendly = raidFriendly;
                pawnGroupWorker = null;
            }
            public CompressWork(int hashKey, Type pawnGroupWorker, int baseNum, int maxPawnNum, bool raidFriendly = false)
            {
                this.allowedCompress = true;
                this.hashKey = hashKey;
                this.baseNum = baseNum;
                this.maxPawnNum = maxPawnNum;
                this.raidFriendly = raidFriendly;
                this.pawnGroupWorker = pawnGroupWorker;
            }
        }
        private static CompressWork m_CompressWork_GeneratePawns = default(CompressWork);
        private static CompressWork m_CompressWork_GenerateAnimals = default(CompressWork);

        internal static IEnumerable<PawnGenOption> SetCompressWork_GeneratePawns(IEnumerable<PawnGenOption> options, PawnGroupMakerParms groupParms)
        {
            if (options.EnumerableNullOrEmpty() || groupParms == null)
            {
                return StateFalse(options);
            }
            if (CompressedRaidMod.ModDisabled())
            {
                return StateFalse(options);
            }
            if (!General.m_AllowPawnGroupKindWorkerTypes.Contains(groupParms.groupKind.workerClass))
            {
                return StateFalse(options);
            }
            if (!CompressedRaidMod.AllowCompress(groupParms, out bool raidFriendly))
            {
                return StateFalse(options);
            }
            int maxPawnNum = CompressedRaidMod.maxRaidPawnsCountValue, pawnCount = 0;
            int baseNum = options.Count();
            if (maxPawnNum >= baseNum)
            {
                return StateFalse(options);
            }

            bool allowedCompress = true;
            if (!CompressedRaidMod.allowMechanoidsValue && !options.Any(x => !(x.kind?.RaceProps?.IsMechanoid ?? false)))
            {
                allowedCompress = false;
            }
            if (!CompressedRaidMod.allowInsectoidsValue && !options.Any(x => !(x.kind?.RaceProps?.FleshType == FleshTypeDefOf.Insectoid)))
            {
                allowedCompress = false;
            }

            List<PawnGenOption> list = new List<PawnGenOption>();
            foreach (PawnGenOption option in options.OrderByDescending(x=> x.Cost))
            {
                if (allowedCompress && pawnCount >= maxPawnNum)
                {
                    break;
                }
                pawnCount++;
                list.Add(option);
            }

            if (allowedCompress)
            {
                m_CompressWork_GeneratePawns = new CompressWork(groupParms.GetHashCode(), groupParms.groupKind.workerClass, baseNum, maxPawnNum, raidFriendly);
#if DEBUG
                if (groupParms.traderKind != null)
                {
                    Log.Message(String.Format("@@@ Compressed Raid: WORKER_CLASS={0}, BASE_NUM={1}, COMPRESSED_NUM={2}, TRADER_KIND={3}", groupParms.groupKind.workerClass, baseNum, maxPawnNum, groupParms.traderKind));
                }
#endif
                return list;
            }
            return StateFalse(options);

            IEnumerable<PawnGenOption> StateFalse(IEnumerable<PawnGenOption> ret)
            {
                m_CompressWork_GeneratePawns.allowedCompress = false;
                return ret;
            }
        }
        internal static void SetCompressWork_GeneratePawns(PawnGroupMakerParms groupParms, ref IEnumerable<PawnGenOption> __result)
        {
            if ((__result?.EnumerableNullOrEmpty() ?? true) || groupParms == null)
            {
                m_CompressWork_GeneratePawns.allowedCompress = false;
                return;
            }
            if (CompressedRaidMod.ModDisabled())
            {
                m_CompressWork_GeneratePawns.allowedCompress = false;
                return;
            }
            if (!General.m_AllowPawnGroupKindWorkerTypes.Contains(groupParms.groupKind.workerClass))
            {
                m_CompressWork_GeneratePawns.allowedCompress = false;
                return;
            }
            if (!CompressedRaidMod.AllowCompress(groupParms, out bool raidFriendly))
            {
                m_CompressWork_GeneratePawns.allowedCompress = false;
                return;
            }
            int maxPawnNum = CompressedRaidMod.maxRaidPawnsCountValue, pawnCount = 0;
            int baseNum = __result.Count();
            if (maxPawnNum >= baseNum)
            {
                m_CompressWork_GeneratePawns.allowedCompress = false;
                return;
            }

            bool allowedCompress = true;
            if (!CompressedRaidMod.allowMechanoidsValue && !__result.Any(x => !(x.kind?.RaceProps?.IsMechanoid ?? false)))
            {
                allowedCompress = false;
            }
            if (!CompressedRaidMod.allowInsectoidsValue && !__result.Any(x => !(x.kind?.RaceProps?.FleshType == FleshTypeDefOf.Insectoid)))
            {
                allowedCompress = false;
            }

            List<PawnGenOption> list = new List<PawnGenOption>();
            foreach (PawnGenOption option in __result.OrderByDescending(x => x.Cost))
            {
                if (allowedCompress && pawnCount >= maxPawnNum)
                {
                    break;
                }
                pawnCount++;
                list.Add(option);
            }

            if (allowedCompress)
            {
                m_CompressWork_GeneratePawns = new CompressWork(groupParms.GetHashCode(), groupParms.groupKind.workerClass, baseNum, maxPawnNum, raidFriendly);
                __result = list;
#if DEBUG
                if (groupParms.traderKind != null)
                {
                    Log.Message(String.Format("@@@ Compressed Raid: WORKER_CLASS={0}, BASE_NUM={1}, COMPRESSED_NUM={2}, TRADER_KIND={3}", groupParms.groupKind.workerClass, baseNum, maxPawnNum, groupParms.traderKind));
                }
#endif
            }
        }

        internal static int SetCompressWork_GenerateAnimals(PawnKindDef animalKind, int baseNum)
        {
            if (animalKind == null || baseNum <= 0)
            {
                return StateFalse(baseNum);
            }
            if (CompressedRaidMod.ModDisabled() || !CompressedRaidMod.allowManhuntersValue)
            {
                return StateFalse(baseNum);
            }
            int maxPawnNum = CompressedRaidMod.maxRaidPawnsCountValue;
            if (maxPawnNum >= baseNum)
            {
                return StateFalse(baseNum);
            }
            if (!CompressedRaidMod.allowMechanoidsValue && (animalKind?.RaceProps?.IsMechanoid ?? false))
            {
                return StateFalse(baseNum);
            }
            if (!CompressedRaidMod.allowInsectoidsValue && animalKind?.RaceProps?.FleshType == FleshTypeDefOf.Insectoid)
            {
                return StateFalse(baseNum);
            }

            int hashKey = animalKind.GetHashCode();
            m_CompressWork_GenerateAnimals = new CompressWork(hashKey, baseNum, maxPawnNum);
#if DEBUG
            Log.Message(String.Format("@@@ Compressed Raid: ANIMAL_KIND={0}, BASE_NUM={1}, COMPRESSED_NUM={2}", animalKind.LabelCap, baseNum, maxPawnNum));
#endif
            return maxPawnNum;
            
            int StateFalse(int ret)
            {
                m_CompressWork_GenerateAnimals.allowedCompress = false;
                return ret;
            }
        }
        
        internal static bool TryGetCompressWork_GeneratePawnsValues(int hashKey, out Type pawnGroupWorker, out int baseNum, out int maxPawnNum, out bool raidFriendly)
        {
            if (!m_CompressWork_GeneratePawns.allowedCompress || hashKey != m_CompressWork_GeneratePawns.hashKey)
            {
                baseNum = 0;
                maxPawnNum = 0;
                raidFriendly = false;
                pawnGroupWorker = null;
                return false;
            }
            baseNum = m_CompressWork_GeneratePawns.baseNum;
            maxPawnNum = m_CompressWork_GeneratePawns.maxPawnNum;
            raidFriendly = m_CompressWork_GeneratePawns.raidFriendly;
            pawnGroupWorker = m_CompressWork_GeneratePawns.pawnGroupWorker;
            return true;
        }
        internal static bool TryGetCompressWork_GenerateAnimalsValues(int hashKey, out int baseNum, out int maxPawnNum, out bool raidFriendly)
        {
            if (!m_CompressWork_GenerateAnimals.allowedCompress || hashKey != m_CompressWork_GenerateAnimals.hashKey)
            {
                baseNum = 0;
                maxPawnNum = 0;
                raidFriendly = false;
                return false;
            }
            baseNum = m_CompressWork_GenerateAnimals.baseNum;
            maxPawnNum = m_CompressWork_GenerateAnimals.maxPawnNum;
            raidFriendly = m_CompressWork_GenerateAnimals.raidFriendly;
            return true;
        }
    }
}
