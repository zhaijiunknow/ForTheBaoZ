using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BattleManager : MonoBehaviour
{
    public PlayerThrustManager redPlayer;
    public PlayerThrustManager bluePlayer;
    public ThrustSpeedConfig speedConfig;
    public BattleRunState runState;
    public float redDistance = 0f;
    public float buleDistance = 0f;
    public float winDistance = 1000f;

    void Update()
    {
        if (runState != null && runState.isPausedForStopover)
            return;

        float diff = redPlayer.TotalThrust - bluePlayer.TotalThrust;
        float abs = Mathf.Abs(diff);
        float tempSpeed = 10;
        float speed = tempSpeed;

        if (diff > 0)
        {
            tempSpeed = speedConfig.GetSpeed(redDistance, abs);
            speed = tempSpeed;
        }
        else if (diff < 0)
        {
            tempSpeed = speedConfig.GetSpeed(redDistance, abs);
            speed = -tempSpeed;
        }
        else
        {
            speed = 0;
        }
        redDistance += speed * Time.deltaTime;
        buleDistance = winDistance - redDistance;

        if (runState != null)
            runState.SetProgress(Mathf.Max(0f, redDistance));

        if (redDistance >= winDistance)
        {
            Debug.Log("🔥 红方获胜！");
            enabled = false;
        }
        else if (redDistance <= -winDistance)
        {
            Debug.Log("💙 蓝方获胜！");
            enabled = false;
        }
    }
}