using UnityEngine;

public class BattleFlowController : MonoBehaviour
{
    [SerializeField] private MySkillManager skillManager;
    [SerializeField] private BattleUI battleUI;
    [SerializeField] private BattleRunState runState;
    [SerializeField] private SetBonusResolver setBonusResolver;

    void Awake()
    {
        if (skillManager == null)
            skillManager = FindObjectOfType<MySkillManager>();
        if (battleUI == null)
            battleUI = FindObjectOfType<BattleUI>();
        if (runState == null)
            runState = FindObjectOfType<BattleRunState>();
        if (setBonusResolver == null)
            setBonusResolver = FindObjectOfType<SetBonusResolver>();
    }

    public void RecordSkill(ItemConfig item)
    {
        if (skillManager == null || item == null || item.nowcd)
            return;

        skillManager._skillQueue.AddSkill(item.goodtype);
    }

    public void ResolveCombo(ItemEffectSystem system, PlayerThrustManager target, int count)
    {
        if (skillManager == null || system == null)
            return;

        for (int i = 0; i < skillManager.qTEItemConfigs.Count; i++)
        {
            if (!skillManager.CheckForCombo(skillManager.qTEItemConfigs[i]))
                continue;

            system.UseItem(target, skillManager.qTEItemConfigs[i], count);
            if (runState != null)
            {
                float setBonus = setBonusResolver != null ? setBonusResolver.GetSequenceBonusMultiplier() : 1f;
                float foodBonus = 1f + runState.foods.Count;
                runState.AddObsession(foodBonus * setBonus);
            }
            break;
        }
    }
}
