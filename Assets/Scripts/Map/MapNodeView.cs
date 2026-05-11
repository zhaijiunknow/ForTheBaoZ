using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MapNodeView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI label;
    [SerializeField] Image background;

    Button button;
    MapNodeData nodeData;
    MapController controller;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void Bind(TextMeshProUGUI textComponent, Image backgroundImage)
    {
        label = textComponent;
        background = backgroundImage;
    }

    public void Setup(MapNodeData data, MapController owner)
    {
        nodeData = data;
        controller = owner;
        if (label != null)
            label.text = GetLabel(nodeData.nodeType);
    }

    public void Refresh(bool isReachable, bool isCurrent)
    {
        button.interactable = isReachable;

        if (background == null)
            return;

        if (nodeData.cleared)
            background.color = new Color(0.3f, 0.85f, 0.45f, 1f);
        else if (isCurrent)
            background.color = new Color(0.95f, 0.8f, 0.2f, 1f);
        else if (isReachable)
            background.color = new Color(0.35f, 0.6f, 1f, 1f);
        else
            background.color = new Color(0.35f, 0.35f, 0.35f, 0.85f);
    }

    void OnClick()
    {
        if (controller != null && nodeData != null)
            controller.SelectNode(nodeData.nodeId);
    }

    string GetLabel(MapNodeType nodeType)
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
