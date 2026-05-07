using System.Text;

namespace FlyRabbit.FloatingTextSystem
{
    public static class TestRoomTools
    {
        /// <summary>
        /// 转为FontSizeAnimationInfo数组
        /// </summary>
        /// <param name="文字大小动画"></param>
        /// <returns></returns>
        public static FontSizeAnimationInfo[] ToFontSizeAnimationInfos(文字大小动画[] 文字大小动画)
        {
            if (文字大小动画 == null || 文字大小动画.Length == 0)
            {
                return null;
            }
            FontSizeAnimationInfo[] result = new FontSizeAnimationInfo[文字大小动画.Length];
            for (int i = 0; i < result.Length; i++)
            {
                文字大小动画 item = 文字大小动画[i];
                result[i] = new FontSizeAnimationInfo(item.开始时间, item.持续时间, item.目标文字大小);
            }
            return result;
        }
        /// <summary>
        /// 转为AlphaAnimationInfo数组
        /// </summary>
        /// <param name="透明度动画"></param>
        /// <returns></returns>
        public static AlphaAnimationInfo[] ToAlphaAnimationInfos(透明度动画[] 透明度动画)
        {
            if (透明度动画 == null || 透明度动画.Length == 0)
            {
                return null;
            }
            AlphaAnimationInfo[] result = new AlphaAnimationInfo[透明度动画.Length];
            for (int i = 0; i < result.Length; i++)
            {
                透明度动画 item = 透明度动画[i];
                result[i] = new AlphaAnimationInfo(item.开始时间, item.持续时间, item.目标透明度);
            }
            return result;
        }
        /// <summary>
        /// 转为PhysicsAnimationInfo数组
        /// </summary>
        /// <param name="物理动画"></param>
        /// <returns></returns>
        public static PhysicsAnimationInfo[] ToPhysicsAnimationInfos(物理动画[] 物理动画)
        {
            if (物理动画 == null || 物理动画.Length == 0)
            {
                return null;
            }
            PhysicsAnimationInfo[] result = new PhysicsAnimationInfo[物理动画.Length];
            for (int i = 0; i < result.Length; i++)
            {
                物理动画 item = 物理动画[i];
                result[i] = new PhysicsAnimationInfo(item.开始时间, item.持续时间, item.最小力度, item.最大力度, item.重力);
            }
            return result;
        }
        /// <summary>
        /// 物理动画数组转为代码
        /// </summary>
        /// <param name="物理动画"></param>
        /// <returns></returns>
        public static string ToPhysicsAnimationInfosCode(物理动画[] 物理动画)
        {
            PhysicsAnimationInfo[] infos = ToPhysicsAnimationInfos(物理动画);
            if (infos == null)
            {
                return "PhysicsAnimationInfo[] physicsAnimationInfos = null;";
            }
            StringBuilder result = new StringBuilder();
            result.Append($"PhysicsAnimationInfo[] physicsAnimationInfos = new PhysicsAnimationInfo[{infos.Length}]");
            result.Append("{");
            for (int i = 0; i < infos.Length; i++)
            {
                result.Append($"new PhysicsAnimationInfo({infos[i].StartTime}f,{infos[i].Duration}f,new Vector2({infos[i].MinVelocity.x}f,{infos[i].MinVelocity.y}f),new Vector2({infos[i].MaxVelocity.x}f,{infos[i].MaxVelocity.y}f),new Vector2({infos[i].Gravity.x}f,{infos[i].Gravity.y}f))");
                if (i != infos.Length - 1)
                {
                    result.Append(",");
                }
            }
            result.Append("};");
            return result.ToString();
        }
        /// <summary>
        /// 文字大小动画数组转为代码
        /// </summary>
        /// <param name="文字大小动画"></param>
        /// <returns></returns>
        public static string ToFontSizeAnimationInfosCode(文字大小动画[] 文字大小动画)
        {
            FontSizeAnimationInfo[] infos = ToFontSizeAnimationInfos(文字大小动画);
            if (infos == null)
            {
                return "FontSizeAnimationInfo[] fontSizeAnimationInfos = null;";
            }
            StringBuilder result = new StringBuilder();
            result.Append($"FontSizeAnimationInfo[] fontSizeAnimationInfos = new FontSizeAnimationInfo[{infos.Length}]");
            result.Append("{");
            for (int i = 0; i < infos.Length; i++)
            {
                result.Append($"new FontSizeAnimationInfo({infos[i].StartTime}f,{infos[i].Duration}f,{infos[i].TargetSize}f)");              
                if (i != infos.Length - 1)
                {
                    result.Append(",");
                }
            }
            result.Append("};");
            return result.ToString();
        }
        /// <summary>
        /// 透明度动画数组转为代码
        /// </summary>
        /// <param name="透明度动画"></param>
        /// <returns></returns>
        public static string ToAlphaAnimationInfosCode(透明度动画[] 透明度动画)
        {
            AlphaAnimationInfo[] infos = ToAlphaAnimationInfos(透明度动画);
            
            if (infos == null)
            {
                return "AlphaAnimationInfo[] alphaAnimationInfos = null;";
            }
            StringBuilder result = new StringBuilder();
            result.Append($"AlphaAnimationInfo[] alphaAnimationInfos = new AlphaAnimationInfo[{infos.Length}]");
            result.Append("{");
            for (int i = 0; i < infos.Length; i++)
            {              
                result.Append($"new AlphaAnimationInfo({infos[i].StartTime}f,{infos[i].Duration}f,{infos[i].TargetAlpha}f)");
                if (i != infos.Length - 1)
                {
                    result.Append(",");
                }
            }
            result.Append("};");
            return result.ToString();
        }
    }
}
