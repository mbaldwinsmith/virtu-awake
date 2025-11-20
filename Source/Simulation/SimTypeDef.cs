using System.Collections.Generic;
using RimWorld;
using Verse;

namespace VirtuAwake
{
    public class SimTypeDef : Def
    {
        public float xpPerTick = 0.05f;
        public Dictionary<SkillDef, float> skillWeights = new Dictionary<SkillDef, float>();
    }
}
