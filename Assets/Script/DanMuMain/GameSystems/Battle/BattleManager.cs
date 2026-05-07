using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BattleManager : MonoBehaviour
{
    public PlayerThrustManager redPlayer;
    public PlayerThrustManager bluePlayer;
    public ThrustSpeedConfig speedConfig;
    public float redDistance = 0f;
    public float buleDistance = 0f;
    public float winDistance = 1000f;

    void Update()
    {
        float diff = redPlayer.TotalThrust - bluePlayer.TotalThrust;
        float abs = Mathf.Abs(diff);
        float tempSpeed = 10;
        float speed = tempSpeed;

        if (diff > 0)
        {
            //红方推进
            tempSpeed = speedConfig.GetSpeed(redDistance, abs);
            speed = tempSpeed;
            //Debug.Log("红方推进 " + speed);
        }
        else if (diff < 0)
        {
            //蓝方推进
            tempSpeed = speedConfig.GetSpeed(redDistance, abs);
            speed = -tempSpeed;
            //Debug.Log("蓝方推进 " + speed);
        }
        else
        {
            speed = 0;
        }
        redDistance += speed * Time.deltaTime;
        buleDistance = winDistance - redDistance;
        
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