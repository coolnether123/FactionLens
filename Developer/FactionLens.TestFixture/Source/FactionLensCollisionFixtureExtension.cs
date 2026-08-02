using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using RimWorldAgent.Extensions;
using Verse;

namespace FactionLens.TestFixture
{
    public sealed class FactionLensCollisionFixtureExtension
        : IRimWorldAgentExtension
    {
        private const string ToolId = "factionlens-collision-fixture";
        private readonly List<WorldObject> spawned = new List<WorldObject>();

        public string Id => "CoolNether123.FactionLens.TestFixture";

        public int AbiVersion => AgentExtensionAbi.CurrentVersion;

        public IEnumerable<AgentToolDefinition> GetTools()
        {
            yield return new AgentToolDefinition(
                ToolId,
                "mod-fixtures",
                ToolId + " <run|cleanup>",
                "Create adjacent settlements for connector-layer verification.",
                true);
        }

        public string Execute(
            string toolId,
            string[] args,
            AgentToolContext context)
        {
            if (!string.Equals(toolId, ToolId, StringComparison.Ordinal))
            {
                return "unknown fixture tool: " + toolId;
            }

            string action = args != null && args.Length > 0
                ? args[0].ToLowerInvariant()
                : "run";
            return action == "run"
                ? Run()
                : action == "cleanup"
                    ? Cleanup()
                    : "usage: " + ToolId + " <run|cleanup>";
        }

        public void OnFrame(AgentFrameContext context)
        {
        }

        public void OnMapLoaded(AgentMapContext context)
        {
            Cleanup();
        }

        public void OnShutdown(AgentShutdownContext context)
        {
            Cleanup();
        }

        private string Run()
        {
            Cleanup();
            Settlement anchor = Find.WorldObjects.Settlements
                .FirstOrDefault(item => item.Faction == Faction.OfPlayer) ??
                Find.WorldObjects.Settlements.FirstOrDefault();
            if (anchor == null)
            {
                return "fixture failed: no settlement anchor";
            }

            var candidates = NearbyEmptyTiles(anchor.Tile, 10);
            for (int index = 0; index < candidates.Count; index++)
            {
                var settlement = (Settlement)WorldObjectMaker.MakeWorldObject(
                    WorldObjectDefOf.Settlement);
                settlement.Tile = candidates[index];
                settlement.SetFaction(Faction.OfPlayer);
                settlement.Name = "Connector Verification Settlement " +
                    (index + 1);
                Find.WorldObjects.Add(settlement);
                spawned.Add(settlement);
            }

            Find.World.renderer.wantedMode = WorldRenderMode.Planet;
            Find.WorldCameraDriver.JumpTo(anchor.Tile);
            return "result=PASS\nanchor=" + anchor.ID +
                "\ncreated=" + spawned.Count +
                "\nids=" + string.Join(",", spawned.Select(item => item.ID));
        }

        private string Cleanup()
        {
            int removed = 0;
            for (int index = spawned.Count - 1; index >= 0; index--)
            {
                WorldObject item = spawned[index];
                if (item != null && Find.WorldObjects.AllWorldObjects.Contains(item))
                {
                    Find.WorldObjects.Remove(item);
                    removed++;
                }
            }

            spawned.Clear();
            return "cleanup=PASS\nremoved=" + removed;
        }

        private static List<PlanetTile> NearbyEmptyTiles(
            PlanetTile anchor,
            int count)
        {
            var result = new List<PlanetTile>();
            var queue = new Queue<PlanetTile>();
            var seen = new HashSet<PlanetTile> { anchor };
            queue.Enqueue(anchor);
            while (queue.Count > 0 && result.Count < count)
            {
                PlanetTile current = queue.Dequeue();
                var neighbors = new List<PlanetTile>();
                Find.WorldGrid.GetTileNeighbors(current, neighbors);
                foreach (PlanetTile neighbor in neighbors)
                {
                    if (!seen.Add(neighbor))
                    {
                        continue;
                    }

                    queue.Enqueue(neighbor);
                    if (!Find.WorldGrid[neighbor].WaterCovered &&
                        !Find.WorldObjects.AnyWorldObjectAt(neighbor))
                    {
                        result.Add(neighbor);
                        if (result.Count == count)
                        {
                            break;
                        }
                    }
                }
            }

            return result;
        }
    }
}
