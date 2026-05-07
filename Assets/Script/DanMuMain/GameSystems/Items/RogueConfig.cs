using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.Button;


[CreateAssetMenu(menuName = "Game/RogueConfig")]
public class RogueConfig : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    [Header("事件描述")]
    public string itemIntro;

    [Header("事件选项")]
    public eventresult[] action;

    public string[] ChoiceName;    

}

public enum eventresult
{
    AddSkill,
    AddMaxSkillCount,
}
