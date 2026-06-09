using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TechCosmos.SkillSystem.Runtime;
using UnityEngine;

public class AISkillSelector
{
    public Difficulty difficulty;
    private Dictionary<ItemConfig, float> skillWeights = new();
    private List<string> mistakeLog = new();

    // 计算技能权重
    public ItemConfig SelectSkill(List<ItemConfig> availableSkills)
    {
        // 基础权重计算（所有难度通用）
        foreach (var skill in availableSkills)
        {
            float weight = 1;

            // 根据难度调整
            weight = ApplyDifficultyAdjustments(weight, skill);

            skillWeights[skill] = weight;
        }

        // 根据难度引入错误
        if (ShouldMakeMistake())
        {
            return MakeIntentionalMistake(availableSkills);
        }

        // 正常选择
        return SelectByWeight(skillWeights);
    }
    private ItemConfig SelectByWeight(Dictionary<ItemConfig, float> skills)
    {
        return skills.OrderBy(s => skillWeights[s.Key]).Last().Key;
    }
    private float ApplyDifficultyAdjustments(float baseWeight, ItemConfig skill)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                // 简单AI：偏爱简单技能，避免连招
                if (skill.goodtype != skilltype.None) baseWeight *= 0.5f;
                break;

            case Difficulty.Normal:
                // 普通AI：平衡选择
                break;

            case Difficulty.Hard:
                // 困难AI：偏爱连招和干扰
                if (skill.goodtype != skilltype.None) baseWeight *= 1.5f;
                break;

            case Difficulty.Insane:
                // 疯狂AI：针对玩家弱点
                if (skill.goodtype != skilltype.None) baseWeight *= 2f;
                break;

            case Difficulty.Endless:
                // 无尽AI：基于机器学习
                
                break;
        }

        return baseWeight;
    }

    private bool ShouldMakeMistake()
    {
        // 不同难度犯错概率不同
        float mistakeChance = difficulty switch
        {
            Difficulty.Easy => 0.3f,    // 30%犯错
            Difficulty.Normal => 0.1f,   // 10%犯错
            Difficulty.Hard => 0.05f,    // 5%犯错
            Difficulty.Insane => 0.02f,  // 2%犯错（故意误导不算犯错）
            _ => 0f
        };

        return UnityEngine.Random.value < mistakeChance;
    }

    private ItemConfig MakeIntentionalMistake(List<ItemConfig> skills)
    {
        // 故意选择最差的技能
        var worstSkill = skills.OrderBy(s => skillWeights[s]).First();
        mistakeLog.Add($"故意犯错: 选择了{worstSkill.itemName}");
        return worstSkill;
    }
    public void IncreaseWeight(ItemConfig key,float a)
    {
        skillWeights[key] += a;
    }
}


// AIBehaviorPattern.cs
public enum AIBehaviorPattern
{
    Reactive,      // 反应式：被动应对玩家行动
    Proactive,     // 主动式：主动建立优势
    Adaptive,      // 适应性：根据玩家风格调整
    Deceptive,     // 欺骗式：故意误导玩家
    Psychological  // 心理战：针对玩家习惯
}
public enum Difficulty
{
    Easy,      // 简单
    Normal,     // 正常
    Hard,      // 困难
    Insane,     // 疯狂
    Endless  // 无尽
}
// 不同难度的行为模式
public class AIDifficultyBehavior
{
    public static readonly Dictionary<Difficulty, AIBehaviorPattern[]> Patterns = new()
    {
        [Difficulty.Easy] = new[] { AIBehaviorPattern.Reactive },
        [Difficulty.Normal] = new[] { AIBehaviorPattern.Reactive, AIBehaviorPattern.Proactive },
        [Difficulty.Hard] = new[] { AIBehaviorPattern.Proactive, AIBehaviorPattern.Adaptive },
        [Difficulty.Insane] = new[] { AIBehaviorPattern.Adaptive, AIBehaviorPattern.Deceptive },
        [Difficulty.Endless] = new[] { AIBehaviorPattern.Deceptive, AIBehaviorPattern.Psychological }
    };
}