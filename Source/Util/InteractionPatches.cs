using HarmonyLib;
using RimWorld;
using Verse;

namespace VirtuAwake
{
    [HarmonyPatch(typeof(InteractionUtility), nameof(InteractionUtility.CanInitiateInteraction))]
    public static class InteractionUtility_CanInitiateInteraction_VRPatch
    {
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            if (__result && VRSessionTracker.IsInVR(pawn))
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(InteractionUtility), nameof(InteractionUtility.CanReceiveInteraction))]
    public static class InteractionUtility_CanReceiveInteraction_VRPatch
    {
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            if (__result && VRSessionTracker.IsInVR(pawn))
            {
                __result = false;
            }
        }
    }
}
