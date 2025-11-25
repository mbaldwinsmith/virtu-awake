using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace VirtuAwake
{
    public class MapComponent_VirtuAwakeEvents : MapComponent
    {
        private const int TickInterval = 1200; // 20 seconds
        private List<SharedDreamChainState> activeChains = new List<SharedDreamChainState>();

        public MapComponent_VirtuAwakeEvents(Map map) : base(map)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref this.activeChains, "activeChains", LookMode.Deep);
            if (this.activeChains == null)
            {
                this.activeChains = new List<SharedDreamChainState>();
            }
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();

            this.TickSharedDreamChains();

            if (!this.map.IsHashIntervalTick(TickInterval))
            {
                return;
            }

            Dictionary<PowerNet, List<Pawn>> vrGroups = FindNetworkedVRPawns(this.map);
            if (vrGroups.Count == 0)
            {
                return;
            }

            if (Prefs.DevMode)
            {
                Log.Message($"[VA][Social] Found {vrGroups.Count} VR power-net group(s) at tick {Find.TickManager.TicksGame}.");
            }

            foreach (List<Pawn> group in vrGroups.Values)
            {
                if (group.NullOrEmpty())
                {
                    continue;
                }

                if (group.Count < 2)
                {
                    if (Prefs.DevMode)
                    {
                        Log.Message($"[VA][Social] Skipping group (count {group.Count}) - need 2+ pawns.");
                    }
                    continue;
                }

                float avgLucidity = 0f;
                float avgInstability = 0f;
                foreach (Pawn pawn in group)
                {
                    avgLucidity += pawn.needs?.TryGetNeed<Need_Lucidity>()?.CurLevel ?? 0f;
                    avgInstability += pawn.health?.hediffSet?.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamedSilentFail("VA_Instability"))?.Severity ?? 0f;
                }

                avgLucidity /= group.Count;
                avgInstability /= group.Count;
                float combined = (avgLucidity + avgInstability) * 0.5f;

                // Base chance scaled by combined state (clamped to avoid negative chances when calm).
                float stateFactor = Mathf.Clamp01(Mathf.InverseLerp(0.35f, 1f, combined));
                float chance = Mathf.Lerp(0.02f, 0.1f, stateFactor);
                float groupFactor = Mathf.Lerp(1f, 1.5f, Mathf.Clamp01((group.Count - 1) / 3f));
                chance *= groupFactor;

                if (Prefs.DevMode)
                {
                    Log.Message($"[VA][Social] Group size {group.Count}, avgLucidity {avgLucidity:F2}, avgInstability {avgInstability:F2}, combined {combined:F2}, chance {chance:P1}.");
                }

                if (!Rand.Chance(chance))
                {
                    TryRollSocialMemories(group, combined);
                    continue;
                }

                Pawn anchor = group.RandomElement();
                MatrixEventSeverity severity = MatrixEventUtility.PickSeverity(avgLucidity, avgInstability, anchor);
                MatrixEventDef evt = MatrixEventUtility.PickEvent(anchor, severity, avgLucidity, avgInstability);
                if (evt == null)
                {
                    TryRollSocialMemories(group, combined);
                    continue;
                }

                MatrixEventChainExtension chainExt = evt.GetModExtension<MatrixEventChainExtension>();
                if (chainExt != null && chainExt.IsChainStart)
                {
                    bool started = TryStartSharedDreamChain(evt, chainExt, group, anchor);
                    if (!started)
                    {
                        ApplyEventToGroup(group, evt, severity);
                    }
                }
                else
                {
                    ApplyEventToGroup(group, evt, severity);
                }

                TryRollSocialMemories(group, combined);
            }
        }

        private static Dictionary<PowerNet, List<Pawn>> FindNetworkedVRPawns(Map map)
        {
            var dict = new Dictionary<PowerNet, List<Pawn>>();
            foreach (Pawn pawn in map.mapPawns.AllPawnsSpawned)
            {
                if (VRSessionTracker.IsInVR(pawn))
                {
                    PowerNet net = VRSessionTracker.GetPowerNet(pawn);
                    if (net == null)
                    {
                        continue;
                    }

                    if (!dict.TryGetValue(net, out var list))
                    {
                        list = new List<Pawn>();
                        dict[net] = list;
                    }
                    list.Add(pawn);
                }
            }
            return dict;
        }

        private void TickSharedDreamChains()
        {
            if (this.activeChains.NullOrEmpty())
            {
                return;
            }

            for (int i = this.activeChains.Count - 1; i >= 0; i--)
            {
                SharedDreamChainState chain = this.activeChains[i];
                if (chain == null)
                {
                    this.activeChains.RemoveAt(i);
                    continue;
                }

                PruneParticipants(chain);
                if (!chain.HasParticipants)
                {
                    this.activeChains.RemoveAt(i);
                    continue;
                }

                chain.ticksUntilNext--;
                if (chain.ticksUntilNext > 0)
                {
                    continue;
                }

                MatrixEventDef next = ResolveNextChainEvent(chain);
                if (next == null)
                {
                    this.activeChains.RemoveAt(i);
                    continue;
                }

                MatrixEventChainExtension nextExt = next.GetModExtension<MatrixEventChainExtension>();
                chain.currentEvent = next;
                chain.stageIndex = nextExt?.stage ?? (chain.stageIndex + 1);
                ApplyEventToGroup(chain.participants, next, next.severity);

                if (HasFurtherStages(nextExt))
                {
                    chain.ticksUntilNext = Mathf.Max(1, nextExt.ticksToNextStage);
                }
                else
                {
                    this.activeChains.RemoveAt(i);
                }
            }
        }

        private void ApplyEventToGroup(List<Pawn> pawns, MatrixEventDef evt, MatrixEventSeverity severity)
        {
            if (pawns == null || evt == null)
            {
                return;
            }

            MatrixEventChainExtension chainExt = evt.GetModExtension<MatrixEventChainExtension>();
            foreach (Pawn pawn in pawns)
            {
                ApplyEvent(pawn, evt, severity);
            }

            if (chainExt != null)
            {
                SharedDreamEventWorker.ResolveGroupEvent(pawns, evt, chainExt);
            }
        }

        private bool TryStartSharedDreamChain(MatrixEventDef evt, MatrixEventChainExtension chainExt, List<Pawn> vrPawns, Pawn instigator)
        {
            if (evt == null || chainExt == null || vrPawns.NullOrEmpty())
            {
                return false;
            }

            string chainId = !string.IsNullOrEmpty(chainExt.chainId) ? chainExt.chainId : evt.defName;
            int maxParticipants = Mathf.Max(chainExt.minParticipants, chainExt.maxParticipants);

            var selected = new List<Pawn>();
            if (instigator != null && instigator.Map == this.map && VRSessionTracker.IsInVR(instigator) && !InActiveChain(instigator, chainId))
            {
                selected.Add(instigator);
            }

            IEnumerable<Pawn> candidates = vrPawns.Where(p => p != null && p.Map == this.map && !InActiveChain(p, chainId) && p != instigator);
            foreach (Pawn pawn in candidates.InRandomOrder())
            {
                if (selected.Count >= maxParticipants)
                {
                    break;
                }
                selected.Add(pawn);
            }

            if (selected.Count < chainExt.minParticipants)
            {
                return false;
            }

            var state = new SharedDreamChainState
            {
                chainId = chainId,
                currentEvent = evt,
                stageIndex = chainExt.stage,
                ticksUntilNext = Mathf.Max(1, chainExt.ticksToNextStage),
                participants = selected
            };

            this.activeChains.Add(state);
            ApplyEventToGroup(selected, evt, evt.severity);
            return true;
        }

        private bool InActiveChain(Pawn pawn, string chainId)
        {
            if (pawn == null || chainId == null || this.activeChains.NullOrEmpty())
            {
                return false;
            }

            foreach (SharedDreamChainState chain in this.activeChains)
            {
                if (chain == null || chain.chainId != chainId || chain.participants == null)
                {
                    continue;
                }

                if (chain.participants.Contains(pawn))
                {
                    return true;
                }
            }

            return false;
        }

        private void PruneParticipants(SharedDreamChainState chain)
        {
            if (chain?.participants == null)
            {
                return;
            }

            PowerNet referenceNet = null;
            if (chain.participants.Count > 0)
            {
                referenceNet = VRSessionTracker.GetPowerNet(chain.participants[0]);
            }

            chain.participants.RemoveAll(p =>
                p == null
                || p.Map != this.map
                || !VRSessionTracker.IsInVR(p)
                || p.Dead
                || p.Downed
                || (referenceNet != null && VRSessionTracker.GetPowerNet(p) != referenceNet));
        }

        private MatrixEventDef ResolveNextChainEvent(SharedDreamChainState chain)
        {
            if (chain == null || chain.currentEvent == null)
            {
                return null;
            }

            MatrixEventChainExtension ext = chain.currentEvent.GetModExtension<MatrixEventChainExtension>();
            string chainId = ext?.chainId ?? chain.chainId;

            if (ext?.nextEventDefs != null)
            {
                foreach (string defName in ext.nextEventDefs)
                {
                    MatrixEventDef def = DefDatabase<MatrixEventDef>.GetNamedSilentFail(defName);
                    if (def != null)
                    {
                        return def;
                    }
                }
            }

            if (!string.IsNullOrEmpty(chainId))
            {
                int targetStage = ext != null ? ext.stage + 1 : chain.stageIndex + 1;
                foreach (MatrixEventDef def in DefDatabase<MatrixEventDef>.AllDefsListForReading)
                {
                    MatrixEventChainExtension nextExt = def?.GetModExtension<MatrixEventChainExtension>();
                    if (nextExt == null)
                    {
                        continue;
                    }

                    if (!string.Equals(nextExt.chainId, chainId))
                    {
                        continue;
                    }

                    if (nextExt.stage == targetStage)
                    {
                        return def;
                    }
                }
            }

            return null;
        }

        private bool HasFurtherStages(MatrixEventChainExtension ext)
        {
            if (ext == null)
            {
                return false;
            }

            if (ext.nextEventDefs != null && ext.nextEventDefs.Count > 0)
            {
                return true;
            }

            if (string.IsNullOrEmpty(ext.chainId))
            {
                return false;
            }

            int nextStage = ext.stage + 1;
            foreach (MatrixEventDef def in DefDatabase<MatrixEventDef>.AllDefsListForReading)
            {
                MatrixEventChainExtension nextExt = def?.GetModExtension<MatrixEventChainExtension>();
                if (nextExt != null && string.Equals(nextExt.chainId, ext.chainId) && nextExt.stage == nextStage)
                {
                    return true;
                }
            }

            return false;
        }

        private void TryRollSocialMemories(List<Pawn> pawns, float combinedState)
        {
            if (pawns.NullOrEmpty() || pawns.Count < 2)
            {
                return;
            }

            SimTypeDef social = DefDatabase<SimTypeDef>.GetNamedSilentFail("VA_Sim_Social");
            if (social == null)
            {
                return;
            }

            float stateFactor = Mathf.Clamp01(Mathf.InverseLerp(0.2f, 0.9f, combinedState));
            float chance = Mathf.Lerp(0.16f, 0.36f, stateFactor); // raised floor + ceiling so socials outpace glitches
            chance *= Mathf.Lerp(1f, 1.35f, Mathf.Clamp01((pawns.Count - 1) / 3f));
            chance *= Mathf.Lerp(1f, 1.15f, Mathf.Clamp01(pawns.Count / 5f)); // larger nets get a small extra push

            if (Prefs.DevMode)
            {
                Log.Message($"[VA][Social] Social roll for {pawns.Count} pawns: combined {combinedState:F2}, stateFactor {stateFactor:F2}, chance {chance:P1}.");
            }

            if (!Rand.Chance(chance))
            {
                if (Prefs.DevMode)
                {
                    Log.Message("[VA][Social] Social roll failed this tick.");
                }
                return;
            }

            SocialMemoryUtility.GiveNetworkSocialMemories(pawns);

            foreach (Pawn pawn in pawns)
            {
                if (pawn == null)
                {
                    continue;
                }

                CompVRPod pod = VRSessionTracker.GetPod(pawn);
                if (pod == null)
                {
                    continue;
                }

                if (Prefs.DevMode)
                {
                    Log.Message($"[VA][Social] Awarding social sim memory to {pawn.LabelShortCap}.");
                }

                VRSimUtility.TryGiveSimMemory(pawn, social);
            }
        }

        private void ApplyEvent(Pawn pawn, MatrixEventDef evt, MatrixEventSeverity severity)
        {
            ThoughtDef thought = VRMemorySelector.SelectEventMemory(pawn, severity, evt.thoughtOnFire);

            if (thought != null)
            {
                pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(thought);
            }

            ApplySideEffects(pawn, evt);

            // Notifications are suppressed; effects still apply.
        }

        private void ApplySideEffects(Pawn pawn, MatrixEventDef evt)
        {
            if (pawn == null || evt == null)
            {
                return;
            }

            if (evt.lucidityOffset != 0f)
            {
                Need_Lucidity lucidity = pawn.needs?.TryGetNeed<Need_Lucidity>();
                if (lucidity != null)
                {
                    lucidity.CurLevel = Mathf.Clamp01(lucidity.CurLevel + evt.lucidityOffset);
                }
            }

            if (evt.instabilityOffset != 0f)
            {
                Hediff inst = pawn.health?.hediffSet?.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamedSilentFail("VA_Instability"));
                if (inst != null)
                {
                    inst.Severity = Mathf.Clamp(inst.Severity + evt.instabilityOffset, 0f, inst.def.maxSeverity);
                }
            }

            if (evt.moodOffset != 0f)
            {
                Thought_Memory memory = (Thought_Memory)ThoughtMaker.MakeThought(ThoughtDefOf.Catharsis);
                memory.moodOffset = (int)evt.moodOffset;
                pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(memory);
            }

            if (evt.hediffToApply != null)
            {
                Hediff existing = pawn.health?.hediffSet?.GetFirstHediffOfDef(evt.hediffToApply);
                if (existing == null)
                {
                    existing = HediffMaker.MakeHediff(evt.hediffToApply, pawn);
                    pawn.health?.AddHediff(existing);
                }
                if (evt.hediffSeverity > 0f)
                {
                    existing.Severity = Mathf.Clamp(existing.Severity + evt.hediffSeverity, 0f, existing.def.maxSeverity);
                }
            }

            if (evt.wakePawn && pawn.CurJobDef?.defName == "VA_UseVRPod")
            {
                pawn.jobs?.EndCurrentJob(Verse.AI.JobCondition.InterruptForced);
            }
        }
    }
}
