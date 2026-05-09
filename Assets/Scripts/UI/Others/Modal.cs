using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(CanvasGroup))]

public class Modal : MonoBehaviour
{
    CanvasGroup canvasGroup;
    GameText text;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        text = GetComponentInChildren<GameText>();
        CloseModal();
    }

    void OnEnable()
    {
        GameEvent.DataSaved.AddListener(OnDataSaved);
    }

    void OnDisable()
    {
        GameEvent.DataSaved.RemoveListener(OnDataSaved);
    }

    void OnDataSaved()
    {
        if (text)
            text.ShowText();

        if (UIController.instance)
            if (UIController.instance.CurPanel == PanelName.PanelOption)
                StartCoroutine(CloseModalDelay());
    }

    void CloseModal()
    {
        canvasGroup.alpha = 0;
    }

    IEnumerator CloseModalDelay()
    {
        canvasGroup.alpha = 1;
        yield return new WaitForSecondsRealtime(1f);
        CloseModal();
    }

}
