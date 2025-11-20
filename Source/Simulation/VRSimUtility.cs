using RimWorld;
using Verse;

namespace VirtuAwake
{
    public static class VRSimUtility
    {
        public static SimTypeDef DefaultSimType => DefDatabase<SimTypeDef>.GetNamedSilentFail("VA_Sim_Recreation");

        public static void ApplySimTraining(Pawn pawn, SimTypeDef simType, int ticks)
        {
            if (pawn?.skills == null)
            {
                return;
            }

            if (simType == null || simType.skillWeights == null || simType.skillWeights.Count == 0)
            {
                return;
            }

            float baseXp = simType.xpPerTick * ticks;
            if (baseXp <= 0f)
            {
                return;
            }

            float totalWeight = 0f;
            foreach (var pair in simType.skillWeights)
            {
                if (pair.Key == null || pair.Value <= 0f)
                {
                    continue;
                }

                totalWeight += pair.Value * PassionFactor(pawn, pair.Key);
            }

            if (totalWeight <= 0f)
            {
                return;
            }

            foreach (var pair in simType.skillWeights)
            {
                if (pair.Key == null || pair.Value <= 0f)
                {
                    continue;
                }

                float weight = pair.Value * PassionFactor(pawn, pair.Key);
                float scaledXp = baseXp * (weight / totalWeight);
                pawn.skills.Learn(pair.Key, scaledXp, direct: true);
            }
        }

        private static float PassionFactor(Pawn pawn, SkillDef skill)
        {
            var passion = pawn.skills?.GetSkill(skill)?.passion ?? Passion.None;
            return passion switch
            {
                Passion.Major => 1.6f,
                Passion.Minor => 1.3f,
                _ => 1f
            };
        }

        public static void TryGiveSimMemory(Pawn pawn, SimTypeDef simType)
        {
            if (pawn == null || simType?.thoughtOnSession == null)
            {
                return;
            }

            pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(simType.thoughtOnSession);
        }
    }
}
