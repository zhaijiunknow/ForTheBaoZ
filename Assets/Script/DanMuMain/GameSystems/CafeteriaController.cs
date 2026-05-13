using System.Collections.Generic;
using UnityEngine;

public class CafeteriaController : MonoBehaviour
{
    [SerializeField] private BattleRunState runState;
    [SerializeField] private List<FoodEntryData> candidateFoods = new List<FoodEntryData>
    {
        new FoodEntryData { foodId = "doufunao_1", displayName = "甜咸之辩", setId = "doufunao", sequenceBonus = 0.1f },
        new FoodEntryData { foodId = "shaokao_1", displayName = "腌入味了", setId = "shaokao", sequenceBonus = 0.1f },
        new FoodEntryData { foodId = "tangmian_1", displayName = "先熬后提", setId = "tangmian", sequenceBonus = 0.1f },
    };

    int candidateIndex;

    void Awake()
    {
        if (runState == null)
            runState = FindObjectOfType<BattleRunState>();
    }

    public FoodEntryData PeekCurrentCandidate()
    {
        if (candidateFoods.Count == 0)
            return null;

        candidateIndex = Mathf.Clamp(candidateIndex, 0, candidateFoods.Count - 1);
        return candidateFoods[candidateIndex];
    }

    public int GetCandidateIndex()
    {
        return candidateIndex;
    }

    public void SetCandidateIndex(int index)
    {
        if (candidateFoods.Count == 0)
        {
            candidateIndex = 0;
            return;
        }

        candidateIndex = Mathf.Clamp(index, 0, candidateFoods.Count - 1);
    }

    public void RefreshCandidate()
    {
        if (candidateFoods.Count == 0 || runState == null)
            return;

        if (!runState.TrySpendObsession(10f))
            return;

        candidateIndex = (candidateIndex + 1) % candidateFoods.Count;
    }

    public bool BuyCurrentCandidate()
    {
        if (runState == null)
            return false;

        FoodEntryData candidate = PeekCurrentCandidate();
        if (candidate == null)
            return false;

        if (!runState.TrySpendObsession(20f))
            return false;

        runState.AddFood(new FoodEntryData
        {
            foodId = candidate.foodId,
            displayName = candidate.displayName,
            setId = candidate.setId,
            level = candidate.level,
            sequenceBonus = candidate.sequenceBonus,
        });
        return true;
    }

    public bool UpgradeLastOwnedFood()
    {
        if (runState == null || runState.foods.Count == 0)
            return false;
        if (!runState.TrySpendObsession(15f))
            return false;

        FoodEntryData food = runState.foods[runState.foods.Count - 1];
        food.level++;
        food.sequenceBonus += 0.05f;
        runState.NotifyChanged();
        return true;
    }
}
