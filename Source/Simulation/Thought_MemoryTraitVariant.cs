using System.Linq;
using RimWorld;
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

        private bool HasTrait(string defName)
        {
            if (string.IsNullOrEmpty(defName) || this.pawn?.story?.traits == null)
            {
                return false;
            }

            return this.pawn.story.traits.HasTrait(DefDatabase<TraitDef>.GetNamedSilentFail(defName));
        }
    }
}
