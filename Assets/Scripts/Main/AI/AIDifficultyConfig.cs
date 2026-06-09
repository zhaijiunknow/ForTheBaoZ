using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/AIDifficultyConfig")]
public class AIDifficultyConfig : ScriptableObject
{
    public Difficulty difficulty;
    [Header("反应时间设置")]
    [Tooltip("基础决策延迟（秒）")]
    public float baseDecisionDelay = 1.5f;

    [Tooltip("决策延迟随机范围（+/-秒）")]
    public float decisionRandomness = 0.5f;

    [Tooltip("快速反应的阈值（进度条变化超过此值时缩短延迟）")]
    public float quickResponseThreshold = 0.2f;

    [Tooltip("快速反应时的延迟倍率")]
    [Range(0.1f, 1f)]
    public float quickResponseMultiplier = 0.5f;

    [Header("技能预判")]
    [Tooltip("是否预测玩家连招")]
    public bool canPredictCombo = false;

    [Tooltip("预测准确率（0-1）")]
    [Range(0f, 1f)]
    public float predictionAccuracy = 0f;

    [Tooltip("提前预测的时间（秒）")]
    public float predictionLeadTime = 0f;
}
