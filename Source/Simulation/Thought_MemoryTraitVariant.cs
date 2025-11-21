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
                var ext = this.def.GetModExtension<TraitMemoryExtension>();
                if (ext == null || ext.variants == null || ext.variants.Count == 0 || this.pawn == null)
                {
                    return base.Description;
                }

                // Prefer combo matches, then single trait.
                foreach (var variant in ext.variants.Where(v => !string.IsNullOrEmpty(v.trait) && !string.IsNullOrEmpty(v.trait2)))
                {
                    if (HasTrait(variant.trait) && HasTrait(variant.trait2))
                    {
                        return variant.text;
                    }
                }

                foreach (var variant in ext.variants.Where(v => !string.IsNullOrEmpty(v.trait) && string.IsNullOrEmpty(v.trait2)))
                {
                    if (HasTrait(variant.trait))
                    {
                        return variant.text;
                    }
                }

                return base.Description;
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
    }
}
