using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace FlyRabbit.FloatingTextSystem
{
    public class FloatingTextSystem : MonoBehaviour
    {
        #region 单例
        private static FloatingTextSystem m_Instance;
        public static FloatingTextSystem Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    GameObject gameObject = new GameObject("Floating Text System");
                    DontDestroyOnLoad(gameObject);
                    m_Instance = gameObject.AddComponent<FloatingTextSystem>();
                }
                return m_Instance;
            }
        }
        #endregion
        /// <summary>
        /// 文本要对准的相机，如果不设置则默认使用主相机，如果主相机也不存在，部分功能会不可用。
        /// </summary>
        public Camera Camera { get; set; }
        /// <summary>
        /// UI文本的CanvasScaler，通过它调整canvas缩放设置
        /// </summary>
        public CanvasScaler CanvasScaler { get; private set; }

        /// <summary>
        /// 世界文本根节点
        /// </summary>
        private Transform m_WorldTextRoot;
        /// <summary>
        /// 世界文本模板
        /// </summary>
        private GameObject m_WorldTextTemplate;
        /// <summary>
        /// 世界文本对象池
        /// </summary>
        private ObjectPool<TextMeshPro> m_WorldTextPool;


        /// <summary>
        /// UI文本根节点
        /// </summary>
        private Transform m_UITextRoot;
        /// <summary>
        /// UI文本模板
        /// </summary>
        private GameObject m_UITextTemplate;
        /// <summary>
        /// UI文本对象池
        /// </summary>
        private ObjectPool<TextMeshProUGUI> m_UITextPool;

        /// <summary>
        /// 当前可用的固定UI文本编号
        /// </summary>
        private int m_CurrentFixedUITextNumber = 0;
        /// <summary>
        /// 当前可用的固定世界文本编号
        /// </summary>
        private int m_CurrentFixedWorldTextNumber = 0;
        /// <summary>
        /// 当前可用的浮动UI文本编号
        /// </summary>
        private int m_CurrentFloatingUITextNumber = 0;

        /// <summary>
        /// 存储所有使用中的固定世界文本，key为编号
        /// </summary>
        private readonly Dictionary<int, TextMeshPro> m_FixedWorldTexts = new Dictionary<int, TextMeshPro>(100);
        /// <summary>
        /// 存储所有使用中的固定UI文本，key为编号
        /// </summary>
        private readonly Dictionary<int, TextMeshProUGUI> m_FixedUITexts = new Dictionary<int, TextMeshProUGUI>(100);
        /// <summary>
        /// 存储所有使用中的浮动UI文本，key为编号
        /// </summary>
        private readonly Dictionary<int, TextMeshProUGUI> m_FloatingUITexts = new Dictionary<int, TextMeshProUGUI>(1000);

        /// <summary>
        /// List Vector3 对象池
        /// </summary>
        private ObjectPool<List<Vector3>> m_Vector3ListPool;
        /// <summary>
        /// List float 对象池
        /// </summary>
        private ObjectPool<List<float>> m_FloatListPool;
        /// <summary>
        /// List bool 对象池
        /// </summary>
        private ObjectPool<List<bool>> m_BoolListPool;
        #region Public Methods
        #region FixedWorldText API
        /// <summary>
        /// 创建一个固定世界文本，可以设置跟随。
        /// </summary>
        /// <param name="content"></param>
        /// <param name="fontSize"></param>
        /// <param name="positionOrOffset">如果没有跟随目标，则为文本的坐标，如果有跟随目标，则为跟随目标坐标的偏移</param>
        /// <param name="followTarget"></param>
        /// <param name="fontAsset"></param>
        /// <returns></returns>
        public int CreateFixedWorldText(string content, float fontSize, Vector3 positionOrOffset, Transform followTarget = null, TMP_FontAsset fontAsset = null)
        {
            TextMeshPro textMeshPro = m_WorldTextPool.Get();
            textMeshPro.text = content;
            textMeshPro.fontSize = fontSize;
            textMeshPro.font = fontAsset;
            textMeshPro.transform.localPosition = positionOrOffset;

            int number = GenerateFixedWorldTextNumber();
            m_FixedWorldTexts.Add(number, textMeshPro);
            StartCoroutine(FixedWorldTextRun(number, positionOrOffset, followTarget));
            return number;
        }
        /// <summary>
        /// 销毁指定的固定世界文本
        /// </summary>
        /// <param name="number">编号</param>
        /// <exception cref="Exception"></exception>
        public void DestroyFixedWorldText(int number)
        {
            TextMeshPro textMeshPro;
            if (m_FixedWorldTexts.TryGetValue(number, out textMeshPro) == false)
            {
#if UNITY_EDITOR
                Debug.Log("您想要销毁的文本已经被销毁，请检查您的代码。");
#endif
                return;
            }
            m_FixedWorldTexts.Remove(number);
            m_WorldTextPool.Release(textMeshPro);
        }
        /// <summary>
        /// 销毁所有固定世界文本
        /// </summary>
        public void DestroyAllFixedWorldText()
        {
            foreach (var item in m_FixedWorldTexts.Values)
            {
                m_WorldTextPool.Release(item);
            }
            m_FixedWorldTexts.Clear();
        }
        #endregion
        #region FixedUIText API      
        /// <summary>
        /// 创建一个固定UI文本
        /// </summary>
        /// <param name="content"></param>
        /// <param name="fontSize"></param>
        /// <param name="positionOrOffset"></param>
        /// <param name="followTarget"></param>
        /// <param name="fontAsset"></param>
        /// <returns></returns>
        public int CreateFixedUIText(string content, float fontSize, Vector3 positionOrOffset, Transform followTarget = null, TMP_FontAsset fontAsset = null)
        {
            TextMeshProUGUI textMeshProUGUI = m_UITextPool.Get();
            textMeshProUGUI.text = content;
            textMeshProUGUI.fontSize = fontSize;
            textMeshProUGUI.font = fontAsset;

            int number = GenerateFixedUITextNumber();
            m_FixedUITexts.Add(number, textMeshProUGUI);
            StartCoroutine(FixedUITextRun(number, positionOrOffset, followTarget));
            return number;
        }
        /// <summary>
        /// 销毁指定的固定UI文本
        /// </summary>
        /// <param name="unmber"></param>
        public void DestroyFixedUIText(int number)
        {
            TextMeshProUGUI textMeshProUGUI;
            if (m_FixedUITexts.TryGetValue(number, out textMeshProUGUI) == false)
            {
#if UNITY_EDITOR
                Debug.Log("您想要销毁的文本已经被销毁，请检查您的代码。");
#endif
                return;
            }
            m_FixedUITexts.Remove(number);
            m_UITextPool.Release(textMeshProUGUI);
        }
        /// <summary>
        /// 销毁所有固定UI文本
        /// </summary>
        public void DestroyAllFixedUIText()
        {
            foreach (var item in m_FixedUITexts.Values)
            {
                m_UITextPool.Release(item);
            }
            m_FixedUITexts.Clear();
        }
        #endregion
        #region FloatingUIText API
        /// <summary>
        /// 创造一个漂浮UI文本
        /// </summary>
        /// <param name="content"></param>
        /// <param name="textStyle"></param>
        /// <param name="positionOrOffset"></param>
        /// <param name="followTarget"></param>
        /// <param name="fontSizeOverride"></param>
        public void CreateFloatingUIText(string content, FloatingTextStyle textStyle, Vector3 positionOrOffset, Transform followTarget = null, TMP_FontAsset fontAsset = null, float? fontSizeOverride = null, Color? colorOverride = null)
        {
            TextMeshProUGUI textMeshProUGUI = m_UITextPool.Get();
            textMeshProUGUI.text = content;
            textMeshProUGUI.font = fontAsset;

            int number = GenerateFloatingUITextNumber();
            m_FloatingUITexts.Add(number, textMeshProUGUI);
            StartCoroutine(FloatingUITextRun(number, textStyle, positionOrOffset, followTarget, fontSizeOverride, colorOverride));
        }
        /// <summary>
        /// 销毁所有漂浮UI文本
        /// </summary>
        public void DestroyAllFloatingUIText()
        {
            foreach (var item in m_FloatingUITexts.Values)
            {
                m_UITextPool.Release(item);
            }
            m_FloatingUITexts.Clear();
        }
        #endregion
        #endregion
        #region Private Methods
        /// <summary>
        /// 生成固定世界文本编号
        /// 如果游戏里有2147483646个固定世界文本会导致死循环，卡死游戏。
        /// </summary>
        /// <returns></returns>
        private int GenerateFixedWorldTextNumber()
        {
            while (true)
            {
                m_CurrentFixedWorldTextNumber++;
                if (m_CurrentFixedWorldTextNumber == int.MaxValue)
                {
                    m_CurrentFixedWorldTextNumber = 1;
                }
                if (m_FixedWorldTexts.ContainsKey(m_CurrentFixedWorldTextNumber) == false)
                {
                    break;
                }
            }
            return m_CurrentFixedWorldTextNumber;
        }
        /// <summary>
        /// 生成固定UI文本编号
        /// 如果游戏里有2147483646个固定UI文本会导致死循环，卡死游戏。
        /// </summary>
        /// <returns></returns>
        private int GenerateFixedUITextNumber()
        {
            while (true)
            {
                m_CurrentFixedUITextNumber++;
                if (m_CurrentFixedUITextNumber == int.MaxValue)
                {
                    m_CurrentFixedUITextNumber = 1;
                }
                if (m_FixedUITexts.ContainsKey(m_CurrentFixedUITextNumber) == false)
                {
                    break;
                }
            }
            return m_CurrentFixedUITextNumber;
        }
        /// <summary>
        /// 生成浮动UI文本编号
        /// 如果游戏里有2147483646个浮动UI文本会导致死循环，卡死游戏。
        /// </summary>
        /// <returns></returns>
        private int GenerateFloatingUITextNumber()
        {
            while (true)
            {
                m_CurrentFloatingUITextNumber++;
                if (m_CurrentFloatingUITextNumber == int.MaxValue)
                {
                    m_CurrentFloatingUITextNumber = 1;
                }
                if (m_FloatingUITexts.ContainsKey(m_CurrentFloatingUITextNumber) == false)
                {
                    break;
                }
            }
            return m_CurrentFloatingUITextNumber;
        }
        /// <summary>
        /// 返回是否在动画时间内
        /// </summary>
        /// <returns></returns>
        private bool IsInAnimationTime(float currentTime, float lifeTime, float startTime, float duration)
        {
            float startAbsolute = lifeTime * startTime;
            float endAbsolute = lifeTime * (startTime + duration);
            return currentTime >= startAbsolute && currentTime <= endAbsolute;
        }
        #endregion
        #region Object Pool Methods
        private TextMeshPro CreateFunc_WorldText()
        {
            GameObject gameObject = Instantiate(m_WorldTextTemplate, m_WorldTextRoot, false);
            TextMeshPro textMeshPro = gameObject.GetComponent<TextMeshPro>();
            return textMeshPro;
        }
        private void ActionOnGet_WorldText(TextMeshPro textMeshPro)
        {
            textMeshPro.gameObject.SetActive(true);
            textMeshPro.enabled = true;
        }
        private void ActionOnRelease_WorldText(TextMeshPro textMeshPro)
        {
            textMeshPro.font = null;
            textMeshPro.text = null;
            textMeshPro.gameObject.SetActive(false);
        }
        private void ActionOnDestroy_WorldText(TextMeshPro textMeshPro)
        {
            Destroy(textMeshPro.gameObject);
        }

        private TextMeshProUGUI CreateFunc_UIText()
        {
            GameObject gameObject = Instantiate(m_UITextTemplate, m_UITextRoot, false);
            TextMeshProUGUI textMeshProUGUI = gameObject.GetComponent<TextMeshProUGUI>();
            return textMeshProUGUI;
        }
        private void ActionOnGet_UIText(TextMeshProUGUI textMeshProUGUI)
        {
            textMeshProUGUI.gameObject.SetActive(true);
            textMeshProUGUI.enabled = true;
        }
        private void ActionOnRelease_UIText(TextMeshProUGUI textMeshProUGUI)
        {
            textMeshProUGUI.font = null;
            textMeshProUGUI.text = null;
            textMeshProUGUI.gameObject.SetActive(false);
        }
        private void ActionOnDestroy_UIText(TextMeshProUGUI textMeshProUGUI)
        {
            Destroy(textMeshProUGUI.gameObject);
        }

        private List<Vector3> CreateFunc_Vector3List()
        {
            return new List<Vector3>(5);
        }
        private void ActionOnRelease_Vector3List(List<Vector3> vector3List)
        {
            vector3List.Clear();
        }

        private List<float> CreateFunc_FloatList()
        {
            return new List<float>(5);
        }
        private void ActionOnRelease_FloatList(List<float> floatList)
        {
            floatList.Clear();
        }
        private List<bool> CreateFunc_BoolList()
        {
            return new List<bool>(5);
        }
        private void ActionOnRelease_BoolList(List<bool> floatList)
        {
            floatList.Clear();
        }
        #endregion
        #region Coroutine
        private IEnumerator FixedWorldTextRun(int number, Vector3 positionOrOffset, Transform followTarget)
        {
            TextMeshPro text;
            if (m_FixedWorldTexts.TryGetValue(number, out text) == false)
            {
                yield break;
            }
            //查找摄像机
            Camera targetCamera = Camera == null ? Camera.main : Camera;
            bool haveFollowTarget = followTarget == null ? false : true;
            while (true)
            {
                //如果文本已被回收，则停止协程
                if (m_FixedWorldTexts.ContainsKey(number) == false)
                {
                    yield break;
                }
                
                //如果摄像机在运行期间消失了,或者运行期间赋值了新的摄像机，重新查找摄像机
                if (Camera != null || targetCamera == null)
                {
                    targetCamera = Camera == null ? Camera.main : Camera;
                }
                //设置朝向摄像机
                if (targetCamera != null)
                {
                    text.transform.rotation = targetCamera.transform.rotation;
                }
                //设置跟随目标
                if (haveFollowTarget)
                {
                    //如果跟随目标已被回收，此文文本也要被回收,然后此协程就可以停止了
                    if (followTarget == null || followTarget.gameObject.activeSelf == false)
                    {
                        DestroyFixedWorldText(number);
                        yield break;
                    }
                    //否则跟随目标
                    text.transform.position = followTarget.position + positionOrOffset;
                }

                yield return null;
            }
        }
        private IEnumerator FixedUITextRun(int number, Vector3 positionOrOffset, Transform followTarget)
        {
            TextMeshProUGUI text;
            if (m_FixedUITexts.TryGetValue(number, out text) == false)
            {
                yield break;
            }
            //查找摄像机
            //如果摄像机不存在，无法正常工作，直接关闭。
            Camera targetCamera = Camera == null ? Camera.main : Camera;
            if (targetCamera == null)
            {
                yield break;
            }
            bool haveFollowTarget = followTarget != null;
            //获得要跟随的点（世界坐标）
            Vector3 FollowPointPosition = followTarget == null ? positionOrOffset : positionOrOffset + followTarget.position;
            while (true)
            {
                //如果文本已被回收，则停止协程
                if (m_FixedUITexts.ContainsKey(number) == false)
                {
                    yield break;
                }
                //如果摄像机在运行期间消失了,或者运行期间赋值了新的摄像机，重新查找摄像机
                //如果还是找不到，本次循环啥也不干了，等待摄像机
                if (Camera != null || targetCamera == null)
                {
                    targetCamera = Camera == null ? Camera.main : Camera;
                    if (targetCamera == null)
                    {
                        text.enabled = false;
                        continue;
                    }
                }
                if (haveFollowTarget)
                {
                    //如果跟随目标已被回收，此文本也要被回收,然后此协程就可以停止了
                    if (followTarget == null || followTarget.gameObject.activeSelf == false)
                    {
                        DestroyFixedUIText(number);
                        yield break;
                    }
                    else
                    {
                        //要跟随的点会随着目标的移动而改变
                        FollowPointPosition = positionOrOffset + followTarget.position;
                    }
                }
                //世界位置转为UI位置
                Vector3 screenPosition = targetCamera.WorldToScreenPoint(FollowPointPosition);
                text.transform.position = screenPosition;
                //决定是否要显示文本,z轴为负说明不需要渲染（不在摄像机前方视野内）。
                text.enabled = (screenPosition.z > 0);

                yield return null;
            }
        }
        private IEnumerator FloatingUITextRun(int number, FloatingTextStyle textStyle, Vector3 positionOrOffset, Transform followTarget, float? fontSizeOverride, Color? colorOverride)
        {
            //查找摄像机
            //如果摄像机不存在，漂浮文字功能无法正常工作，直接关闭。
            Camera targetCamera = Camera == null ? Camera.main : Camera;
            if (targetCamera == null)
            {
                yield break;
            }
            //获得文本对象,获得失败漂浮文字功能无法正常工作，直接关闭。
            TextMeshProUGUI text;
            if (m_FloatingUITexts.TryGetValue(number, out text) == false)
            {
                yield break;
            }
            //获得存在时间
            float lifeTime = textStyle.LifeTime;
            //获得字体大小
            float fontSize = fontSizeOverride.HasValue ? fontSizeOverride.Value : textStyle.FontSize;
            //获得文本颜色
            Color color = colorOverride.HasValue ? colorOverride.Value : textStyle.Color;
            //获得物理动画信息
            PhysicsAnimationInfo[] physicsAnimationInfos = textStyle.PhysicsAnimationInfos;
            //获得字体大小动画信息
            FontSizeAnimationInfo[] fontSizeAnimationInfos = textStyle.FontSizeAnimationInfos;
            //获得透明度动画信息
            AlphaAnimationInfo[] alphaAnimationInfos = textStyle.AlphaAnimationInfos;


            //→获得初始屏幕坐标，（不包含跟随目标的位置和偏移量）
            Vector3 ScreenPosition = textStyle.PositionOffset;
            //获得要跟随的点（世界坐标）
            Vector3 FollowPointPosition = followTarget == null ? positionOrOffset : positionOrOffset + followTarget.position;

            //给文本对象赋初始值,确保第一帧时数据正确
            text.fontSize = fontSize;
            text.color = color;
            text.horizontalAlignment = textStyle.Alignment;
            text.transform.position = targetCamera.WorldToScreenPoint(FollowPointPosition) + textStyle.PositionOffset;
            //获取物理动画的力度值(随机）
            List<Vector3> Velocitys = m_Vector3ListPool.Get();
            if (physicsAnimationInfos != null && physicsAnimationInfos.Length > 0)
            {
                for (int i = 0; i < physicsAnimationInfos.Length; i++)
                {
                    PhysicsAnimationInfo item = physicsAnimationInfos[i];
                    float x = UnityEngine.Random.Range(item.MinVelocity.x, item.MaxVelocity.x);
                    float y = UnityEngine.Random.Range(item.MinVelocity.y, item.MaxVelocity.y);
                    float z = UnityEngine.Random.Range(item.MinVelocity.z, item.MaxVelocity.z);
                    Velocitys.Add(new Vector3(x, y, z));
                }
            }
            //获得容器，存储字体动画开始时的字体大小
            List<float> startFontSizes = m_FloatListPool.Get();
            //获得容器，存储是否已赋值字体动画开始时的字体大小
            List<bool> isSetStartFontSizes = m_BoolListPool.Get();
            //初始化值
            if (fontSizeAnimationInfos != null && fontSizeAnimationInfos.Length > 0)
            {
                for (int i = 0; i < fontSizeAnimationInfos.Length; i++)
                {
                    startFontSizes.Add(0);
                    isSetStartFontSizes.Add(false);
                }
            }
            //获得容器，存储透明度动画开始时透明度信息
            List<float> startAlphas = m_FloatListPool.Get();
            //获得容器，存储是否已赋值透明度动画开始时的透明度信息
            List<bool> isSetStartAlphas = m_BoolListPool.Get();
            //初始化值
            if (alphaAnimationInfos != null && alphaAnimationInfos.Length > 0)
            {
                for (int i = 0; i < alphaAnimationInfos.Length; i++)
                {
                    startAlphas.Add(0);
                    isSetStartAlphas.Add(false);
                }
            }

            float timer = 0;
            while (timer < lifeTime)
            {
                yield return null;
                timer += Time.deltaTime;
                timer = Mathf.Clamp(timer, 0, lifeTime);
                //如果文本已被回收，则停止动画
                if (m_FloatingUITexts.ContainsKey(number) == false)
                {
                    break;
                }
                //如果摄像机在运行期间消失了,或者运行期间赋值了新的摄像机，重新查找摄像机
                //如果还是找不到，本次循环啥也不干了，等待摄像机
                if (Camera != null || targetCamera == null)
                {
                    targetCamera = Camera == null ? Camera.main : Camera;
                    if (targetCamera == null)
                    {
                        text.enabled = false;
                        continue;
                    }
                }
                //要跟随的点会随着目标的移动而改变
                if (followTarget != null && followTarget.gameObject.activeSelf == true)
                {
                    FollowPointPosition = positionOrOffset + followTarget.position;
                }

                //物理动画
                if (physicsAnimationInfos != null && physicsAnimationInfos.Length > 0)
                {
                    for (int i = 0; i < physicsAnimationInfos.Length; i++)
                    {
                        PhysicsAnimationInfo item = physicsAnimationInfos[i];

                        if (IsInAnimationTime(timer, lifeTime, item.StartTime, item.Duration))
                        {
                            float time = timer - (lifeTime * item.StartTime);
                            Vector3 velocity = Velocitys[i] + (item.Gravity * time);
                            ScreenPosition += velocity * Time.deltaTime;
                        }
                    }
                }
                //字体大小动画
                if (fontSizeAnimationInfos != null && fontSizeAnimationInfos.Length > 0)
                {
                    for (int i = 0; i < fontSizeAnimationInfos.Length; i++)
                    {

                        FontSizeAnimationInfo item = fontSizeAnimationInfos[i];
                        if (IsInAnimationTime(timer, lifeTime, item.StartTime, item.Duration))
                        {
                            //获取动画开始时的字体大小
                            if (isSetStartFontSizes[i] == false)
                            {
                                isSetStartFontSizes[i] = true;
                                startFontSizes[i] = text.fontSize;
                            }
                            float lerp = (timer - (lifeTime * item.StartTime)) / (lifeTime * item.Duration);
                            text.fontSize = UnityEngine.Mathf.Lerp(startFontSizes[i], fontSize * item.TargetSize, lerp);
                        }
                    }
                }
                //透明度动画
                if (alphaAnimationInfos != null && alphaAnimationInfos.Length > 0)
                {
                    for (int i = 0; i < alphaAnimationInfos.Length; i++)
                    {
                        AlphaAnimationInfo item = alphaAnimationInfos[i];
                        if (IsInAnimationTime(timer, lifeTime, item.StartTime, item.Duration))
                        {
                            if (isSetStartAlphas[i] == false)
                            {
                                isSetStartAlphas[i] = true;
                                startAlphas[i] = text.color.a;
                            }
                            float lerp = (timer - (lifeTime * item.StartTime)) / (lifeTime * item.Duration);
                            Color currentColor = text.color;
                            currentColor.a = UnityEngine.Mathf.Lerp(startAlphas[i], item.TargetAlpha, lerp);
                            text.color = currentColor;
                        }
                    }
                }

                //跟随目标,获得最终渲染位置
                Vector3 targetScreenPosition = ScreenPosition + targetCamera.WorldToScreenPoint(FollowPointPosition);
                text.transform.position = targetScreenPosition;
                //决定是否要显示文本,z轴为负说明不需要渲染（不在摄像机前方视野内）。
                text.enabled = (targetScreenPosition.z > 0);

            }

            //放回对象池
            if (m_FloatingUITexts.ContainsKey(number) && m_FloatingUITexts[number] == text)
            {
                m_UITextPool.Release(text);
                m_FloatingUITexts.Remove(number);
            }
            m_Vector3ListPool.Release(Velocitys);
            m_FloatListPool.Release(startFontSizes);
            m_BoolListPool.Release(isSetStartFontSizes);
            m_FloatListPool.Release(startAlphas);
            m_BoolListPool.Release(isSetStartAlphas);
        }
        #endregion
        #region Unity Methods
        private void Awake()
        {

            //创建根节点
            {   //世界文本根节点
                GameObject root = new GameObject("World Text Root");
                root.transform.SetParent(transform, false);
                m_WorldTextRoot = root.transform;
            }
            {   //UI文本根节点
                GameObject root = new GameObject("UI Text Root");
                root.AddComponent<RectTransform>();
                root.transform.SetParent(transform, false);
                m_UITextRoot = root.transform;

                Canvas canvas = root.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 32767;//最高

                CanvasScaler canvasScaler = root.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

                CanvasScaler = canvasScaler;
            }
            //创建模版
            {   //世界文本模板
                m_WorldTextTemplate = new GameObject("World Text Template");
                m_WorldTextTemplate.transform.SetParent(m_WorldTextRoot);

                TextMeshPro textMeshPro = m_WorldTextTemplate.AddComponent<TextMeshPro>();
                textMeshPro.enableWordWrapping = false;//关闭自动换行
                textMeshPro.raycastTarget = false;//不接受射线检测
                textMeshPro.verticalAlignment = VerticalAlignmentOptions.Baseline;//垂直对齐设置为基线对齐
                textMeshPro.horizontalAlignment = HorizontalAlignmentOptions.Geometry;//水平对齐设置为居中

                RectTransform rectTransform = m_WorldTextTemplate.GetComponent<RectTransform>();
                rectTransform.localPosition = Vector3.zero;
                rectTransform.position = Vector3.zero;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.pivot = Vector2.zero;

                m_WorldTextTemplate.SetActive(false);
            }
            {
                //UI文本模板
                m_UITextTemplate = new GameObject("UI Text Template");
                m_UITextTemplate.transform.SetParent(m_UITextRoot);

                TextMeshProUGUI textMeshProUGUI = m_UITextTemplate.AddComponent<TextMeshProUGUI>();//会自动挂载RectTransform和canvas render
                textMeshProUGUI.enableWordWrapping = false;//关闭自动换行
                textMeshProUGUI.raycastTarget = false;//不接受射线检测
                textMeshProUGUI.verticalAlignment = VerticalAlignmentOptions.Baseline;//垂直对齐设置为基线对齐
                textMeshProUGUI.horizontalAlignment = HorizontalAlignmentOptions.Geometry;//水平对齐设置为居中

                RectTransform rectTransform = m_UITextTemplate.GetComponent<RectTransform>();
                rectTransform.localPosition = Vector3.zero;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.pivot = Vector2.zero;

                m_UITextTemplate.SetActive(false);
            }
            //初始化对象池
            {
                m_WorldTextPool = new ObjectPool<TextMeshPro>(CreateFunc_WorldText, ActionOnGet_WorldText, ActionOnRelease_WorldText, ActionOnDestroy_WorldText, false, 100);
                m_UITextPool = new ObjectPool<TextMeshProUGUI>(CreateFunc_UIText, ActionOnGet_UIText, ActionOnRelease_UIText, ActionOnDestroy_UIText, false, 100);
                m_Vector3ListPool = new ObjectPool<List<Vector3>>(CreateFunc_Vector3List, null, ActionOnRelease_Vector3List, null, false, 100);
                m_FloatListPool = new ObjectPool<List<float>>(CreateFunc_FloatList, null, ActionOnRelease_FloatList, null, false, 100);
                m_BoolListPool = new ObjectPool<List<bool>>(CreateFunc_BoolList, null, ActionOnRelease_BoolList, null, false, 100);
            }
        }

        private void Start()
        {
            //纠错机制
#if UNITY_EDITOR
            if (m_Instance != this)
            {
                Debug.LogError("你不应该手动挂载此脚本,请移除它。");
            }
#endif
        }
        #endregion
    }
}
