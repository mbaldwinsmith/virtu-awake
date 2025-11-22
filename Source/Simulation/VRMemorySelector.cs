using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace VirtuAwake
{
    public static class VRMemorySelector
    {
        public static ThoughtDef SelectSimMemory(Pawn pawn, SimTypeDef simType, SkillDef dominantSkill, int tier)
        {
            ThoughtDef comboThought = PickComboThought(pawn, tier);

            ThoughtDef tierThought = tier switch
            {
                1 => simType?.thoughtTier1,
                2 => simType?.thoughtTier2,
                _ => simType?.thoughtTier3
            };

            ThoughtDef chosen = comboThought
                                ?? tierThought
                                ?? simType?.thoughtOnSession
                                ?? simType?.thoughtTier1;

            if (chosen == null)
            {
                chosen = PickTraitMemory(pawn) ?? PickBaseMemory(tier);
            }

            return chosen;
        }

        public static ThoughtDef SelectEventMemory(Pawn pawn, MatrixEventSeverity severity, ThoughtDef overrideThought)
        {
            if (overrideThought != null)
            {
                return overrideThought;
            }

            return PickTraitMemory(pawn) ?? PickBaseEventMemory(severity);
        }

        public static ThoughtDef PickTraitMemory(Pawn pawn)
        {
            if (pawn?.story?.traits == null)
            {
                return null;
            }

            var candidates = new List<ThoughtDef>();

            TryAddTraitMemory(pawn, candidates, "Pessimist", "VA_VRTrait_Pessimist");
            TryAddTraitMemory(pawn, candidates, "Optimist", "VA_VRTrait_Optimist");
            TryAddTraitMemory(pawn, candidates, "TooSmart", "VA_VRTrait_TooSmart");
            TryAddTraitMemory(pawn, candidates, "Paranoid", "VA_VRTrait_Paranoid");
            TryAddTraitMemory(pawn, candidates, "Sanguine", "VA_VRTrait_Sanguine");
            TryAddTraitMemory(pawn, candidates, "Masochist", "VA_VRTrait_Masochist");
            TryAddTraitMemory(pawn, candidates, "Pyromaniac", "VA_VRTrait_Pyromaniac");
            TryAddTraitMemory(pawn, candidates, "Ascetic", "VA_VRTrait_Ascetic");
            TryAddTraitMemory(pawn, candidates, "Greedy", "VA_VRTrait_Greedy");
            TryAddTraitMemory(pawn, candidates, "Kind", "VA_VRTrait_Kind");
            TryAddTraitMemory(pawn, candidates, "BodyPurist", "VA_VRTrait_BodyPurist");
            TryAddTraitMemory(pawn, candidates, "Psychopath", "VA_VRTrait_Psychopath");
            TryAddTraitMemory(pawn, candidates, "PsychicallySensitive", "VA_VRTrait_PsychicSensitive");
            TryAddTraitMemory(pawn, candidates, "PsychicallyHypersensitive", "VA_VRTrait_PsychicSensitive");
            TryAddTraitMemory(pawn, candidates, "Nudist", "VA_VRTrait_Nudist");

            if (candidates.Count == 0)
            {
                return null;
            }

            return candidates.RandomElement();
        }

        public static ThoughtDef PickBaseMemory(int tier)
        {
            return tier switch
            {
                1 => DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRMemory_Base_SoftGlint"),
                2 => DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRMemory_Base_ParallaxEcho"),
                _ => DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRMemory_Base_HeavyExit") ?? DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRMemory_Base_CrisisStutter")
            };
        }

        public static ThoughtDef PickBaseEventMemory(MatrixEventSeverity severity)
        {
            return severity switch
            {
                MatrixEventSeverity.Minor => DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRMemory_Base_SoftGlint"),
                MatrixEventSeverity.Moderate => DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRMemory_Base_ParallaxEcho"),
                MatrixEventSeverity.Major => DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRMemory_Base_HeavyExit"),
                MatrixEventSeverity.Crisis => DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRMemory_Base_CrisisStutter"),
                _ => null
            };
        }

        private static ThoughtDef PickComboThought(Pawn pawn, int tier)
        {
            if (pawn?.story?.traits == null)
            {
                return null;
            }

            ThoughtDef best = null;
            int bestTraitCount = 1;

            foreach (var def in DefDatabase<ThoughtDef>.AllDefsListForReading)
            {
                if (string.IsNullOrEmpty(def?.defName) || !def.defName.StartsWith("VA_VRCombo_"))
                {
                    continue;
                }

                var ext = def.GetModExtension<TraitMemoryExtension>();
                if (ext?.variants == null)
                {
                    continue;
                }

                foreach (var variant in ext.variants)
                {
                    if (variant == null)
                    {
                        continue;
                    }

                    if (variant.tier > 0 && variant.tier != tier)
                    {
                        continue;
                    }

                    List<string> traits = GetTraitsForVariant(variant);
                    int traitCount = traits.Count;
                    if (traitCount <= 1)
                    {
                        continue;
                    }

                    if (!traits.All(t => HasTrait(pawn, t)))
                    {
                        continue;
                    }

                    if (traitCount > bestTraitCount)
                    {
                        bestTraitCount = traitCount;
                        best = def;
                    }
                }
            }

            return best;
        }

        private static List<string> GetTraitsForVariant(TraitMemoryVariant variant)
        {
            var traits = new List<string>();
            if (variant == null)
            {
                return traits;
            }

            if (variant.traits != null)
            {
                foreach (var t in variant.traits)
                {
                    if (!string.IsNullOrEmpty(t) && !traits.Contains(t))
                    {
                        traits.Add(t);
                    }
                }
            }

            if (!string.IsNullOrEmpty(variant.trait) && !traits.Contains(variant.trait))
            {
                traits.Add(variant.trait);
            }

            if (!string.IsNullOrEmpty(variant.trait2) && !traits.Contains(variant.trait2))
            {
                traits.Add(variant.trait2);
            }

            return traits;
        }

        private static bool HasTrait(Pawn pawn, string traitDefName)
        {
            return pawn?.story?.traits?.HasTrait(DefDatabase<TraitDef>.GetNamedSilentFail(traitDefName)) ?? false;
        }

        private static void TryAddTraitMemory(Pawn pawn, List<ThoughtDef> list, string traitDefName, string thoughtDefName)
        {
            if (!HasTrait(pawn, traitDefName))
            {
                return;
            }

            ThoughtDef thought = DefDatabase<ThoughtDef>.GetNamedSilentFail(thoughtDefName);
            if (thought != null)
            {
                list.Add(thought);
            }
        }
    }
}
