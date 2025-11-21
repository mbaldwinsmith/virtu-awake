using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace VirtuAwake
{
    public class JobDriver_UseVirtuDreamPod : JobDriver
    {
        private Building Pod => this.job.targetA.Thing as Building;
        private CompVRPod PodComp => Pod?.GetComp<CompVRPod>();

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOn(() => PodComp != null && PodComp.CurrentUser != null && PodComp.CurrentUser != pawn);
            this.AddEndCondition(() => Pod != null && Pod.Spawned ? JobCondition.Ongoing : JobCondition.Incompletable);

            // 1. Move onto the pod footprint
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell);

            // 2. Lie inside and remain immersed
            var immerse = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Never,
                handlingFacing = true
            };

            immerse.initAction = () =>
            {
                pawn.pather.StopDead();
                pawn.jobs.posture = PawnPosture.LayingInBed;
                PodComp?.SetUser(pawn);
            };

            immerse.tickAction = () =>
            {
                bool canBenefit = PodComp != null && PodComp.CanProvideBenefits(pawn);

                if (Pod != null)
                {
                    pawn.rotationTracker.FaceCell(Pod.Position);
                }

                // Joy/rest handled by CompVRPod after benefit threshold.
            };

            immerse.AddFinishAction(() =>
            {
                pawn.jobs.posture = PawnPosture.Standing;
                PodComp?.SetUser(null);
            });

            yield return immerse;
        }
    }
}
