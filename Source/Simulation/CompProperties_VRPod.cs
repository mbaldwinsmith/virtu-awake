using Verse;

namespace VirtuAwake
{
    public class CompProperties_VRPod : CompProperties
    {
        public int tickInterval = 250;

        public CompProperties_VRPod()
        {
            this.compClass = typeof(CompVRPod);
        }
    }
}
