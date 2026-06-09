using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// �������ж��й�����
/// ʹ�ö���+ʱ��������������У�֧���Զ��������ڼ�¼������ƥ��
/// </summary>
public class SkillSequenceQueue
{
    #region �ڲ����ݽṹ

    /// <summary>
    /// ���ܼ�¼�ڵ�
    /// </summary>
    public class SkillNode
    {
        public float Timestamp { get; private set; }
        public skilltype SkillType { get; private set; }

        public SkillNode(float timestamp,skilltype skillType)
        {
            Timestamp = timestamp;
            SkillType = skillType;
        }
        public override string ToString()
        {
            return $"[{Timestamp:F2}s] {SkillType}";
        }

        public SkillNode Clone()
        {
            return new SkillNode(Timestamp, SkillType);
        }
    }

    /// <summary>
    /// ����ƥ����
    /// </summary>
    public class SequenceMatchResult
    {
        public bool IsMatch { get; set; }
        public List<skilltype> MatchedSequence { get; set; }
        public List<SkillNode> MatchedNodes { get; set; }
        public int StartIndex { get; set; }
        public float TotalTime { get; set; }
        public float MatchAccuracy { get; set; }
        public object Tag { get; set; }  // ��������

        public SequenceMatchResult()
        {
            MatchedSequence = new List<skilltype>();
            MatchedNodes = new List<SkillNode>();
        }

        public override string ToString()
        {
            return IsMatch ?
                $"ƥ��ɹ�: {string.Join("��", MatchedSequence)} (��ʱ:{TotalTime:F2}s, ����:{MatchAccuracy:F2})" :
                "��ƥ��";
        }
    }

    /// <summary>
    /// ͳ����Ϣ
    /// </summary>
    public struct QueueStats
    {
        public int TotalCount;
        public int ActiveCount;
        public float OldestTimestamp;
        public float NewestTimestamp;
        public float TimeSpan;
        public Dictionary<skilltype, int> SkillFrequency;

        public override string ToString()
        {
            return $"����ͳ��: {ActiveCount}/{TotalCount} ����¼, ʱ����: {TimeSpan:F2}s";
        }
    }

    #endregion

    #region �������ֶ�

    private readonly LinkedList<SkillNode> _skillQueue = new();
    private readonly List<SkillNode> _removedNodes = new(); // ���ڼ�¼�Ƴ��Ľڵ�

    // ����
    private float _maxRecordTime = 10f; // Ĭ������¼ʱ�䣨�룩
    private int _maxQueueSize = 5;     // �����д�С
    private bool _autoCleanup = true;   // �Ƿ��Զ��������ڼ�¼

    // ͳ��
    private int _totalAddedCount = 0;
    private QueueStats _currentStats;

    // �¼�
    public event Action<SkillNode> OnSkillAdded;
    public event Action<SkillNode> OnSkillRemoved;
    public event Action<SequenceMatchResult> OnSequenceMatched;

    #endregion

    #region ���캯��

    public SkillSequenceQueue() { }

    public SkillSequenceQueue(float maxRecordTime, int maxQueueSize = 20, bool autoCleanup = true)
    {
        _maxRecordTime = Mathf.Max(0.1f, maxRecordTime);
        _maxQueueSize = Mathf.Max(1, maxQueueSize);
        _autoCleanup = autoCleanup;
    }

    #endregion

    #region �������� - �������Ƴ�

    /// <summary>
    /// ���Ӽ��ܼ�¼������
    /// </summary>
    /// <returns>�����Ƿ�ɹ�</returns>
    public bool AddSkill(skilltype skillType)
    {
        float currentTime = Time.time;

        // �Զ��������ڼ�¼
        if (_autoCleanup)
        {
            CleanupExpired(currentTime);
        }

        // �����д�С����
        if (_skillQueue.Count >= _maxQueueSize)
        {
            RemoveOldest();
        }

        // �����½ڵ�
        var newNode = new SkillNode(currentTime, skillType);

        // ���ӵ�����β��
        _skillQueue.AddLast(newNode);
        _totalAddedCount++;

        // ����ͳ��
        UpdateStats();

        // �����¼�
        OnSkillAdded?.Invoke(newNode);

        return true;
    }

