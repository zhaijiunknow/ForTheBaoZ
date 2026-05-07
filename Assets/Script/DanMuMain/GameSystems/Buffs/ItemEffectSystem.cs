using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class ItemEffectSystem : MonoBehaviour
{
    public PlayerThrustManager owner;
    public PlayerThrustManager enemy;

    public void UseItem(ItemConfig config, int count = 1)
    {
        if (config.nowcd)
        {
            Debug.Log($"{config.itemName}正在冷却，冷却时间{config.CD}");
            return;
        }
        StartCoroutine(ApplyItemEffect(config, count));
    }
    
    public void UseItem(PlayerThrustManager target,ItemConfig config, int count = 1)
    {
        if (config.nowcd)
        {
            Debug.Log($"{config.itemName}正在冷却，冷却时间{config.CD}");
            return;
        }
        StartCoroutine(ApplyItemEffect(config, count));
    }
    private IEnumerator ApplyItemCD(ItemConfig config)
    {
        yield return new WaitForSeconds(config.CD);
        config.nowcd = false;

    }
    private IEnumerator ApplyItemEffect(ItemConfig config, int count)
    {
        var inst = config;
        config.nowcd = true;
        StartCoroutine(ApplyItemCD(config));
        var (bonus, bonusDuration) = inst.GetMultiBonusWithTime(count);
        Debug.Log($"{owner.playerName} 使用 {inst.itemName} ×{count} (倍率{bonus}, 加成持续 {bonusDuration}s)");

        // 防护罩
        if (inst.isShield)
            owner.ActivateShield(inst.shieldDuration);
    
        // 减少敌方临时推力
        if (inst.reduceEnemyTemp && !enemy.IsShieldActive)
            enemy.ApplyTempMultiplierDebuff(inst.reduceEnemyTempRate, inst.reduceEnemyTempTime);

        // 永久推力加成（支持数量与持续时间）
        if (inst.permanentAdd != 0)
        {
            // 总加成值 = 基础 * 数量 * 倍率
            float totalAdd = inst.permanentAdd * count * bonus;

            // 如果存在多道具持续时间（bonusDuration > 0）
            if (bonusDuration > 0)
            {
                // 基础加成部分（持续存在）
                float baseAdd = inst.permanentAdd * count;

                // 临时加成部分（随持续时间消失）
                float tempExtra = totalAdd - baseAdd;

                owner.AddPermanentThrust(baseAdd);
                owner.AddPermanentThrust(tempExtra);

                Debug.Log($"永久推力 +{totalAdd} （基础{baseAdd} + 临时{tempExtra}，持续{bonusDuration}s）");

                yield return new WaitForSeconds(bonusDuration);

                // 还原临时加成部分
                owner.AddPermanentThrust(-tempExtra);
                Debug.Log($"永久推力临时加成结束 -{tempExtra}");
            }
            else
            {
                // 没有持续时间的情况，直接永久增加
                owner.AddPermanentThrust(totalAdd);
                Debug.Log($"永久推力 +{totalAdd} （无持续时间，永久生效）");
            }
        }

        // 临时推力
        if (inst.tempAdd != 0)
        {
            if (bonusDuration > 0)
            {
                yield return StartCoroutine(ApplyTempBuff(owner, inst, count, bonus, bonusDuration));

                float remain = Mathf.Max(0f, inst.duration - bonusDuration);
                if (remain > 0)
                    yield return StartCoroutine(ApplyTempBuff(owner, inst, count, 1f, remain));
            }
            else
            {
                yield return StartCoroutine(ApplyTempBuff(owner, inst, count, bonus, inst.duration));
            }
        }

        // 敌方永久推力 Debuff
        if (inst.reduceEnemyPermanent && !enemy.IsShieldActive)
        {
            float reduceVal = enemy.permanentThrust * inst.reduceEnemyPermanentRate;
            enemy.AddPermanentThrust(-reduceVal);
            yield return new WaitForSeconds(inst.reduceEnemyPermanentTime);
            enemy.AddPermanentThrust(reduceVal);
        }
    }


    private IEnumerator ApplyTempBuff(PlayerThrustManager target, ItemConfig cfg, int itemCount = 1, float bonus = 1f, float overrideDuration = -1f)
    {
        var inst = cfg;
        float baseAdd = inst.tempAdd * itemCount * bonus;
        float applied = baseAdd;
        target.AddTempThrust(applied);

        float elapsed = 0f;
        float totalDur = overrideDuration > 0 ? overrideDuration : inst.duration;
        float decayDur = inst.enableDecay ? Mathf.Clamp(inst.decayEndTime, 0f, totalDur) : 0f;
        float decayStart = totalDur - decayDur;

        while (elapsed < totalDur)
        {
            elapsed += Time.deltaTime;

            float multiplier = 1f;
            if (inst.startBoostTime > 0f && elapsed <= inst.startBoostTime)
                multiplier = inst.startMultiplier;

            if (inst.enableDecay && decayDur > 0f && elapsed >= decayStart)
            {
                float t = Mathf.InverseLerp(decayStart, totalDur, elapsed);
                multiplier *= Mathf.Lerp(1f, inst.decayEndRate, t);
            }

            float desired = baseAdd * multiplier;
            float delta = desired - applied;
            if (Mathf.Abs(delta) > 1e-5f)
            {
                target.AddTempThrust(delta);
                applied = desired;
            }

            yield return null;
        }

        target.AddTempThrust(-applied);
    }
    
}
