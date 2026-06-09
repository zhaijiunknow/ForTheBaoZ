using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyManager : MonoBehaviour
{
    public List<AIDifficultyConfig> configs = new List<AIDifficultyConfig>();
    public BattleUI battleUI;
    public AISkillSelector aISkillSelector = new AISkillSelector();
    public Difficulty difficulty;
    private AIDifficultyConfig aimode;
    private Coroutine mycon;

    // Start is called before the first frame update
    void Start()
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                aimode = configs[0];
                break;
            case Difficulty.Normal:
                aimode = configs[1];
                break;
            case Difficulty.Hard:
                aimode = configs[2];
                break;
            case Difficulty.Insane:
                aimode = configs[2];
                break;
            case Difficulty.Endless:
                aimode = configs[2];
                break;
        }
        aISkillSelector.difficulty = difficulty;
        StartCoroutine(star());
    }
    IEnumerator star()
    {
        yield return new WaitUntil(() => DynamicData.GameStart == true);
        Debug.Log("AIº”‘ÿ");
        startbattle();
    }
    public void startbattle()
    {
        float react = Random.Range(-aimode.decisionRandomness, aimode.decisionRandomness);
        react += aimode.baseDecisionDelay;
        mycon = StartCoroutine(useItem(react));
    }

    IEnumerator useItem(float waits)
    {
        yield return new WaitForSeconds(waits);
        ItemConfig one = aISkillSelector.SelectSkill(battleUI.items);
        float w = Random.Range(0, aimode.quickResponseThreshold);
        yield return new WaitForSeconds(w);
        battleUI.Buleues(one);
        startbattle();
    }
    public void endbattle()
    {
        StopCoroutine(mycon);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
