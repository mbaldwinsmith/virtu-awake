using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
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

            FloatMenuOption deepOption = new FloatMenuOption("Deep VR session (long)", () =>
            {
                Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("VA_UseVRPodDeep"), this);
                job.playerForced = true;
                pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            });

            yield return FloatMenuUtility.DecoratePrioritizedTask(deepOption, pawn, this);

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

            CompVRPod comp = this.GetComp<CompVRPod>();
            if (comp == null)
            {
                yield break;
            }

            Pawn activePawn = comp.FindActiveOrPendingUser();

            yield return this.BuildStatusGizmo(comp, activePawn);

            if (activePawn != null)
            {
                yield return this.BuildCancelGizmo(comp, activePawn);

                if (comp.CurrentUser != null)
                {
                    yield return this.BuildEjectGizmo(comp, activePawn);
                }
            }
        }

        private Gizmo BuildStatusGizmo(CompVRPod comp, Pawn pawn)
        {
            string label = pawn != null ? $"VR: {pawn.LabelShortCap}" : "VR pod (idle)";
            return new Command_Action
            {
                defaultLabel = label,
                defaultDesc = this.BuildStatusTooltip(comp, pawn),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/Wait", false) ?? BaseContent.BadTex,
                action = () => { },
                Order = -19f
            };
        }

        private Gizmo BuildCancelGizmo(CompVRPod comp, Pawn pawn)
        {
            return new Command_Action
            {
                defaultLabel = "Cancel session",
                defaultDesc = $"Stop {pawn.LabelShortCap}'s VR job and free this pod.",
                icon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel", true),
                action = () =>
                {
                    if (!comp.TryEndSessionFor(pawn, $"{pawn.LabelShortCap}'s VR session cancelled.", MessageTypeDefOf.TaskCompletion))
                    {
                        Messages.Message("No VR session to cancel.", new LookTargets(this), MessageTypeDefOf.RejectInput, historical: false);
                    }
                },
                Order = -18f
            };
        }

        private Gizmo BuildEjectGizmo(CompVRPod comp, Pawn pawn)
        {
            return new Command_Action
            {
                defaultLabel = "Force eject",
                defaultDesc = $"Interrupt {pawn.LabelShortCap}'s immersion and eject them immediately.",
                icon = ContentFinder<Texture2D>.Get("UI/Designators/Deconstruct", true),
                action = () =>
                {
                    if (!comp.TryEndSessionFor(pawn, $"{pawn.LabelShortCap} ejected from VR pod.", MessageTypeDefOf.NegativeEvent))
                    {
                        Messages.Message("No pawn is currently inside.", new LookTargets(this), MessageTypeDefOf.RejectInput, historical: false);
                    }
                },
                Order = -17f
            };
        }

        private string BuildStatusTooltip(CompVRPod comp, Pawn pawn)
        {
            var sb = new StringBuilder();
            if (pawn != null)
            {
                sb.AppendLine($"User: {pawn.LabelShortCap}");

                SimTypeDef simType = comp.ResolveSimTypeFor(pawn);
                if (simType != null)
                {
                    sb.AppendLine($"Session type: {simType.label}");
                }

                Need_Lucidity lucidity = pawn.needs?.TryGetNeed<Need_Lucidity>();
                if (lucidity != null)
                {
                    sb.AppendLine($"Lucidity: {lucidity.CurLevelPercentage.ToStringPercent()}");
                }

                Hediff inst = pawn.health?.hediffSet?.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamedSilentFail("VA_Instability"));
                if (inst != null)
                {
                    float instPct = Mathf.Clamp01(inst.Severity / inst.def.maxSeverity);
                    sb.AppendLine($"Instability: {instPct.ToStringPercent()}");
                }

                int benefitTicks = comp.TicksUntilBenefit(pawn);
                sb.AppendLine(benefitTicks <= 0
                    ? "Benefits active (joy, XP, lucidity, instability)."
                    : $"Benefits unlock in {FormatSeconds(benefitTicks)}");

                int memoryTicks = comp.TicksUntilMemory(pawn);
                sb.AppendLine(memoryTicks <= 0
                    ? "Next memory roll queued."
                    : $"Next memory roll in {FormatSeconds(memoryTicks)}");
            }
            else
            {
                sb.AppendLine("No active or queued user.");
                sb.AppendLine("Assign a pawn or use the float menu to start a session.");
            }

            return sb.ToString().TrimEnd();
        }

        private static string FormatSeconds(int ticks)
        {
            float seconds = ticks / 60f;
            if (seconds < 1f)
            {
                return "<1s";
            }

            return $"{seconds:0.#}s";
        }
    }
}
