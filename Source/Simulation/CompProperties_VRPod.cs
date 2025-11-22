using Verse;

namespace VirtuAwake
{
    public class CompProperties_VRPod : CompProperties
    {
        public int tickInterval = 200;
        public SimTypeDef simType;
        public float lucidityGainPerTick = 0.0015f;
        public float lucidityGainLongTermMultiplier = 1.25f;
        public float instabilityGainPerTick = 0.0022f;
        public float instabilityGainLongTermMultiplier = 1.4f;
        public float instabilityGainHighLucidityPerTick = 0.003f;
        public float instabilityHighLucidityThreshold = 0.55f;
        public float instabilityDecayPerTick = 0.00035f;
        public int memoryIntervalTicks = 1400;
        public int minimumBenefitTicks = 900;
        public float joyGainPerTick = 0.017f;
        public float joyLongTermMultiplier = 1.0f;
        public float restGainPerTickLongTerm = 0.0008f;

        public CompProperties_VRPod()
        {
            this.compClass = typeof(CompVRPod);
        }
    }
}
