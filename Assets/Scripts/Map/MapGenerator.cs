using System.Collections.Generic;
using UnityEngine;

public static class MapGenerator
{
    public static MapGraphData CreateSampleRun()
    {
        MapGraphData graph = new MapGraphData();

        MapNodeData start = CreateNode("start", 0, 0, MapNodeType.Start, new Vector2(0f, -320f));
        MapNodeData battleA = CreateNode("battle_a", 1, 0, MapNodeType.NormalBattle, new Vector2(-260f, -120f));
        MapNodeData battleB = CreateNode("battle_b", 1, 1, MapNodeType.NormalBattle, new Vector2(0f, -120f));
        MapNodeData battleC = CreateNode("battle_c", 1, 2, MapNodeType.EliteBattle, new Vector2(260f, -120f));
        MapNodeData eventNode = CreateNode("event", 2, 0, MapNodeType.Event, new Vector2(-180f, 100f));
        MapNodeData treasureNode = CreateNode("treasure", 2, 1, MapNodeType.Treasure, new Vector2(180f, 100f));
        MapNodeData boss = CreateNode("boss", 3, 0, MapNodeType.Boss, new Vector2(0f, 320f));

        start.nextNodeIds.AddRange(new[] { battleA.nodeId, battleB.nodeId, battleC.nodeId });
        battleA.nextNodeIds.Add(eventNode.nodeId);
        battleB.nextNodeIds.Add(eventNode.nodeId);
        battleB.nextNodeIds.Add(treasureNode.nodeId);
        battleC.nextNodeIds.Add(treasureNode.nodeId);
        eventNode.nextNodeIds.Add(boss.nodeId);
        treasureNode.nextNodeIds.Add(boss.nodeId);

        start.visited = true;
        graph.currentNodeId = start.nodeId;
        graph.nodes = new List<MapNodeData>
        {
            start,
            battleA,
            battleB,
            battleC,
            eventNode,
            treasureNode,
            boss,
        };
        return graph;
    }

    static MapNodeData CreateNode(string nodeId, int layer, int indexInLayer, MapNodeType nodeType, Vector2 position)
    {
        return new MapNodeData
        {
            nodeId = nodeId,
            layer = layer,
            indexInLayer = indexInLayer,
            nodeType = nodeType,
            uiPosition = position,
        };
    }
}
