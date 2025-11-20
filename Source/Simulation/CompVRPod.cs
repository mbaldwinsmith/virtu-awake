using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace VirtuAwake
{
    public class CompVRPod : ThingComp
    {
        private readonly HashSet<Pawn> currentUsers = new HashSet<Pawn>();

        public CompProperties_VRPod Props => (CompProperties_VRPod)this.props;

        public Pawn CurrentUser => this.currentUsers.Count > 0 ? this.currentUsers.First() : null;

        public IReadOnlyCollection<Pawn> CurrentUsers => this.currentUsers;

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
            if (this.currentUsers.Count == 0)
            {
                return;
            }

            // TODO: Hook Lucidity/Instability progression here.
        }

        public void SetUser(Pawn pawn)
        {
            this.currentUsers.Clear();
            if (pawn != null)
            {
                this.currentUsers.Add(pawn);
            }
        }

        public void AddUser(Pawn pawn)
        {
            if (pawn != null)
            {
                this.currentUsers.Add(pawn);
            }
        }

        public void RemoveUser(Pawn pawn)
        {
            if (pawn != null)
            {
                this.currentUsers.Remove(pawn);
            }
        }

        public bool HasOtherUser(Pawn pawn)
        {
            return this.currentUsers.Any() && this.currentUsers.Any(u => u != pawn);
        }

        public override string CompInspectStringExtra()
        {
            if (this.currentUsers.Count > 0)
            {
                return $"Current user(s): {string.Join(", ", this.currentUsers.Select(u => u.LabelShortCap))}";
            }

            return "Idle.";
        }
    }
}
