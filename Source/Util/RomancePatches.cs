using HarmonyLib;
using RimWorld;
using Verse;

namespace VirtuAwake
{
    [HarmonyPatch(typeof(LovePartnerRelationUtility), "TryDevelopLoveRelation")]
    public static class LovePartnerRelationUtility_TryDevelopLoveRelation_VRPatch
    {
        public static bool Prefix(Pawn initiator, Pawn target, ref bool __result)
        {
            if (VRSessionTracker.IsInVR(initiator) || VRSessionTracker.IsInVR(target))
            {
                __result = false;
                return false;
            }

            return true;
        }
    }
}
