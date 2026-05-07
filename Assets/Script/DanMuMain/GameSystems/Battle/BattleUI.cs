using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public PlayerThrustManager red;
    public PlayerThrustManager bule;
    public BattleManager batMgr;
    public ItemEffectSystem redSystem;
    public ItemEffectSystem buleSystem;
    public MySkillManager myskill;
    public List<ItemConfig> items;
    //public ItemConfig item1;
    //public ItemConfig item2;
    //public ItemConfig item3;
    //public ItemConfig item4;
    //public ItemConfig item5;
    //public ItemConfig item6;
    //public ItemConfig item7;
    //public ItemConfig item8;
    public Text redThrustText;
    public Text blueThrustText;
    public Text redPerThrustText;
    public Text bluePerThrustText;
    public Text redTempThrustText;
    public Text blueTempThrustText;
    public Text redDisText;
    public Text buleDisText;
    public Text timebackText;
    public Text redRevealTimeText;
    public Text buleRevealTimeText;
    public Text redFinishTimeText;
    public Text buleFinishTimeText;
    public Image redImg;
    public Image buleImg;
    public InputField propCount;
    public GameObject reversalGo;
    public Transform myprop;
    public Font Aa;
    
    private float targetRedFill;
    private float targetBlueFill;
    private float currentRedFill;
    private float currentBlueFill;
    public float endDistance = 100f;
    private void Start()
    {
        timebackText.text = "";
        ReStartGameTimer();
        SetGameButton();
        SetItemCD();
    }

    private void OnDisable()
    {
        SetItemCD();
    }
    #region Timer
    public void SetItemCD()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].nowcd = false;
        }
    }
    public void SetGameButton()
    {
        for (int i = myprop.childCount; i > 0; i--)
        {
            myprop.GetChild(i - 1).gameObject.SetActive(false);

        }
        for (int i = 0; i < myskill.skillcount; i++)
        {
            if (i >= myskill.myskillbox.Count)
            {
                break;
            }
            GameObject button = Instantiate(new GameObject(),myprop);
            button.AddComponent<Image>();
            button.AddComponent<Button>();
            GameObject text = Instantiate(new GameObject(), button.transform);
            text.AddComponent<Text>();
            Text t = text.GetComponent<Text>();
            t.text = items[myskill.myskillbox[i]].itemName;
            t.color = Color.black;
            t.font = Aa;
            t.resizeTextForBestFit = true;
            t.alignment = TextAnchor.MiddleCenter;
            switch (myskill.myskillbox[i])
            {
                case 0:
                    button.GetComponent<Button>().onClick.AddListener(OnClick_RedItem1);
                    break;
                case 1:
                    button.GetComponent<Button>().onClick.AddListener(OnClick_RedItem2);
                    break;
                case 2:
                    button.GetComponent<Button>().onClick.AddListener(OnClick_RedItem3);
                    break;
                case 3:
                    button.GetComponent<Button>().onClick.AddListener(OnClick_RedItem4);
                    break;
                case 4:
                    button.GetComponent<Button>().onClick.AddListener(OnClick_RedItem5);
                    break;
                case 5:
                    button.GetComponent<Button>().onClick.AddListener(OnClick_RedItem6);
                    break;
                case 6:
                    button.GetComponent<Button>().onClick.AddListener(OnClick_RedItem7);
                    break;
                case 7:
                    button.GetComponent<Button>().onClick.AddListener(OnClick_RedItem8);
                    break;
            }
        }
    }
    public void ReStartGameTimer()
    {
        DynamicData.GameStart = false;
        StartCoroutine(TimeStarBack());
    }

    public float startingTime = 180f; // 3 minutes in seconds
    IEnumerator TimeStarBack()
    {
        DynamicData.TimerBack = startingTime;
        
        while (DynamicData.TimerBack > 0)
        {
            if (DynamicData.GameStart)
            {
                DynamicData.TimerBack -= Time.deltaTime;
            }
            
            timebackText.text = FormatTime(DynamicData.TimerBack);
            yield return null;
        }
        timebackText.text = "Time's Up!";
        DynamicData.GameStart = false;
        var thrustDis = (int)(1000 + batMgr.redDistance) - (int)(2000 - (1000 + batMgr.redDistance));
        string str = "平手";
        if (thrustDis > 0)
        {
            str = "红方胜利";
        }
        else if (thrustDis < 0)
        {
            str = "蓝方胜利";
        }
        
        UISystem.Ins.resultUI.Show(str);
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    #endregion

    void Update()
    {
        if (!DynamicData.GameStart) return;
            
        redThrustText.text = $"红方总推力: {red.TotalThrust:F0}";
        blueThrustText.text = $"蓝方总推力: {bule.TotalThrust:F0}";
        redPerThrustText.text = $"红方永久推力: {red.permanentThrust:F0}";
        bluePerThrustText.text = $"蓝方永久推力: {bule.permanentThrust:F0}";
        redTempThrustText.text = $"红方临时推力: {red.TempThrust:F0}";
        blueTempThrustText.text = $"蓝方临时推力: {bule.TempThrust:F0}";
        redDisText.text = $"红方里程数： {(int)(1000 + batMgr.redDistance)}";
        buleDisText.text = $"蓝方里程数： {(int)(2000 - (1000 + batMgr.redDistance))}";
        // 计算目标填充量
        float targetFill = Mathf.Clamp((1000 + batMgr.redDistance) / 2000f, 0, 1);
        targetRedFill = targetFill;
        targetBlueFill = 1 - targetFill;

        redImg.fillAmount = targetRedFill;
        buleImg.fillAmount = targetBlueFill;
        
        float redVal = (int)(1000 + batMgr.redDistance);
        float blueVal = (int)(2000 - (1000 + batMgr.redDistance));
        
        redFinishTimeText.transform.parent.gameObject.SetActive(redVal <= endDistance);
        buleFinishTimeText.transform.parent.gameObject.SetActive(blueVal <= endDistance);
        
        redFinishTimeText.text = $"{(int)(1000 + batMgr.redDistance)}m";
        buleFinishTimeText.text = $"{(int)(2000 - (1000 + batMgr.redDistance))}m";
    }

    #region click

    public void OnRestartGameClick()
    {
        SceneManager.LoadScene(0);
    }

    public void OnClick_AddRedClamp()
    {
        red.permanentThrust++;
    }
    
    public void OnClick_RedItem1()
    {
        int count = int.Parse(propCount.text);
        if (!items[0].nowcd)
        {
            myskill._skillQueue.AddSkill(items[0].goodtype);
        }
        if (count > 0)
            redSystem.UseItem(bule, items[0], count);
        for (int i = 0; i < myskill.qTEItemConfigs.Count; i++) 
        {
            if (myskill.CheckForCombo(myskill.qTEItemConfigs[i]))
            {
                redSystem.UseItem(bule, myskill.qTEItemConfigs[i], count);
                break;
            }
        }
        
    }
    
    public void OnClick_RedItem2()
    {
        int count = int.Parse(propCount.text);
        if (!items[1].nowcd)
        {
            myskill._skillQueue.AddSkill(items[1].goodtype);
        }
        if (count > 0)
            redSystem.UseItem(bule, items[1], count);
        for (int i = 0; i < myskill.qTEItemConfigs.Count; i++)
        {
            if (myskill.CheckForCombo(myskill.qTEItemConfigs[i]))
            {
                redSystem.UseItem(bule, myskill.qTEItemConfigs[i], count);
                break;
            }
        }
    }
    
    public void OnClick_RedItem3()
    {
        int count = int.Parse(propCount.text);
        if (!items[2].nowcd)
        {
            myskill._skillQueue.AddSkill(items[2].goodtype);
        }
        if (count > 0)
            redSystem.UseItem(bule, items[2], count);
        for (int i = 0; i < myskill.qTEItemConfigs.Count; i++)
        {
            if (myskill.CheckForCombo(myskill.qTEItemConfigs[i]))
            {
                redSystem.UseItem(bule, myskill.qTEItemConfigs[i], count);
                break;
            }
        }
    }
    
    public void OnClick_RedItem4()
    {
        int count = int.Parse(propCount.text);
        if (!items[3].nowcd)
        {
            myskill._skillQueue.AddSkill(items[3].goodtype);
        }
        if (count > 0)
            redSystem.UseItem(bule, items[3], count);
        for (int i = 0; i < myskill.qTEItemConfigs.Count; i++)
        {
            if (myskill.CheckForCombo(myskill.qTEItemConfigs[i]))
            {
                redSystem.UseItem(bule, myskill.qTEItemConfigs[i], count);
                break;
            }
        }
    }
    
    public void OnClick_RedItem5()
    {
        int count = int.Parse(propCount.text);
        if (!items[4].nowcd)
        {
            myskill._skillQueue.AddSkill(items[4].goodtype);
        }
        if (count > 0)
            redSystem.UseItem(bule, items[4], count);
        for (int i = 0; i < myskill.qTEItemConfigs.Count; i++)
        {
            if (myskill.CheckForCombo(myskill.qTEItemConfigs[i]))
            {
                redSystem.UseItem(bule, myskill.qTEItemConfigs[i], count);
                break;
            }
        }
    }
    
    public void OnClick_RedItem6()
    {
        int count = int.Parse(propCount.text);
        if (!items[5].nowcd)
        {
            myskill._skillQueue.AddSkill(items[5].goodtype);
        }
        if (count > 0)
            redSystem.UseItem(bule, items[5], count);
        for (int i = 0; i < myskill.qTEItemConfigs.Count; i++)
        {
            if (myskill.CheckForCombo(myskill.qTEItemConfigs[i]))
            {
                redSystem.UseItem(bule, myskill.qTEItemConfigs[i], count);
                break;
            }
        }
    }
    
    public void OnClick_RedItem7()
    {
        int count = int.Parse(propCount.text);
        if (!items[6].nowcd)
        {
            myskill._skillQueue.AddSkill(items[6].goodtype);
        }
        if (count > 0)
            redSystem.UseItem(bule, items[6], count);
        for (int i = 0; i < myskill.qTEItemConfigs.Count; i++)
        {
            if (myskill.CheckForCombo(myskill.qTEItemConfigs[i]))
            {
                redSystem.UseItem(bule, myskill.qTEItemConfigs[i], count);
                break;
            }
        }
    }
    
    public void OnClick_RedItem8()
    {
        int count = int.Parse(propCount.text);
        if (!items[7].nowcd)
        {
            myskill._skillQueue.AddSkill(items[7].goodtype);
        }
        if (count > 0)
            redSystem.UseItem(bule, items[7], count);
        for (int i = 0; i < myskill.qTEItemConfigs.Count; i++)
        {
            if (myskill.CheckForCombo(myskill.qTEItemConfigs[i]))
            {
                redSystem.UseItem(bule, myskill.qTEItemConfigs[i], count);
                break;
            }
        }
    }
    
    public void OnClick_AddBuleClamp()
    {
        bule.permanentThrust++;
    }
    public void Buleues(ItemConfig item)
    {
        int count = int.Parse(propCount.text);
        if (count > 0)
            buleSystem.UseItem(red, item, count);
    }
    public void OnClick_BuleItem1()
    {
        int count = int.Parse(propCount.text);
        if (count > 0)
            buleSystem.UseItem(red, items[0], count);
    }
    
    public void OnClick_BuleItem2()
    {
        int count = int.Parse(propCount.text);
        if (count > 0)
            buleSystem.UseItem(red, items[1], count);
    }
   
    public void OnClick_BuleItem3()
    {
        int count = int.Parse(propCount.text);
        if (count > 0)
            buleSystem.UseItem(red, items[2], count);
    }
    
    public void OnClick_BuleItem4()
    {
        int count = int.Parse(propCount.text);
        if (count > 0)
            buleSystem.UseItem(red, items[3], count);
    }
    
    public void OnClick_BuleItem5()
    {
        int count = int.Parse(propCount.text);
        if (count > 0)
            buleSystem.UseItem(red, items[4], count);
    }
    
    public void OnClick_BuleItem6()
    {
        int count = int.Parse(propCount.text);
        if (count > 0)
            buleSystem.UseItem(red, items[5], count);
    }
    
    public void OnClick_BuleItem7()
    {
        int count = int.Parse(propCount.text);
        if (count > 0)
            buleSystem.UseItem(red, items[6], count);
    }
    
    public void OnClick_BuleItem8()
    {
        int count = int.Parse(propCount.text);
        if (count > 0)
            buleSystem.UseItem(red, items[7], count);
    }
    #endregion

    public void Reversal()
    {
        reversalGo.SetActive(true);
        DOVirtual.DelayedCall(2f, () =>
        {
            reversalGo.SetActive(false);
        });
    }

    private float _currentTime = 100;
    public void ReversalTime(bool isRed)
    {
        _currentTime = 100;
        if (isRed)
        {
            redRevealTimeText.text = "100s"; // 初始值
            redRevealTimeText.transform.parent.gameObject.SetActive(true);
            DOTween.To(() => _currentTime, x => 
                {
                    _currentTime = x;
                    redRevealTimeText.text = Mathf.CeilToInt(_currentTime) + "s";
                },
                0,
                100f
            ).SetEase(Ease.Linear)
            .OnComplete(() => redRevealTimeText.transform.parent.gameObject.SetActive(false));
        }
        else
        {
            buleRevealTimeText.text = "100s"; // 初始值
            buleRevealTimeText.transform.parent.gameObject.SetActive(true);
            DOTween.To(() => _currentTime, x => 
                {
                    _currentTime = x;
                    buleRevealTimeText.text = Mathf.CeilToInt(_currentTime) + "s";
                },
                0,
                100f
            ).SetEase(Ease.Linear)
            .OnComplete(() => buleRevealTimeText.transform.parent.gameObject.SetActive(false));
        }
        
    }

    public void EndReversalTime(bool isRed)
    {
        if (isRed)
        {
            redRevealTimeText.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            buleRevealTimeText.transform.parent.gameObject.SetActive(false);
        }
    }
    
}