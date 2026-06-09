using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoguelikeManager : MonoBehaviour
{
    public static RoguelikeManager Instance { get; private set; }

    public List<RogueConfig> rogueConfigs = new List<RogueConfig>();
    public BattleUI battleUI;
    public MySkillManager myskill;
    public Font Aa;

    public Image image;
    public Transform buttonfield;
    public TextMeshProUGUI itemname;
    public TextMeshProUGUI itemintro;

    RogueConfig activeConfig;
    Action completion;
    readonly List<GameObject> spawnedButtons = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public bool TryStartReward(Action onCompleted)
    {
        RogueConfig config = GetRandomConfig();
        if (config == null)
            return false;

        activeConfig = config;
        completion = onCompleted;
        ShowConfig(config);
        return true;
    }

    RogueConfig GetRandomConfig()
    {
        List<RogueConfig> candidates = rogueConfigs.FindAll(config => config != null);
        if (candidates.Count == 0)
            return null;

        return candidates[UnityEngine.Random.Range(0, candidates.Count)];
    }

    void ShowConfig(RogueConfig config)
    {
        if (itemname != null)
            itemname.text = config.itemName;
        if (itemintro != null)
            itemintro.text = config.itemIntro;
        if (image != null)
            image.sprite = config.icon;

        SetChoiceButtons(config);
    }

    void SetChoiceButtons(RogueConfig config)
    {
        ClearButtons();

        int choiceCount = Mathf.Min(config.ChoiceName != null ? config.ChoiceName.Length : 0, config.action != null ? config.action.Length : 0);
        for (int i = 0; i < choiceCount; i++)
        {
            eventresult result = config.action[i];
            GameObject buttonObject = new GameObject($"Choice_{i}", typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(buttonfield, false);

            Image background = buttonObject.GetComponent<Image>();
            background.color = Color.white;

            Button button = buttonObject.GetComponent<Button>();
            int index = i;
            button.onClick.AddListener(() => SelectChoice(index));

            GameObject textObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(buttonObject.transform, false);

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Text text = textObject.GetComponent<Text>();
            text.text = config.ChoiceName[i];
            text.color = Color.black;
            text.font = Aa;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;

            spawnedButtons.Add(buttonObject);
        }
    }

    void SelectChoice(int index)
    {
        if (activeConfig == null || activeConfig.action == null || index < 0 || index >= activeConfig.action.Length)
            return;

        ApplyResult(activeConfig.action[index]);
        FinishNode();
    }

    void ApplyResult(eventresult result)
    {
        switch (result)
        {
            case eventresult.AddSkill:
                AddSkill();
                break;
            case eventresult.AddMaxSkillCount:
                AddMaxSkillCount();
                break;
            case eventresult.Recover:
            case eventresult.Continue:
                break;
        }
    }

    void FinishNode()
    {
        ClearButtons();
        if (itemname != null)
            itemname.text = string.Empty;
        if (itemintro != null)
            itemintro.text = string.Empty;
        if (image != null)
            image.sprite = null;

        Action finished = completion;
        activeConfig = null;
        completion = null;
        finished?.Invoke();
    }

    void ClearButtons()
    {
        for (int i = 0; i < spawnedButtons.Count; i++)
        {
            if (spawnedButtons[i] != null)
                Destroy(spawnedButtons[i]);
        }

        spawnedButtons.Clear();

        if (buttonfield == null)
            return;

        for (int i = buttonfield.childCount - 1; i >= 0; i--)
        {
            Transform child = buttonfield.GetChild(i);
            if (child != null && child.gameObject != null)
                Destroy(child.gameObject);
        }
    }

    public void AddSkill()
    {
        if (battleUI == null || myskill == null || battleUI.items.Count == 0)
            return;

        int add = UnityEngine.Random.Range(0, battleUI.items.Count);
        myskill.myskillbox.Add(add);
        GameManager.SaveCurrentRun();
    }

    public void AddMaxSkillCount()
    {
        if (myskill == null)
            return;

        if (myskill.skillcount < 9)
        {
            myskill.skillcount++;
            if (battleUI != null)
                battleUI.SetGameButton();
            GameManager.SaveCurrentRun();
        }
    }
}
