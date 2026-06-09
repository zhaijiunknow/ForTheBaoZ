using System.Collections;
using UnityEngine;

public class BattleReversalSystem : MonoBehaviour
{
    [Header("战斗玩家引用")]
    public PlayerThrustManager redPlayer;
    public PlayerThrustManager bluePlayer;
    public BattleManager batMgr;
    public BattleRunState runState;
    [Header("触发条件")]
    public float triggerDistance = 100f;
    public float reverseEndDistance = 100f;

    [Header("效果配置")]
    public float boostMultiplier = 2f;
    public float boostDuration = 100f;

    private bool redTriggered = false;
    private bool blueTriggered = false;

    private void Update()
    {
        if (!DynamicData.GameStart || (runState != null && runState.isPausedForStopover)) return;
        CheckBattleState();
    }

    private void CheckBattleState()
    {
        float redVal = (int)(1000 + batMgr.redDistance);
        float blueVal = (int)(2000 - (1000 + batMgr.redDistance));
        if (redVal < reverseEndDistance && !redTriggered)
        {
            StartCoroutine(TriggerReversal(redPlayer, true));
            redTriggered = true;
            UISystem.Ins.battleUI.Reversal();
        }
        else if (blueVal < reverseEndDistance && !blueTriggered)
        {
            StartCoroutine(TriggerReversal(bluePlayer, false));
            blueTriggered = true;
            UISystem.Ins.battleUI.Reversal();
        }
    }

    private IEnumerator TriggerReversal(PlayerThrustManager weaker,bool isRed)
    {
        Debug.Log($"🔥 {weaker.playerName} 触发绝地反击！推力×{boostMultiplier} 持续{boostDuration}s");
        weaker.ApplyTempMultiplierBuff(boostMultiplier, boostDuration);
        UISystem.Ins.battleUI.ReversalTime(isRed);
        float elapsed = 0f;
        while (elapsed < boostDuration)
        {
            elapsed += Time.deltaTime;

            float disVal = 0;
            if (isRed)
                disVal = (int)(1000 + batMgr.redDistance);
            else
                disVal = (int)(2000 - (1000 + batMgr.redDistance));

            if (disVal > reverseEndDistance)
            {
                Debug.Log($"🔥 {weaker.playerName} 绝地反击提前结束（已反超 {disVal:F1}）");
                weaker.RemoveTempMultiplierBuff(boostMultiplier);
                UISystem.Ins.battleUI.EndReversalTime(isRed);
                yield break;
            }

            if (!DynamicData.GameStart) yield break;

            yield return null;
        }

        weaker.RemoveTempMultiplierBuff(boostMultiplier);
        UISystem.Ins.battleUI.EndReversalTime(isRed);
        Debug.Log($"🔥 {weaker.playerName} 绝地反击结束");
    }
}
