using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerThrustManager : MonoBehaviour
{
    [Header("推力基础参数")]
    public string playerName = "Red";
    public float permanentThrust = 1000f;
    private float rawTempSum = 0f;
    private float tempMultiplier = 1f;
    private readonly Dictionary<int, float> multiplierStack = new();

    [FormerlySerializedAs("debuffImmune")] public bool IsShieldActive = false;

    public float TempThrust => Mathf.Max(0f, rawTempSum * tempMultiplier);
    public float TotalThrust => Mathf.Max(0f, permanentThrust + TempThrust);

    public void AddPermanentThrust(float delta)
    {
        permanentThrust = Mathf.Max(0f, permanentThrust + delta);
    }

    public void AddTempThrust(float delta)
    {
        rawTempSum = Mathf.Max(0f, rawTempSum + delta);
    }

    public void ApplyTempMultiplierDebuff(float rate, float duration)
    {
        if (IsShieldActive) return;
        StartCoroutine(TempMultiplierDebuffRoutine(rate, duration));
    }

    private IEnumerator TempMultiplierDebuffRoutine(float rate, float duration)
    {
        Debug.Log("rate: " + rate + " duration:" + duration);
        int id = Random.Range(1, int.MaxValue);
        multiplierStack[id] = Mathf.Max(0f, 1f - rate);
        RecalcMultiplier();
        yield return new WaitForSeconds(duration);
        multiplierStack.Remove(id);
        RecalcMultiplier();
    }

    private void RecalcMultiplier()
    {
        float mult = 1f;
        foreach (var kv in multiplierStack)
            mult *= kv.Value;
        tempMultiplier = Mathf.Max(0f, mult);
    }

    public void ActivateShield(float duration)
    {
        StartCoroutine(ShieldRoutine(duration));
    }

    private IEnumerator ShieldRoutine(float duration)
    {
        IsShieldActive = true;
        Debug.Log($"🛡 {playerName} 启动防护罩");
        yield return new WaitForSeconds(duration);
        IsShieldActive = false;
        Debug.Log($"🛡 {playerName} 防护罩结束");
    }
    
    public void ApplyTempMultiplierBuff(float rate, float duration)
    {
        int id = Random.Range(1, int.MaxValue);
        StartCoroutine(TempBuffRoutine(id, rate, duration));
    }

    private IEnumerator TempBuffRoutine(int id, float rate, float duration)
    {
        multiplierStack[id] = rate;
        RecalcMultiplier();
        yield return new WaitForSeconds(duration);
        multiplierStack.Remove(id);
        RecalcMultiplier();
    }

    public void RemoveTempMultiplierBuff(float rate)
    {
        // 防御性移除
        List<int> keysToRemove = new List<int>();
        foreach (var kv in multiplierStack)
            if (Mathf.Approximately(kv.Value, rate))
                keysToRemove.Add(kv.Key);

        foreach (var key in keysToRemove)
            multiplierStack.Remove(key);
        RecalcMultiplier();
    }

}