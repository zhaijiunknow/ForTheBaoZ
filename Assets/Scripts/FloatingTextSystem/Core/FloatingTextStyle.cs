using TMPro;
using UnityEngine;

namespace FlyRabbit.FloatingTextSystem
{
    public class FloatingTextStyle
    {
        /// <summary>
        /// 存在时间，以秒为单位。
        /// 取值范围（>0）
        /// </summary>
        public float LifeTime { get; private set; }
        /// <summary>
        /// 起始位置偏移量（屏幕空间）
        /// </summary>
        public Vector3 PositionOffset { get; private set; }
        /// <summary>
        /// 起始文字大小。
        /// 取值范围（>0）
        /// </summary>
        public float FontSize { get; private set; }
        /// <summary>
        /// 起始文本颜色
        /// </summary>
        public Color Color { get; private set; }
        /// <summary>
        /// 对齐方式,决定了文本物体原点相对于文本的位置
        /// </summary>
        public HorizontalAlignmentOptions Alignment { get; private set; }
        /// <summary>
        /// 物理动画信息
        /// </summary>
        public PhysicsAnimationInfo[] PhysicsAnimationInfos { get; private set; }
        /// <summary>
        /// 字体大小动画信息
        /// </summary>
        public FontSizeAnimationInfo[] FontSizeAnimationInfos { get; private set; }
        /// <summary>
        /// 透明度动画信息
        /// </summary>
        public AlphaAnimationInfo[] AlphaAnimationInfos { get; private set; }

        public FloatingTextStyle(float lifeTime, Vector2 positionOffset, float fontSize, Color color, HorizontalAlignmentOptions alignment,
                                 PhysicsAnimationInfo[] physicsAnimationInfos, FontSizeAnimationInfo[] fontSizeAnimationInfos, AlphaAnimationInfo[] alphaAnimationInfos)
        {
            LifeTime = Mathf.Max(0, lifeTime);
            PositionOffset = new Vector3(positionOffset.x, positionOffset.y, 0);
            FontSize = Mathf.Max(0, fontSize);
            Color = color;
            Alignment = alignment;
            PhysicsAnimationInfos = physicsAnimationInfos;
            FontSizeAnimationInfos = fontSizeAnimationInfos;
            AlphaAnimationInfos = alphaAnimationInfos;
        }
    }
}
