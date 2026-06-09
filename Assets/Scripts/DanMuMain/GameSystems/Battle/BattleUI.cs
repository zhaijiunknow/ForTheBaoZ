using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public Text obsessionText;
    public Text phaseText;
    public Text stopoverText;
    public Text foodText;
    public Text checkpointTitleText;
    public Text checkpointDescText;
    public GameObject checkpointPanel;
    public GameObject stopoverPanel;
    public Image redImg;
    public Image buleImg;
    public InputField propCount;
    public GameObject reversalGo;
    public Transform myprop;
    public Font Aa;
    public BattleFlowController flowController;
    public BattleRunState runState;
    public BattlePhaseController phaseController;
    public StopoverController stopoverController;
    public CafeteriaController cafeteriaController;
    public SetBonusResolver setBonusResolver;

    private float targetRedFill;
    private float targetBlueFill;
    public float endDistance = 100f;

    private void Start()
    {
        timebackText.text = "";
        if (flowController == null)
            flowController = FindObjectOfType<BattleFlowController>();
        if (runState == null)
            runState = FindObjectOfType<BattleRunState>();
        if (phaseController == null)
            phaseController = FindObjectOfType<BattlePhaseController>();
        if (stopoverController == null)
            stopoverController = FindObjectOfType<StopoverController>();
        if (cafeteriaController == null)
            cafeteriaController = FindObjectOfType<CafeteriaController>();
        if (setBonusResolver == null)
            setBonusResolver = FindObjectOfType<SetBonusResolver>();
        ReStartGameTimer();
        SetGameButton();
        SetItemCD();
        RefreshCheckpointIntro();
        RefreshOverlayState();
    }

    private void OnDisable()
    {
        SetItemCD();
    }

    public void SetItemCD()
    {
        for (int i = 0; i < items.Count; i++)
            items[i].nowcd = false;
    }

    public void SetGameButton()
    {
        for (int i = myprop.childCount; i > 0; i--)
            Destroy(myprop.GetChild(i - 1).gameObject);

        for (int i = 0; i < myskill.skillcount; i++)
        {
            if (i >= myskill.myskillbox.Count)
                break;

            int itemIndex = myskill.myskillbox[i];
            GameObject button = new GameObject($"SkillButton_{i}");
            button.transform.SetParent(myprop, false);
            button.AddComponent<RectTransform>();
            button.AddComponent<Image>();
            Button clickButton = button.AddComponent<Button>();

            GameObject text = new GameObject("Label");
            text.transform.SetParent(button.transform, false);
            RectTransform textRect = text.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Text t = text.AddComponent<Text>();
            t.text = items[itemIndex].itemName;
            t.color = Color.black;
            t.font = Aa;
            t.resizeTextForBestFit = true;
            t.alignment = TextAnchor.MiddleCenter;

            int capturedIndex = itemIndex;
            clickButton.onClick.AddListener(() => UseRedItem(capturedIndex));
        }
    }

    public void ReStartGameTimer()
    {
        DynamicData.GameStart = false;
        StartCoroutine(TimeStarBack());
    }

    public float startingTime = 180f;
    IEnumerator TimeStarBack()
    {
        DynamicData.TimerBack = startingTime;

        while (DynamicData.TimerBack > 0)
        {
            if (DynamicData.GameStart)
                DynamicData.TimerBack -= Time.deltaTime;

            timebackText.text = FormatTime(DynamicData.TimerBack);
            yield return null;
        }
        timebackText.text = "Time's Up!";
        DynamicData.GameStart = false;
        var thrustDis = (int)(1000 + batMgr.redDistance) - (int)(2000 - (1000 + batMgr.redDistance));
        string str = "平手";
        if (thrustDis > 0)
            str = "红方胜利";
        else if (thrustDis < 0)
            str = "蓝方胜利";

        UISystem.Ins.resultUI.Show(str);
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void Update()
    {
        RefreshOverlayState();

        if (!DynamicData.GameStart && (runState == null || !runState.isPausedForStopover))
            return;

        redThrustText.text = $"红方总推力: {red.TotalThrust:F0}";
        blueThrustText.text = $"蓝方总推力: {bule.TotalThrust:F0}";
        redPerThrustText.text = $"红方永久推力: {red.permanentThrust:F0}";
        bluePerThrustText.text = $"蓝方永久推力: {bule.permanentThrust:F0}";
        redTempThrustText.text = $"红方临时推力: {red.TempThrust:F0}";
        blueTempThrustText.text = $"蓝方临时推力: {bule.TempThrust:F0}";
        redDisText.text = $"红方里程数： {(int)(1000 + batMgr.redDistance)}";
        buleDisText.text = $"蓝方里程数： {(int)(2000 - (1000 + batMgr.redDistance))}";
        float targetFill = Mathf.Clamp((1000 + batMgr.redDistance) / 2000f, 0, 1);
        targetRedFill = targetFill;
        targetBlueFill = 1 - targetFill;

        redImg.fillAmount = targetRedFill;
        buleImg.fillAmount = targetBlueFill;

        float redVal = (int)(1000 + batMgr.redDistance);
        float blueVal = (int)(2000 - (1000 + batMgr.redDistance));

        redFinishTimeText.transform.parent.gameObject.SetActive(redVal <= endDistance);
        buleFinishTimeText.transform.parent.gameObject.SetActive(blueVal <= endDistance);
        redFinishTimeText.text = $"{redVal}m";
        buleFinishTimeText.text = $"{blueVal}m";

        if (obsessionText != null && runState != null)
            obsessionText.text = $"执念: {runState.obsession:F0}";
        if (phaseText != null && runState != null)
        {
            BattleCheckpointData checkpoint = runState.GetNextCheckpoint();
            phaseText.text = checkpoint == null
                ? "阶段：终局"
                : $"阶段 {runState.currentPhaseIndex + 1} / 目标 {checkpoint.targetProgress:F0}";
        }
        if (stopoverText != null && runState != null)
            stopoverText.text = runState.isPausedForStopover ? "停靠中：等待选择" : $"连续跳过: {runState.skippedRestCount}";
        if (foodText != null && runState != null)
        {
            FoodEntryData candidate = cafeteriaController != null ? cafeteriaController.PeekCurrentCandidate() : null;
            string setBonus = setBonusResolver != null ? $" 套装倍率x{setBonusResolver.GetSequenceBonusMultiplier():F2}" : string.Empty;
            foodText.text = candidate == null
                ? $"食物数: {runState.foods.Count}{setBonus}"
                : $"候选: {candidate.displayName} / 已有食物: {runState.foods.Count}{setBonus}";
        }
    }

    void RefreshOverlayState()
    {
        bool introOpen = !DynamicData.GameStart && runState != null && !runState.isPausedForStopover;
        bool stopoverOpen = runState != null && runState.isPausedForStopover;
        bool choosingRest = stopoverOpen && stopoverController != null && stopoverController.IsChoosingRest();

        if (checkpointPanel != null)
            checkpointPanel.SetActive(introOpen);
        if (stopoverPanel != null)
            stopoverPanel.SetActive(stopoverOpen);

        SetStopoverActionVisibility(choosingRest);
    }

    public void RefreshCheckpointIntro()
    {
        if (runState == null)
            return;

        BattleCheckpointData checkpoint = runState.GetNextCheckpoint();
        if (checkpointTitleText != null)
            checkpointTitleText.text = checkpoint == null ? "终局阶段" : $"检查点 {runState.currentPhaseIndex + 1}";

        if (checkpointDescText != null)
        {
            if (checkpoint == null)
            {
                checkpointDescText.text = "现实反击即将开始。\n保持当前构筑，准备进入终局冲刺。";
            }
            else
            {
                checkpointDescText.text =
                    $"目标推进值：{checkpoint.targetProgress:F0}\n" +
                    $"当前起始技能：{myskill.skillcount}\n" +
                    "确认后开始这场连续飞升。";
            }
        }
    }

    public void OnStartBattleClick()
    {
        DynamicData.GameStart = true;
        GameManager.SaveCurrentRun();
        RefreshOverlayState();
    }

    void SetStopoverActionVisibility(bool choosingRest)
    {
        SetActiveIfFound("TakeRestButton", choosingRest);
        SetActiveIfFound("SkipRestButton", choosingRest);
        SetActiveIfFound("BuyFoodButton", !choosingRest);
        SetActiveIfFound("RefreshFoodButton", !choosingRest);
        SetActiveIfFound("UpgradeFoodButton", !choosingRest);
    }

    void SetActiveIfFound(string objectName, bool active)
    {
        Transform target = transform.Find($"StopoverPanel/{objectName}");
        if (target != null)
            target.gameObject.SetActive(active);
    }

    void UseRedItem(int itemIndex)
    {
        int count = ParseCount();
        if (itemIndex < 0 || itemIndex >= items.Count)
            return;

        if (flowController != null)
            flowController.RecordSkill(items[itemIndex]);
        if (count > 0)
            redSystem.UseItem(bule, items[itemIndex], count);
        if (flowController != null)
            flowController.ResolveCombo(redSystem, bule, count);
    }

    int ParseCount()
    {
        if (propCount == null || string.IsNullOrWhiteSpace(propCount.text))
            return 1;

        return int.TryParse(propCount.text, out int count) ? Mathf.Max(1, count) : 1;
    }

    public void OnRestartGameClick()
    {
        GameManager.StartNewRun();
    }

    public void OnClick_AddRedClamp()
    {
        red.permanentThrust++;
    }

    public void OnClick_RedItem1() => UseRedItem(0);
    public void OnClick_RedItem2() => UseRedItem(1);
    public void OnClick_RedItem3() => UseRedItem(2);
    public void OnClick_RedItem4() => UseRedItem(3);
    public void OnClick_RedItem5() => UseRedItem(4);
    public void OnClick_RedItem6() => UseRedItem(5);
    public void OnClick_RedItem7() => UseRedItem(6);
    public void OnClick_RedItem8() => UseRedItem(7);

    public void OnClick_AddBuleClamp()
    {
        bule.permanentThrust++;
    }

    public void Buleues(ItemConfig item)
    {
        int count = ParseCount();
        if (count > 0)
            buleSystem.UseItem(red, item, count);
    }

    public void OnClick_BuleItem1() => Buleues(items[0]);
    public void OnClick_BuleItem2() => Buleues(items[1]);
    public void OnClick_BuleItem3() => Buleues(items[2]);
    public void OnClick_BuleItem4() => Buleues(items[3]);
    public void OnClick_BuleItem5() => Buleues(items[4]);
    public void OnClick_BuleItem6() => Buleues(items[5]);
    public void OnClick_BuleItem7() => Buleues(items[6]);
    public void OnClick_BuleItem8() => Buleues(items[7]);

    public void OnBuyFoodClick()
    {
        if (cafeteriaController != null)
            cafeteriaController.BuyCurrentCandidate();
    }

    public void OnRefreshFoodClick()
    {
        if (cafeteriaController != null)
            cafeteriaController.RefreshCandidate();
    }

    public void OnUpgradeFoodClick()
    {
        if (cafeteriaController != null)
            cafeteriaController.UpgradeLastOwnedFood();
    }

    public void OnTakeRestClick()
    {
        if (stopoverController != null)
            stopoverController.TakeRest();
    }

    public void OnSkipRestClick()
    {
        if (stopoverController != null)
            stopoverController.SkipRest();
    }

    public void OnConfirmPreparationClick()
    {
        if (stopoverController != null)
            stopoverController.ConfirmPreparation();
    }

    public void OnOpenRewardChoiceClick()
    {
        if (stopoverController != null)
            stopoverController.TriggerRewardChoice();
    }

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
            redRevealTimeText.text = "100s";
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
            buleRevealTimeText.text = "100s";
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
            redRevealTimeText.transform.parent.gameObject.SetActive(false);
        else
            buleRevealTimeText.transform.parent.gameObject.SetActive(false);
    }
}
