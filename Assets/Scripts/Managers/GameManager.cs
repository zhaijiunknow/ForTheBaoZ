using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(DataManager))]
[RequireComponent(typeof(AudioManager))]
[RequireComponent(typeof(InputManager))]
public class GameManager : MonoBehaviour
{
    public static GameStatus CurGameStatus { get; private set; }
    public static SceneName CurScene { get; private set; }

    public static DataManager Data { get; private set; }
    public static AudioManager Audio { get; private set; }
    public static InputManager NewInput { get; private set; }
    public static int SavedSkillCount { get; private set; }
    public static List<int> SavedSkillBox { get; private set; } = new List<int>();
    public static BattleProgressSaveData CurrentBattleSave { get; private set; }

    static List<IGameManager> _managerList;
    public static GameManager instance;

    void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        _managerList = new List<IGameManager>();

        Data = GetComponent<DataManager>();
        NewInput = GetComponent<InputManager>();
        Audio = GetComponent<AudioManager>();

        _managerList.Add(Audio);
        _managerList.Add(NewInput);
    }

    void Start()
    {
        Data.Startup();
        StartCoroutine(StartupManagers());
    }

    IEnumerator StartupManagers()
    {
        while (Data.Status != ManagerStatus.Started)
            yield return null;

        foreach (IGameManager manager in _managerList)
            manager.Startup();

        yield return null;
        int numManagers = _managerList.Count;
        int numReady = 0;

        while (numReady < numManagers)
        {
            int lastReady = numReady;
            numReady = 0;
            foreach (IGameManager manager in _managerList)
            {
                if (manager.Status == ManagerStatus.Started)
                    numReady++;
            }
            if (numReady > lastReady)
            {
                GameEvent.ManagerProgress.Invoke(numReady, numManagers);
            }
            yield return null;
        }

        SetGameStatus(GameStatus.Loaded);

        yield return 0.1f;

        if (Data.SaveCSV.GetSettingsInt(OptionName.PresetProgress) == (int)DataStatus.NoPresetData)
            SwitchScene(SceneName.Preset);
        else
            SwitchScene(SceneName.Menu);
    }

    public static void SetGameStatus(GameStatus _status)
    {
        CurGameStatus = _status;

        if (CurGameStatus == GameStatus.Paused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;

        if (NewInput == null)
            return;

        if (CurGameStatus == GameStatus.Playing)
            NewInput.SwitchToPlayer();
        else
            NewInput.SwitchToUI();
    }

    public static void SwitchScene(SceneName _scene)
    {
        CurScene = _scene;
        SceneManager.LoadScene(_scene.ToString());
    }

    public static BattleProgressSaveData GetCurrentBattleSave()
    {
        return CurrentBattleSave;
    }

    public static void StartNewRun()
    {
        ClearRunSave();
        SavedSkillCount = 3;
        SavedSkillBox = new List<int> { 0, 1, 2 };
        CurrentBattleSave = CreateDefaultBattleSave();
        SaveCurrentRun();
        SetGameStatus(GameStatus.Playing);
        SwitchScene(SceneName.Battle);
    }

    public static bool HasSavedRun()
    {
        return Data != null
            && Data.SaveCSV != null
            && Data.SaveCSV.GetSettingsInt(OptionName.HasRunSave) == 1
            && Data.HasBattleRunData();
    }

    public static void SaveCurrentRun()
    {
        if (Data == null)
            return;

        CurrentBattleSave = CaptureBattleSaveFromScene() ?? CurrentBattleSave ?? CreateDefaultBattleSave();
        Data.SaveBattleRunData(CurrentBattleSave);
        Data.ChangeSaveData(OptionName.HasRunSave, 1);
        Data.ChangeSaveData(OptionName.ResumeScene, 1);
        Data.ChangeSaveData(OptionName.RunSaveVersion, CurrentBattleSave.version);
        Data.SaveGame();
    }

    public static bool LoadSavedRun()
    {
        if (Data == null || !Data.HasBattleRunData())
            return false;

        BattleProgressSaveData battleSaveData = Data.LoadBattleRunData();
        if (battleSaveData == null)
        {
            ClearRunSave();
            return false;
        }

        CurrentBattleSave = battleSaveData;
        SavedSkillCount = battleSaveData.skillCount;
        SavedSkillBox = battleSaveData.mySkillBox != null ? new List<int>(battleSaveData.mySkillBox) : new List<int>();
        return true;
    }

    public static bool ResumeSavedRun()
    {
        if (!LoadSavedRun())
            return false;

        SetGameStatus(GameStatus.Playing);
        SwitchScene(SceneName.Battle);
        return true;
    }

    public static void ClearRunSave()
    {
        SavedSkillCount = 0;
        SavedSkillBox = new List<int>();
        CurrentBattleSave = null;

        if (Data == null)
            return;

        Data.DeleteBattleRunData();
        Data.ChangeSaveData(OptionName.HasRunSave, 0);
        Data.ChangeSaveData(OptionName.ResumeScene, 0);
        Data.ChangeSaveData(OptionName.RunSaveVersion, 1);
        Data.SaveGame();
    }

    public static void ApplySavedSkillState(MySkillManager skillManager)
    {
        if (skillManager == null || SavedSkillBox == null || SavedSkillBox.Count == 0)
            return;

        skillManager.ApplySavedSkillState(SavedSkillCount, SavedSkillBox);
    }

    public static void CaptureSkillSnapshot()
    {
        MySkillManager skillManager = Object.FindObjectOfType<MySkillManager>();
        if (skillManager == null)
            return;

        SavedSkillCount = skillManager.skillcount;
        SavedSkillBox = skillManager.GetPlayerSkillBoxSnapshot();
    }

    static BattleProgressSaveData CreateDefaultBattleSave()
    {
        return new BattleProgressSaveData
        {
            version = 1,
            checkpointId = "phase_1",
            currentProgress = 0f,
            currentPhaseIndex = 0,
            nextTargetProgress = 300f,
            isPausedForStopover = false,
            isBossPhase = false,
            skippedRestCount = 0,
            sprintObsessionMultiplier = 1f,
            obsession = 0f,
            foods = new List<FoodEntryData>(),
            reachedCheckpointIds = new List<string>(),
            skillCount = SavedSkillCount,
            mySkillBox = new List<int>(SavedSkillBox),
            redDistance = 0f,
            cafeteriaCandidateIndex = 0,
            gameStarted = false,
            remainingTime = 180f,
        };
    }

    static BattleProgressSaveData CaptureBattleSaveFromScene()
    {
        BattleRunState runState = Object.FindObjectOfType<BattleRunState>();
        BattleManager battleManager = Object.FindObjectOfType<BattleManager>();
        CafeteriaController cafeteriaController = Object.FindObjectOfType<CafeteriaController>();
        if (runState == null || battleManager == null)
            return null;

        CaptureSkillSnapshot();
        BattleCheckpointData checkpoint = runState.GetNextCheckpoint();
        return new BattleProgressSaveData
        {
            version = 1,
            checkpointId = checkpoint != null ? checkpoint.checkpointId : "boss",
            currentProgress = runState.currentProgress,
            currentPhaseIndex = runState.currentPhaseIndex,
            nextTargetProgress = checkpoint != null ? checkpoint.targetProgress : battleManager.winDistance,
            isPausedForStopover = runState.isPausedForStopover,
            isBossPhase = runState.isBossPhase,
            skippedRestCount = runState.skippedRestCount,
            sprintObsessionMultiplier = runState.sprintObsessionMultiplier,
            obsession = runState.obsession,
            foods = new List<FoodEntryData>(runState.foods),
            reachedCheckpointIds = new List<string>(runState.reachedCheckpointIds),
            skillCount = SavedSkillCount,
            mySkillBox = new List<int>(SavedSkillBox),
            redDistance = battleManager.redDistance,
            cafeteriaCandidateIndex = cafeteriaController != null ? cafeteriaController.GetCandidateIndex() : 0,
            gameStarted = DynamicData.GameStart,
            remainingTime = DynamicData.TimerBack,
        };
    }
}
