using System.Collections.Generic;
using RimWorld;
using Verse;

namespace VirtuAwake
{
    public class SimTypeDef : Def
    {
        public float xpPerTick = 0.05f;
        public Dictionary<SkillDef, float> skillWeights = new Dictionary<SkillDef, float>();
        public ThoughtDef thoughtOnSession;
        public ThoughtDef thoughtTier1;
        public ThoughtDef thoughtTier2;
        public ThoughtDef thoughtTier3;
        public SkillDef primarySkill;
        public float joyGainTier1 = 0.04f;
        public float joyGainTier2 = 0.07f;
        public float joyGainTier3 = 0.10f;
    }
}
