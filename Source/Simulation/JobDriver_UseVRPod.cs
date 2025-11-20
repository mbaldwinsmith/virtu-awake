using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace VirtuAwake
{
    public class JobDriver_UseVRPod : JobDriver
    {
        private const int ImmersionTicks = 2500;

        private Building Pod => this.job.targetA.Thing as Building;

        private CompVRPod PodComp => this.Pod?.GetComp<CompVRPod>();

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
            this.FailOn(() => this.Pod == null);
            this.FailOn(() => this.PodComp == null);

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);

            var enter = new Toil
            {
                initAction = () => this.PodComp?.SetUser(this.pawn),
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            enter.AddFinishAction(() => this.PodComp?.SetUser(null));
            yield return enter;

            var immerse = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = ImmersionTicks
            };
            immerse.WithProgressBarToilDelay(TargetIndex.A);
            immerse.tickAction = () =>
            {
                if (this.pawn.needs?.joy != null)
                {
                    this.pawn.needs.joy.GainJoy(0.001f, JoyKindDefOf.Meditative);
                }
            };
            immerse.AddFinishAction(() => this.PodComp?.SetUser(null));
            yield return immerse;

            var exit = new Toil
            {
                initAction = () => this.PodComp?.SetUser(null),
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return exit;
        }
    }
}
