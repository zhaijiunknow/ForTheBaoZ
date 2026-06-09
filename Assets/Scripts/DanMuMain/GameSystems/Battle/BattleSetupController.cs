using System.Collections.Generic;
using UnityEngine;

public class BattleSetupController : MonoBehaviour
{
    [SerializeField] private BattleRunState runState;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private MySkillManager skillManager;
    [SerializeField] private CafeteriaController cafeteriaController;

    [Header("Initial Loadout")]
    [SerializeField] private int startingSkillCount = 3;
    [SerializeField] private int[] startingSkills = { 0, 1, 2 };

    void Awake()
    {
        if (runState == null)
            runState = FindObjectOfType<BattleRunState>();
        if (battleManager == null)
            battleManager = FindObjectOfType<BattleManager>();
        if (skillManager == null)
            skillManager = FindObjectOfType<MySkillManager>();
        if (cafeteriaController == null)
            cafeteriaController = FindObjectOfType<CafeteriaController>();
    }

    void Start()
    {
        InitializeRun();
    }

    void InitializeRun()
    {
        if (runState == null)
            return;

        BattleProgressSaveData saveData = GameManager.GetCurrentBattleSave();
        if (saveData != null)
        {
            ApplyBattleSave(saveData);
            return;
        }

        runState.currentProgress = 0f;
        runState.currentPhaseIndex = 0;
        runState.isPausedForStopover = false;
        runState.isBossPhase = false;
        runState.skippedRestCount = 0;
        runState.obsession = 0f;
        runState.sprintObsessionMultiplier = 1f;
        runState.foods.Clear();
        runState.reachedCheckpointIds.Clear();

        if (battleManager != null)
        {
            battleManager.redDistance = 0f;
            battleManager.buleDistance = battleManager.winDistance;
        }

        if (skillManager != null)
        {
            skillManager.skillcount = startingSkillCount;
            skillManager.myskillbox.Clear();
            for (int i = 0; i < startingSkills.Length; i++)
                skillManager.myskillbox.Add(startingSkills[i]);
        }

        runState.NotifyChanged();
    }

    void ApplyBattleSave(BattleProgressSaveData saveData)
    {
        runState.currentProgress = saveData.currentProgress;
        runState.currentPhaseIndex = saveData.currentPhaseIndex;
        runState.isPausedForStopover = saveData.isPausedForStopover;
        runState.isBossPhase = saveData.isBossPhase;
        runState.skippedRestCount = saveData.skippedRestCount;
        runState.obsession = saveData.obsession;
        runState.sprintObsessionMultiplier = saveData.sprintObsessionMultiplier;
        runState.foods = saveData.foods != null ? new List<FoodEntryData>(saveData.foods) : new List<FoodEntryData>();
        runState.reachedCheckpointIds = saveData.reachedCheckpointIds != null ? new List<string>(saveData.reachedCheckpointIds) : new List<string>();

        if (battleManager != null)
        {
            battleManager.redDistance = saveData.redDistance;
            battleManager.buleDistance = battleManager.winDistance - saveData.redDistance;
        }

        if (skillManager != null)
        {
            skillManager.skillcount = saveData.skillCount;
            skillManager.myskillbox.Clear();
            if (saveData.mySkillBox != null)
                skillManager.myskillbox.AddRange(saveData.mySkillBox);
        }

        if (cafeteriaController != null)
            cafeteriaController.SetCandidateIndex(saveData.cafeteriaCandidateIndex);

        DynamicData.TimerBack = saveData.remainingTime > 0f ? saveData.remainingTime : 180f;
        DynamicData.GameStart = saveData.gameStarted;
        runState.NotifyChanged();
    }
}
