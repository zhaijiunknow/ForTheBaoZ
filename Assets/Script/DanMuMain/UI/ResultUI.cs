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
        bool isVictory = txt != null && txt.text != "蓝方胜利";

        if (GameManager.PendingBattleEntry != null)
        {
            GameManager.CompleteBattle(new BattleResultData
            {
                sourceNodeId = GameManager.PendingBattleEntry.sourceNodeId,
                isVictory = isVictory,
            });
        }

        GameManager.SwitchScene(SceneName.Map);
    }

}
