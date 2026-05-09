using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    public Text TextSave;

    void OnEnable()
    {
        GameEvent.DataSaved.AddListener(OnDataSaved);
    }

    void OnDisable()
    {
        GameEvent.DataSaved.RemoveListener(OnDataSaved);
    }

    void Start()
    {
        ShowText();
    }

    void Update()
    {

    }

    void OnDataSaved()
    {
        ShowText();
    }

    void ShowText()
    {
        TextSave.text = "返回标题将保存游戏，游戏已保存" + GameManager.Data.CurLevelProgress + "次";
    }
}
