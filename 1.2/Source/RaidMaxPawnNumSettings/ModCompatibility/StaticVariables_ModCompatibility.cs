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
        //public const string MOD_PowerfulPsycastAI_Patch1_TypeName = "PowerfulEmpire.PawnGroupKindWorker_Psycast";
        //public const string MOD_PowerfulPsycastAI_Patch1_MethodName = "GeneratePawns";
        //public static readonly Type[] MOD_PowerfulPsycastAI_Patch1_ArgumentsTypes = new Type[] { typeof(PawnGroupMakerParms), typeof(PawnGroupMaker), typeof(List<Pawn>), typeof(bool) };
    }
}
