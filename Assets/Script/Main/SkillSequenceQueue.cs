using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 技能序列队列管理器
/// 使用队列+时间戳管理技能序列，支持自动清理过期记录和序列匹配
/// </summary>
public class SkillSequenceQueue
{
    #region 内部数据结构

    /// <summary>
    /// 技能记录节点
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
    /// 序列匹配结果
    /// </summary>
    public class SequenceMatchResult
    {
        public bool IsMatch { get; set; }
        public List<skilltype> MatchedSequence { get; set; }
        public List<SkillNode> MatchedNodes { get; set; }
        public int StartIndex { get; set; }
        public float TotalTime { get; set; }
        public float MatchAccuracy { get; set; }
        public object Tag { get; set; }  // 附加数据

        public SequenceMatchResult()
        {
            MatchedSequence = new List<skilltype>();
            MatchedNodes = new List<SkillNode>();
        }

        public override string ToString()
        {
            return IsMatch ?
                $"匹配成功: {string.Join("→", MatchedSequence)} (耗时:{TotalTime:F2}s, 精度:{MatchAccuracy:F2})" :
                "无匹配";
        }
    }

    /// <summary>
    /// 统计信息
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
            return $"队列统计: {ActiveCount}/{TotalCount} 个记录, 时间跨度: {TimeSpan:F2}s";
        }
    }

    #endregion

    #region 属性与字段

    private readonly LinkedList<SkillNode> _skillQueue = new();
    private readonly List<SkillNode> _removedNodes = new(); // 用于记录移除的节点

    // 配置
    private float _maxRecordTime = 10f; // 默认最大记录时间（秒）
    private int _maxQueueSize = 5;     // 最大队列大小
    private bool _autoCleanup = true;   // 是否自动清理过期记录

    // 统计
    private int _totalAddedCount = 0;
    private QueueStats _currentStats;

    // 事件
    public event Action<SkillNode> OnSkillAdded;
    public event Action<SkillNode> OnSkillRemoved;
    public event Action<SequenceMatchResult> OnSequenceMatched;

    #endregion

    #region 构造函数

    public SkillSequenceQueue() { }

    public SkillSequenceQueue(float maxRecordTime, int maxQueueSize = 20, bool autoCleanup = true)
    {
        _maxRecordTime = Mathf.Max(0.1f, maxRecordTime);
        _maxQueueSize = Mathf.Max(1, maxQueueSize);
        _autoCleanup = autoCleanup;
    }

    #endregion

    #region 公共方法 - 添加与移除

    /// <summary>
    /// 添加技能记录到队列
    /// </summary>
    /// <returns>添加是否成功</returns>
    public bool AddSkill(skilltype skillType)
    {
        float currentTime = Time.time;

        // 自动清理过期记录
        if (_autoCleanup)
        {
            CleanupExpired(currentTime);
        }

        // 检查队列大小限制
        if (_skillQueue.Count >= _maxQueueSize)
        {
            RemoveOldest();
        }

        // 创建新节点
        var newNode = new SkillNode(currentTime, skillType);

        // 添加到队列尾部
        _skillQueue.AddLast(newNode);
        _totalAddedCount++;

        // 更新统计
        UpdateStats();

        // 触发事件
        OnSkillAdded?.Invoke(newNode);

        return true;
    }

    /// <summary>
    /// 批量添加技能记录
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
    /// 移除最旧的技能记录
    /// </summary>
    /// <returns>被移除的记录，如果队列为空则返回null</returns>
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
    /// 清理所有过期记录（基于当前时间）
    /// </summary>
    /// <returns>清理的数量</returns>
    public int CleanupExpired(float currentTime = -1f)
    {
        if (currentTime < 0) currentTime = Time.time;

        int count = 0;
        float expireTime = currentTime - _maxRecordTime;

        // 移除所有时间戳小于expireTime的记录
        while (_skillQueue.Count > 0 && _skillQueue.First.Value.Timestamp < expireTime)
        {
            var removed = RemoveOldest();
            if (removed != null) count++;
        }

        return count;
    }

    /// <summary>
    /// 清空整个队列
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

    #region 公共方法 - 查询与获取

    /// <summary>
    /// 获取当前技能序列（仅技能ID）
    /// </summary>
    public List<skilltype> GetCurrentSequence()
    {
        return _skillQueue.Select(node => node.SkillType).ToList();
    }

    /// <summary>
    /// 获取当前技能节点序列
    /// </summary>
    public List<SkillNode> GetCurrentNodes()
    {
        return _skillQueue.Select(node => node.Clone()).ToList();
    }

    /// <summary>
    /// 获取指定时间范围内的技能序列
    /// </summary>
    public List<skilltype> GetSequenceInTimeRange(float startTime, float endTime)
    {
        return _skillQueue
            .Where(node => node.Timestamp >= startTime && node.Timestamp <= endTime)
            .Select(node => node.SkillType)
            .ToList();
    }

    /// <summary>
    /// 获取最近的N个技能
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
    /// 获取最近一个技能
    /// </summary>
    public skilltype GetLastSkill()
    {
        return _skillQueue.Count > 0 ? _skillQueue.Last.Value.SkillType : skilltype.None;
    }

    /// <summary>
    /// 获取最近一个技能节点
    /// </summary>
    public SkillNode GetLastNode()
    {
        return _skillQueue.Count > 0 ? _skillQueue.Last.Value.Clone() : null;
    }

    /// <summary>
    /// 获取指定索引的技能
    /// </summary>
    public skilltype GetSkillAt(int index)
    {
        if (index < 0 || index >= _skillQueue.Count) return skilltype.None;

        return _skillQueue.ElementAt(index).SkillType;
    }

    /// <summary>
    /// 获取指定索引的技能节点
    /// </summary>
    public SkillNode GetNodeAt(int index)
    {
        if (index < 0 || index >= _skillQueue.Count) return null;

        return _skillQueue.ElementAt(index).Clone();
    }

    /// <summary>
    /// 检查序列是否包含指定技能
    /// </summary>
    public bool Contains(skilltype skillId)
    {
        return _skillQueue.Any(node => node.SkillType == skillId);
    }

    /// <summary>
    /// 检查序列是否以指定技能序列开头
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
    /// 检查序列是否以指定技能序列结尾
    /// </summary>
    public bool EndsWith(IEnumerable<skilltype> sequence)
    {
        if (sequence == null) return false;

        var seqArray = sequence.ToArray();
        if (seqArray.Length == 0 || seqArray.Length > _skillQueue.Count) return false;

        // 从队列末尾开始比较
        var queueArray = _skillQueue.Select(n => n.SkillType).ToArray();
        int queueIndex = queueArray.Length - seqArray.Length;

        for (int i = 0; i < seqArray.Length; i++)
        {
            if (queueArray[queueIndex + i] != seqArray[i]) return false;
        }

        return true;
    }

    /// <summary>
    /// 查找技能在序列中的位置
    /// </summary>
    /// <returns>所有找到的位置索引列表</returns>
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
    /// 获取技能频率统计
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
    /// 获取队列统计信息
    /// </summary>
    public QueueStats GetStats()
    {
        return _currentStats;
    }

    /// <summary>
    /// 获取移除的历史记录
    /// </summary>
    public List<SkillNode> GetRemovedHistory()
    {
        return new List<SkillNode>(_removedNodes);
    }

    #endregion

    #region 公共方法 - 序列匹配

    /// <summary>
    /// 检查是否匹配指定序列
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

        // 如果指定了开始索引，调整搜索范围
        int searchStart = Mathf.Max(0, startIndex);
        int availableCount = _skillQueue.Count - searchStart;

        if (availableCount < targetList.Count)
        {
            result.IsMatch = false;
            return result;
        }

        // 精确顺序匹配
        if (exactOrder)
        {
            return MatchExactSequence(targetList, searchStart, maxTimeWindow);
        }
        // 模糊匹配（只需要包含这些技能，不要求顺序）
        else
        {
            return MatchFuzzySequence(targetList, searchStart, maxTimeWindow);
        }
    }

    /// <summary>
    /// 检查是否匹配任何给定的序列
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
    /// 查找所有匹配的序列
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

        // 从每个可能的位置开始尝试匹配
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
    /// 查找最长的连续匹配序列
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
                continue; // 比当前最佳匹配短，跳过

            var result = MatchSequence(seqList, true, maxTimeWindow);
            if (result.IsMatch && result.MatchedSequence.Count > bestMatch.MatchedSequence.Count)
            {
                bestMatch = result;
            }
        }

        return bestMatch;
    }

    #endregion

    #region 私有方法 - 匹配算法

    /// <summary>
    /// 精确顺序匹配
    /// </summary>
    private SequenceMatchResult MatchExactSequence(List<skilltype> targetSequence, int startIndex, float maxTimeWindow)
    {
        var result = new SequenceMatchResult();
        var matchedNodes = new List<SkillNode>();

        // 收集匹配的节点
        int queueIndex = 0;
        int matchIndex = 0;

        foreach (var node in _skillQueue)
        {
            if (queueIndex < startIndex)
            {
                queueIndex++;
                continue;
            }

            if (matchIndex >= targetSequence.Count)
                break;

            if (node.SkillType == targetSequence[matchIndex])
            {
                matchedNodes.Add(node);
            }
            if (matchedNodes.Count != 0)
            {
                matchIndex++;
            }
            queueIndex++;
        }

        // 检查是否完全匹配
        if (matchIndex != targetSequence.Count)
        {
            result.IsMatch = false;
            return result;
        }

        // 检查时间窗口
        if (maxTimeWindow > 0 && matchedNodes.Count > 0)
        {
            float totalTime = matchedNodes.Last().Timestamp - matchedNodes.First().Timestamp;
            if (totalTime > maxTimeWindow)
            {
                result.IsMatch = false;
                return result;
            }

            result.TotalTime = totalTime;
            result.MatchAccuracy = 1f - Mathf.Clamp01(totalTime / maxTimeWindow);
        }

        // 构建成功结果
        result.IsMatch = true;
        result.MatchedSequence = new List<skilltype>(targetSequence);
        result.MatchedNodes = matchedNodes.Select(n => n.Clone()).ToList();
        result.StartIndex = startIndex;

        // 触发事件
        OnSequenceMatched?.Invoke(result);

        return result;
    }

    /// <summary>
    /// 模糊匹配（集合包含）
    /// </summary>
    private SequenceMatchResult MatchFuzzySequence(List<skilltype> targetSequence, int startIndex, float maxTimeWindow)
    {
        var result = new SequenceMatchResult();

        // 获取指定范围内的技能
        var candidateNodes = _skillQueue
            .Skip(startIndex)
            .ToList();

        if (candidateNodes.Count < targetSequence.Count)
        {
            result.IsMatch = false;
            return result;
        }

        // 检查时间窗口
        if (maxTimeWindow > 0 && candidateNodes.Count > 0)
        {
            float totalTime = candidateNodes.Last().Timestamp - candidateNodes.First().Timestamp;
            if (totalTime > maxTimeWindow)
            {
                result.IsMatch = false;
                return result;
            }
        }

        // 检查是否包含所有目标技能
        var candidateSkills = candidateNodes.Select(n => n.SkillType).ToList();
        
        for (int i = 0; i < targetSequence.Count; i++) 
        {
            if (Contains(targetSequence[i]))
            {
                continue;
            }
            result.IsMatch = false;
            return result;
        }



        // 构建成功结果
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

        // 触发事件
        OnSequenceMatched?.Invoke(result);

        return result;
    }

    #endregion

    #region 私有方法 - 工具方法

    /// <summary>
    /// 更新统计信息
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

    #region 属性访问器

    /// <summary>
    /// 获取队列中的技能数量
    /// </summary>
    public int Count => _skillQueue.Count;

    /// <summary>
    /// 队列是否为空
    /// </summary>
    public bool IsEmpty => _skillQueue.Count == 0;

    /// <summary>
    /// 最大记录时间（秒）
    /// </summary>
    public float MaxRecordTime
    {
        get => _maxRecordTime;
        set => _maxRecordTime = Mathf.Max(0.1f, value);
    }

    /// <summary>
    /// 最大队列大小
    /// </summary>
    public int MaxQueueSize
    {
        get => _maxQueueSize;
        set => _maxQueueSize = Mathf.Max(1, value);
    }

    /// <summary>
    /// 是否自动清理
    /// </summary>
    public bool AutoCleanup
    {
        get => _autoCleanup;
        set => _autoCleanup = value;
    }

    /// <summary>
    /// 总添加次数
    /// </summary>
    public int TotalAddedCount => _totalAddedCount;

    #endregion

}

