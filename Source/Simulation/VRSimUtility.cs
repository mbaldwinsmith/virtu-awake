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

            foreach (var pair in simType.skillWeights)
            {
                if (pair.Key == null || pair.Value <= 0f)
                {
                    continue;
                }

                pawn.skills.Learn(pair.Key, baseXp * pair.Value, direct: true);
            }
        }
    }
}
