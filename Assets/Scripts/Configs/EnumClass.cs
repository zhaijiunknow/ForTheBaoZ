using UnityEngine;
using System.Collections.Generic;
using System;

public interface IGameManager
{
    ManagerStatus Status { get; }
    void Startup();
}

public class GameBundle
{
    protected DataCSV dataCSV;
    public GameBundle() { }
}

public class OptionBundle : GameBundle
{
    List<OptionContent> optionContents;

    public OptionBundle(DataCSV _dataCSV)
    {
        dataCSV = _dataCSV;
        optionContents = new List<OptionContent>();

        for (int row = 0; row < dataCSV.RowCount; row++)
        {
            string curRowName = dataCSV.GetString(row, 0);

            if (curRowName == "end")
            {
                int sectionEndLine = row;
                int sectionStartLine = GetOptionNameIndex(row);
                string startRowName = dataCSV.GetString(sectionStartLine, 0);
                OptionContent toAddOptionContent = new OptionContent(startRowName, dataCSV.ColCount);

                for (int sectionCol = 0; sectionCol < dataCSV.ColCount; sectionCol++)
                {
                    for (int sectionRow = sectionStartLine + 1; sectionRow < sectionEndLine + 1; sectionRow++)
                    {
                        string toAddValue = dataCSV.GetString(sectionRow, sectionCol);
                        toAddOptionContent.optionValuesLists[sectionCol].Add(toAddValue);
                    }
                }
                optionContents.Add(toAddOptionContent);
            }
        }
    }

    public OptionContent GetOptionContent(OptionName _optionName)
    {
        return optionContents.Find(i => i.optionName == _optionName.ToString());
    }

    bool IsOptionName(string value)
    {
        return Enum.TryParse<OptionName>(value, out _);
    }

    //get the OptionName index by the "end" index
    int GetOptionNameIndex(int _endIndex)
    {
        for (int i = _endIndex; i > 0; i--)
        {
            if (IsOptionName(dataCSV.RowNameList[i]))
                return i;
        }
        return 0;
    }
}

public class OptionContent
{
    public string optionName;
    public List<string>[] optionValuesLists;

    public OptionContent(string _optionName, int _colCount)
    {
        optionName = _optionName;
        optionValuesLists = new List<string>[_colCount];
        for (int i = 0; i < _colCount; i++)
        {
            optionValuesLists[i] = new List<string>();
        }
    }

    public int GetOptionValuesLength()
    {
        return optionValuesLists[1].Count;
    }

    public List<string> GetOptionValuesByLan(int _lan)
    {
        return optionValuesLists[_lan];
    }
}

