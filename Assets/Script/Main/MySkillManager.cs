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

    void Start()
    {
        _skillQueue = new SkillSequenceQueue(5f, 5);

        _skillQueue.OnSkillAdded += OnSkillAdded;
        _skillQueue.OnSkillRemoved += OnSkillRemoved;
        _skillQueue.OnSequenceMatched += OnSequenceMatched;

        GameManager.ApplySavedSkillState(this);
    }

    public List<int> GetPlayerSkillBoxSnapshot()
    {
        return myskillbox == null ? new List<int>() : new List<int>(myskillbox);
    }

    public void ApplySavedSkillState(int savedSkillCount, List<int> savedSkillBox)
    {
        skillcount = Mathf.Max(0, savedSkillCount);
        if (myskillbox == null)
            myskillbox = new List<int>();
        else
            myskillbox.Clear();

        if (savedSkillBox != null)
            myskillbox.AddRange(savedSkillBox);
    }

    public bool CheckForCombo(QTEItemConfig qteitem)
    {
        var result = _skillQueue.MatchSequence(qteitem.combo, exactOrder: qteitem.exactOrder, maxTimeWindow: qteitem.maxComboTime);

        if (result.IsMatch)
        {
            Debug.Log($"���δ���: {result}");
            _skillQueue.Clear();
        }

        return result.IsMatch;
    }
    void OnSkillAdded(SkillSequenceQueue.SkillNode node)
    {
        Debug.Log($"��������: {node}");
    }
    void OnSkillRemoved(SkillSequenceQueue.SkillNode node)
    {
        Debug.Log($"�����Ƴ�: {node}");
    }
    void OnSequenceMatched(SkillSequenceQueue.SequenceMatchResult result)
    {
        Debug.Log($"����ƥ��: {result}");
    }
    void Update()
    {
    }
}
