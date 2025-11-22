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
        public int stabilizationJobDurationTicks = 600;
        public int stabilizationCooldownTicks = 3600;
        public float stabilizationSeverityReductionPerTick = 0.0002f;
        public float stabilizationMaxSeverityReductionPerJob = 0.22f;
        public float stabilizationXpPerTick = 0.08f;
        public float stabilizationMinInstability = 0.08f;
        public int stabilizationLongSessionTicks = 1800;

        public CompProperties_VRPod()
        {
            this.compClass = typeof(CompVRPod);
        }
    }
}
