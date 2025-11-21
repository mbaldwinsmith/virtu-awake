using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace VirtuAwake
{
    public class Thought_MemoryTraitVariant : Thought_Memory
    {
        public override string Description
        {
            get
            {
                var baseDescription = base.Description;
                var ext = this.def.GetModExtension<TraitMemoryExtension>();
                if (ext == null || ext.variants == null || ext.variants.Count == 0 || this.pawn == null)
                {
                    return baseDescription;
                }

                var overlays = CollectOverlays(ext);
                if (overlays.Count == 0)
                {
                    return baseDescription;
                }

                string description = baseDescription;
                foreach (var overlay in overlays)
                {
                    description = AppendVariantText(description, overlay);
                }

                return description;
            }
        }

        public override float MoodOffset()
        {
            float offset = base.MoodOffset();
            offset += TraitMoodDelta(this.pawn, GetSkillFromDefName(this.def?.defName));
            return offset;
        }

        private bool HasTrait(string defName)
        {
            if (string.IsNullOrEmpty(defName) || this.pawn?.story?.traits == null)
            {
                return false;
            }

            return this.pawn.story.traits.HasTrait(DefDatabase<TraitDef>.GetNamedSilentFail(defName));
        }

        private static SkillDef GetSkillFromDefName(string defName)
        {
            if (string.IsNullOrEmpty(defName))
            {
                return null;
            }

            if (defName.Contains("Shooting")) return SkillDefOf.Shooting;
            if (defName.Contains("Melee")) return SkillDefOf.Melee;
            if (defName.Contains("Construction")) return SkillDefOf.Construction;
            if (defName.Contains("Mining")) return SkillDefOf.Mining;
            if (defName.Contains("Cooking")) return SkillDefOf.Cooking;
            if (defName.Contains("Plants")) return SkillDefOf.Plants;
            if (defName.Contains("Animals")) return SkillDefOf.Animals;
            if (defName.Contains("Crafting")) return SkillDefOf.Crafting;
            if (defName.Contains("Artistic")) return SkillDefOf.Artistic;
            if (defName.Contains("Medical")) return SkillDefOf.Medicine;
            if (defName.Contains("Social")) return SkillDefOf.Social;
            if (defName.Contains("Intellectual")) return SkillDefOf.Intellectual;

            return null;
        }

        private float TraitMoodDelta(Pawn pawn, SkillDef skill)
        {
            if (pawn?.story?.traits == null)
            {
                return 0f;
            }

            float delta = 0f;

            // Global modifiers (Documentation: Sanguine +1 tier, Depressive -2, Neurotic -1)
            if (HasTrait("Sanguine"))
            {
                delta += 1f;
            }

            if (HasTrait("Depressive"))
            {
                delta -= 2f;
            }

            if (HasTrait("Neurotic") || HasTrait("VeryNeurotic"))
            {
                delta -= 1f;
            }

            // Mild optimism/pessimism outside the main table
            if (HasTrait("Optimist"))
            {
                delta += 0.5f;
            }

            if (HasTrait("Pessimist"))
            {
                delta -= 0.5f;
            }

            // Skill-flavoured modifiers from MEMORY_GUIDE
            if (skill == SkillDefOf.Melee && HasTrait("Brawler"))
            {
                delta += 1f;
            }

            if (skill == SkillDefOf.Cooking && HasTrait("Gourmand"))
            {
                delta += 1f;
            }

            if ((skill == SkillDefOf.Social || skill == SkillDefOf.Animals || skill == SkillDefOf.Medicine || skill == SkillDefOf.Artistic) && HasTrait("Kind"))
            {
                delta += 1f;
            }

            if ((skill == SkillDefOf.Social || skill == SkillDefOf.Animals || skill == SkillDefOf.Artistic || skill == SkillDefOf.Medicine) &&
                (HasTrait("PsychicallySensitive") || HasTrait("PsychicallyHypersensitive")))
            {
                delta += 0.5f;
            }

            if ((skill == SkillDefOf.Social || skill == SkillDefOf.Animals || skill == SkillDefOf.Artistic || skill == SkillDefOf.Medicine) &&
                (HasTrait("PsychicallyDull") || HasTrait("PsychicallyDeaf")))
            {
                delta -= 0.5f;
            }

            // Psy-sensitive tilt down slightly on hollow/mechanical sims
            if ((skill == SkillDefOf.Construction || skill == SkillDefOf.Mining || skill == SkillDefOf.Intellectual || skill == SkillDefOf.Crafting || skill == SkillDefOf.Shooting) &&
                (HasTrait("PsychicallySensitive") || HasTrait("PsychicallyHypersensitive")))
            {
                delta -= 0.25f;
            }
            if ((skill == SkillDefOf.Construction || skill == SkillDefOf.Mining || skill == SkillDefOf.Intellectual || skill == SkillDefOf.Crafting || skill == SkillDefOf.Shooting) &&
                (HasTrait("PsychicallyDull") || HasTrait("PsychicallyDeaf")))
            {
                delta += 0.1f;
            }

            if ((skill == SkillDefOf.Medicine || skill == SkillDefOf.Shooting) && HasTrait("BodyPurist"))
            {
                delta -= 1f;
            }

            // Ascetic: prefers simple/quiet sims; applies a small penalty on expressive/flashy ones.
            if ((skill == SkillDefOf.Artistic || skill == SkillDefOf.Social) && HasTrait("Ascetic"))
            {
                delta -= 0.75f;
            }

            // Ascetic bonus on plain/quiet sims.
            if ((skill == SkillDefOf.Mining || skill == SkillDefOf.Construction || skill == SkillDefOf.Plants || skill == SkillDefOf.Intellectual || skill == SkillDefOf.Crafting) && HasTrait("Ascetic"))
            {
                delta += 0.5f;
            }

            // Clamp to keep offsets reasonable (+/- 3 tiers worth of base mood spread).
            return Mathf.Clamp(delta, -3f, 3f);
        }

        private static string AppendVariantText(string baseDescription, string variantText)
        {
            if (string.IsNullOrEmpty(variantText))
            {
                return baseDescription;
            }

            if (string.IsNullOrEmpty(baseDescription))
            {
                return variantText;
            }

            return $"{baseDescription} {variantText}".Trim();
        }

        private List<string> CollectOverlays(TraitMemoryExtension ext)
        {
            const int MaxOverlays = 3;
            var overlays = new List<string>(MaxOverlays);
            var usedTraits = new HashSet<string>();
            int currentTier = GetTierFromDefName(this.def?.defName);

            // Prefer combo matches first to capture bespoke pairs without losing singles later.
            foreach (var variant in ext.variants)
            {
                if (overlays.Count >= MaxOverlays)
                {
                    break;
                }

                if (variant == null || string.IsNullOrWhiteSpace(variant.text))
                {
                    continue;
                }

                if (variant.tier > 0 && variant.tier != currentTier)
                {
                    continue;
                }

                var traits = GetTraitsForVariant(variant);
                if (traits.Count < 2)
                {
                    continue;
                }

                if (overlays.Count >= MaxOverlays)
                {
                    break;
                }

                if (traits.Any(t => usedTraits.Contains(t)))
                {
                    continue;
                }

                if (traits.Any(t => !HasTrait(t)))
                {
                    continue;
                }

                overlays.Add(variant.text.Trim());
                foreach (var t in traits)
                {
                    usedTraits.Add(t);
                }
            }

            // Then add single-trait overlays until the cap is reached.
            foreach (var variant in ext.variants)
            {
                if (overlays.Count >= MaxOverlays)
                {
                    break;
                }

                if (variant == null || string.IsNullOrWhiteSpace(variant.text))
                {
                    continue;
                }

                if (variant.tier > 0 && variant.tier != currentTier)
                {
                    continue;
                }

                var traits = GetTraitsForVariant(variant);
                if (traits.Count != 1)
                {
                    continue;
                }

                string trait = traits[0];
                if (!HasTrait(trait) || usedTraits.Contains(trait))
                {
                    continue;
                }

                overlays.Add(variant.text.Trim());
                usedTraits.Add(trait);
            }

            return overlays;
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

        private static int GetTierFromDefName(string defName)
        {
            if (string.IsNullOrEmpty(defName))
            {
                return 0;
            }

            if (defName.EndsWith("_T1"))
            {
                return 1;
            }

            if (defName.EndsWith("_T2"))
            {
                return 2;
            }

            if (defName.EndsWith("_T3"))
            {
                return 3;
            }

            return 0;
        }
    }
}
