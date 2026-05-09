using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class RebindOverlay : MonoBehaviour
{
    Text textRebindPrompt;

    void Awake()
    {
        textRebindPrompt = GetComponentInChildren<Text>();
    }

    void OnEnable()
    {
        GameEvent.RebindingStarted.Invoke();
        textRebindPrompt.text = GameManager.Data.TitleCSV.GetString(TitleName.Wait.ToString(), GameManager.Data.CurLan);
    }

    void OnDisable()
    {
        GameEvent.RebindingCompleted.Invoke();
    }
}
