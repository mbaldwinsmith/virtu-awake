using RimWorld;
using UnityEngine;
using Verse;

namespace VirtuAwake
{
    /// <summary>
    /// Shows lucidity/instability bars for the pawn currently in a VR pod.
    /// </summary>
    public class Gizmo_VRPawnStatus : Gizmo
    {
        private readonly Pawn pawn;
        private readonly Need_Lucidity lucidity;
        private readonly Hediff instability;

        public Gizmo_VRPawnStatus(Pawn pawn)
        {
            this.pawn = pawn;
            this.lucidity = pawn?.needs?.TryGetNeed<Need_Lucidity>();
            this.instability = pawn?.health?.hediffSet?.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamedSilentFail("VA_Instability"));
            this.Order = -10f;
        }

        public override float GetWidth(float maxWidth)
        {
            return 240f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Widgets.DrawWindowBackground(rect);

            Rect inner = rect.ContractedBy(6f);
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(inner.x, inner.y, inner.width, 24f), pawn.LabelShortCap);

            float barHeight = 18f;
            float gap = 4f;
            float y = inner.y + 24f + gap;

            if (lucidity != null)
            {
                DrawBar(inner.x, y, inner.width, barHeight, lucidity.CurLevel, new Color(0.35f, 0.7f, 1f), "Lucidity");
                y += barHeight + gap;
            }

            if (instability != null)
            {
                float inst = Mathf.Clamp01(instability.Severity / instability.def.maxSeverity);
                DrawBar(inner.x, y, inner.width, barHeight, inst, new Color(0.9f, 0.4f, 0.4f), "Instability");
            }

            return new GizmoResult(GizmoState.Clear);
        }

        private static void DrawBar(float x, float y, float width, float height, float fill, Color color, string label)
        {
            Rect barRect = new Rect(x, y, width, height);
            Widgets.DrawBoxSolid(barRect, Color.grey * 0.2f);
            Widgets.FillableBar(barRect, fill, SolidColorMaterials.NewSolidColorTexture(color), null, false);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(barRect, $"{label}: {fill.ToStringPercent()}");
            Text.Anchor = TextAnchor.UpperLeft;
        }
    }
}
