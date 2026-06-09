using UnityEngine;

public class ObsessionSystem : MonoBehaviour
{
    [SerializeField] private BattleRunState runState;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private float gainScale = 0.02f;

    float carryover;

    void Awake()
    {
        if (runState == null)
            runState = FindObjectOfType<BattleRunState>();
        if (battleManager == null)
            battleManager = FindObjectOfType<BattleManager>();
    }

    void Update()
    {
        if (runState == null || battleManager == null || !DynamicData.GameStart || runState.isPausedForStopover)
            return;

        float advantage = Mathf.Max(battleManager.redDistance, 0f);
        float gain = advantage * gainScale * runState.sprintObsessionMultiplier * Time.deltaTime;
        carryover += gain;
        if (carryover < 1f)
            return;

        float whole = Mathf.Floor(carryover);
        carryover -= whole;
        runState.AddObsession(whole);
    }
}
