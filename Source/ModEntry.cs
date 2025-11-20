using HarmonyLib;
using Verse;

namespace VirtuAwake
{
    /// <summary>
    /// Entry point for Virtu-Awake. Sets up Harmony patching.
    /// </summary>
    public class VirtuAwakeMod : Mod
    {
        public VirtuAwakeMod(ModContentPack content)
            : base(content)
        {
            var harmony = new Harmony("MarkBaldwinSmith.VirtuAwake");
            harmony.PatchAll();
        }
    }
}
