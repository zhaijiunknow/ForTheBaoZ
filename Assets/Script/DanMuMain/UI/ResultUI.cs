using System;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : BaseUI
{
    [SerializeField] private Text txt;

    public override void Show(object ojs = null)
    {
        base.Show(ojs);
        if (ojs != null) txt.text = ojs.ToString();
    }

    public void OnRestartGameClick()
    {
        GameManager.SwitchScene(SceneName.Map);
    }

}
