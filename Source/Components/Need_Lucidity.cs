using RimWorld;
using UnityEngine;
using Verse;

namespace VirtuAwake
{
    public class Need_Lucidity : Need
    {
        private const float DecayPerDay = 0.08f;

        public Need_Lucidity(Pawn pawn) : base(pawn)
        {
            this.curLevelInt = 0f;
        }

        protected override bool IsFrozen => !InVRSession;

        public override void NeedInterval()
        {
            if (this.IsFrozen)
            {
                this.CurLevel = 0f;
                return;
            }

            float decayPerInterval = DecayPerDay / GenDate.TicksPerDay * 150f; // NeedInterval is every 150 ticks
            this.CurLevel = Mathf.Clamp01(this.CurLevel - decayPerInterval);
        }

        public override string GetTipString()
        {
            return $"{this.LabelCap}: {this.CurLevelPercentage.ToStringPercent()}";
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

                return defName == "VA_UseVRPod" || defName == "VA_UseVRPodSocial";
            }
        }
    }
}
