using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BattleProgressSaveData
{
    public int version = 1;
    public string checkpointId;
    public float currentProgress;
    public int currentPhaseIndex;
    public float nextTargetProgress;
    public bool isPausedForStopover;
    public bool isBossPhase;
    public int skippedRestCount;
    public float sprintObsessionMultiplier;
    public float obsession;
    public List<string> reachedCheckpointIds = new List<string>();
    public List<FoodEntryData> foods = new List<FoodEntryData>();
    public int skillCount;
    public List<int> mySkillBox = new List<int>();
    public float redDistance;
    public int cafeteriaCandidateIndex;
    public bool gameStarted;
    public float remainingTime;
}
