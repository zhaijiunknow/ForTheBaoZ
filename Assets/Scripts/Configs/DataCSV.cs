using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class DataCSV
{
    List<string[]> csvData; //根据CSV生成的数组，相当于缓存，对其修改后，通过SaveData()保存回CSV源文件

    public Dictionary<string, int> RowNameDic { get; private set; } //用来存放csv第0列的内容和行号
    public List<string> RowNameList { get; private set; } //用来存放csv第0列的内容
    public int ColCount => csvData[0].Length;
    public int RowCount => csvData.Count;

    public DataCSV(string _fileName, string _extraPath = "") //默认路径为Asset/StreamingAssets
    {
        csvData = new List<string[]>();
        RowNameDic = new Dictionary<string, int>();
        RowNameList = new List<string>();
        LoadData(_fileName, _extraPath); //如果有子目录，_extraPath格式为 folder/
    }

    #region save and load
    public void SaveData(string _url)
    {
        using (StreamWriter sw = new StreamWriter(_url, false))
        {
            foreach (var row in csvData)
            {
                sw.WriteLine(string.Join(",", row));
            }
        }
    }

    public void LoadData(string _fileName, string _extraPath = "")
    {
        string path = Path.Combine(Application.streamingAssetsPath, _extraPath);
        string url = Path.Combine(path, _fileName);

        ReadFile(url);
    }

    void ReadFile(string _url)
    {
        csvData.Clear();
        StreamReader reader = null;
        try
        {
            reader = File.OpenText(_url);
        }
        catch
        {
            Debug.Log("File not find!");
            return;
        }

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            var fields = ParseCsvLine(line);
            csvData.Add(fields);
        }
        reader.Close();
        reader.Dispose();

        InitRowDic();
        InitRowList();
    }
    #endregion save and load

    #region get and set
    public string GetString(int row, int col)
    {
        string rawData = csvData[row][col];
        if (rawData != null)
        {
            if (rawData.Length != rawData.Trim().Length)
            {
                rawData = rawData.Trim(); //如果存在首尾空格，将被自动去除；有时候表格会为最后一列添加隐形空格，非人为因素造成
                Debug.LogWarning("There is an extra space in: " + rawData); //如不需要，可以把警告关掉
            }
        }
        return rawData;
    }

    public string GetString(string rowName, int col)
    {
        int row = GetRowIndex(rowName);
        return GetString(row, col);
    }

    public int GetInt(string rowName, int col)
    {
        int row = GetRowIndex(rowName);
        return int.Parse(csvData[row][col]);
    }

    public int GetSettingsInt(OptionName optionName) //for OptionName only
    {
        int row = GetRowIndex(optionName);
        return int.Parse(csvData[row][GameConfig.SettingsCol]);
    }

    public void SetCell(int row, int col, string value)
    {
        csvData[row][col] = value;
    }

    public void SetCell(string rowName, int col, string value)
    {
        int row = GetRowIndex(rowName);
        csvData[row][col] = value;
    }

    public void SetCell(OptionName optionName, string value) //for OptionName only
    {
        int row = GetRowIndex(optionName);
        csvData[row][GameConfig.SettingsCol] = value;
    }
    #endregion get and set

    //private function
    void InitRowDic()
    {
        RowNameDic.Clear();
        for (int row = 0; row < RowCount; row++)
        {
            string rowName = GetString(row, 0);
            if (string.IsNullOrEmpty(rowName)) //如果是空单元格，跳过
            {
                // Debug.LogWarning("empty row name ");
                continue;
            }
            if (RowNameDic.ContainsKey(GetString(row, 0))) //如果有重复单元格，跳过
            {
                // Debug.LogWarning( "duplicate row name " + key);
                continue;
            }
            else
                RowNameDic.Add(GetString(row, 0), row);
        }
    }

    void InitRowList()
    {
        RowNameList.Clear();
        for (int row = 0; row < RowCount; row++)
        {
            RowNameList.Add(GetString(row, 0));
        }
    }

    int GetRowIndex(string _rowName)
    {
        if (!RowNameDic.ContainsKey(_rowName))
        {
            Debug.LogError(_rowName + " not found!");
            return 0;
        }
        return RowNameDic[_rowName];
    }

    int GetRowIndex(OptionName _optionName)
    {
        string _rowName = _optionName.ToString();
        if (!RowNameDic.ContainsKey(_rowName))
        {
            Debug.LogError(_rowName + " not found!");
            return 0;
        }
        return RowNameDic[_rowName];
    }

    //如果单元格内存在双引号（通常用于保护英文逗号），将去掉双引号
    string[] ParseCsvLine(string line)
    {
        List<string> fields = new List<string>();
        bool inQuotes = false;
        string currentField = "";

        foreach (char c in line)
        {
            if (c == '"' && !inQuotes)
            {
                inQuotes = true; // Start of quoted field
            }
            else if (c == '"' && inQuotes)
            {
                inQuotes = false; // End of quoted field
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(currentField);
                currentField = ""; // Start a new field
            }
            else
            {
                currentField += c; // Add character to the current field
            }
        }

        // Add the last field
        fields.Add(currentField);

        return fields.ToArray();
    }
}