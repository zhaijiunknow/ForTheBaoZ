using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/ThrustSpeedConfig")]
public class ThrustSpeedConfig : ScriptableObject
{
    [Tooltip("所有的里程阶段配置")]
    public List<DistanceStage> distanceStages = new List<DistanceStage>();

    /// <summary>
    /// 获取当前里程和推力差对应的速度
    /// </summary>
    public float GetSpeed(float distance, float thrustDiff)
    {
        //Debug.Log("dis " + distance + "  " + thrustDiff);
        foreach (var stage in distanceStages)
        {
            if (distance <= stage.maxDistance && distance > stage.minDistance)
            {
                foreach (var range in stage.thrustRanges)
                {
                    if (thrustDiff <= range.maxThrustDiff && thrustDiff > range.minThrustDiff)
                    {
                        return range.speed;
                    }
                }

                // 若推力差超出定义范围，用最后一个速度
                if (stage.thrustRanges.Count > 0)
                    return stage.thrustRanges[stage.thrustRanges.Count - 1].speed;
            }
        }

        // 没匹配上时返回 0
        return 0f;
    }
}

[Serializable]
public class DistanceStage
{
    [Header("里程区间 (minDistance < x <= maxDistance)")]
    public float minDistance;
    public float maxDistance;

    [Header("推力差与速度映射")]
    public List<ThrustSpeedRange> thrustRanges = new List<ThrustSpeedRange>();
}

[Serializable]
public class ThrustSpeedRange
{
    [Header("推力差区间 (minThrustDiff < x <= maxThrustDiff)")]
    public float minThrustDiff;
    public float maxThrustDiff;

    [Header("对应速度")]
    public float speed;
}