    /// <summary>
    /// �������Ӽ��ܼ�¼
    /// </summary>
    public int AddSkills(IEnumerable<skilltype> skillType, float baseTime = 0f, float timeInterval = 0.5f)
    {
        int count = 0;
        float currentTime = baseTime > 0 ? baseTime : Time.time;

        foreach (var type in skillType)
        {
            if (AddSkill(type))
            {
                count++;
                currentTime += timeInterval;
            }
        }

        return count;
    }

    /// <summary>
    /// �Ƴ���ɵļ��ܼ�¼
    /// </summary>
    /// <returns>���Ƴ��ļ�¼���������Ϊ���򷵻�null</returns>
    public SkillNode RemoveOldest()
    {
        if (_skillQueue.Count == 0) return null;

        var oldest = _skillQueue.First.Value;
        _skillQueue.RemoveFirst();
        _removedNodes.Add(oldest);

        OnSkillRemoved?.Invoke(oldest);
        UpdateStats();

        return oldest;
    }

    /// <summary>
    /// �������й��ڼ�¼�����ڵ�ǰʱ�䣩
    /// </summary>
    /// <returns>����������</returns>
    public int CleanupExpired(float currentTime = -1f)
    {
        if (currentTime < 0) currentTime = Time.time;

        int count = 0;
        float expireTime = currentTime - _maxRecordTime;

        // �Ƴ�����ʱ���С��expireTime�ļ�¼
        while (_skillQueue.Count > 0 && _skillQueue.First.Value.Timestamp < expireTime)
        {
            var removed = RemoveOldest();
            if (removed != null) count++;
        }

        return count;
    }

    /// <summary>
    /// �����������
    /// </summary>
    public void Clear()
    {
        foreach (var node in _skillQueue)
        {
            OnSkillRemoved?.Invoke(node);
        }

        _skillQueue.Clear();
        UpdateStats();
    }
     
    #endregion

    #region �������� - ��ѯ���ȡ

    /// <summary>
    /// ��ȡ��ǰ�������У�������ID��
    /// </summary>
    public List<skilltype> GetCurrentSequence()
    {
        return _skillQueue.Select(node => node.SkillType).ToList();
    }

    /// <summary>
    /// ��ȡ��ǰ���ܽڵ�����
    /// </summary>
    public List<SkillNode> GetCurrentNodes()
    {
        return _skillQueue.Select(node => node.Clone()).ToList();
    }

    /// <summary>
    /// ��ȡָ��ʱ�䷶Χ�ڵļ�������
    /// </summary>
    public List<skilltype> GetSequenceInTimeRange(float startTime, float endTime)
    {
        return _skillQueue
            .Where(node => node.Timestamp >= startTime && node.Timestamp <= endTime)
            .Select(node => node.SkillType)
            .ToList();
    }

    /// <summary>
    /// ��ȡ�����N������
    /// </summary>
    public List<skilltype> GetRecentSkills(int count)
    {
        if (count <= 0) return new List<skilltype>();

        return _skillQueue
            .TakeLast(Mathf.Min(count, _skillQueue.Count))
            .Select(node => node.SkillType)
            .ToList();
    }

    /// <summary>
    /// ��ȡ���һ������
    /// </summary>
    public skilltype GetLastSkill()
    {
        return _skillQueue.Count > 0 ? _skillQueue.Last.Value.SkillType : skilltype.None;
    }

    /// <summary>
    /// ��ȡ���һ�����ܽڵ�
    /// </summary>
    public SkillNode GetLastNode()
    {
        return _skillQueue.Count > 0 ? _skillQueue.Last.Value.Clone() : null;
    }

    /// <summary>
    /// ��ȡָ�������ļ���
    /// </summary>
    public skilltype GetSkillAt(int index)
    {
        if (index < 0 || index >= _skillQueue.Count) return skilltype.None;

        return _skillQueue.ElementAt(index).SkillType;
    }

    /// <summary>
    /// ��ȡָ�������ļ��ܽڵ�
    /// </summary>
    public SkillNode GetNodeAt(int index)
    {
        if (index < 0 || index >= _skillQueue.Count) return null;

        return _skillQueue.ElementAt(index).Clone();
    }

    /// <summary>
    /// ��������Ƿ����ָ������
    /// </summary>
    public bool Contains(skilltype skillId)
    {
        return _skillQueue.Any(node => node.SkillType == skillId);
    }

