using Verse;

namespace VirtuAwake
{
    public class CompProperties_VRPod : CompProperties
    {
        public int tickInterval = 250;
        public SimTypeDef simType;
        public float lucidityGainPerTick = 0.0006f;
        public float lucidityGainLongTermMultiplier = 1.25f;
        public float instabilityGainPerTick = 0.0002f;
        public float instabilityGainLongTermMultiplier = 1.4f;
        public float instabilityGainHighLucidityPerTick = 0.00012f;
        public float instabilityHighLucidityThreshold = 0.6f;
        public float instabilityDecayPerTick = 0.00008f;
        public int memoryIntervalTicks = 1200;
        public int minimumBenefitTicks = 900;

        public CompProperties_VRPod()
        {
            this.compClass = typeof(CompVRPod);
        }
    }
}
