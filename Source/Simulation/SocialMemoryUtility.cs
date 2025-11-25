using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace VirtuAwake
{
    public static class SocialMemoryUtility
    {
        private static readonly ThoughtDef Ambient = DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRSocial_Link_Ambient");
        private static readonly ThoughtDef Friend = DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRSocial_Link_Friend");
        private static readonly ThoughtDef BestFriend = DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRSocial_Link_BestFriend");
        private static readonly ThoughtDef Lover = DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRSocial_Link_Lover");
        private static readonly ThoughtDef Family = DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRSocial_Link_Family");
        private static readonly ThoughtDef Rival = DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRSocial_Link_Rival");
        private static readonly ThoughtDef Empathic = DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRSocial_Link_Empathic");
        private static readonly ThoughtDef PsychicChord = DefDatabase<ThoughtDef>.GetNamedSilentFail("VA_VRSocial_Link_PsychicChord");

        public static void GiveNetworkSocialMemories(List<Pawn> pawns)
        {
            if (pawns.NullOrEmpty())
            {
                return;
            }

            foreach (Pawn pawn in pawns)
            {
                if (pawn?.needs?.mood?.thoughts?.memories == null)
                {
                    continue;
                }

                Pawn partner = PickPartner(pawn, pawns);
                ThoughtDef def = PickThoughtForPair(pawn, partner);

                if (def == null)
                {
                    continue;
                }

                pawn.needs.mood.thoughts.memories.TryGainMemory(def, partner);
                if (Prefs.DevMode)
                {
                    Log.Message($"[VA][Social] {pawn.LabelShortCap} gained social memory {def.defName} with {(partner?.LabelShortCap ?? "(null)") }.");
                }
            }
        }

        private static Pawn PickPartner(Pawn pawn, List<Pawn> pawns)
        {
            if (pawn == null || pawns.NullOrEmpty())
            {
                return null;
            }

            Pawn best = null;
            float bestScore = float.MinValue;

            foreach (Pawn other in pawns)
            {
                if (other == null || other == pawn)
                {
                    continue;
                }

                float opinion = pawn.relations?.OpinionOf(other) ?? 0f;
                float score = opinion;

                if (pawn.relations?.DirectRelationExists(PawnRelationDefOf.Spouse, other) == true ||
                    pawn.relations?.DirectRelationExists(PawnRelationDefOf.Fiance, other) == true ||
                    pawn.relations?.DirectRelationExists(PawnRelationDefOf.Lover, other) == true)
                {
                    score += 35f;
                }

                if (IsFamily(pawn, other))
                {
                    score += 15f;
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    best = other;
                }
            }

            return best;
        }

        private static ThoughtDef PickThoughtForPair(Pawn pawn, Pawn partner)
        {
            if (pawn == null || partner == null)
            {
                return Ambient;
            }

            bool lover = pawn.relations?.DirectRelationExists(PawnRelationDefOf.Spouse, partner) == true
                         || pawn.relations?.DirectRelationExists(PawnRelationDefOf.Fiance, partner) == true
                         || pawn.relations?.DirectRelationExists(PawnRelationDefOf.Lover, partner) == true;

            if (lover && Lover != null)
            {
                return Lover;
            }

            if (IsFamily(pawn, partner) && Family != null)
            {
                return Family;
            }

            float opinion = pawn.relations?.OpinionOf(partner) ?? 0f;

            if (opinion >= 70f && BestFriend != null)
            {
                return BestFriend;
            }

            if (opinion >= 30f && Friend != null)
            {
                return Friend;
            }

            if (opinion <= -25f && Rival != null)
            {
                return Rival;
            }

            // Trait-influenced picks
            if (pawn.story?.traits != null)
            {
                if ((HasTrait(pawn, "Kind") || HasTrait(pawn, "Empath")) && Empathic != null)
                {
                    return Empathic;
                }

                if ((HasTrait(pawn, "PsychicallySensitive") || HasTrait(pawn, "PsychicallyHypersensitive")) && PsychicChord != null)
                {
                    return PsychicChord;
                }
            }

            return Ambient;
        }

        private static bool HasTrait(Pawn pawn, string traitDefName)
        {
            if (pawn?.story?.traits == null || string.IsNullOrWhiteSpace(traitDefName))
            {
                return false;
            }

            return pawn.story.traits.HasTrait(DefDatabase<TraitDef>.GetNamedSilentFail(traitDefName));
        }

        private static bool IsFamily(Pawn pawn, Pawn other)
        {
            if (pawn == null || other == null)
            {
                return false;
            }

            PawnRelationDef rel = PawnRelationUtility.GetMostImportantRelation(pawn, other);
            if (rel == null)
            {
                return false;
            }

            return IsBloodRelation(rel);
        }

        private static bool IsBloodRelation(PawnRelationDef def)
        {
            if (def == null)
            {
                return false;
            }

            // Prefer explicit defName checks to avoid version-specific properties.
            string name = def.defName;
            switch (name)
            {
                case "Parent":
                case "Child":
                case "Grandparent":
                case "Grandchild":
                case "Sibling":
                case "HalfSibling":
                case "Cousin":
                case "AuntOrUncle":
                case "NieceOrNephew":
                case "Ancestor":
                case "Descendent":
                    return true;
                default:
                    return false;
            }
        }
    }
}