    /// <summary>
    /// ��������Ƿ���ָ���������п�ͷ
    /// </summary>
    public bool StartsWith(IEnumerable<skilltype> sequence)
    {
        if (sequence == null) return false;

        var seqArray = sequence.ToArray();
        if (seqArray.Length == 0 || seqArray.Length > _skillQueue.Count) return false;

        int index = 0;
        foreach (var node in _skillQueue)
        {
            if (index >= seqArray.Length) break;
            if (node.SkillType != seqArray[index]) return false;
            index++;
        }

        return true;
    }

    /// <summary>
    /// ��������Ƿ���ָ���������н�β
    /// </summary>
    public bool EndsWith(IEnumerable<skilltype> sequence)
    {
        if (sequence == null) return false;

        var seqArray = sequence.ToArray();
        if (seqArray.Length == 0 || seqArray.Length > _skillQueue.Count) return false;

        // �Ӷ���ĩβ��ʼ�Ƚ�
        var queueArray = _skillQueue.Select(n => n.SkillType).ToArray();
        int queueIndex = queueArray.Length - seqArray.Length;

        for (int i = 0; i < seqArray.Length; i++)
        {
            if (queueArray[queueIndex + i] != seqArray[i]) return false;
        }

        return true;
    }

    /// <summary>
    /// ���Ҽ����������е�λ��
    /// </summary>
    /// <returns>�����ҵ���λ�������б�</returns>
    public List<int> FindAllPositions(skilltype skillId)
    {
        var positions = new List<int>();

        int index = 0;
        foreach (var node in _skillQueue)
        {
            if (node.SkillType == skillId)
            {
                positions.Add(index);
            }
            index++;
        }

        return positions;
    }

    /// <summary>
    /// ��ȡ����Ƶ��ͳ��
    /// </summary>
    public Dictionary<skilltype, int> GetSkillFrequency()
    {
        var frequency = new Dictionary<skilltype, int>();

        foreach (var node in _skillQueue)
        {
            if (frequency.ContainsKey(node.SkillType))
            {
                frequency[node.SkillType]++;
            }
            else
            {
                frequency[node.SkillType] = 1;
            }
        }

        return frequency;
    }

    /// <summary>
    /// ��ȡ����ͳ����Ϣ
    /// </summary>
    public QueueStats GetStats()
    {
        return _currentStats;
    }

    /// <summary>
    /// ��ȡ�Ƴ�����ʷ��¼
    /// </summary>
    public List<SkillNode> GetRemovedHistory()
    {
        return new List<SkillNode>(_removedNodes);
    }

    #endregion

    #region �������� - ����ƥ��

    /// <summary>
    /// ����Ƿ�ƥ��ָ������
    /// </summary>
    public SequenceMatchResult MatchSequence(IEnumerable<skilltype> targetSequence, bool exactOrder = true,
                                             float maxTimeWindow = -1f, int startIndex = 0)
    {
        var result = new SequenceMatchResult();

        if (_skillQueue.Count == 0)
        {
            result.IsMatch = false;
            return result;
        }

        var targetList = targetSequence.ToList();
        if (targetList.Count == 0)
        {
            result.IsMatch = false;
            return result;
        }

        // ���ָ���˿�ʼ����������������Χ
        int searchStart = Mathf.Max(0, startIndex);
        int availableCount = _skillQueue.Count - searchStart;

        if (availableCount < targetList.Count)
        {
            result.IsMatch = false;
            return result;
        }

        // ��ȷ˳��ƥ��
        if (exactOrder)
        {
            return MatchExactSequence(targetList, searchStart, maxTimeWindow);
        }
        // ģ��ƥ�䣨ֻ��Ҫ������Щ���ܣ���Ҫ��˳��
        else
        {
            return MatchFuzzySequence(targetList, searchStart, maxTimeWindow);
        }
    }

    /// <summary>
    /// ����Ƿ�ƥ���κθ���������
    /// </summary>
    public SequenceMatchResult MatchAnySequence(IEnumerable<IEnumerable<skilltype>> sequences,
                                                bool exactOrder = true, float maxTimeWindow = -1f)
    {
        if (sequences == null)
        {
            return new SequenceMatchResult { IsMatch = false };
        }

        foreach (var sequence in sequences)
        {
            var result = MatchSequence(sequence, exactOrder, maxTimeWindow);
            if (result.IsMatch)
            {
                return result;
            }
        }

        return new SequenceMatchResult { IsMatch = false };
    }

