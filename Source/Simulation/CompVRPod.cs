using System.Collections.Generic;
using RimWorld;
using Verse;

namespace VirtuAwake
{
    public class CompVRPod : ThingComp
    {
        private Pawn currentUser;

        public CompProperties_VRPod Props => (CompProperties_VRPod)this.props;

        public Pawn CurrentUser => this.currentUser;

        public override void CompTick()
        {
            base.CompTick();

            if (this.parent.IsHashIntervalTick(this.Props.tickInterval))
            {
                this.TickVR();
            }
        }

        private void TickVR()
        {
            if (this.currentUser == null || !this.currentUser.Spawned || this.currentUser.Dead)
            {
                return;
            }

            // TODO: Hook Lucidity/Instability progression here.
        }

        public void SetUser(Pawn pawn)
        {
            this.currentUser = pawn;
        }

        public override string CompInspectStringExtra()
        {
            if (this.currentUser != null)
            {
                return $"Current user: {this.currentUser.LabelShortCap}";
            }

            return "Idle.";
        }
    }
}
