using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace VirtuAwake
{
    public static class SharedDreamEventWorker
    {
        public static void ResolveGroupEvent(List<Pawn> pawns, MatrixEventDef evt, MatrixEventChainExtension chainExt)
        {
            if (pawns.NullOrEmpty() || evt == null || chainExt == null)
            {
                return;
            }

            float avgLucidity = 0f;
            float avgInstability = 0f;
            int lucidityCount = 0;
            int instabilityCount = 0;

            foreach (Pawn pawn in pawns)
            {
                Need_Lucidity lucidity = pawn?.needs?.TryGetNeed<Need_Lucidity>();
                if (lucidity != null)
                {
                    avgLucidity += lucidity.CurLevel;
                    lucidityCount++;
                }

                Hediff inst = pawn?.health?.hediffSet?.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamedSilentFail("VA_Instability"));
                if (inst != null)
                {
                    avgInstability += inst.Severity;
                    instabilityCount++;
                }
            }

            if (lucidityCount > 0)
            {
                avgLucidity /= lucidityCount;
            }

            if (instabilityCount > 0)
            {
                avgInstability /= instabilityCount;
            }

            float lucidityLerp = Mathf.Clamp01(chainExt.syncLucidityLerp);
            float instabilityLerp = Mathf.Clamp01(chainExt.syncInstabilityLerp);

            foreach (Pawn pawn in pawns)
            {
                if (pawn == null)
                {
                    continue;
                }

                Need_Lucidity lucidity = pawn.needs?.TryGetNeed<Need_Lucidity>();
                if (lucidity != null && lucidityCount > 0 && lucidityLerp > 0f)
                {
                    lucidity.CurLevel = Mathf.Clamp01(Mathf.Lerp(lucidity.CurLevel, avgLucidity, lucidityLerp));
                }

                Hediff inst = pawn.health?.hediffSet?.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamedSilentFail("VA_Instability"));
                if (inst != null && instabilityCount > 0 && instabilityLerp > 0f)
                {
                    float blended = inst.Severity + (avgInstability - inst.Severity) * instabilityLerp;
                    inst.Severity = Mathf.Clamp(blended, 0f, inst.def.maxSeverity);
                }

                if (chainExt.wakeAll || chainExt.forceEject)
                {
                    WakeFromVR(pawn, chainExt.forceEject);
                }
            }
        }

        private static void WakeFromVR(Pawn pawn, bool forceEject)
        {
            if (pawn == null)
            {
                return;
            }

            Job curJob = pawn.CurJob;
            Building pod = curJob?.targetA.Thing as Building;
            CompVRPod comp = pod?.TryGetComp<CompVRPod>();

            if (pawn.jobs != null)
            {
                pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
            }

            if (!forceEject)
            {
                return;
            }

            comp?.RemoveUser(pawn);
        }
    }
}
