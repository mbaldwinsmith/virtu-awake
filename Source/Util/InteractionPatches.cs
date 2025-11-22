using HarmonyLib;
using RimWorld;
using Verse;

namespace VirtuAwake
{
    [HarmonyPatch(typeof(InteractionUtility), nameof(InteractionUtility.TryInteractWith))]
    public static class InteractionUtility_TryInteractWith_VRPatch
    {
        public static bool Prefix(Pawn pawn, Pawn target, InteractionDef intDef, ref bool __result)
        {
            if (VRSessionTracker.IsInVR(pawn) || VRSessionTracker.IsInVR(target))
            {
                __result = false;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Pawn_InteractionsTracker), "TryInteractWith")]
    public static class Pawn_InteractionsTracker_TryInteractWith_VRPatch
    {
        public static bool Prefix(Pawn ___pawn, Pawn recipient, ref bool __result)
        {
            if (VRSessionTracker.IsInVR(___pawn) || VRSessionTracker.IsInVR(recipient))
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
