using UnityEngine;

public class AlphaAnimationInfo
{
    /// <summary>
    /// 在文本的生命周期内，动画在哪个时间点开始工作，
    /// 这是个百分比数值，取值范围（0~1）
    /// </summary>
    public float StartTime { get; private set; }
    /// <summary>
    /// 在文本的生命周期内，动画的持续时间
    /// 这是个百分比数值，取值范围（0~1）
    /// </summary>
    public float Duration { get; private set; }
    /// <summary>
    /// 动画结束时，期望的透明度,
    /// 取值范围（0~1）
    /// </summary>
    public float TargetAlpha { get; private set; }


    public AlphaAnimationInfo(float startTime, float duration, float targetAlpha)
    {
        StartTime = Mathf.Clamp01(startTime);
        Duration = Mathf.Clamp(duration, 0.01f, 1f - StartTime);
        TargetAlpha = Mathf.Clamp01(targetAlpha);
#if UNITY_EDITOR
        if (Mathf.Approximately(startTime, StartTime) == false)
        {
            Debug.LogWarning($"AlphaAnimationInfo 参数已自动修正: StartTime({startTime}→{StartTime})");
        }
        if (Mathf.Approximately(duration, Duration) == false)
        {
            Debug.LogWarning($"AlphaAnimationInfo 参数已自动修正:  Duration({duration}→{Duration})");
        }
        if (Mathf.Approximately(targetAlpha, TargetAlpha) == false)
        {
            Debug.LogWarning($"AlphaAnimationInfo 参数已自动修正:  TargetSize({targetAlpha}→{TargetAlpha})");
        }
#endif
    }
}
