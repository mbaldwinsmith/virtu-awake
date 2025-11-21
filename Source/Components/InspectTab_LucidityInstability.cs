using System.Text;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace VirtuAwake
{
    [StaticConstructorOnStartup]
    public static class InspectTab_LucidityInstability
    {
        static InspectTab_LucidityInstability()
        {
            var harmony = new Harmony("VirtuAwake.InspectTab.LucidityInstability");
            harmony.Patch(
                original: AccessTools.Method(typeof(Pawn), nameof(Pawn.GetInspectString)),
                postfix: new HarmonyMethod(typeof(InspectTab_LucidityInstability), nameof(Postfix))
            );
        }

        public static void Postfix(Pawn __instance, ref string __result)
        {
            if (__instance == null || !IsInVRJob(__instance))
            {
                return;
            }

            Need_Lucidity lucidity = __instance.needs?.TryGetNeed<Need_Lucidity>();
            if (lucidity == null)
            {
                return;
            }

            StringBuilder sb = new StringBuilder(__result ?? string.Empty);
            if (lucidity != null)
            {
                sb.AppendLine("Lucidity: " + lucidity.CurLevelPercentage.ToStringPercent());
            }

            __result = sb.ToString().TrimEndNewlines();
        }

        private static bool IsInVRJob(Pawn pawn)
        {
            JobDef job = pawn.CurJobDef;
            if (job == null) return false;
            return job.defName == "VA_UseVRPod" || job.defName == "VA_UseVirtuDreamPod" || job.defName == "VA_UseVRPodSocial";
        }
    }
}
