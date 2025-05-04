using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace CompressedRaid
{
    [StaticConstructorOnStartup]
    public class MustNegativeEffectDef : Def 
    {
        //weak reference by defName string
        public List<string> drugs;
    }
    [StaticConstructorOnStartup]
    public class ExcludeDrugDef : Def
    {
        //weak reference by defName string
        public List<string> drugs;
    }
}
