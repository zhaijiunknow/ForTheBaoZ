using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    [SerializeField] RectTransform nodeRoot;
    [SerializeField] TextMeshProUGUI infoText;

    readonly Dictionary<string, MapNodeView> nodeViews = new Dictionary<string, MapNodeView>();

    void Start()
    {
        EnsureRun();
        ApplyLastBattleResult();
        BuildMap();
        RefreshMap();
    }

    void EnsureRun()
    {
        if (GameManager.CurrentMapRun == null)
            GameManager.StartMapRun(MapGenerator.CreateSampleRun());
    }

    void ApplyLastBattleResult()
    {
        if (GameManager.LastBattleResult == null || GameManager.CurrentMapRun == null)
            return;

        MapNodeData node = GameManager.CurrentMapRun.GetNode(GameManager.LastBattleResult.sourceNodeId);
        if (node != null && GameManager.LastBattleResult.isVictory)
        {
            node.cleared = true;
            node.visited = true;
            GameManager.CurrentMapRun.currentNodeId = node.nodeId;
            GameManager.CurrentMapRun.pendingNodeId = null;
        }

        GameManager.ClearBattleResult();
    }

    void BuildMap()
    {
        if (nodeRoot == null || GameManager.CurrentMapRun == null)
            return;

        for (int i = nodeRoot.childCount - 1; i >= 0; i--)
            Destroy(nodeRoot.GetChild(i).gameObject);

        nodeViews.Clear();

        foreach (MapNodeData node in GameManager.CurrentMapRun.nodes)
        {
            GameObject nodeObject = CreateNodeObject(node);
            MapNodeView nodeView = nodeObject.GetComponent<MapNodeView>();
            nodeView.Setup(node, this);
            nodeViews[node.nodeId] = nodeView;
        }
    }

    GameObject CreateNodeObject(MapNodeData node)
    {
        GameObject nodeObject = new GameObject($"Node_{node.nodeId}", typeof(RectTransform), typeof(Image), typeof(Button), typeof(MapNodeView));
        nodeObject.layer = LayerMask.NameToLayer("UI");
        nodeObject.transform.SetParent(nodeRoot, false);

        RectTransform rect = nodeObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(160f, 60f);
        rect.anchoredPosition = node.uiPosition;

        Image image = nodeObject.GetComponent<Image>();
        image.color = Color.gray;

        GameObject textObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.layer = LayerMask.NameToLayer("UI");
        textObject.transform.SetParent(nodeObject.transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.black;
        text.enableAutoSizing = true;
        text.fontSizeMin = 18;
        text.fontSizeMax = 32;

        MapNodeView nodeView = nodeObject.GetComponent<MapNodeView>();
        nodeView.Bind(text, image);

        return nodeObject;
    }

    public void SelectNode(string nodeId)
    {
        if (GameManager.CurrentMapRun == null || !GameManager.CurrentMapRun.CanReach(nodeId))
            return;

        MapNodeData node = GameManager.CurrentMapRun.GetNode(nodeId);
        if (node == null)
            return;

        node.visited = true;
        GameManager.CurrentMapRun.pendingNodeId = node.nodeId;

        if (node.nodeType == MapNodeType.NormalBattle || node.nodeType == MapNodeType.EliteBattle || node.nodeType == MapNodeType.Boss)
        {
            GameManager.QueueBattle(new BattleEntryData
            {
                sourceNodeId = node.nodeId,
                sourceNodeType = node.nodeType,
            });
            GameManager.SwitchScene(SceneName.Battle);
            return;
        }

        node.cleared = true;
        GameManager.CurrentMapRun.currentNodeId = node.nodeId;
        GameManager.CurrentMapRun.pendingNodeId = null;
        ApplyNodeReward(node);
        RefreshMap();
    }

    void ApplyNodeReward(MapNodeData node)
    {
        if (RoguelikeManager.Instance != null)
            RoguelikeManager.Instance.ApplyMapReward(node.nodeType);

        if (node.nodeType == MapNodeType.Event)
            SetInfo("事件：获得一个额外技能。", node);
        else if (node.nodeType == MapNodeType.Treasure)
            SetInfo("宝箱：增加一个技能栏位。", node);
        else if (node.nodeType == MapNodeType.Rest)
            SetInfo("休息：本轮先作为安全节点。", node);
        else
            SetInfo("继续前进。", node);
    }

    void RefreshMap()
    {
        if (GameManager.CurrentMapRun == null)
            return;

        foreach (MapNodeData node in GameManager.CurrentMapRun.nodes)
        {
            if (!nodeViews.TryGetValue(node.nodeId, out MapNodeView nodeView))
                continue;

            bool isCurrent = GameManager.CurrentMapRun.currentNodeId == node.nodeId;
            bool isReachable = GameManager.CurrentMapRun.CanReach(node.nodeId);
            nodeView.Refresh(isReachable, isCurrent);
        }

        MapNodeData currentNode = GameManager.CurrentMapRun.GetNode(GameManager.CurrentMapRun.currentNodeId);
        SetInfo("请选择下一节点。", currentNode);
    }

    void SetInfo(string prefix, MapNodeData node)
    {
        if (infoText == null)
            return;

        if (node == null)
            infoText.text = prefix;
        else
            infoText.text = $"{prefix}\n当前位置：{GetNodeName(node.nodeType)}";
    }

    string GetNodeName(MapNodeType nodeType)
    {
        switch (nodeType)
        {
            case MapNodeType.Start:
                return "起点";
            case MapNodeType.NormalBattle:
                return "普通战";
            case MapNodeType.EliteBattle:
                return "精英战";
            case MapNodeType.Event:
                return "事件";
            case MapNodeType.Treasure:
                return "宝箱";
            case MapNodeType.Rest:
                return "休息";
            case MapNodeType.Boss:
                return "Boss";
            default:
                return nodeType.ToString();
        }
    }
}
