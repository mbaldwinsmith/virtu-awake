using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace VirtuAwake
{
    public class JobDriver_StabilizeVRPod : JobDriver
    {
        private const TargetIndex PodInd = TargetIndex.A;
        private const TargetIndex OccupantInd = TargetIndex.B;

        private Building Pod => this.job?.GetTarget(PodInd).Thing as Building;
        private CompVRPod PodComp => this.Pod?.GetComp<CompVRPod>();
        private Pawn Occupant => this.job?.GetTarget(OccupantInd).Pawn;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(PodInd);
            this.FailOn(() => this.PodComp == null);
            this.FailOn(() => this.pawn.WorkTypeIsDisabled(WorkTypeDefOf.Research));
            this.FailOn(() => this.Occupant == null || this.Occupant.Dead || this.Occupant.Map != this.pawn.Map);
            this.FailOn(() => !VRSessionTracker.IsInVR(this.Occupant));
            this.FailOn(() => this.PodComp.CurrentUser != this.Occupant);

            yield return Toils_Goto.GotoThing(PodInd, PathEndMode.InteractionCell);

            int duration = this.PodComp?.StabilizationJobDuration ?? 600;
            if (this.job != null && this.job.count > 0)
            {
                duration = this.job.count;
            }

            float totalReduction = 0f;
            var stabilize = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = duration
            };

            stabilize.WithProgressBarToilDelay(PodInd);
            stabilize.tickAction = () =>
            {
                if (this.PodComp == null)
                {
                    this.EndJobWith(JobCondition.Incompletable);
                    return;
                }

                float reduced = this.PodComp.ApplyStabilizationTick(this.pawn, this.Occupant);
                totalReduction += reduced;

                if (totalReduction >= (this.PodComp.Props?.stabilizationMaxSeverityReductionPerJob ?? 0.22f))
                {
                    this.ReadyForNextToil();
                }
            };
            stabilize.AddFinishAction(() => this.PodComp?.MarkStabilized(this.Occupant));
            stabilize.activeSkill = () => SkillDefOf.Intellectual;

            yield return stabilize;
        }
    }
}
