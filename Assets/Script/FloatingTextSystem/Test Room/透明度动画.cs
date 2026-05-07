using UnityEngine;
using System;
namespace FlyRabbit.FloatingTextSystem
{
    [Serializable]
    public class 透明度动画
    {
        [Range(0, 1)]
        public float 开始时间;
        [Range(0, 1)]
        public float 持续时间;
        [Range(0, 1)]
        public float 目标透明度;
    }
}
