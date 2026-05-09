using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static GameManager;

public class InputGuideItem : MonoBehaviour
{
    [SerializeField] InputActionReference actionRefer;
    [SerializeField] Image imageIcon;
    [SerializeField] Text textAction;

    RectTransform rect, parentRect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        parentRect = GetComponentInParent<RectTransform>();
    }

    void OnEnable()
    {
        ShowInputGuide();
    }

    public void ShowInputGuide()
    {
        ShowIcon();
        ShowText();
        RefreshWidth();
    }

    void ShowIcon()
    {
        //由于Move, Navigate是一个复合按键（包含上下左右），如果直接设置，只能拿到第一个按键的icon，所以为它们添加一个特殊方法
        if (actionRefer.action.name == "Move")
            imageIcon.sprite = GameManager.NewInput.GetBindingIconForMove();
        else if (actionRefer.action.name == "Navigate")
            imageIcon.sprite = GameManager.NewInput.GetBindingIconForNavigate();
        else
            imageIcon.sprite = GameManager.NewInput.GetBindingIcon(actionRefer.action.name);
    }

    void ShowText()
    {
        textAction.text = GameManager.Data.TitleCSV.GetString(actionRefer.action.name, GameManager.Data.CurLan);
    }

    //当文字长度变化时，刷新Horizontal Layout以自适应宽度，如果上下级都有Horizontal Layout，那么都要刷新
    void RefreshWidth()
    {
        if (rect)
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        if (parentRect)
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
    }
}
