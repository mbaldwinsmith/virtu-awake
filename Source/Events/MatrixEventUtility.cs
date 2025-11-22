using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace VirtuAwake
{
    public static class MatrixEventUtility
    {
        public static MatrixEventSeverity PickSeverity(float lucidity, float instability, Pawn pawn)
        {
            float combined = (lucidity + instability) * 0.5f;

            float minor = 0.45f;
            float moderate = 0.35f;
            float major = 0.15f;
            float crisis = 0.05f;

            minor += TraitTilt(pawn, 0.05f);
            moderate += TraitTilt(pawn, -0.02f);
            major += TraitTilt(pawn, -0.01f);
            crisis += TraitTilt(pawn, -0.02f);

            float severityBias = combined;
            minor *= 1f - severityBias * 0.4f;
            moderate *= 1f + severityBias * 0.2f;
            major *= 1f + severityBias * 0.5f;
            crisis *= 1f + severityBias * 0.7f;

            float total = minor + moderate + major + crisis;
            if (total <= 0f)
            {
                return MatrixEventSeverity.Minor;
            }

            float roll = Rand.Range(0f, total);
            if (roll < minor) return MatrixEventSeverity.Minor;
            roll -= minor;
            if (roll < moderate) return MatrixEventSeverity.Moderate;
            roll -= moderate;
            if (roll < major) return MatrixEventSeverity.Major;
            return MatrixEventSeverity.Crisis;
        }

        public static MatrixEventDef PickEvent(Pawn pawn, MatrixEventSeverity severity, float lucidity, float instability)
        {
            var pool = new List<MatrixEventDef>();
            foreach (MatrixEventDef def in DefDatabase<MatrixEventDef>.AllDefsListForReading)
            {
                if (def == null || def.severity != severity)
                {
                    continue;
                }

                MatrixEventChainExtension chainExt = def.GetModExtension<MatrixEventChainExtension>();
                if (chainExt != null && chainExt.chainOnly)
                {
                    continue;
                }

                if (lucidity < def.minLucidity || lucidity > def.maxLucidity)
                {
                    continue;
                }

                if (instability < def.minInstability || instability > def.maxInstability)
                {
                    continue;
                }

                pool.Add(def);
            }

            if (pool.Count == 0)
            {
                return null;
            }

            return pool.RandomElementByWeight(def => Mathf.Max(0.01f, def.baseWeight));
        }

        private static float TraitTilt(Pawn pawn, float delta)
        {
            if (pawn?.story?.traits == null)
            {
                return 0f;
            }

            if (HasTrait(pawn, "Paranoid") || HasTrait(pawn, "TooSmart") || HasTrait(pawn, "PsychicallyHypersensitive"))
            {
                return delta * 2f;
            }

            if (HasTrait(pawn, "Optimist") || HasTrait(pawn, "Sanguine"))
            {
                return -delta * 0.5f;
            }

            return 0f;
        }

        private static bool HasTrait(Pawn pawn, string defName)
        {
            return pawn?.story?.traits?.HasTrait(DefDatabase<TraitDef>.GetNamedSilentFail(defName)) ?? false;
        }
    }
}
