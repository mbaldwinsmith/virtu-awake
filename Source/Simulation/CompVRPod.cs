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
        private readonly HashSet<int> breakoutTriggered = new HashSet<int>();
        private const bool DebugVR = false;

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
                    bool deepSession = pawn?.CurJob?.def?.defName == "VA_UseVRPodDeep";
                    SustainVitals(pawn, deepSession);
                    ApplyComfort(pawn);
                    ApplyTraitSatisfiers(pawn);
                    ApplyEnvironmentImmunity(pawn);
                    ApplyJoy(pawn, simType);
                    TickMemories(pawn, simType, ref memTimer);
                    Need_Lucidity lucidity = pawn.needs?.TryGetNeed<Need_Lucidity>();
                    Hediff inst = pawn.health?.hediffSet?.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamedSilentFail("VA_Instability"));
                    CheckBreakout(pawn, lucidity);
                    TryTriggerGlitch(pawn, lucidity, inst, simType);
                }

                this.memoryTimers[pawn] = memTimer;
                this.benefitTimers[pawn] = benefitTimer;
            }
        }

        public void SetUser(Pawn pawn)
        {
            foreach (Pawn existing in this.currentUsers)
            {
                VRSessionTracker.Unregister(existing);
            }
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
                VRSessionTracker.Register(pawn);
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
                VRSessionTracker.Register(pawn);
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
                VRSessionTracker.Unregister(pawn);
                this.breakoutTriggered.Remove(pawn.thingIDNumber);
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

        public Pawn FindActiveOrPendingUser()
        {
            if (this.CurrentUser != null)
            {
                return this.CurrentUser;
            }

            Map map = this.parent?.Map;
            if (map == null)
            {
                return null;
            }

            foreach (Pawn pawn in map.mapPawns.AllPawnsSpawned)
            {
                Job job = pawn?.CurJob;
                if (job == null || job.targetA.Thing != this.parent)
                {
                    continue;
                }

                if (IsVRJob(job.def))
                {
                    return pawn;
                }
            }

            return null;
        }

        public int TicksUntilBenefit(Pawn pawn)
        {
            if (pawn == null)
            {
                return this.Props.minimumBenefitTicks;
            }

            this.benefitTimers.TryGetValue(pawn, out int timer);
            return Mathf.Max(0, this.Props.minimumBenefitTicks - timer);
        }

        public int TicksUntilMemory(Pawn pawn)
        {
            if (pawn == null)
            {
                return this.Props.memoryIntervalTicks;
            }

            this.memoryTimers.TryGetValue(pawn, out int timer);
            return Mathf.Max(0, this.Props.memoryIntervalTicks - timer);
        }

        public bool TryEndSessionFor(Pawn pawn, string message = null, MessageTypeDef messageType = null)
        {
            if (pawn == null)
            {
                return false;
            }

            bool affected = false;

            Job curJob = pawn.CurJob;
            if (curJob != null && curJob.targetA.Thing == this.parent && IsVRJob(curJob.def))
            {
                pawn.jobs?.EndCurrentJob(JobCondition.InterruptForced, startNewJob: true, canReturnToPool: true);
                affected = true;
            }

            if (this.currentUsers.Contains(pawn))
            {
                this.RemoveUser(pawn);
                affected = true;
            }

            this.parent.Map?.reservationManager?.ReleaseAllForTarget(this.parent);

            if (affected && !string.IsNullOrEmpty(message))
            {
                Messages.Message(message, new LookTargets(this.parent), messageType ?? MessageTypeDefOf.TaskCompletion, historical: false);
            }

            return affected;
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

            float lucidityGain = this.Props.lucidityGainPerTick * TraitLucidityFactor(pawn);
            lucidityGain *= MoodLucidityFactor(pawn);

            lucidity.CurLevel = Mathf.Clamp01(lucidity.CurLevel + lucidityGain);

            Hediff instability = EnsureInstability(pawn);
            if (instability == null)
            {
                return;
            }

            float instGain = this.Props.instabilityGainPerTick * TraitInstabilityFactor(pawn);

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
                factor += 0.2f;
            }

            if (HasTrait(traits, "Paranoid") || HasTrait(traits, "Nervous"))
            {
                factor += 0.15f;
            }

            if (HasTrait(traits, "PsychicallySensitive") || HasTrait(traits, "PsychicallyHypersensitive"))
            {
                factor += 0.1f;
            }

            if (HasTrait(traits, "Sanguine"))
            {
                factor -= 0.1f;
            }

            if (HasTrait(traits, "Gourmand") || HasTrait(traits, "Bloodlust") || HasTrait(traits, "Greedy"))
            {
                factor -= 0.08f;
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

        private float MoodLucidityFactor(Pawn pawn)
        {
            float mood = pawn.needs?.mood?.CurLevel ?? 0.5f;

            if (mood < 0.3f || mood > 0.8f)
            {
                return 1.5f;
            }

            if (mood > 0.45f && mood < 0.65f)
            {
                return -0.3f; // mild good mood slowly cools lucidity
            }

            if (mood >= 0.65f && mood <= 0.8f)
            {
                return 0.6f; // slightly good mood grows slowly
            }

            return 1f;
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

            return job.defName == "VA_UseVRPod" || job.defName == "VA_UseVRPodSocial" || job.defName == "VA_UseVRPodDeep";
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

            float gain = this.Props.joyGainPerTick;

            float beforeJoy = joy.CurLevel;
            joy.GainJoy(gain, simType?.primarySkill == SkillDefOf.Social ? JoyKindDefOf.Social : JoyKindDefOf.Meditative);

            if (Prefs.DevMode && this.parent.IsHashIntervalTick(600))
            {
                Log.Message($"[VA] {pawn.LabelShortCap}: joy {beforeJoy:F3}->{joy.CurLevel:F3} (gain {gain:F4}) in pod {this.parent.Label ?? this.parent.def.defName}.");
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

        private void TryTriggerGlitch(Pawn pawn, Need_Lucidity lucidity, Hediff instability, SimTypeDef simType)
        {
            if (pawn == null || lucidity == null)
            {
                return;
            }

            float luc = lucidity.CurLevel;
            float inst = Mathf.Clamp01(instability?.Severity ?? 0f);
            float mood = pawn.needs?.mood?.CurLevel ?? 0.5f;

            GlitchSeverity severity = DetermineGlitchSeverity(luc, inst);
            if (severity == GlitchSeverity.None)
            {
                return;
            }

            float combined = (luc + inst) * 0.5f;
            float chance = Mathf.Lerp(0.02f, 0.18f, Mathf.InverseLerp(0.45f, 0.95f, combined));
            chance += Mathf.Lerp(0f, 0.05f, Mathf.InverseLerp(0.6f, 1f, inst));
            if (severity == GlitchSeverity.Major)
            {
                chance += 0.03f;
            }
            else if (severity == GlitchSeverity.Mid)
            {
                chance += 0.015f;
            }
            chance *= TraitGlitchFactor(pawn);
            chance *= MoodGlitchFactor(mood);
            chance *= this.Props.tickInterval / 200f;
            chance = Mathf.Clamp(chance, 0.005f, 0.35f);

            if (!Rand.Chance(chance))
            {
                return;
            }

            ThoughtDef glitch = PickGlitchThought(pawn, luc, inst, severity);
            if (glitch == null)
            {
                return;
            }

            pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(glitch);

            if (Prefs.DevMode)
            {
                Log.Message($"[VA] Glitch memory {glitch.defName} applied to {pawn.LabelShortCap} (lucidity {luc:F2}, instability {inst:F2}, mood {mood:F2}).");
            }
        }

        private ThoughtDef PickGlitchThought(Pawn pawn, float luc, float inst, GlitchSeverity severity)
        {
            List<ThoughtDef> pool = ResolveGlitchPool(pawn, severity);
            if (pool.NullOrEmpty())
            {
                return DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRGlitch_Negative");
            }

            pool = pool.Where(def => def != null && !HasActiveThought(pawn, def)).ToList();
            if (pool.Count == 0)
            {
                pool = ResolveGlitchPool(pawn, severity);
            }

            return pool.Where(d => d != null).TryRandomElement(out ThoughtDef chosen)
                ? chosen
                : DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRGlitch_Negative");
        }

        private GlitchSeverity DetermineGlitchSeverity(float luc, float inst)
        {
            if (luc >= 0.78f || inst >= 0.78f || (luc >= 0.65f && inst >= 0.65f))
            {
                return GlitchSeverity.Major;
            }

            if (luc >= 0.6f || inst >= 0.6f || (luc >= 0.5f && inst >= 0.5f))
            {
                return GlitchSeverity.Mid;
            }

            if (luc >= 0.45f || inst >= 0.5f)
            {
                return GlitchSeverity.Minor;
            }

            return GlitchSeverity.None;
        }

        private List<ThoughtDef> ResolveGlitchPool(Pawn pawn, GlitchSeverity severity)
        {
            TraitSet traits = pawn.story?.traits;
            bool paranoid = HasTrait(traits, "Paranoid") || HasTrait(traits, "Nervous");
            bool psychic = HasTrait(traits, "PsychicallySensitive") || HasTrait(traits, "PsychicallyHypersensitive");
            bool tooSmart = HasTrait(traits, "TooSmart");
            bool purist = HasTrait(traits, "BodyPurist");
            bool sanguine = HasTrait(traits, "Sanguine") || HasTrait(traits, "Optimist") || HasTrait(traits, "BodyModder");
            bool psychopath = HasTrait(traits, "Psychopath");

            var pool = new List<ThoughtDef>();

            if (severity == GlitchSeverity.Major)
            {
                pool.Add(DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRGlitch_NegativeMajor"));
                pool.Add(DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRGlitch_NegativeMajor_Dissonance"));
                if (paranoid && psychic)
                {
                    pool.Add(DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRGlitch_NegativeMajor_ParanoidSensitive"));
                }
                if (purist && (psychic || tooSmart))
                {
                    pool.Add(DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRGlitch_NegativeMajor_Purist"));
                }
            }
            else if (severity == GlitchSeverity.Mid || severity == GlitchSeverity.Minor)
            {
                pool.Add(DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRGlitch_Negative"));
                pool.Add(DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRGlitch_Negative_Shatter"));
                pool.Add(DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRGlitch_Negative_Echo"));
                if (paranoid && psychic)
                {
                    pool.Add(DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRGlitch_Negative_ParanoidSensitive"));
                }
            }

            if (psychopath)
            {
                pool.Add(DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRGlitch_Neutral_Psychopath"));
            }
            else
            {
                pool.Add(DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRGlitch_Neutral"));
                pool.Add(DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRGlitch_Neutral_Archive"));
                pool.Add(DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRGlitch_Neutral_Detached"));
            }

            if (sanguine && severity != GlitchSeverity.Major)
            {
                pool.Add(DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRGlitch_Positive"));
                pool.Add(DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRGlitch_Positive_Calm"));
                pool.Add(DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRGlitch_Positive_SanguineModder"));
                pool.Add(DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRGlitch_Positive_Halo"));
            }
            else if (severity != GlitchSeverity.Major)
            {
                pool.Add(DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRGlitch_Positive_Calm"));
            }

            return pool;
        }

        private float TraitGlitchFactor(Pawn pawn)
        {
            TraitSet traits = pawn.story?.traits;
            if (traits == null)
            {
                return 1f;
            }

            float factor = 1f;
            if (HasTrait(traits, "Paranoid") || HasTrait(traits, "TooSmart") || HasTrait(traits, "Nervous"))
            {
                factor += 0.25f;
            }
            if (HasTrait(traits, "PsychicallySensitive") || HasTrait(traits, "PsychicallyHypersensitive") || HasTrait(traits, "BodyPurist"))
            {
                factor += 0.15f;
            }
            if (HasTrait(traits, "Sanguine") || HasTrait(traits, "BodyModder"))
            {
                factor -= 0.2f;
            }
            if (HasTrait(traits, "Psychopath"))
            {
                factor -= 0.25f;
            }

            return Mathf.Max(0.25f, factor);
        }

        private float MoodGlitchFactor(float mood)
        {
            if (mood < 0.3f)
            {
                return 1.3f;
            }

            if (mood > 0.8f)
            {
                return 1.15f;
            }

            if (mood > 0.45f && mood < 0.65f)
            {
                return 0.85f;
            }

            return 1f;
        }

        private static bool HasActiveThought(Pawn pawn, ThoughtDef thought)
        {
            var memories = pawn.needs?.mood?.thoughts?.memories?.Memories;
            if (memories == null || thought == null)
            {
                return false;
            }

            foreach (var mem in memories)
            {
                if (mem?.def == thought && mem.CurStageIndex >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void SustainVitals(Pawn pawn, bool deepSession)
        {
            var rest = pawn.needs?.rest;
            if (rest != null)
            {
                rest.CurLevel = Mathf.Clamp01(rest.CurLevel + 0.004f);
            }

            var food = pawn.needs?.food;
            if (food != null)
            {
                float target = deepSession ? 0.95f : 0.8f;
                if (food.CurLevel < target)
                {
                    food.CurLevel = target;
                }
            }
        }

        private void ApplyComfort(Pawn pawn)
        {
            var comfort = pawn.needs?.comfort;
            if (comfort != null)
            {
                float luc = pawn.needs?.TryGetNeed<Need_Lucidity>()?.CurLevel ?? 0f;
                float delta = Mathf.Lerp(0.08f, -0.12f, luc); // high lucidity erodes comfort
                comfort.CurLevel = Mathf.Clamp01(comfort.CurLevel + delta);
            }
        }

        private void ApplyTraitSatisfiers(Pawn pawn)
        {
            TraitSet traits = pawn.story?.traits;
            if (traits == null)
            {
                return;
            }

            var memories = pawn.needs?.mood?.thoughts?.memories;
            if (memories == null)
            {
                return;
            }

            if (HasTrait(traits, "Nudist"))
            {
                var nudistThought = DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRPod_NudistSatisfied");
                if (nudistThought != null && !HasActiveThought(pawn, nudistThought))
                {
                    memories.TryGainMemory(nudistThought);
                }
            }

            if (HasTrait(traits, "Undergrounder"))
            {
                var undergrounderThought = DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRPod_UndergrounderShelter");
                if (undergrounderThought != null && !HasActiveThought(pawn, undergrounderThought))
                {
                    memories.TryGainMemory(undergrounderThought);
                }

                var outdoors = pawn.needs?.outdoors;
                if (outdoors != null)
                {
                    outdoors.CurLevel = outdoors.MaxLevel;
                }
            }
        }

        private void ApplyEnvironmentImmunity(Pawn pawn)
        {
            var hediffs = pawn.health?.hediffSet?.hediffs;
            if (hediffs != null)
            {
                foreach (var h in hediffs)
                {
                    if (h == null)
                    {
                        continue;
                    }

                    if (h.def == HediffDefOf.Hypothermia || h.def == HediffDefOf.Heatstroke)
                    {
                        h.Severity = 0f;
                    }
                }
            }

            var memories = pawn.needs?.mood?.thoughts?.memories;
            if (memories != null)
            {
                var soaked = DefDatabase<ThoughtDef>.GetNamedSilentFail("SoakingWet");
                if (soaked != null)
                {
                    memories.RemoveMemoriesOfDef(soaked);
                }
            }
        }

        private void CheckBreakout(Pawn pawn, Need_Lucidity lucidity)
        {
            if (pawn == null || lucidity == null)
            {
                return;
            }

            if (lucidity.CurLevel < 0.9f)
            {
                this.breakoutTriggered.Remove(pawn.thingIDNumber);
                return;
            }

            if (this.breakoutTriggered.Contains(pawn.thingIDNumber))
            {
                return;
            }

            this.breakoutTriggered.Add(pawn.thingIDNumber);
            TryTriggerBreakout(pawn);
        }

        private void TryTriggerBreakout(Pawn pawn)
        {
            if (pawn.InMentalState || pawn.Downed || pawn.Dead)
            {
                return;
            }

            TraitSet traits = pawn.story?.traits;
            MentalStateDef def = null;

            if (HasTrait(traits, "Nervous") || HasTrait(traits, "Wimp"))
            {
                def = GetMentalState("PanicFlee");
            }
            else if (HasTrait(traits, "Gourmand"))
            {
                def = GetMentalState("FoodBinge");
            }
            else if (HasTrait(traits, "ChemicalInterest") || HasTrait(traits, "ChemicalFascination"))
            {
                def = GetMentalState("DrugBinge");
            }
            else if (HasTrait(traits, "Pyromaniac"))
            {
                def = GetMentalState("FireStartingSpree");
            }
            else if (HasTrait(traits, "Bloodlust") || HasTrait(traits, "Berserk") || HasTrait(traits, "Cannibal"))
            {
                def = GetMentalState("Berserk");
            }
            else if (HasTrait(traits, "Depressive"))
            {
                def = GetMentalState("SadWander");
            }
            else if (HasTrait(traits, "Abrasive"))
            {
                def = GetMentalState("InsultingSpree");
            }

            def ??= GetMentalState("Berserk");
            pawn.mindState?.mentalStateHandler?.TryStartMentalState(def, "VR breakout", forceWake: true);
        }

        private MentalStateDef GetMentalState(string defName)
        {
            return DefDatabase<MentalStateDef>.GetNamedSilentFail(defName) ?? MentalStateDefOf.Berserk;
        }

        private enum GlitchSeverity
        {
            None,
            Minor,
            Mid,
            Major
        }
    }
}
