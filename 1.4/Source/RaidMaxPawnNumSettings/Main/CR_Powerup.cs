using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace CompressedRaid
{

    [DefOf]
    public static class CR_PowerupDefOf
    {
        public static HediffDef CR_Powerup1;
        public static HediffDef CR_Powerup2;
        public static HediffDef CR_Powerup3;
        public static HediffDef CR_Powerup4;
        public static HediffDef CR_Powerup5;
        public static HediffDef CR_Powerup6;
        public static HediffDef CR_Powerup7;
        public static HediffDef CR_Powerup8;
        public static HediffDef CR_Powerup9;
        public static HediffDef CR_Powerup10;
        public static HediffDef CR_Powerup11;
        public static HediffDef CR_Powerup12;
        public static HediffDef CR_Powerup13;
        public static HediffDef CR_Powerup14;
        public static HediffDef CR_Powerup15;
        public static HediffDef CR_Powerup16;
        public static HediffDef CR_Powerup17;
        public static HediffDef CR_Powerup18;
        public static HediffDef CR_Powerup19;
        public static HediffDef CR_Powerup20;
    }

    [StaticConstructorOnStartup]
    public abstract class CR_Powerup : HediffWithComps
    {
        public override void PostMake()
        {
            base.PostMake();
            m_RaidFriendly = pawn.Faction != null && !FactionUtility.HostileTo(Faction.OfPlayer, pawn.Faction);
        }
        private void RemoveThis()
        {
            this.pawn.health.RemoveHediff(this);
        }

        private bool IsPanicFree()
        {
            bool? work = this.pawn?.mindState?.mentalStateHandler?.CurState?.def == MentalStateDefOf.PanicFlee;
            return work == null ? false : (bool)work;
        }

        private bool IsKidnap()
        {
            bool? work = this.pawn?.CurJob?.def == JobDefOf.Kidnap;
            return work == null ? false : (bool)work;
        }

        private bool IsSteal()
        {
            bool? work = this.pawn?.CurJob?.def == JobDefOf.Steal;
            return work == null ? false : (bool)work;
        }

        public override void Notify_PawnDied()
        {
            RemoveThis();
        }


        public override void Tick()
        {
            base.Tick();

            Pawn pawn = this.pawn;
            if (pawn == null)
            {
                return;
            }

            int tick = Find.TickManager.TicksGame;
            if (m_GameTick == int.MinValue)
            {
                m_GameTick = tick;
            }

            if (Math.Abs(tick - m_GameTick) >= 2)
            {
                RestoreData();
            }

            m_GameTick = tick;

            if (!m_FirstMapSetting && pawn.Map != null)
            {
                m_FirstMapSetting = true;
            }

            if (pawn.Dead || pawn.Downed || (m_FirstMapSetting && pawn.Map == null) || (pawn.Faction != null && pawn.Faction.IsPlayer) || IsPanicFree() || IsKidnap() || IsSteal() || (!m_RaidFriendly && pawn.Faction != null && !FactionUtility.HostileTo(Faction.OfPlayer, pawn.Faction)))
            {
                RemoveThis();
                return;
            }

            if (this.CurStage.painFactor == 1.1f)
            {
                RestoreData();
            }
        }

        public void CreateSaveData()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.CurStage.painFactor);

            foreach (StatModifier sm in this.CurStage.statOffsets)
            {
                sb.Append(",");
                sb.Append("sm:" + sm.stat.defName + ":");
                sb.Append(sm.value);
            }

            foreach (PawnCapacityModifier pcm in this.CurStage.capMods)
            {
                sb.Append(",");
                sb.Append("pcm:" + pcm.capacity.defName + ":");
                sb.Append(pcm.offset);
            }

            this.m_SaveDataField = sb.ToString();
        }

        private void RestoreData()
        {
            if (this.m_SaveDataField == null)
            {
                RemoveThis();
                return;
            }
            this.CurStage.statOffsets = new List<StatModifier>();
            this.CurStage.capMods = new List<PawnCapacityModifier>();
            string[] saveArray = m_SaveDataField.Split(',');
            for (int i = 0; i < saveArray.Count(); i++)
            {
                float value;
                string saveItem = saveArray[i];
                if (i == 0)
                {
                    if (float.TryParse(saveItem, out value))
                    {
                        this.CurStage.painFactor = value;
                    }
                }
                else
                {
                    string[] saveItemArray = saveItem.Split(':');
                    if (saveItemArray[0] == "sm")
                    {
                        if (float.TryParse(saveItemArray[2], out value))
                        {
                            this.CurStage.statOffsets.Add(new StatModifier() { stat = StatDef.Named(saveItemArray[1]), value = value });
                        }
                    }
                    else if (saveItemArray[0] == "pcm")
                    {
                        if (float.TryParse(saveItemArray[2], out value))
                        {
                            this.CurStage.capMods.Add(new PawnCapacityModifier() { capacity = DefDatabase<PawnCapacityDef>.GetNamed(saveItemArray[1]), offset = value });
                        }
                    }
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<string>(ref this.m_SaveDataField, "CR_PowerupSaveDataField", null, false);
            Scribe_Values.Look<bool>(ref this.m_FirstMapSetting, "CR_PowerupFirstMapSetting", false, false);
            Scribe_Values.Look<bool>(ref this.m_RaidFriendly, "CR_PowerupRaidFriendly", false, false);
        }

        public abstract int Order { get; }

        private string m_SaveDataField;
        private bool m_FirstMapSetting;
        private bool m_RaidFriendly;
        private static int m_GameTick = int.MinValue;
    }

    [StaticConstructorOnStartup]
    public class CR_Powerup1 : CR_Powerup
    {
        public override int Order { get { return 1; } }
    }

    [StaticConstructorOnStartup]
    public class CR_Powerup2 : CR_Powerup
    {
        public override int Order { get { return 2; } }
    }

    [StaticConstructorOnStartup]
    public class CR_Powerup3 : CR_Powerup
    {
        public override int Order { get { return 3; } }
    }

    [StaticConstructorOnStartup]
    public class CR_Powerup4 : CR_Powerup
    {
        public override int Order { get { return 4; } }
    }

    [StaticConstructorOnStartup]
    public class CR_Powerup5 : CR_Powerup
    {
        public override int Order { get { return 5; } }
    }

    [StaticConstructorOnStartup]
    public class CR_Powerup6 : CR_Powerup
    {
        public override int Order { get { return 6; } }
    }

    [StaticConstructorOnStartup]
    public class CR_Powerup7 : CR_Powerup
    {
        public override int Order { get { return 7; } }
    }

    [StaticConstructorOnStartup]
    public class CR_Powerup8 : CR_Powerup
    {
        public override int Order { get { return 8; } }
    }

    [StaticConstructorOnStartup]
    public class CR_Powerup9 : CR_Powerup
    {
        public override int Order { get { return 9; } }
    }

    [StaticConstructorOnStartup]
    public class CR_Powerup10 : CR_Powerup
    {
        public override int Order { get { return 10; } }
    }

    [StaticConstructorOnStartup]
    public class CR_Powerup11 : CR_Powerup
    {
        public override int Order { get { return 11; } }
    }

    [StaticConstructorOnStartup]
    public class CR_Powerup12 : CR_Powerup
    {
        public override int Order { get { return 12; } }
    }

    [StaticConstructorOnStartup]
    public class CR_Powerup13 : CR_Powerup
    {
        public override int Order { get { return 13; } }
    }

    [StaticConstructorOnStartup]
    public class CR_Powerup14 : CR_Powerup
    {
        public override int Order { get { return 14; } }
    }

    [StaticConstructorOnStartup]
    public class CR_Powerup15 : CR_Powerup
    {
        public override int Order { get { return 15; } }
    }

    [StaticConstructorOnStartup]
    public class CR_Powerup16 : CR_Powerup
    {
        public override int Order { get { return 16; } }
    }

    [StaticConstructorOnStartup]
    public class CR_Powerup17 : CR_Powerup
    {
        public override int Order { get { return 17; } }
    }

    [StaticConstructorOnStartup]
    public class CR_Powerup18 : CR_Powerup
    {
        public override int Order { get { return 18; } }
    }

    [StaticConstructorOnStartup]
    public class CR_Powerup19 : CR_Powerup
    {
        public override int Order { get { return 19; } }
    }

    [StaticConstructorOnStartup]
    public class CR_Powerup20 : CR_Powerup
    {
        public override int Order { get { return 20; } }
    }

}
