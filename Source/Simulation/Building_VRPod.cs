using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace VirtuAwake
{
    public class Building_VRPod : Building
    {
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn pawn)
        {
            foreach (FloatMenuOption option in base.GetFloatMenuOptions(pawn))
            {
                yield return option;
            }

            if (pawn == null || !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                yield break;
            }

            if (this.IsForbidden(pawn))
            {
                yield return new FloatMenuOption("ForbiddenLower".Translate(), null);
                yield break;
            }

            if (!pawn.CanReserveAndReach(this, PathEndMode.InteractionCell, Danger.Some))
            {
                yield return new FloatMenuOption("CannotReach".Translate(), null);
                yield break;
            }

            CompVRPod podComp = this.GetComp<CompVRPod>();
            if (podComp != null && podComp.CurrentUser != null && podComp.CurrentUser != pawn)
            {
                yield return new FloatMenuOption("InUseLower".Translate(podComp.CurrentUser.LabelShort), null);
                yield break;
            }

            if (pawn.Downed || pawn.InMentalState)
            {
                yield return new FloatMenuOption("CannotUseReason".Translate("Incapable".Translate()), null);
                yield break;
            }

            FloatMenuOption useOption = new FloatMenuOption("Use VR pod (recreation)", () =>
            {
                Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("VA_UseVRPod"), this);
                job.playerForced = true;
                pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            });

            yield return FloatMenuUtility.DecoratePrioritizedTask(useOption, pawn, this);
        }
    }
}
