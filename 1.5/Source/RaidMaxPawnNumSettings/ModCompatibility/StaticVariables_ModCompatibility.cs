using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CompressedRaid
{
    public static class StaticVariables_ModCompatibility
    {
        //other mods compatibility section
        //MSER
        public const string MOD_MSER_ID = "Abraxas.MSERevived";
        public const string MOD_MSER_HarmonyID = "OrenoMSE";
        public static readonly bool MOD_MSER_PatchAllowed = false; // 元MODの動作に変更を加えるのでMSER作者に許可をとりつけたらtrueにする
        public static bool MOD_MSER_Active = false;
        public const string MOD_MSER_Patch1_TypeName = "OrenoMSE.MSE_VanillaExtender";
        public const string MOD_MSER_Patch1_MethodName = "HediffApplyHediffs";
        public static readonly Type[] MOD_MSER_Patch1_ArgumentsTypes = new Type[] { typeof(Hediff), typeof(Pawn), typeof(BodyPartRecord) };

        //NilchEi's Powerful Psycast AI
        public const string MOD_PowerfulPsycastAI_ID = "nilchei.powerfulpsycastai";
        public static readonly bool MOD_PowerfulPsycastAI_PatchAllowed = true;// 元MODの動作に変更を加えないのでひとまずtrueにするが、お咎めがあったらfalseにする。
        public static bool MOD_PowerfulPsycastAI_Active = false;

        //NoMoreRelationShip禁止袭击生成亲戚
        public const string MOD_NoMoreRelationShip_ID = "None1637.NoMoreRelationShip";
        public static readonly bool MOD_NoMoreRelationShip_PatchAllowed = true;// 元MODの動作に変更を加えるが排他的な動作ではないのでひとまずtrueにするが、お咎めがあったらfalseにする。
        public static bool MOD_NoMoreRelationShip_Active = false;
        public const string MOD_NoMoreRelationShip_Patch1_TypeName = "NoMoreRelative.PawnGroupKindWorker_Normal_Patch";
        public const string MOD_NoMoreRelationShip_Patch1_MethodName = "Prefix";
        public static readonly Type[] MOD_NoMoreRelationShip_Patch1_ArgumentsTypes = new Type[] { typeof(PawnGroupMakerParms), typeof(PawnGroupMaker), typeof(List<Pawn>), typeof(bool) };
    }
}
