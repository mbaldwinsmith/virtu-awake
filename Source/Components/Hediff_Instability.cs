using RimWorld;
using UnityEngine;
using Verse;

namespace VirtuAwake
{
    /// <summary>
    /// Represents how thin or unstable the VR experience feels to the pawn.
    /// Severity ranges from 0..1; higher values will later drive glitches and negative effects.
    /// </summary>
    public class Hediff_Instability : HediffWithComps
    {
        private const float DecayPerDay = 0.05f;

        public override bool Visible => true;

        public override void Tick()
        {
            base.Tick();

            if (!InVRSession)
            {
                float decayPerTick = DecayPerDay / GenDate.TicksPerDay;
                this.Severity = Mathf.Max(0f, this.Severity - decayPerTick);

                if (this.Severity <= 0f)
                {
                    this.pawn.health?.RemoveHediff(this);
                }
            }
        }

        private bool InVRSession
        {
            get
            {
                string defName = this.pawn?.CurJobDef?.defName;
                if (string.IsNullOrEmpty(defName))
                {
                    return false;
                }

                return defName == "VA_UseVRPod" || defName == "VA_UseVRPodSocial" || defName == "VA_UseVRPodDeep";
            }
        }
    }
}
