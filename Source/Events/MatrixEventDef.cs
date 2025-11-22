using RimWorld;
using Verse;

namespace VirtuAwake
{
    public class MatrixEventDef : Def
    {
        public MatrixEventSeverity severity;
        public float baseWeight = 1f;
        public float minLucidity = 0f;
        public float minInstability = 0f;
        public float maxLucidity = 1f;
        public float maxInstability = 1f;
        public ThoughtDef thoughtOnFire;
        public string letterLabelKey;
        public string letterTextKey;
        public LetterDef letterDef;
        public float lucidityOffset;
        public float instabilityOffset;
        public float moodOffset;
        public HediffDef hediffToApply;
        public float hediffSeverity;
        public bool wakePawn;
    }
}
