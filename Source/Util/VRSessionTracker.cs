using System.Collections.Generic;
using Verse;

namespace VirtuAwake
{
    public static class VRSessionTracker
    {
        private static readonly HashSet<int> ActivePawnIds = new HashSet<int>();

        public static void Register(Pawn pawn)
        {
            if (pawn != null)
            {
                ActivePawnIds.Add(pawn.thingIDNumber);
            }
        }

        public static void Unregister(Pawn pawn)
        {
            if (pawn != null)
            {
                ActivePawnIds.Remove(pawn.thingIDNumber);
            }
        }

        public static bool IsInVR(Pawn pawn)
        {
            if (pawn == null)
            {
                return false;
            }

            return ActivePawnIds.Contains(pawn.thingIDNumber);
        }
    }
}
