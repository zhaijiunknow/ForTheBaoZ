using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum skilltype
{
    None,
    qte,
    a,
    b,
    c
}
public class MySkillManager : MonoBehaviour
{
    public int skillcount;

    public List<int> myskillbox;

    public List<int> enemyskillbox;

    public SkillSequenceQueue _skillQueue;

    public List<QTEItemConfig> qTEItemConfigs;

    // Start is called before the first frame update
    void Start()
    {
        // 初始化队列，最大记录5秒，最多5个技能
        _skillQueue = new SkillSequenceQueue(5f, 5);

        // 订阅事件
        _skillQueue.OnSkillAdded += OnSkillAdded;
        _skillQueue.OnSkillRemoved += OnSkillRemoved;
        _skillQueue.OnSequenceMatched += OnSequenceMatched;

    }
    public bool CheckForCombo(QTEItemConfig qteitem)
    {
        var result = _skillQueue.MatchSequence(qteitem.combo, exactOrder: qteitem.exactOrder, maxTimeWindow: qteitem.maxComboTime);

        if (result.IsMatch)
        {
            Debug.Log($"连段触发: {result}");
            _skillQueue.Clear();
        }
        
        return result.IsMatch;
    }
    void OnSkillAdded(SkillSequenceQueue.SkillNode node)
    {
        Debug.Log($"技能添加: {node}");
    }
    void OnSkillRemoved(SkillSequenceQueue.SkillNode node)
    {
        Debug.Log($"技能移除: {node}");
    }
    void OnSequenceMatched(SkillSequenceQueue.SequenceMatchResult result)
    {
        Debug.Log($"序列匹配: {result}");
    }
    // Update is called once per frame
    void Update()
    {
    }
}
