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

    public DataCSV SaveCSV { get; private set; }
    public DataCSV OptionCSV { get; private set; }
    public DataCSV TitleCSV { get; private set; }

    public OptionBundle OptionBundle { get; private set; }

    public void Startup()
    {
        //首先读取csv文件，文件必须放在Assets/StreamingAssets目录下
        SaveCSV = new DataCSV("defaultSave.csv");  //存档缓存，通过ChangePlayerData修改，通过SaveGame保存
        OptionCSV = new DataCSV("options.csv"); //所有OptionName及选项的翻译，不用改
        TitleCSV = new DataCSV("titles.csv"); //所有按钮及标题的翻译，不用改

        //再新建GameBundle, GameBundle用于读取特定的复杂的csv数据结构
        OptionBundle = new OptionBundle(OptionCSV);

        //如果没有创建过存档，将自动创建一个存档；如创建过存档，将读取存档
        urlPlayerData = Path.Combine(Application.persistentDataPath, "save.csv");
        if (!File.Exists(urlPlayerData))
            SaveCSV.SaveData(urlPlayerData);
        else
            SaveCSV.LoadData(urlPlayerData);

        Status = ManagerStatus.Started;
    }

    void Update()
    {
       
    }


    //修改存档缓存（整数类型）
    public void ChangeSaveData(OptionName optionName, int value)
    {
        SaveCSV.SetCell(optionName, value.ToString());
    }

    //修改存档缓存（字符串类型）
    public void ChangeSaveData(OptionName optionName, string value)
    {
        SaveCSV.SetCell(optionName, value);
    }

    //保存存档
    public void SaveGame()
    {
        SaveCSV.SaveData(urlPlayerData);
        GameEvent.DataSaved.Invoke();
    }

    //读取存档
    public void LoadGame()
    {
        SaveCSV.LoadData(urlPlayerData);
    }


    //Option相关
    public void SwitchFullscreen(int curValue)
    {
        if (curValue == (int)OptionSetting.On)
            Screen.SetResolution(1920, 1080, true);
        else
            Screen.SetResolution(1920, 1080, false);
    }

}
