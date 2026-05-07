using UnityEngine;

namespace FlyRabbit.FloatingTextSystem
{
    [System.Serializable]
    public class 文字大小动画
    {
        [Range(0, 1)]
        public float 开始时间;
        [Range(0, 1)]
        public float 持续时间;
        [Range(0, 5)]
        [Tooltip("这是一个百分比数值（相较于初始文字大小）。")]
        public float 目标文字大小;
    }
}
