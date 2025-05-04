using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace CompressedRaid
{

    [DefOf]
    public static class CR_DummyForCompatibilityDefOf
    {
        public static HediffDef CR_DummyForCompatibility;
    }

    [StaticConstructorOnStartup]
    public class CR_DummyForCompatibility : HediffWithComps
    {

        private void RemoveThis()
        {
            this.pawn.health.RemoveHediff(this);
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);

            bool surgeryAllowed = ShouldAllowOperations(this.pawn);
            if (surgeryAllowed)
            {
                //Log.Message(String.Format("PostAdd来て条件合致したので消します。:surgeryAllowed={0}", surgeryAllowed));
                RemoveThis();
            }
        }

        public override void Tick()
        {
            base.Tick();
            RemoveThis();
        }

        private static bool ShouldAllowOperations(Pawn pawn)
        {
            return !pawn.Dead && pawn.def.AllRecipes.Any((RecipeDef x) => x.AvailableNow && x.AvailableOnNow(pawn)) && (pawn.Faction == Faction.OfPlayer || (pawn.IsPrisonerOfColony || (pawn.HostFaction == Faction.OfPlayer && !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Moving))) || ((!pawn.RaceProps.IsFlesh || pawn.Faction == null || !pawn.Faction.HostileTo(Faction.OfPlayer)) && (!pawn.RaceProps.Humanlike && pawn.Downed)));
        }
    }
}
