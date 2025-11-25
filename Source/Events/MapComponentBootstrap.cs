using RimWorld;
using Verse;

namespace VirtuAwake
{
    [StaticConstructorOnStartup]
    public static class MapComponentBootstrap
    {
        static MapComponentBootstrap()
        {
            LongEventHandler.ExecuteWhenFinished(AddComponents);
        }

        private static void AddComponents()
        {
            var maps = Find.Maps;
            if (maps == null)
            {
                return;
            }

            foreach (Map map in maps)
            {
                if (map == null)
                {
                    continue;
                }

                if (map.components == null)
                {
                    map.components = new System.Collections.Generic.List<MapComponent>();
                }

                if (map.GetComponent<MapComponent_VirtuAwakeEvents>() == null)
                {
                    map.components.Add(new MapComponent_VirtuAwakeEvents(map));
                }
            }
        }
    }
}
