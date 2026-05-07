using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUI : BaseUI
{
    
    public void OnStartGameClick()
    {
        DynamicData.GameStart = true;
        gameObject.SetActive(false);
    }

}
