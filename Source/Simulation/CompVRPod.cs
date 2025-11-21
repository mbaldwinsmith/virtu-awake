using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace VirtuAwake
{
    public class CompVRPod : ThingComp
    {
        private readonly HashSet<Pawn> currentUsers = new HashSet<Pawn>();
        private readonly Dictionary<Pawn, SimTypeDef> sessionSimTypes = new Dictionary<Pawn, SimTypeDef>();
        private readonly Dictionary<Pawn, int> memoryTimers = new Dictionary<Pawn, int>();

        public CompProperties_VRPod Props => (CompProperties_VRPod)this.props;

        public Pawn CurrentUser => this.currentUsers.Count > 0 ? this.currentUsers.First() : null;

        public IReadOnlyCollection<Pawn> CurrentUsers => this.currentUsers;

        public override void CompTick()
        {
            base.CompTick();

            if (this.parent.IsHashIntervalTick(this.Props.tickInterval))
            {
                this.TickVR();
            }
        }

        private void TickVR()
        {
            if (this.currentUsers.Count == 0)
            {
                return;
            }

            foreach (Pawn pawn in this.currentUsers.ToList())
            {
                if (pawn == null || pawn.Dead || !pawn.Spawned)
                {
                    this.currentUsers.Remove(pawn);
                    this.sessionSimTypes.Remove(pawn);
                    this.memoryTimers.Remove(pawn);
                    continue;
                }

                SimTypeDef simType = this.ResolveSimTypeFor(pawn);
                VRSimUtility.ApplySimTraining(pawn, simType, this.Props.tickInterval);
                ApplyLucidityAndInstability(pawn);
                TickMemories(pawn, simType);
            }
        }

        public void SetUser(Pawn pawn)
        {
            this.currentUsers.Clear();
            this.sessionSimTypes.Clear();
            this.memoryTimers.Clear();
            if (pawn != null)
            {
                this.currentUsers.Add(pawn);
                this.ResolveSimTypeFor(pawn);
            }
        }

        public void AddUser(Pawn pawn)
        {
            if (pawn != null)
            {
                this.currentUsers.Add(pawn);
                this.ResolveSimTypeFor(pawn);
                this.memoryTimers[pawn] = 0;
            }
        }

        public void RemoveUser(Pawn pawn)
        {
            if (pawn != null)
            {
                this.currentUsers.Remove(pawn);
                this.sessionSimTypes.Remove(pawn);
                this.memoryTimers.Remove(pawn);
            }
        }

        public bool HasOtherUser(Pawn pawn)
        {
            return this.currentUsers.Any() && this.currentUsers.Any(u => u != pawn);
        }

        public SimTypeDef ResolveSimTypeFor(Pawn pawn)
        {
            if (pawn == null)
            {
                return this.Props.simType ?? VRSimUtility.DefaultSimType;
            }

            if (this.sessionSimTypes.TryGetValue(pawn, out var sim))
            {
                return sim;
            }

            SimTypeDef chosen = VRSimUtility.ResolveBestSimTypeForPawn(pawn, this.Props.simType);
            this.sessionSimTypes[pawn] = chosen;
            return chosen;
        }

        public override string CompInspectStringExtra()
        {
            if (this.currentUsers.Count > 0)
            {
                return $"Current user(s): {string.Join(", ", this.currentUsers.Select(u => u.LabelShortCap))}";
            }

            return "Idle.";
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }

            Pawn occupant = this.CurrentUser;
            if (occupant != null && IsVRJob(occupant.CurJobDef))
            {
                yield return new Gizmo_VRPawnStatus(occupant);
            }
        }

        private void ApplyLucidityAndInstability(Pawn pawn)
        {
            if (pawn == null)
            {
                return;
            }

            pawn.needs?.AddOrRemoveNeedsAsAppropriate();
            Need_Lucidity lucidity = pawn.needs?.TryGetNeed<Need_Lucidity>();
            if (lucidity == null)
            {
                return;
            }

            bool longTerm = pawn.CurJob?.def?.defName == "VA_UseVirtuDreamPod";
            float lucidityGain = this.Props.lucidityGainPerTick * TraitLucidityFactor(pawn);
            if (longTerm)
            {
                lucidityGain *= this.Props.lucidityGainLongTermMultiplier;
            }

            lucidity.CurLevel = Mathf.Clamp01(lucidity.CurLevel + lucidityGain);

            Hediff instability = EnsureInstability(pawn);
            if (instability == null)
            {
                return;
            }

            float instGain = this.Props.instabilityGainPerTick * TraitInstabilityFactor(pawn);
            if (longTerm)
            {
                instGain *= this.Props.instabilityGainLongTermMultiplier;
            }

            if (lucidity.CurLevelPercentage >= this.Props.instabilityHighLucidityThreshold)
            {
                instGain += this.Props.instabilityGainHighLucidityPerTick;
            }

            if (instGain > 0f)
            {
                instability.Severity = Mathf.Clamp(instability.Severity + instGain, 0f, instability.def.maxSeverity);
            }
            else if (this.Props.instabilityDecayPerTick > 0f)
            {
                instability.Severity = Mathf.Max(0f, instability.Severity - this.Props.instabilityDecayPerTick);
            }
        }

        private Hediff EnsureInstability(Pawn pawn)
        {
            if (pawn?.health?.hediffSet == null)
            {
                return null;
            }

            Hediff inst = pawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed("VA_Instability"));
            if (inst == null)
            {
                inst = HediffMaker.MakeHediff(DefDatabase<HediffDef>.GetNamed("VA_Instability"), pawn);
                pawn.health.AddHediff(inst);
            }

            return inst;
        }

        private float TraitLucidityFactor(Pawn pawn)
        {
            float factor = 1f;
            TraitSet traits = pawn.story?.traits;
            if (traits == null)
            {
                return factor;
            }

            if (HasTrait(traits, "TooSmart"))
            {
                factor += 0.12f;
            }

            if (HasTrait(traits, "Sanguine"))
            {
                factor += 0.08f;
            }

            if (HasTrait(traits, "Depressive"))
            {
                factor -= 0.15f;
            }

            if (HasTrait(traits, "Neurotic") || HasTrait(traits, "VeryNeurotic"))
            {
                factor -= 0.1f;
            }

            return Mathf.Max(0.2f, factor);
        }

        private float TraitInstabilityFactor(Pawn pawn)
        {
            float factor = 1f;
            TraitSet traits = pawn.story?.traits;
            if (traits == null)
            {
                return factor;
            }

            if (HasTrait(traits, "TooSmart") || HasTrait(traits, "PsychicallySensitive") || HasTrait(traits, "PsychicallyHypersensitive"))
            {
                factor += 0.1f;
            }

            if (HasTrait(traits, "Sanguine") || HasTrait(traits, "Optimist"))
            {
                factor -= 0.05f;
            }

            if (HasTrait(traits, "Depressive") || HasTrait(traits, "Pessimist"))
            {
                factor += 0.08f;
            }

            return Mathf.Max(0.2f, factor);
        }

        private static bool IsVRJob(JobDef job)
        {
            if (job == null)
            {
                return false;
            }

            return job.defName == "VA_UseVRPod" || job.defName == "VA_UseVirtuDreamPod" || job.defName == "VA_UseVRPodSocial";
        }

        private static bool HasTrait(TraitSet set, string defName)
        {
            if (set == null || string.IsNullOrEmpty(defName))
            {
                return false;
            }

            return set.HasTrait(DefDatabase<TraitDef>.GetNamedSilentFail(defName));
        }

        private void TickMemories(Pawn pawn, SimTypeDef simType)
        {
            if (pawn?.needs?.mood?.thoughts?.memories == null)
            {
                return;
            }

            int timer;
            this.memoryTimers.TryGetValue(pawn, out timer);
            timer += this.Props.tickInterval;

            bool hasVRMemory = HasActiveVRMemory(pawn);
            if (!hasVRMemory || timer >= this.Props.memoryIntervalTicks)
            {
                VRSimUtility.TryGiveSimMemory(pawn, simType);
                timer = 0;
            }

            this.memoryTimers[pawn] = timer;
        }

        private static bool HasActiveVRMemory(Pawn pawn)
        {
            var memories = pawn.needs?.mood?.thoughts?.memories?.Memories;
            if (memories == null)
            {
                return false;
            }

            foreach (var mem in memories)
            {
                string defName = mem?.def?.defName;
                if (string.IsNullOrEmpty(defName))
                {
                    continue;
                }

                if (defName.StartsWith("VA_VRMemory") || defName.StartsWith("VA_VRCombo"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
