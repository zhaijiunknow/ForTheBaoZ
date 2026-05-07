using UnityEngine;

[System.Serializable]
public class FontSizeAnimationInfo
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
    /// 动画结束时，期望的字体大小，
    /// 这是个百分比数值,取值范围（>0）
    /// </summary>
    public float TargetSize { get; private set; }

    public FontSizeAnimationInfo(float startTime, float duration, float targetSize)
    {
        StartTime = Mathf.Clamp01(startTime);
        Duration = Mathf.Clamp(duration, 0.01f, 1f - StartTime);
        TargetSize = Mathf.Max(0, targetSize);
#if UNITY_EDITOR
        if (Mathf.Approximately(startTime, StartTime) == false)
        {
            Debug.LogWarning($"FontSizeAnimationInfo 参数已自动修正: StartTime({startTime}→{StartTime})");
        }
        if (Mathf.Approximately(duration, Duration) == false)
        {
            Debug.LogWarning($"FontSizeAnimationInfo 参数已自动修正:  Duration({duration}→{Duration})");
        }
        if (Mathf.Approximately(targetSize, TargetSize) == false)
        {
            Debug.LogWarning($"FontSizeAnimationInfo 参数已自动修正:  TargetSize({targetSize}→{TargetSize})");
        }
#endif
    }
}
