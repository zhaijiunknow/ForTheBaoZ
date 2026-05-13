using UnityEngine;

public class StopoverController : MonoBehaviour
{
    [SerializeField] private BattleRunState runState;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private CafeteriaController cafeteriaController;
    [SerializeField] private RoguelikeManager roguelikeManager;
    [SerializeField] private bool autoRest = false;

    BattleCheckpointData pendingCheckpoint;
    bool isChoosingRest;

    void Awake()
    {
        if (runState == null)
            runState = FindObjectOfType<BattleRunState>();
        if (battleManager == null)
            battleManager = FindObjectOfType<BattleManager>();
        if (cafeteriaController == null)
            cafeteriaController = FindObjectOfType<CafeteriaController>();
        if (roguelikeManager == null)
            roguelikeManager = FindObjectOfType<RoguelikeManager>();
    }

    public void PromptStopover(BattleCheckpointData checkpoint)
    {
        if (runState == null || checkpoint == null)
            return;

        pendingCheckpoint = checkpoint;
        isChoosingRest = true;
        ResetTemporaryThrust();
        runState.SetPausedForStopover(true);
        DynamicData.GameStart = false;
        GameManager.SaveCurrentRun();

        if (autoRest)
            TakeRest();
    }

    public bool IsChoosingRest()
    {
        return isChoosingRest;
    }

    public bool IsPreparingStopover()
    {
        return pendingCheckpoint != null && !isChoosingRest;
    }

    public void TakeRest()
    {
        if (runState == null)
            return;

        isChoosingRest = false;
        runState.RegisterRestTaken();
        if (cafeteriaController != null)
            cafeteriaController.PeekCurrentCandidate();
        GameManager.SaveCurrentRun();
    }

    public void SkipRest()
    {
        if (runState == null)
            return;

        isChoosingRest = false;
        runState.RegisterRestSkipped();
        GameManager.SaveCurrentRun();
    }

    public void ConfirmPreparation()
    {
        ResumeBattle();
    }

    public void TriggerRewardChoice()
    {
        if (roguelikeManager == null)
            return;

        roguelikeManager.TryStartReward(null);
    }

    public void ResumeBattle()
    {
        pendingCheckpoint = null;
        isChoosingRest = false;
        if (runState != null)
            runState.SetPausedForStopover(false);
        DynamicData.GameStart = true;
        GameManager.SaveCurrentRun();
    }

    void ResetTemporaryThrust()
    {
        if (battleManager == null)
            return;

        if (battleManager.redPlayer != null)
            battleManager.redPlayer.ClearTemporaryState();
        if (battleManager.bluePlayer != null)
            battleManager.bluePlayer.ClearTemporaryState();
    }
}