    /// <summary>
    /// ��������ƥ�������
    /// </summary>
    public List<SequenceMatchResult> FindAllMatches(IEnumerable<skilltype> targetSequence,
                                                    bool exactOrder = true, float maxTimeWindow = -1f)
    {
        var results = new List<SequenceMatchResult>();

        if (targetSequence == null || _skillQueue.Count == 0)
            return results;

        var targetList = targetSequence.ToList();
        if (targetList.Count == 0)
            return results;

        // ��ÿ�����ܵ�λ�ÿ�ʼ����ƥ��
        for (int startIndex = 0; startIndex <= _skillQueue.Count - targetList.Count; startIndex++)
        {
            var result = MatchSequence(targetList, exactOrder, maxTimeWindow, startIndex);
            if (result.IsMatch)
            {
                results.Add(result);
            }
        }

        return results;
    }

    /// <summary>
    /// �����������ƥ������
    /// </summary>
    public SequenceMatchResult FindLongestMatch(IEnumerable<IEnumerable<skilltype>> possibleSequences,
                                                float maxTimeWindow = -1f)
    {
        var bestMatch = new SequenceMatchResult { IsMatch = false };

        if (possibleSequences == null)
            return bestMatch;

        foreach (var sequence in possibleSequences)
        {
            var seqList = sequence.ToList();
            if (seqList.Count <= bestMatch.MatchedSequence.Count)
                continue; // �ȵ�ǰ���ƥ��̣�����

            var result = MatchSequence(seqList, true, maxTimeWindow);
            if (result.IsMatch && result.MatchedSequence.Count > bestMatch.MatchedSequence.Count)
            {
                bestMatch = result;
            }
        }

        return bestMatch;
    }

    #endregion

    #region ˽�з��� - ƥ���㷨

    /// <summary>
    /// ��ȷ˳��ƥ��
    /// </summary>
    private SequenceMatchResult MatchExactSequence(List<skilltype> targetSequence, int startIndex, float maxTimeWindow)
    {
        var result = new SequenceMatchResult();
        var matchedNodes = _skillQueue
            .Skip(startIndex)
            .Take(targetSequence.Count)
            .ToList();

        if (matchedNodes.Count < targetSequence.Count)
        {
            result.IsMatch = false;
            return result;
        }

        for (int i = 0; i < targetSequence.Count; i++)
        {
            if (matchedNodes[i].SkillType != targetSequence[i])
            {
                result.IsMatch = false;
                return result;
            }
        }

        if (matchedNodes.Count > 0)
        {
            result.TotalTime = matchedNodes.Last().Timestamp - matchedNodes.First().Timestamp;

            if (maxTimeWindow > 0)
            {
                if (result.TotalTime > maxTimeWindow)
                {
                    result.IsMatch = false;
                    return result;
                }

                result.MatchAccuracy = 1f - Mathf.Clamp01(result.TotalTime / maxTimeWindow);
            }
        }

        result.IsMatch = true;
        result.MatchedSequence = new List<skilltype>(targetSequence);
        result.MatchedNodes = matchedNodes.Select(n => n.Clone()).ToList();
        result.StartIndex = startIndex;

        OnSequenceMatched?.Invoke(result);

        return result;
    }

    /// <summary>
    /// ģ��ƥ�䣨���ϰ�����
    /// </summary>
    private SequenceMatchResult MatchFuzzySequence(List<skilltype> targetSequence, int startIndex, float maxTimeWindow)
    {
        var result = new SequenceMatchResult();

        var candidateNodes = _skillQueue
            .Skip(startIndex)
            .ToList();

        if (candidateNodes.Count < targetSequence.Count)
        {
            result.IsMatch = false;
            return result;
        }

        if (maxTimeWindow > 0 && candidateNodes.Count > 0)
        {
            float totalTime = candidateNodes.Last().Timestamp - candidateNodes.First().Timestamp;
            if (totalTime > maxTimeWindow)
            {
                result.IsMatch = false;
                return result;
            }
        }

        var candidateSkills = candidateNodes.Select(n => n.SkillType).ToList();

        for (int i = 0; i < targetSequence.Count; i++)
        {
            if (!candidateSkills.Contains(targetSequence[i]))
            {
                result.IsMatch = false;
                return result;
            }
        }

        result.IsMatch = true;
        result.MatchedSequence = candidateSkills;
        result.MatchedNodes = candidateNodes.Select(n => n.Clone()).ToList();
        result.StartIndex = startIndex;

        if (candidateNodes.Count > 0)
        {
            result.TotalTime = candidateNodes.Last().Timestamp - candidateNodes.First().Timestamp;
            if (maxTimeWindow > 0)
            {
                result.MatchAccuracy = 1f - Mathf.Clamp01(result.TotalTime / maxTimeWindow);
            }
        }

        OnSequenceMatched?.Invoke(result);

        return result;
    }

