using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [SerializeField] Slider slider_progress;

    void OnEnable()
    {
        GameEvent.ManagerProgress.AddListener(OnManagersProgress);

    }
    void OnDisable()
    {
        GameEvent.ManagerProgress.RemoveListener(OnManagersProgress);
    }

    private void OnManagersProgress(int numReady, int numModules)
    {
        float progress = (float)numReady / numModules;
        slider_progress.value = progress;
    }

}
