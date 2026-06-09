using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/RogueConfig")]
public class RogueConfig : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    [Header("事件介绍")]
    [TextArea(3, 6)]
    public string itemIntro;

    [Header("事件选项")]
    public eventresult[] action;
    public string[] ChoiceName;
}

public enum eventresult
{
    AddSkill,
    AddMaxSkillCount,
    Recover,
    Continue,
}