/// <summary>
/// 技能序列队列的调试显示工具
/// </summary>
public static class SkillSequenceQueueExtensions
{
    /// <summary>
    /// 将技能序列转换为可视化字符串
    /// </summary>
    public static string ToVisualString(this SkillSequenceQueue queue, bool includeTimestamps = true)
    {
        if (queue == null) return "null";

        var nodes = queue.GetCurrentNodes();
        if (nodes.Count == 0) return "空队列";

        var sb = new System.Text.StringBuilder();
        sb.Append("技能序列: ");

        for (int i = 0; i < nodes.Count; i++)
        {
            if (i > 0) sb.Append(" → ");

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
    /// 打印队列详细信息
    /// </summary>
    public static void PrintDebugInfo(this SkillSequenceQueue queue, string label = "技能队列")
    {
        if (queue == null)
        {
            Debug.Log($"{label}: null");
            return;
        }

        var stats = queue.GetStats();
        var sequence = queue.GetCurrentSequence();

        Debug.Log($"{label}:\n" +
                  $"  数量: {stats.ActiveCount}/{stats.TotalCount}\n" +
                  $"  时间跨度: {stats.TimeSpan:F2}s\n" +
                  $"  序列: {string.Join("→", sequence)}\n" +
                  $"  频率: {string.Join(", ", stats.SkillFrequency.Select(kv => $"{kv.Key}:{kv.Value}"))}"
                  );
    }
}