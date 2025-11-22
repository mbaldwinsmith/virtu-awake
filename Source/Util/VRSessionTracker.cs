using System.Collections.Generic;
using RimWorld;
using Verse;

namespace VirtuAwake
{
    public static class VRSessionTracker
    {
        private static readonly HashSet<int> ActivePawnIds = new HashSet<int>();
        private static readonly Dictionary<int, CompVRPod> PawnPods = new Dictionary<int, CompVRPod>();

        public static void Register(Pawn pawn, CompVRPod pod)
        {
            if (pawn == null)
            {
                return;
            }

            ActivePawnIds.Add(pawn.thingIDNumber);

            if (pod != null)
            {
                PawnPods[pawn.thingIDNumber] = pod;
            }
        }

        public static void Unregister(Pawn pawn)
        {
            if (pawn == null)
            {
                return;
            }

            ActivePawnIds.Remove(pawn.thingIDNumber);
            PawnPods.Remove(pawn.thingIDNumber);
        }

        public static bool IsInVR(Pawn pawn)
        {
            if (pawn == null)
            {
                return false;
            }

            return ActivePawnIds.Contains(pawn.thingIDNumber);
        }

        public static CompVRPod GetPod(Pawn pawn)
        {
            if (pawn == null)
            {
                return null;
            }

            PawnPods.TryGetValue(pawn.thingIDNumber, out var pod);
            return pod;
        }

        public static PowerNet GetPowerNet(Pawn pawn)
        {
            CompVRPod pod = GetPod(pawn);
            CompPowerTrader power = pod?.parent?.TryGetComp<CompPowerTrader>();
            return power?.PowerNet;
        }
    }
}
