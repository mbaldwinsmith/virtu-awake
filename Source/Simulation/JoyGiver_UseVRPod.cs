using RimWorld;
using Verse;
using Verse.AI;

namespace VirtuAwake
{
    public class JoyGiver_UseVRPod : JoyGiver_InteractBuilding
    {
        protected override bool CanInteractWith(Pawn pawn, Thing t, bool forced = false)
        {
            if (t == null || pawn == null)
            {
                return false;
            }

            if (t.IsForbidden(pawn))
            {
                return false;
            }

            if (!pawn.CanReserveAndReach(t, PathEndMode.InteractionCell, Danger.Some, 1, -1, null, forced))
            {
                return false;
            }

            CompVRPod comp = t.TryGetComp<CompVRPod>();
            if (comp != null && comp.CurrentUser != null && comp.CurrentUser != pawn)
            {
                return false;
            }

            return true;
        }

        protected override Job TryGivePlayJob(Pawn pawn, Thing t)
        {
            if (t == null)
            {
                return null;
            }

            // Use whichever job is specified on the JoyGiverDef.
            if (this.def?.jobDef == null)
            {
                return null;
            }

            return JobMaker.MakeJob(this.def.jobDef, t);
        }
    }
}
