using UnityEngine;

namespace FlyRabbit.FloatingTextSystem
{
    [System.Serializable]
    public class PhysicsAnimationInfo
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
        /// 要施加的力度的最小取值，系统会在最小和最大之间随机取值。
        /// </summary>
        public Vector3 MinVelocity { get; private set; }
        /// <summary>
        /// 要施加的力度的最大取值，系统会在最小和最大之间随机取值。
        /// </summary>
        public Vector3 MaxVelocity { get; private set; }
        /// <summary>
        /// 受到的重力
        /// </summary>
        public Vector3 Gravity { get; private set; }

        public PhysicsAnimationInfo(float startTime, float duration, Vector2 minVelocity, Vector2 maxVelocity, Vector2 gravity)
        {
            StartTime = Mathf.Clamp01(startTime);
            Duration = Mathf.Clamp(duration, 0.01f, 1f - StartTime);
            MinVelocity = new Vector3(minVelocity.x, minVelocity.y, 0);
            MaxVelocity = new Vector3(maxVelocity.x, maxVelocity.y, 0);
            Gravity = gravity;
#if UNITY_EDITOR
            if (Mathf.Approximately(startTime, StartTime) == false)
            {
                Debug.LogWarning($"FontSizeAnimationInfo 参数已自动修正: StartTime({startTime}→{StartTime})");
            }
            if (Mathf.Approximately(duration, Duration) == false)
            {
                Debug.LogWarning($"FontSizeAnimationInfo 参数已自动修正:  Duration({duration}→{Duration})");
            }
#endif
        }
    }
}
