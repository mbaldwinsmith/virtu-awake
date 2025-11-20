using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace VirtuAwake
{
    public class WorkGiver_UseVRPod : WorkGiver_Scanner
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

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t.def != VRPodDef)
            {
                return false;
            }

            if (!pawn.IsColonistPlayerControlled)
            {
                return false;
            }

            if (pawn.Downed || pawn.InMentalState)
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

            if (!pawn.CanReserveAndReach(t, PathEndMode, Danger.None))
            {
                return false;
            }

            var comp = t.TryGetComp<CompVRPod>();
            if (comp != null && comp.CurrentUser != null && comp.CurrentUser != pawn)
            {
                return false;
            }

            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("VA_UseVRPod"), t);
        }
    }
}
