using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/QTEItemConfig")]
public class QTEItemConfig : ItemConfig
{
    [Header("连携前置")]
    public List<skilltype> combo;

    [Tooltip("是否必须完全按顺序")]
    public bool exactOrder = true;

    [Header("时间限制")]
    [Tooltip("最大连招时间")]
    public float maxComboTime = 2f;

}
