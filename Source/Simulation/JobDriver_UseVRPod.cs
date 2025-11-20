using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace VirtuAwake
{
    public class JobDriver_UseVRPod : JobDriver
    {
        private const int SessionDurationTicks = 2500;

        private Building Pod => this.job.targetA.Thing as Building;
        private CompVRPod PodComp => Pod?.GetComp<CompVRPod>();

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOn(() => PodComp != null && PodComp.HasOtherUser(pawn));
            this.AddEndCondition(() =>
                Pod != null && Pod.Spawned ? JobCondition.Ongoing : JobCondition.Incompletable);

            // 1. Go stand on the pod's cell (root cell)
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell);

            // 2. Lie down on the pod and "immerse"
            var lieDown = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = SessionDurationTicks,
                handlingFacing = true
            };

            lieDown.initAction = () =>
            {
                // Stop moving, lie down on the pod
                pawn.pather.StopDead();
                pawn.jobs.posture = PawnPosture.LayingOnGroundFaceUp;
                PodComp?.SetUser(pawn);
            };

            lieDown.tickAction = () =>
            {
                // Keep facing the pod's rotation (optional)
                if (Pod != null)
                {
                    pawn.rotationTracker.FaceCell(Pod.Position);
                }

                // Simple recreation gain while lying in the pod
                var joy = pawn.needs?.joy;
                if (joy != null)
                {
                    joy.GainJoy(0.0005f, JoyKindDefOf.Meditative);
                    if (joy.CurLevel >= 0.999f)
                    {
                        pawn.jobs.curDriver?.EndJobWith(JobCondition.Succeeded);
                    }
                }

                if (PodComp?.Props?.simType != null)
                {
                    VRSimUtility.ApplySimTraining(pawn, PodComp.Props.simType, 1);
                }
            };

            lieDown.AddFinishAction(() =>
            {
                // Stand up and clear user
                pawn.jobs.posture = PawnPosture.Standing;
                PodComp?.SetUser(null);
            });

            lieDown.WithProgressBar(TargetIndex.A, () =>
                1f - (lieDown.actor.jobs.curDriver.ticksLeftThisToil / (float)SessionDurationTicks));

            yield return lieDown;
        }
    }
}
