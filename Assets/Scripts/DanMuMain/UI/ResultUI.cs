using UnityEngine;
using UnityEngine.UI;

public class ResultUI : BaseUI
{
    [SerializeField] private Text txt;

    public override void Show(object ojs = null)
    {
        base.Show(ojs);

        if (ojs != null && txt != null)
            txt.text = ojs.ToString();
    }

    public void OnRestartGameClick()
    {
        Hide();
        GameManager.StartNewRun();
    }

    public void OnGoTitleClick()
    {
        GameManager.SaveCurrentRun();
        Hide();
        GameManager.SetGameStatus(GameStatus.Loaded);
        GameManager.SwitchScene(SceneName.Menu);
    }
}
