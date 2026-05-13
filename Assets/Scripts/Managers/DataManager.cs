using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DataManager : MonoBehaviour, IGameManager
{
    public ManagerStatus Status { get; private set; }
    public int CurLevelProgress { get => SaveCSV.GetSettingsInt(OptionName.LevelProgress); }
    public int CurLan { get => SaveCSV.GetSettingsInt(OptionName.Lan); }

    public OptionData_SO OptionData;

    string urlPlayerData;
    string urlBattleSaveData;

    public DataCSV SaveCSV { get; private set; }
    public DataCSV OptionCSV { get; private set; }
    public DataCSV TitleCSV { get; private set; }

    public OptionBundle OptionBundle { get; private set; }

    public void Startup()
    {
        SaveCSV = new DataCSV("defaultSave.csv");
        OptionCSV = new DataCSV("options.csv");
        TitleCSV = new DataCSV("titles.csv");

        OptionBundle = new OptionBundle(OptionCSV);

        urlPlayerData = Path.Combine(Application.persistentDataPath, "save.csv");
        urlBattleSaveData = Path.Combine(Application.persistentDataPath, "battleRunSave.json");
        if (!File.Exists(urlPlayerData))
            SaveCSV.SaveData(urlPlayerData);
        else
            SaveCSV.LoadData(urlPlayerData);

        EnsureSaveKey(OptionName.HasRunSave, "0");
        EnsureSaveKey(OptionName.ResumeScene, "0");
        EnsureSaveKey(OptionName.RunSaveVersion, "1");
        SaveGame();

        Status = ManagerStatus.Started;
    }

    void Update()
    {
    }

    public void ChangeSaveData(OptionName optionName, int value)
    {
        SaveCSV.SetCell(optionName, value.ToString());
    }

    public void ChangeSaveData(OptionName optionName, string value)
    {
        SaveCSV.SetCell(optionName, value);
    }

    public void SaveGame()
    {
        SaveCSV.SaveData(urlPlayerData);
        GameEvent.DataSaved.Invoke();
    }

    public void LoadGame()
    {
        SaveCSV.LoadData(urlPlayerData);
    }

    public void SaveBattleRunData(BattleProgressSaveData battleSaveData)
    {
        if (battleSaveData == null)
            return;

        File.WriteAllText(urlBattleSaveData, JsonUtility.ToJson(battleSaveData, true));
    }

    public BattleProgressSaveData LoadBattleRunData()
    {
        if (!File.Exists(urlBattleSaveData))
            return null;

        string json = File.ReadAllText(urlBattleSaveData);
        if (string.IsNullOrWhiteSpace(json))
            return null;

        return JsonUtility.FromJson<BattleProgressSaveData>(json);
    }

    public bool HasBattleRunData()
    {
        return File.Exists(urlBattleSaveData);
    }

    public void DeleteBattleRunData()
    {
        if (File.Exists(urlBattleSaveData))
            File.Delete(urlBattleSaveData);
    }

    void EnsureSaveKey(OptionName optionName, string defaultValue)
    {
        if (SaveCSV.RowNameDic.ContainsKey(optionName.ToString()))
            return;

        DataCSV defaultSaveCsv = new DataCSV("defaultSave.csv");
        if (!defaultSaveCsv.RowNameDic.ContainsKey(optionName.ToString()))
            return;

        string[] newRow = new string[defaultSaveCsv.ColCount];
        int rowIndex = defaultSaveCsv.RowNameDic[optionName.ToString()];
        for (int col = 0; col < defaultSaveCsv.ColCount; col++)
            newRow[col] = defaultSaveCsv.GetString(rowIndex, col);

        if (newRow.Length > GameConfig.SettingsCol)
            newRow[GameConfig.SettingsCol] = defaultValue;

        int insertIndex = FindInsertIndexAfterGameSection();
        SaveCSV.InsertRow(insertIndex, newRow);
    }

    int FindInsertIndexAfterGameSection()
    {
        int lastGameRow = SaveCSV.RowCount;
        for (int row = 0; row < SaveCSV.RowCount; row++)
        {
            string rowName = SaveCSV.GetString(row, 0);
            if (rowName == "[Game]")
            {
                lastGameRow = row + 1;
                continue;
            }

            if (lastGameRow < SaveCSV.RowCount)
            {
                if (string.IsNullOrEmpty(rowName) || rowName.StartsWith("["))
                    return row;
                lastGameRow = row + 1;
            }
        }

        return lastGameRow;
    }

    public void SwitchFullscreen(int curValue)
    {
        if (curValue == (int)OptionSetting.On)
            Screen.SetResolution(1920, 1080, true);
        else
            Screen.SetResolution(1920, 1080, false);
    }
}
