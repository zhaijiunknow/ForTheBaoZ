using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    [SerializeField] RectTransform nodeRoot;
    [SerializeField] TextMeshProUGUI infoText;

    void Start()
    {
        if (infoText != null)
            infoText.text = "地图层已弃用。当前主流程已切换到 Battle。";

        if (nodeRoot != null)
            nodeRoot.gameObject.SetActive(false);
    }
}
