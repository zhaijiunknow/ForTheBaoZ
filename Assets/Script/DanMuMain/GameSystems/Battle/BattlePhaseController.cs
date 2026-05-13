using UnityEngine;

public class BattlePhaseController : MonoBehaviour
{
    [SerializeField] private BattleRunState runState;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private StopoverController stopoverController;

    void Awake()
    {
        if (runState == null)
            runState = FindObjectOfType<BattleRunState>();
        if (battleManager == null)
            battleManager = FindObjectOfType<BattleManager>();
        if (stopoverController == null)
            stopoverController = FindObjectOfType<StopoverController>();
    }

    void Update()
    {
        if (runState == null || battleManager == null || !DynamicData.GameStart)
            return;

        runState.SetProgress(Mathf.Max(0f, battleManager.redDistance));
        BattleCheckpointData checkpoint = runState.GetNextCheckpoint();
        if (checkpoint == null || runState.HasReachedCheckpoint(checkpoint.checkpointId))
            return;

        if (runState.currentProgress < checkpoint.targetProgress)
            return;

        runState.MarkCheckpointReached(checkpoint);
        if (checkpoint.canRest && stopoverController != null)
        {
            stopoverController.PromptStopover(checkpoint);
        }
        else if (checkpoint.grantsSprintBonus)
        {
            runState.RegisterRestSkipped();
        }
    }
}
