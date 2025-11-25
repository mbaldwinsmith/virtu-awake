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

            if (!pawn.CanReach(this, PathEndMode.InteractionCell, Danger.Some))
            {
                yield return new FloatMenuOption("CannotReach".Translate(), null);
                yield break;
            }

            if (pawn.Downed || pawn.InMentalState)
            {
                yield return new FloatMenuOption("CannotUseReason".Translate("Incapable".Translate()), null);
                yield break;
            }

            CompVRPod podComp = this.GetComp<CompVRPod>();
            Pawn activeUser = podComp?.CurrentUser;
            bool inUseByOther = podComp != null && activeUser != null && activeUser != pawn;

            if (inUseByOther)
            {
                yield return new FloatMenuOption("InUseLower".Translate(activeUser.LabelShort), null);
            }

            bool canReserveForVR = pawn.CanReserveAndReach(this, PathEndMode.InteractionCell, Danger.Some);

            if (!inUseByOther && canReserveForVR)
            {
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
            }
            else if (!inUseByOther && !canReserveForVR)
            {
                yield return new FloatMenuOption("CannotReserve".Translate(), null);
            }

            string failReason = null;
            Pawn stabilizeTarget = null;
            bool canStabilize = podComp != null && podComp.TryGetStabilizationTarget(pawn, true, out stabilizeTarget, out failReason);
            if (canStabilize)
            {
                if (pawn.CanReserve(this, 1, -1, null, ignoreOtherReservations: true))
                {
                    var stabilizeOption = new FloatMenuOption("Stabilise simulation (Research)", () =>
                    {
                        Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("VA_StabilizeVRPod"), this, stabilizeTarget);
                        job.count = podComp.StabilizationJobDuration;
                        job.playerForced = true;
                        pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    });

                    yield return FloatMenuUtility.DecoratePrioritizedTask(stabilizeOption, pawn, this);
                }
                else
                {
                    yield return new FloatMenuOption("Reserved".Translate(), null);
                }
            }
            else if (activeUser != null && !string.IsNullOrEmpty(failReason))
            {
                yield return new FloatMenuOption($"Cannot stabilise: {failReason}", null);
            }
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

            AppendNetworkInfo(comp, sb);

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

        private void AppendNetworkInfo(CompVRPod comp, StringBuilder sb)
        {
            CompPowerTrader power = this.GetComp<CompPowerTrader>();
            PowerNet net = power?.PowerNet;
            if (net == null)
            {
                sb.AppendLine("Network: none (no power link).");
                return;
            }

            int podCount = 0;
            foreach (CompPower compPower in net.transmitters)
            {
                Building building = compPower?.parent as Building;
                if (building?.GetComp<CompVRPod>() != null)
                {
                    podCount++;
                }
            }

            var mapPawns = this.Map?.mapPawns?.AllPawnsSpawned;
            var networkUsers = new List<string>();
            if (mapPawns != null)
            {
                foreach (Pawn p in mapPawns)
                {
                    if (p == null || !VRSessionTracker.IsInVR(p))
                    {
                        continue;
                    }

                    if (VRSessionTracker.GetPowerNet(p) == net)
                    {
                        networkUsers.Add(p.LabelShortCap);
                    }
                }
            }

            string users = networkUsers.Count > 0 ? string.Join(", ", networkUsers) : "none";
            sb.AppendLine($"Network: {podCount} pod(s); VR users on net: {users}");
        }
    }
}
