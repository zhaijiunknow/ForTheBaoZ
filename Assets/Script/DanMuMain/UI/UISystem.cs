using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UISystem : MonoBehaviour
{
    public static UISystem Ins = null;

    public MainUI mainUI;
    public ResultUI resultUI;
    public BattleUI battleUI;
    private void Awake()
    {
        Ins = this;
    }

    private void OnDestroy()
    {
        Ins = null;
    }
}
