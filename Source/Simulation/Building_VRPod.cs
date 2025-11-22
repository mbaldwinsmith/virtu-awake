using System;
using System.Collections.Generic;
using System.Linq;
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

            foreach (Pawn partner in pawn.Map.mapPawns.FreeColonistsSpawned)
            {
                if (partner == pawn || partner.Downed || partner.InMentalState || partner == null)
                {
                    continue;
                }

                if (!partner.CanReserve(this))
                {
                    continue;
                }

                Building partnerPod = FindAvailableLinkedPod(partner, this);
                if (partnerPod == null)
                {
                    continue;
                }

                var option = new FloatMenuOption($"Social VR (linked pods) with {partner.LabelShortCap}", () =>
                {
                    // Initiator uses this pod; partner uses a linked free pod.
                    var jobA = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("VA_UseVRPod"), this);
                    jobA.playerForced = true;
                    pawn.jobs.TryTakeOrderedJob(jobA, JobTag.Misc);

                    var jobB = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("VA_UseVRPod"), partnerPod);
                    jobB.playerForced = true;
                    partner.jobs.TryTakeOrderedJob(jobB, JobTag.Misc);
                });

                yield return FloatMenuUtility.DecoratePrioritizedTask(option, pawn, this);
            }
        }

        private Building FindAvailableLinkedPod(Pawn user, Building exclude)
        {
            if (user.Map == null)
            {
                return null;
            }

            ThingRequest request = ThingRequest.ForDef(this.def);
            Predicate<Thing> validator = t =>
            {
                if (t == exclude)
                {
                    return false;
                }

                if (t.IsForbidden(user) || !user.CanReserve(t))
                {
                    return false;
                }

                CompVRPod comp = t.TryGetComp<CompVRPod>();
                return comp != null && !comp.CurrentUsers.Any();
            };

            return GenClosest.ClosestThingReachable(user.Position, user.Map, request, PathEndMode.OnCell,
                TraverseParms.For(user), 9999f, validator) as Building;
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos())
            {
                yield return g;
            }
        }
    }
}
