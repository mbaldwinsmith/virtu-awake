using HarmonyLib;
using RimWorld;
using Verse;

namespace VirtuAwake
{
    [HarmonyPatch(typeof(Pawn_InteractionsTracker), "TryInteractWith")]
    public static class Pawn_InteractionsTracker_TryInteractWith_VRPatch
    {
        public static bool Prefix(Pawn ___pawn, Pawn recipient, ref bool __result)
        {
            bool initiatorInVR = VRSessionTracker.IsInVR(___pawn);
            bool recipientInVR = VRSessionTracker.IsInVR(recipient);

            // If neither pawn is in VR, allow normal interactions.
            if (!initiatorInVR && !recipientInVR)
            {
                return true;
            }

            // Block interactions if only one pawn is in VR.
            if (!initiatorInVR || !recipientInVR)
            {
                __result = false;
                return false;
            }

            // Both are in VR: only allow if they share the same power network.
            PowerNet initiatorNet = VRSessionTracker.GetPowerNet(___pawn);
            PowerNet recipientNet = VRSessionTracker.GetPowerNet(recipient);
            if (initiatorNet != null && initiatorNet == recipientNet)
            {
                return true;
            }

            __result = false;
            return false;
        }
    }
}
