using UnityEngine;
namespace FlyRabbit.FloatingTextSystem
{
    [System.Serializable]
    public class 物理动画
    {
        [Range(0, 1)]
        public float 开始时间;
        [Range(0, 1)]
        public float 持续时间;
        public Vector2 最小力度;
        public Vector2 最大力度;
        public Vector2 重力;
    }
}
