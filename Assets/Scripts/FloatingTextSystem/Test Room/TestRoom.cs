using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace FlyRabbit.FloatingTextSystem
{
    [RequireComponent(typeof(Button))]
    public class TestRoom : MonoBehaviour
    {
        private Button m_TestButton;

        public string 测试文本 = "10086";
        public TMP_FontAsset 字体文件;
        [Min(0)]
        public float 存在时间 = 0;
        public Vector2 起始位置偏移 = Vector2.zero;
        [Min(0)]
        public float 起始文字大小 = 50;
        public Color 起始文本颜色 = Color.white;
        public HorizontalAlignmentOptions 对齐方式 = HorizontalAlignmentOptions.Left;
        public 物理动画[] 物理动画;
        public 文字大小动画[] 文字大小动画;
        public 透明度动画[] 透明度动画;

        private void Test()
        {
            //获取物理动画信息
            PhysicsAnimationInfo[] physicsAnimationInfos = TestRoomTools.ToPhysicsAnimationInfos(物理动画);
            //获取字体大小动画信息
            FontSizeAnimationInfo[] fontSizeAnimationInfos = TestRoomTools.ToFontSizeAnimationInfos(文字大小动画);
            //获取透明度动画信息
            AlphaAnimationInfo[] alphaAnimationInfos = TestRoomTools.ToAlphaAnimationInfos(透明度动画);
            //组装，调用
            FloatingTextStyle Style = new FloatingTextStyle(存在时间, 起始位置偏移, 起始文字大小, 起始文本颜色, 对齐方式, physicsAnimationInfos, fontSizeAnimationInfos, alphaAnimationInfos);
            FloatingTextSystem.Instance.CreateFloatingUIText(测试文本, Style, Vector3.down, null, 字体文件);
        }

        private void Awake()
        {
            m_TestButton = GetComponent<Button>();
            m_TestButton.onClick.AddListener(Test);
        }
#if UNITY_EDITOR
        [ContextMenu("转为代码")]
        private void Coding()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine(TestRoomTools.ToPhysicsAnimationInfosCode(物理动画));
            result.AppendLine(TestRoomTools.ToFontSizeAnimationInfosCode(文字大小动画));
            result.AppendLine(TestRoomTools.ToAlphaAnimationInfosCode(透明度动画));
            result.Append($"FloatingTextStyle style = new FloatingTextStyle({存在时间}f, new Vector2({起始位置偏移.x}f,{起始位置偏移.y}f), {起始文字大小}f, new Color({起始文本颜色.r}f, {起始文本颜色.g}f, {起始文本颜色.b}f, {起始文本颜色.a}f), HorizontalAlignmentOptions.{对齐方式}, physicsAnimationInfos, fontSizeAnimationInfos, alphaAnimationInfos);");
            TextEditor textEditor = new TextEditor();
            textEditor.text = result.ToString();
            textEditor.SelectAll();
            textEditor.Copy();
            EditorUtility.DisplayDialog("转为代码", "已转为代码并复制到剪贴板。", "确定");
        }
#endif
    }
}
