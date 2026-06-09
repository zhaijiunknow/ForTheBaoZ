using System.Collections.Generic;
using UnityEngine;

public class SetBonusResolver : MonoBehaviour
{
    [SerializeField] private BattleRunState runState;

    readonly Dictionary<string, int> setCounts = new Dictionary<string, int>();

    void Awake()
    {
        if (runState == null)
            runState = FindObjectOfType<BattleRunState>();
    }

    void OnEnable()
    {
        if (runState != null)
            runState.OnStateChanged += Rebuild;
        Rebuild();
    }

    void OnDisable()
    {
        if (runState != null)
            runState.OnStateChanged -= Rebuild;
    }

    public int GetSetCount(string setId)
    {
        if (string.IsNullOrEmpty(setId) || !setCounts.TryGetValue(setId, out int count))
            return 0;

        return count;
    }

    public float GetSequenceBonusMultiplier()
    {
        float bonus = 1f;
        foreach (var pair in setCounts)
        {
            if (pair.Value >= 4)
                bonus += 0.25f;
            else if (pair.Value >= 2)
                bonus += 0.1f;
        }
        return bonus;
    }

    void Rebuild()
    {
        setCounts.Clear();
        if (runState == null)
            return;

        for (int i = 0; i < runState.foods.Count; i++)
        {
            FoodEntryData food = runState.foods[i];
            if (food == null || string.IsNullOrEmpty(food.setId))
                continue;

            if (!setCounts.ContainsKey(food.setId))
                setCounts[food.setId] = 0;
            setCounts[food.setId]++;
        }
    }
}
