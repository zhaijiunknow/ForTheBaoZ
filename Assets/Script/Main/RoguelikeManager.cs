using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoguelikeManager : MonoBehaviour
{
    public List<RogueConfig> rogueConfigs = new List<RogueConfig>();
    public BattleUI battleUI;
    public MySkillManager myskill;
    public Font Aa;

    public Image image;
    public Transform buttonfield;
    public TextMeshProUGUI itemname;
    public TextMeshProUGUI itemintro;

    // Start is called before the first frame update
    void Start()
    {
        int no = Random.Range(0, rogueConfigs.Count);
        SetChoiceButton(no);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetChoiceButton(int no)
    {
        for (int i = buttonfield.childCount; i > 0; i--)
        {
            buttonfield.GetChild(i - 1).gameObject.SetActive(false);
        }
        for (int i = 0; i < rogueConfigs[no].ChoiceName.Length; i++)
        {
            GameObject button = Instantiate(new GameObject(), buttonfield);
            button.AddComponent<Image>();
            button.AddComponent<Button>();
            GameObject text = Instantiate(new GameObject(), button.transform);
            text.AddComponent<Text>();
            Text t = text.GetComponent<Text>();
            t.text = rogueConfigs[no].ChoiceName[i];
            t.color = Color.black;
            t.font = Aa;
            t.resizeTextForBestFit = true;
            t.alignment = TextAnchor.MiddleCenter;
            switch (rogueConfigs[no].action[i])
            {
                case eventresult.AddSkill:
                    button.GetComponent<Button>().onClick.AddListener(AddSkill);
                    break;
                case eventresult.AddMaxSkillCount:
                    button.GetComponent<Button>().onClick.AddListener(AddMaxSkillCount);
                    break;
            }
        }
    }




    #region Èâ¸ëÊÂ¼þº¯Êý
    public void AddSkill()
    {
        int add = Random.Range(0, battleUI.items.Count);
        myskill.myskillbox.Add(add);
    }

    public void AddMaxSkillCount()
    {
        if (myskill.skillcount<9)
        {
            myskill.skillcount++;
            battleUI.SetGameButton();
        }
    }

    #endregion
}
