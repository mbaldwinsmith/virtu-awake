using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace VirtuAwake
{
    public static class VRSimUtility
    {
        public static SimTypeDef DefaultSimType => DefDatabase<SimTypeDef>.GetNamedSilentFail("VA_Sim_Recreation");

        public static void ApplySimTraining(Pawn pawn, SimTypeDef simType, int ticks)
        {
            if (pawn?.skills == null)
            {
                return;
            }

            if (simType == null || simType.skillWeights == null || simType.skillWeights.Count == 0)
            {
                return;
            }

            float baseXp = simType.xpPerTick * ticks * 0.5f; // much slower XP to match slower recreation
            // Small random variance independent of skill level to keep sessions from feeling identical.
            baseXp *= Rand.Range(0.9f, 1.1f);
            if (baseXp <= 0f)
            {
                return;
            }

            float totalWeight = 0f;
            foreach (var pair in simType.skillWeights)
            {
                if (pair.Key == null || pair.Value <= 0f)
                {
                    continue;
                }

                totalWeight += pair.Value * PassionFactor(pawn, pair.Key);
            }

            if (totalWeight <= 0f)
            {
                return;
            }

            foreach (var pair in simType.skillWeights)
            {
                if (pair.Key == null || pair.Value <= 0f)
                {
                    continue;
                }

                float weight = pair.Value * PassionFactor(pawn, pair.Key);
                float scaledXp = baseXp * (weight / totalWeight);
                pawn.skills.Learn(pair.Key, scaledXp, direct: true);
            }
        }

        private static float PassionFactor(Pawn pawn, SkillDef skill)
        {
            var passion = pawn.skills?.GetSkill(skill)?.passion ?? Passion.None;
            return passion switch
            {
                Passion.Major => 1.6f,
                Passion.Minor => 1.3f,
                _ => 1f
            };
        }

        public static SimTypeDef ResolveBestSimTypeForPawn(Pawn pawn, SimTypeDef fallback)
        {
            var scored = new List<(SimTypeDef sim, float weight)>();

            foreach (SimTypeDef sim in DefDatabase<SimTypeDef>.AllDefsListForReading)
            {
                if (sim.primarySkill == null || sim.skillWeights == null || sim.skillWeights.Count == 0)
                {
                    continue;
                }

                float score = ScoreSimForPawn(pawn, sim);
                if (fallback != null && sim == fallback)
                {
                    score *= 1.05f;
                }

                if (score <= 0f)
                {
                    continue;
                }

                // Add a deterministic random variance so picks vary over time.
                int seed = pawn?.thingIDNumber.GetHashCode() ?? 0;
                int mix = seed ^ sim.defName.GetHashCode();
                Rand.PushState(mix);
                float variance = Rand.Range(0.65f, 1.35f);
                Rand.PopState();

                float finalWeight = score * variance;
                scored.Add((sim, finalWeight));
            }

            if (scored.Count == 0)
            {
                return fallback ?? DefaultSimType;
            }

            var top = scored.OrderByDescending(s => s.weight).Take(3).ToList();
            return top.TryRandomElementByWeight(s => s.weight, out var chosen)
                ? chosen.sim
                : fallback ?? DefaultSimType;
        }

        public static void TryGiveSimMemory(Pawn pawn, SimTypeDef simType)
        {
            if (pawn == null || simType == null)
            {
                return;
            }

            SkillDef dominantSkill = GetDominantSkill(pawn, simType);
            int tier = PickTier(pawn, dominantSkill);

            float joyGain = simType.joyGainTier1;
            joyGain = tier switch
            {
                1 => simType.joyGainTier1,
                2 => simType.joyGainTier2,
                _ => simType.joyGainTier3
            };

            ThoughtDef chosen = VRMemorySelector.SelectSimMemory(pawn, simType, dominantSkill, tier);

            if (chosen != null)
            {
                pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(chosen);
            }
            else if (simType.thoughtOnSession != null)
            {
                pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(simType.thoughtOnSession);
            }

            var joy = pawn.needs?.joy;
            if (joy != null && joyGain > 0f)
            {
                JoyKindDef kind = (dominantSkill ?? simType.primarySkill) == SkillDefOf.Social ? JoyKindDefOf.Social : JoyKindDefOf.Meditative;
                joy.GainJoy(joyGain, kind);
            }
        }

        private static float ScoreSimForPawn(Pawn pawn, SimTypeDef sim)
        {
            if (pawn?.skills == null || sim?.skillWeights == null)
            {
                return 0f;
            }

            float score = 0f;
            foreach (var pair in sim.skillWeights)
            {
                if (pair.Key == null || pair.Value <= 0f)
                {
                    continue;
                }

                float passion = PassionFactor(pawn, pair.Key);
                float levelFactor = 0.7f + ((pawn.skills.GetSkill(pair.Key)?.Level ?? 0) / 40f);
                score += pair.Value * passion * levelFactor;
            }

            return score;
        }

        private static SkillDef GetDominantSkill(Pawn pawn, SimTypeDef simType)
        {
            if (pawn?.skills == null || simType?.skillWeights == null || simType.skillWeights.Count == 0)
            {
                return simType?.primarySkill;
            }

            SkillDef chosen = simType.primarySkill;
            float best = 0f;
            foreach (var pair in simType.skillWeights)
            {
                if (pair.Key == null || pair.Value <= 0f)
                {
                    continue;
                }

                float score = pair.Value * PassionFactor(pawn, pair.Key);
                score *= 0.7f + ((pawn.skills.GetSkill(pair.Key)?.Level ?? 0) / 40f);

                if (score > best)
                {
                    best = score;
                    chosen = pair.Key;
                }
            }

            return chosen ?? simType.primarySkill;
        }

        private static int PickTier(Pawn pawn, SkillDef skill)
        {
            int level = pawn?.skills?.GetSkill(skill)?.Level ?? 0;
            int tier = level < 6 ? 1 : level < 12 ? 2 : 3;

            float bias = TierBiasFromTraits(pawn, skill);
            float upChance = 0.12f + Mathf.Max(0f, bias) * 0.06f;
            float downChance = 0.08f + Mathf.Max(0f, -bias) * 0.05f;

            // Add a little extra chance to bump up when near thresholds.
            if (tier == 1 && level >= 5) upChance += 0.05f;
            if (tier == 2 && level >= 11) upChance += 0.05f;
            if (tier == 3 && level <= 13) downChance += 0.04f;

            if (Rand.Chance(upChance))
            {
                tier = Mathf.Min(3, tier + 1);
            }
            else if (Rand.Chance(downChance))
            {
                tier = Mathf.Max(1, tier - 1);
            }

            return tier;
        }

        private static float TierBiasFromTraits(Pawn pawn, SkillDef skill)
        {
            if (pawn?.story?.traits == null)
            {
                return 0f;
            }

            float bias = 0f;

            // Global positives
            if (HasTrait(pawn, "Sanguine"))
            {
                bias += 0.6f;
            }
            if (HasTrait(pawn, "Optimist"))
            {
                bias += 0.25f;
            }

            // Global negatives
            if (HasTrait(pawn, "Depressive") || HasTrait(pawn, "Pessimist"))
            {
                bias -= 0.6f;
            }
            if (HasTrait(pawn, "Neurotic") || HasTrait(pawn, "VeryNeurotic") || HasTrait(pawn, "Nervous"))
            {
                bias -= 0.35f;
            }
            if (HasTrait(pawn, "SlowLearner"))
            {
                bias -= 0.35f;
            }

            // Skill-flavoured adjustments
            if (skill == SkillDefOf.Melee && HasTrait(pawn, "Brawler"))
            {
                bias += 0.5f;
            }
            if (skill == SkillDefOf.Cooking && HasTrait(pawn, "Gourmand"))
            {
                bias += 0.4f;
            }
            if ((skill == SkillDefOf.Social || skill == SkillDefOf.Animals || skill == SkillDefOf.Medicine || skill == SkillDefOf.Artistic) && HasTrait(pawn, "Kind"))
            {
                bias += 0.4f;
            }
            if (HasTrait(pawn, "BodyPurist") && (skill == SkillDefOf.Medicine || skill == SkillDefOf.Shooting))
            {
                bias -= 0.35f;
            }
            if (HasTrait(pawn, "PsychicallySensitive") || HasTrait(pawn, "PsychicallyHypersensitive"))
            {
                bias += 0.15f;
            }
            if (HasTrait(pawn, "PsychicallyDull") || HasTrait(pawn, "PsychicallyDeaf"))
            {
                bias -= 0.15f;
            }
            // Emotional sims feel better for psy-sensitive, worse for psy-dull.
            if (skill == SkillDefOf.Social || skill == SkillDefOf.Animals || skill == SkillDefOf.Artistic || skill == SkillDefOf.Medicine)
            {
                if (HasTrait(pawn, "PsychicallySensitive") || HasTrait(pawn, "PsychicallyHypersensitive"))
                {
                    bias += 0.1f;
                }
                if (HasTrait(pawn, "PsychicallyDull") || HasTrait(pawn, "PsychicallyDeaf"))
                {
                    bias -= 0.1f;
                }
            }
            else if (skill == SkillDefOf.Construction || skill == SkillDefOf.Mining || skill == SkillDefOf.Intellectual || skill == SkillDefOf.Crafting || skill == SkillDefOf.Shooting)
            {
                if (HasTrait(pawn, "PsychicallySensitive") || HasTrait(pawn, "PsychicallyHypersensitive"))
                {
                    bias -= 0.1f;
                }
                if (HasTrait(pawn, "PsychicallyDull") || HasTrait(pawn, "PsychicallyDeaf"))
                {
                    bias += 0.05f;
                }
            }

            // Ascetic bonus on plain/quiet sims, penalty on expressive ones.
            if (skill == SkillDefOf.Artistic || skill == SkillDefOf.Social)
            {
                if (HasTrait(pawn, "Ascetic"))
                {
                    bias -= 0.25f;
                }
            }
            else if (skill == SkillDefOf.Mining || skill == SkillDefOf.Construction || skill == SkillDefOf.Plants || skill == SkillDefOf.Intellectual || skill == SkillDefOf.Crafting)
            {
                if (HasTrait(pawn, "Ascetic"))
                {
                    bias += 0.25f;
                }
            }

            return bias;
        }

        private static bool HasTrait(Pawn pawn, string traitDefName)
        {
            if (pawn?.story?.traits == null || string.IsNullOrEmpty(traitDefName))
            {
                return false;
            }

            return pawn.story.traits.HasTrait(DefDatabase<TraitDef>.GetNamedSilentFail(traitDefName));
        }

    }
}
