using System;
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
        static CompVRPod()
        {
            Log.Message("[VA] CompVRPod static ctor loaded.");
        }

        private readonly HashSet<Pawn> currentUsers = new HashSet<Pawn>();
        private readonly Dictionary<Pawn, SimTypeDef> sessionSimTypes = new Dictionary<Pawn, SimTypeDef>();
        private readonly Dictionary<Pawn, int> memoryTimers = new Dictionary<Pawn, int>();
        private readonly Dictionary<Pawn, int> benefitTimers = new Dictionary<Pawn, int>();
        private const bool DebugVR = true;

        public CompProperties_VRPod Props => (CompProperties_VRPod)this.props;

        public Pawn CurrentUser => this.currentUsers.Count > 0 ? this.currentUsers.First() : null;

        public IReadOnlyCollection<Pawn> CurrentUsers => this.currentUsers;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (Prefs.DevMode)
            {
                Log.Message($"[VA] VRPod comp spawned on {this.parent.Label ?? this.parent.def.defName} at tick {Find.TickManager.TicksGame}.");
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            if ((Prefs.DevMode || DebugVR) && this.parent.IsHashIntervalTick(1000))
            {
                Log.Message($"[VA] CompTick heartbeat on {this.parent.Label ?? this.parent.def.defName} at tick {Find.TickManager.TicksGame} (users: {this.currentUsers.Count}).");
            }

            if (this.parent.IsHashIntervalTick(this.Props.tickInterval))
            {
                if (Prefs.DevMode && this.parent.IsHashIntervalTick(this.Props.tickInterval * 4))
                {
                    Log.Message($"[VA] CompTick on {this.parent.Label ?? this.parent.def.defName} at tick {Find.TickManager.TicksGame} (users: {this.currentUsers.Count}).");
                }
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

                int memTimer = 0;
                this.memoryTimers.TryGetValue(pawn, out memTimer);
                memTimer += this.Props.tickInterval;

                int benefitTimer = 0;
                this.benefitTimers.TryGetValue(pawn, out benefitTimer);
                benefitTimer += this.Props.tickInterval;

                bool prevBenefit = (benefitTimer - this.Props.tickInterval) >= this.Props.minimumBenefitTicks;
                bool canBenefit = benefitTimer >= this.Props.minimumBenefitTicks;
                bool justUnlocked = !prevBenefit && canBenefit;

                if ((Prefs.DevMode || DebugVR) && justUnlocked)
                {
                    Log.Message($"[VA] {pawn.LabelShortCap}: benefits unlocked after {benefitTimer} ticks in pod {this.parent.Label ?? this.parent.def.defName}.");
                }

                if ((Prefs.DevMode || DebugVR) && this.parent.IsHashIntervalTick(500))
                {
                    Need joy = pawn.needs?.joy;
                    Log.Message($"[VA] {pawn.LabelShortCap}: timers mem={memTimer} benefit={benefitTimer} canBenefit={canBenefit} joy={joy?.CurLevel:F3} lucidity={(pawn.needs?.TryGetNeed<Need_Lucidity>()?.CurLevel ?? 0f):F3} in pod {this.parent.Label ?? this.parent.def.defName}.");
                }

                SimTypeDef simType = this.ResolveSimTypeFor(pawn);
                if (canBenefit)
                {
                    VRSimUtility.ApplySimTraining(pawn, simType, this.Props.tickInterval);
                    ApplyLucidityAndInstability(pawn);
                    ApplyJoy(pawn, simType);
                    TickMemories(pawn, simType, ref memTimer);
                }

                this.memoryTimers[pawn] = memTimer;
                this.benefitTimers[pawn] = benefitTimer;
            }
        }

        public void SetUser(Pawn pawn)
        {
            this.currentUsers.Clear();
            this.sessionSimTypes.Clear();
            this.memoryTimers.Clear();
            this.benefitTimers.Clear();
            if (pawn != null)
            {
                this.currentUsers.Add(pawn);
                this.ResolveSimTypeFor(pawn);
                this.memoryTimers[pawn] = 0;
                this.benefitTimers[pawn] = 0;
                if (Prefs.DevMode)
                {
                    Log.Message($"[VA] SetUser: {pawn.LabelShortCap} added to {this.parent.Label ?? this.parent.def.defName} at tick {Find.TickManager.TicksGame}.");
                }
            }
        }

        public void AddUser(Pawn pawn)
        {
            if (pawn != null)
            {
                this.currentUsers.Add(pawn);
                this.ResolveSimTypeFor(pawn);
                this.memoryTimers[pawn] = 0;
                this.benefitTimers[pawn] = 0;
                if (Prefs.DevMode)
                {
                    Log.Message($"[VA] AddUser: {pawn.LabelShortCap} added to {this.parent.Label ?? this.parent.def.defName} at tick {Find.TickManager.TicksGame}.");
                }
            }
        }

        public void RemoveUser(Pawn pawn)
        {
            if (pawn != null)
            {
                this.currentUsers.Remove(pawn);
                this.sessionSimTypes.Remove(pawn);
                this.memoryTimers.Remove(pawn);
                this.benefitTimers.Remove(pawn);
                if (Prefs.DevMode)
                {
                    Log.Message($"[VA] RemoveUser: {pawn.LabelShortCap} removed from {this.parent.Label ?? this.parent.def.defName} at tick {Find.TickManager.TicksGame}.");
                }
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
                EnsureLucidityNeed(occupant);
                EnsureInstability(occupant);
                yield return new Gizmo_VRPawnStatus(occupant);
            }
        }

        private void ApplyLucidityAndInstability(Pawn pawn)
        {
            if (pawn == null)
            {
                return;
            }

            Need_Lucidity lucidity = EnsureLucidityNeed(pawn);
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
                inst.Severity = 0.01f;
                pawn.health.AddHediff(inst);
            }

            return inst;
        }

        private Need_Lucidity EnsureLucidityNeed(Pawn pawn)
        {
            if (pawn?.needs == null)
            {
                return null;
            }

            Need_Lucidity need = pawn.needs.TryGetNeed<Need_Lucidity>();
            if (need != null)
            {
                return need;
            }

            NeedDef def = DefDatabase<NeedDef>.GetNamedSilentFail("VA_Lucidity");
            if (def == null)
            {
                return null;
            }

            try
            {
                Need newNeed = (Need)System.Activator.CreateInstance(def.needClass, pawn);
                newNeed.def = def;
                newNeed.CurLevel = 0f;
                pawn.needs.AllNeeds.Add(newNeed);
                return newNeed as Need_Lucidity;
            }
            catch
            {
                return null;
            }
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

        public bool CanProvideBenefits(Pawn pawn)
        {
            int timer = 0;
            this.benefitTimers.TryGetValue(pawn, out timer);
            return timer >= this.Props.minimumBenefitTicks;
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

        private void ApplyJoy(Pawn pawn, SimTypeDef simType)
        {
            var joy = pawn.needs?.joy;
            if (joy == null)
            {
                return;
            }

            bool longTerm = pawn.CurJob?.def?.defName == "VA_UseVirtuDreamPod";
            float gain = this.Props.joyGainPerTick * (longTerm ? this.Props.joyLongTermMultiplier : 1f);

            float beforeJoy = joy.CurLevel;
            joy.GainJoy(gain, simType?.primarySkill == SkillDefOf.Social ? JoyKindDefOf.Social : JoyKindDefOf.Meditative);

            if (Prefs.DevMode && this.parent.IsHashIntervalTick(600))
            {
                Log.Message($"[VA] {pawn.LabelShortCap}: joy {beforeJoy:F3}->{joy.CurLevel:F3} (gain {gain:F4}) in pod {this.parent.Label ?? this.parent.def.defName}.");
            }

            if (longTerm)
            {
                var rest = pawn.needs?.rest;
                if (rest != null)
                {
                    rest.CurLevel = Mathf.Min(rest.MaxLevel, rest.CurLevel + this.Props.restGainPerTickLongTerm);
                    if (Prefs.DevMode && this.parent.IsHashIntervalTick(1200))
                    {
                        Log.Message($"[VA] {pawn.LabelShortCap}: rest now {rest.CurLevel:F3} in long-term sim.");
                    }
                }
            }
        }

        private void TickMemories(Pawn pawn, SimTypeDef simType, ref int timer)
        {
            if (pawn?.needs?.mood?.thoughts?.memories == null)
            {
                return;
            }

            if (timer >= this.Props.memoryIntervalTicks)
            {
                if (Prefs.DevMode)
                {
                    Log.Message($"[VA] {pawn.LabelShortCap}: memory tick (timer {timer}), simType {(simType != null ? simType.defName : "null")}.");
                }
                VRSimUtility.TryGiveSimMemory(pawn, simType);
                timer = 0;
            }
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
