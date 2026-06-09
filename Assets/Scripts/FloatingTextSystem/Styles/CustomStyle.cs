using UnityEngine;
using TMPro;
namespace FlyRabbit.FloatingTextSystem
{
    public static class CustomStyle
    {
        /// <summary>
        /// 暴击！
        /// </summary>
        public static FloatingTextStyle Crit { get; private set; }
        /// <summary>
        /// 模仿原神的伤害飘字（只有向上的位移）
        /// </summary>
        public static FloatingTextStyle Genshin { get; private set; }
        /// <summary>
        /// 在左右30度范围内向上飞
        /// </summary>
        public static FloatingTextStyle Custom1 { get; private set; }
        /// <summary>
        /// 保持0.2秒大号字体，然后缩小，再然后向上飘逐渐透明消失
        /// </summary>
        public static FloatingTextStyle Custom2 { get; private set; }
        static CustomStyle()
        {
            {
                PhysicsAnimationInfo[] physicsAnimationInfos = new PhysicsAnimationInfo[2] { new PhysicsAnimationInfo(0.1f, 0.15f, new Vector2(-666f, -666f), new Vector2(-666f, -666f), new Vector2(0f, 0f)), new PhysicsAnimationInfo(0.5f, 0.5f, new Vector2(250f, 500f), new Vector2(250f, 500f), new Vector2(0f, -1200f)) };
                FontSizeAnimationInfo[] fontSizeAnimationInfos = new FontSizeAnimationInfo[1] { new FontSizeAnimationInfo(0.1f, 0.15f, 0.4f) };
                AlphaAnimationInfo[] alphaAnimationInfos = new AlphaAnimationInfo[1] { new AlphaAnimationInfo(0.7f, 0.3f, 0f) };
                Crit = new FloatingTextStyle(1f, new Vector2(100f, 100f), 150f, new Color(1f, 1f, 1f, 1f), HorizontalAlignmentOptions.Left, physicsAnimationInfos, fontSizeAnimationInfos, alphaAnimationInfos);
            }
            {
                PhysicsAnimationInfo[] physicsAnimationInfos = new PhysicsAnimationInfo[1] { new PhysicsAnimationInfo(0f, 1f, new Vector2(0f, 50f), new Vector2(0f, 50f), new Vector2(0f, 0f)) };
                FontSizeAnimationInfo[] fontSizeAnimationInfos = new FontSizeAnimationInfo[1] { new FontSizeAnimationInfo(0f, 0.2f, 0.25f) };
                AlphaAnimationInfo[] alphaAnimationInfos = new AlphaAnimationInfo[1] { new AlphaAnimationInfo(0.9f, 0.1f, 0f) };
                Genshin = new FloatingTextStyle(1f, new Vector3(0f, 0f), 250f, new Color(1f, 1f, 1f, 1f), HorizontalAlignmentOptions.Center, physicsAnimationInfos, fontSizeAnimationInfos, alphaAnimationInfos);
            }
            {
                PhysicsAnimationInfo[] physicsAnimationInfos = new PhysicsAnimationInfo[1] { new PhysicsAnimationInfo(0f, 1f, new Vector2(-250f, 433f), new Vector2(250f, 433f), new Vector2(0f, -1000f)) };
                FontSizeAnimationInfo[] fontSizeAnimationInfos = new FontSizeAnimationInfo[1] { new FontSizeAnimationInfo(0f, 1f, 1.25f) };
                AlphaAnimationInfo[] alphaAnimationInfos = new AlphaAnimationInfo[1] { new AlphaAnimationInfo(0f, 1f, 0f) };
                Custom1 = new FloatingTextStyle(0.5f, new Vector2(0f, 0f), 50f, new Color(1f, 1f, 1f, 1f), HorizontalAlignmentOptions.Center, physicsAnimationInfos, fontSizeAnimationInfos, alphaAnimationInfos);
            }
            {
                PhysicsAnimationInfo[] physicsAnimationInfos = new PhysicsAnimationInfo[1] { new PhysicsAnimationInfo(0.4f, 0.6f, new Vector2(0f, 100f), new Vector2(0f, 100f), new Vector2(0f, 0f)) };
                FontSizeAnimationInfo[] fontSizeAnimationInfos = new FontSizeAnimationInfo[1] { new FontSizeAnimationInfo(0.2f, 0.2f, 0.5f) };
                AlphaAnimationInfo[] alphaAnimationInfos = new AlphaAnimationInfo[1] { new AlphaAnimationInfo(0.4f, 0.6f, 0f) };
                Custom2 = new FloatingTextStyle(1f, new Vector3(0f, 0f), 100f, new Color(1f, 1f, 1f, 1f), HorizontalAlignmentOptions.Center, physicsAnimationInfos, fontSizeAnimationInfos, alphaAnimationInfos);
            }
        }
    }
}
