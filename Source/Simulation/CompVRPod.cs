using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace VirtuAwake
{
    public class CompVRPod : ThingComp
    {
        private readonly HashSet<Pawn> currentUsers = new HashSet<Pawn>();
        private readonly Dictionary<Pawn, SimTypeDef> sessionSimTypes = new Dictionary<Pawn, SimTypeDef>();

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

            foreach (Pawn pawn in this.currentUsers.ToList())
            {
                if (pawn == null || pawn.Dead || !pawn.Spawned)
                {
                    this.currentUsers.Remove(pawn);
                    this.sessionSimTypes.Remove(pawn);
                    continue;
                }

                SimTypeDef simType = this.ResolveSimTypeFor(pawn);
                VRSimUtility.ApplySimTraining(pawn, simType, this.Props.tickInterval);
                // TODO: Hook Lucidity/Instability progression here.
            }
        }

        public void SetUser(Pawn pawn)
        {
            this.currentUsers.Clear();
            this.sessionSimTypes.Clear();
            if (pawn != null)
            {
                this.currentUsers.Add(pawn);
                this.ResolveSimTypeFor(pawn);
            }
        }

        public void AddUser(Pawn pawn)
        {
            if (pawn != null)
            {
                this.currentUsers.Add(pawn);
                this.ResolveSimTypeFor(pawn);
            }
        }

        public void RemoveUser(Pawn pawn)
        {
            if (pawn != null)
            {
                this.currentUsers.Remove(pawn);
                this.sessionSimTypes.Remove(pawn);
            }
        }

        public bool HasOtherUser(Pawn pawn)
        {
            return this.currentUsers.Any() && this.currentUsers.Any(u => u != pawn);
        }

        public SimTypeDef ResolveSimTypeFor(Pawn pawn)
        {
            if (pawn == null)
            {
                return this.Props.simType ?? VRSimUtility.DefaultSimType;
            }

            if (this.sessionSimTypes.TryGetValue(pawn, out var sim))
            {
                return sim;
            }

            SimTypeDef chosen = VRSimUtility.ResolveBestSimTypeForPawn(pawn, this.Props.simType);
            this.sessionSimTypes[pawn] = chosen;
            return chosen;
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
