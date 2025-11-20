using System.Collections.Generic;
using Verse;

namespace VirtuAwake
{
    public class TraitMemoryVariant
    {
        public string trait;
        public string trait2;
        public string text;
    }

    public class TraitMemoryExtension : DefModExtension
    {
        public List<TraitMemoryVariant> variants = new List<TraitMemoryVariant>();
    }
}
