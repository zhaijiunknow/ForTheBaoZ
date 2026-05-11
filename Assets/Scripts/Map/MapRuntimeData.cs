using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapNodeData
{
    public string nodeId;
    public int layer;
    public int indexInLayer;
    public MapNodeType nodeType;
    public Vector2 uiPosition;
    public List<string> nextNodeIds = new List<string>();
    public bool visited;
    public bool cleared;
}

[Serializable]
public class MapGraphData
{
    public List<MapNodeData> nodes = new List<MapNodeData>();
    public string currentNodeId;
    public string pendingNodeId;

    public MapNodeData GetNode(string nodeId)
    {
        return nodes.Find(node => node.nodeId == nodeId);
    }

    public bool CanReach(string nodeId)
    {
        if (string.IsNullOrEmpty(currentNodeId))
            return false;

        MapNodeData currentNode = GetNode(currentNodeId);
        return currentNode != null && currentNode.nextNodeIds.Contains(nodeId);
    }
}

public enum MapNodeType
{
    Start,
    NormalBattle,
    EliteBattle,
    Event,
    Treasure,
    Rest,
    Boss,
}

[Serializable]
public class BattleEntryData
{
    public string sourceNodeId;
    public MapNodeType sourceNodeType;

    public bool IsBattleNode()
    {
        return sourceNodeType == MapNodeType.NormalBattle || sourceNodeType == MapNodeType.EliteBattle || sourceNodeType == MapNodeType.Boss;
    }
}

[Serializable]
public class BattleResultData
{
    public string sourceNodeId;
    public bool isVictory;
}
