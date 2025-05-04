using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace CompressedRaid
{
    [StaticConstructorOnStartup]
    public class AllowPawnGroupKindWorkerTypeDef : Def 
    {
        public List<string> workerTypeNames;
    }
}
