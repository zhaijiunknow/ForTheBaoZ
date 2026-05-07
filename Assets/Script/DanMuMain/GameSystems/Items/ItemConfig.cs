using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/ItemConfig")]
public class ItemConfig : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    [Header("冷却时间")]
    public float CD;

    [Header("当前冷却")]
    public bool nowcd = false;

    [Header("符文")]
    public skilltype goodtype;

    [Header("推力加成")]
    [Tooltip("永久推力增加值（直接加到 permanentThrust）")]
    public float permanentAdd;

    [Tooltip("单个道具的基础临时推力（绝对值），例如 100 表示 +100 临时推力")]
    public float tempAdd;

    [Tooltip("临时推力持续时间（秒）")]
    public float duration;

    [Header("初始倍数（可选）")]
    [Tooltip("前 startBoostTime 秒的倍率，例如 3 表示前期×3")]
    public float startMultiplier = 1f;

    [Tooltip("开始初始倍率持续时间（秒），例如 5 表示前5秒倍数生效）")]
    public float startBoostTime = 0f;

    [Header("衰减控制（末尾阶段衰减）")]
    [Tooltip("是否启用末尾线性衰减（从 base 值线性到 decayEndRate*base）")]
    public bool enableDecay = false;

    [Tooltip("衰减阶段持续时间（秒）。例如设置为10表示在持续时间的最后10秒开始线性衰减）")]
    public float decayEndTime = 0f;

    [Tooltip("衰减结束时相对于 base 的比例（0~1）。0 表示衰减到 0；0.5 表示剩 50%")]
    [Range(0f, 1f)]
    public float decayEndRate = 0f;

    [Header("对敌方影响（Debuff）")]
    public bool reduceEnemyPermanent;
    [Range(0f,1f)]
    public float reduceEnemyPermanentRate;
    public float reduceEnemyPermanentTime;

    public bool reduceEnemyTemp;
    [Range(0f,1f)]
    public float reduceEnemyTempRate;
    public float reduceEnemyTempTime;

    [Header("防护罩（免疫Debuff）")]
    public bool isShield;
    public float shieldDuration;

    [Header("多道具加成")]
    public List<MultiItemBonus> multiItemBonuses = new()
    {
        new MultiItemBonus(){ itemCount = 10, thrustBonus = 1.3f },
        new MultiItemBonus(){ itemCount = 66, thrustBonus = 1.5f },
        new MultiItemBonus(){ itemCount = 188, thrustBonus = 2.0f },
        new MultiItemBonus(){ itemCount = 520, thrustBonus = 3.0f },
    };
    
    public (float bonus, float duration) GetMultiBonusWithTime(int count)
    {
        float bonus = 1f;
        float duration = 0f;
        foreach (var entry in multiItemBonuses)
        {
            if (count >= entry.itemCount)
            {
                bonus = entry.thrustBonus;
                duration = entry.duration;
            }
        }
        return (bonus, duration);
    }

}

[Serializable]
public class MultiItemBonus
{
    public int itemCount;        // 同时使用数量
    public float thrustBonus;    // 推力倍率
    public float duration = 0f;  // 多道具加成持续时间（秒）
}
