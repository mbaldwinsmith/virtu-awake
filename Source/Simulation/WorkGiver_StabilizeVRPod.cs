using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace VirtuAwake
{
    public class WorkGiver_StabilizeVRPod : WorkGiver_Scanner
    {
        private static readonly ThingDef VRPodDef = DefDatabase<ThingDef>.GetNamed("VA_VRPod_Basic");

        public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            if (pawn.Map == null)
            {
                yield break;
            }

            foreach (Thing thing in pawn.Map.listerThings.ThingsOfDef(VRPodDef))
            {
                yield return thing;
            }
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return pawn.Map == null || pawn.WorkTypeIsDisabled(WorkTypeDefOf.Research);
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t.def != VRPodDef)
            {
                return false;
            }

            if (pawn.Downed || pawn.InMentalState)
            {
                return false;
            }

            if (pawn.WorkTypeIsDisabled(WorkTypeDefOf.Research))
            {
                return false;
            }

            if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                return false;
            }

            if (t.IsForbidden(pawn))
            {
                return false;
            }

            if (!pawn.CanReserveAndReach(t, PathEndMode, Danger.Some, 1, -1, null, ignoreOtherReservations: true))
            {
                return false;
            }

            CompVRPod comp = t.TryGetComp<CompVRPod>();
            if (comp == null)
            {
                return false;
            }

            return comp.TryGetStabilizationTarget(pawn, forced, out _, out _);
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            CompVRPod comp = t.TryGetComp<CompVRPod>();
            Pawn occupant = comp?.CurrentUser;

            Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("VA_StabilizeVRPod"), t, occupant);
            job.count = comp?.StabilizationJobDuration ?? 600;
            job.playerForced = forced;
            return job;
        }
    }
}
