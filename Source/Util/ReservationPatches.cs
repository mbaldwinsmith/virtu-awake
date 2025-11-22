using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace VirtuAwake
{
    /// <summary>
    /// Prevents pawns from reserving VR-immersed pawns for vanilla jobs (rescue, arrest, carry, strip, etc.).
    /// </summary>
    [HarmonyPatch(typeof(ReservationUtility), nameof(ReservationUtility.CanReserve))]
    public static class ReservationUtility_CanReserve_VRPatch
    {
        public static bool Prefix(Pawn claimant, LocalTargetInfo target, int maxPawns, int stackCount, ReservationLayerDef layer, bool ignoreOtherReservations, ref bool __result)
        {
            Pawn targetPawn = target.Thing as Pawn;
            if (targetPawn != null && claimant != targetPawn && VRSessionTracker.IsInVR(targetPawn))
            {
                __result = false;
                return false;
            }

            return true;
        }
    }
}
