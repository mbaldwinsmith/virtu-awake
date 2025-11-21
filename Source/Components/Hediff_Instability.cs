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
        public override bool Visible => InVRSession;

        public override void Tick()
        {
            base.Tick();

            if (!InVRSession)
            {
                // Outside VR, instability should not apply; remove the hediff to keep health tidy.
                this.pawn.health?.RemoveHediff(this);
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

                return defName == "VA_UseVRPod" || defName == "VA_UseVirtuDreamPod" || defName == "VA_UseVRPodSocial";
            }
        }
    }
}
