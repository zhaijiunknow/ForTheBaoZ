using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BattleCheckpointData
{
    public string checkpointId;
    public float targetProgress;
    public bool canRest = true;
    public bool grantsSprintBonus = true;
}

[Serializable]
public class FoodEntryData
{
    public string foodId;
    public string displayName;
    public string setId;
    public int level = 1;
    public float sequenceBonus;
}

public class BattleRunState : MonoBehaviour
{
    public static BattleRunState Instance { get; private set; }

    [Header("Run State")]
    public float currentProgress;
    public int currentPhaseIndex;
    public bool isPausedForStopover;
    public bool isBossPhase;
    public int skippedRestCount;
    public float obsession;
    public float sprintObsessionMultiplier = 1f;

    [Header("Config")]
    public List<BattleCheckpointData> checkpoints = new List<BattleCheckpointData>
    {
        new BattleCheckpointData { checkpointId = "phase_1", targetProgress = 300f },
        new BattleCheckpointData { checkpointId = "phase_2", targetProgress = 650f },
        new BattleCheckpointData { checkpointId = "boss", targetProgress = 1000f, canRest = false, grantsSprintBonus = false },
    };

    [Header("Collections")]
    public List<FoodEntryData> foods = new List<FoodEntryData>();
    public List<string> reachedCheckpointIds = new List<string>();

    public event Action OnStateChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public BattleCheckpointData GetNextCheckpoint()
    {
        if (currentPhaseIndex < 0 || currentPhaseIndex >= checkpoints.Count)
            return null;

        return checkpoints[currentPhaseIndex];
    }

    public void SetProgress(float progress)
    {
        currentProgress = Mathf.Max(currentProgress, progress);
        NotifyChanged();
    }

    public bool HasReachedCheckpoint(string checkpointId)
    {
        return reachedCheckpointIds.Contains(checkpointId);
    }

    public void MarkCheckpointReached(BattleCheckpointData checkpoint)
    {
        if (checkpoint == null || reachedCheckpointIds.Contains(checkpoint.checkpointId))
            return;

        reachedCheckpointIds.Add(checkpoint.checkpointId);
        currentPhaseIndex = Mathf.Min(currentPhaseIndex + 1, checkpoints.Count - 1);
        if (checkpoint.checkpointId == "boss")
            isBossPhase = true;
        NotifyChanged();
    }

    public void SetPausedForStopover(bool paused)
    {
        isPausedForStopover = paused;
        NotifyChanged();
    }

    public void AddObsession(float amount)
    {
        if (amount <= 0f)
            return;

        obsession += amount;
        NotifyChanged();
    }

    public bool TrySpendObsession(float amount)
    {
        if (amount < 0f || obsession < amount)
            return false;

        obsession -= amount;
        NotifyChanged();
        return true;
    }

    public void RegisterRestTaken()
    {
        skippedRestCount = 0;
        sprintObsessionMultiplier = 1f;
        NotifyChanged();
    }

    public void RegisterRestSkipped()
    {
        skippedRestCount++;
        sprintObsessionMultiplier = 1f + skippedRestCount * 0.25f;
        NotifyChanged();
    }

    public void AddFood(FoodEntryData food)
    {
        if (food == null)
            return;

        foods.Add(food);
        NotifyChanged();
    }

    public void NotifyChanged()
    {
        OnStateChanged?.Invoke();
    }
}
