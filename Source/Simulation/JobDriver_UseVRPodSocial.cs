using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace VirtuAwake
{
    public class JobDriver_UseVRPodSocial : JobDriver
    {
        private const int SessionDurationTicks = 3000;

        private Building Pod => this.job?.targetA.Thing as Building;
        private Pawn Partner => this.job?.targetB.Pawn;
        private CompVRPod PodComp => this.Pod?.GetComp<CompVRPod>();

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 2, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOn(() => this.job?.targetA.Thing == null || this.Pod == null);
            this.FailOn(() => PodComp != null && PodComp.HasOtherUser(pawn) && (Partner == null || PodComp.HasOtherUser(Partner)));
            this.AddEndCondition(() =>
                Pod != null && Pod.Spawned ? JobCondition.Ongoing : JobCondition.Incompletable);

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell);

            var immerse = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = SessionDurationTicks,
                handlingFacing = true
            };

            immerse.initAction = () =>
            {
                pawn.pather.StopDead();
                pawn.jobs.posture = PawnPosture.LayingOnGroundFaceUp;
                PodComp?.AddUser(pawn);
            };

            immerse.tickAction = () =>
            {
                if (Pod == null || PodComp == null || Pod.Destroyed)
                {
                    pawn.jobs.curDriver?.EndJobWith(JobCondition.Incompletable);
                    return;
                }

                if (Pod != null)
                {
                    pawn.rotationTracker.FaceCell(Pod.Position);
                }

                if (Partner != null && Partner.Spawned && Partner.Position.InHorDistOf(pawn.Position, 1.5f))
                {
                    pawn.rotationTracker.FaceCell(Partner.Position);
                }

                // Joy handled by CompVRPod after benefit threshold.
            };

            immerse.AddFinishAction(() =>
            {
                pawn.jobs.posture = PawnPosture.Standing;
                if (PodComp != null)
                {
                    SimTypeDef simType = PodComp.ResolveSimTypeFor(pawn);
                    VRSimUtility.TryGiveSimMemory(pawn, simType);
                }
                PodComp?.RemoveUser(pawn);
            });

            immerse.WithProgressBar(TargetIndex.A, () =>
                1f - (immerse.actor.jobs.curDriver.ticksLeftThisToil / (float)SessionDurationTicks));

            yield return immerse;
        }
    }
}