    #endregion

    #region ˽�з��� - ���߷���

    /// <summary>
    /// ����ͳ����Ϣ
    /// </summary>
    private void UpdateStats()
    {
        _currentStats.TotalCount = _totalAddedCount;
        _currentStats.ActiveCount = _skillQueue.Count;

        if (_skillQueue.Count > 0)
        {
            _currentStats.OldestTimestamp = _skillQueue.First.Value.Timestamp;
            _currentStats.NewestTimestamp = _skillQueue.Last.Value.Timestamp;
            _currentStats.TimeSpan = _currentStats.NewestTimestamp - _currentStats.OldestTimestamp;
        }
        else
        {
            _currentStats.OldestTimestamp = 0;
            _currentStats.NewestTimestamp = 0;
            _currentStats.TimeSpan = 0;
        }

        _currentStats.SkillFrequency = GetSkillFrequency();
    }

    #endregion

    #region ���Է�����

    /// <summary>
    /// ��ȡ�����еļ�������
    /// </summary>
    public int Count => _skillQueue.Count;

    /// <summary>
    /// �����Ƿ�Ϊ��
    /// </summary>
    public bool IsEmpty => _skillQueue.Count == 0;

    /// <summary>
    /// ����¼ʱ�䣨�룩
    /// </summary>
    public float MaxRecordTime
    {
        get => _maxRecordTime;
        set => _maxRecordTime = Mathf.Max(0.1f, value);
    }

    /// <summary>
    /// �����д�С
    /// </summary>
    public int MaxQueueSize
    {
        get => _maxQueueSize;
        set => _maxQueueSize = Mathf.Max(1, value);
    }

    /// <summary>
    /// �Ƿ��Զ�����
    /// </summary>
    public bool AutoCleanup
    {
        get => _autoCleanup;
        set => _autoCleanup = value;
    }

    /// <summary>
    /// �����Ӵ���
    /// </summary>
    public int TotalAddedCount => _totalAddedCount;

    #endregion

}

/// <summary>
/// �������ж��еĵ�����ʾ����
/// </summary>
public static class SkillSequenceQueueExtensions
{
    /// <summary>
    /// ����������ת��Ϊ���ӻ��ַ���
    /// </summary>
    public static string ToVisualString(this SkillSequenceQueue queue, bool includeTimestamps = true)
    {
        if (queue == null) return "null";

        var nodes = queue.GetCurrentNodes();
        if (nodes.Count == 0) return "�ն���";

        var sb = new System.Text.StringBuilder();
        sb.Append("��������: ");

        for (int i = 0; i < nodes.Count; i++)
        {
            if (i > 0) sb.Append(" �� ");

            if (includeTimestamps)
            {
                float timeSinceStart = nodes[i].Timestamp - nodes[0].Timestamp;
                sb.Append($"{nodes[i].SkillType}(+{timeSinceStart:F1}s)");
            }
            else
            {
                sb.Append(nodes[i].SkillType);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// ��ӡ������ϸ��Ϣ
    /// </summary>
    public static void PrintDebugInfo(this SkillSequenceQueue queue, string label = "���ܶ���")
    {
        if (queue == null)
        {
            Debug.Log($"{label}: null");
            return;
        }

        var stats = queue.GetStats();
        var sequence = queue.GetCurrentSequence();

        Debug.Log($"{label}:\n" +
                  $"  ����: {stats.ActiveCount}/{stats.TotalCount}\n" +
                  $"  ʱ����: {stats.TimeSpan:F2}s\n" +
                  $"  ����: {string.Join("��", sequence)}\n" +
                  $"  Ƶ��: {string.Join(", ", stats.SkillFrequency.Select(kv => $"{kv.Key}:{kv.Value}"))}"
                  );
    }
}