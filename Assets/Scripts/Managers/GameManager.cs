using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//如果你需要添加新的Manager，遵循注释(step1-4)添加即可
[RequireComponent(typeof(DataManager))]
[RequireComponent(typeof(AudioManager))]
[RequireComponent(typeof(InputManager))]
//[RequireComponent(typeof(YourManager))] //step 1

public class GameManager : MonoBehaviour
{
    public static GameStatus CurGameStatus { get; private set; }
    public static SceneName CurScene { get; private set; }

    public static DataManager Data { get; private set; }
    public static AudioManager Audio { get; private set; }
    public static InputManager NewInput { get; private set; }
    // public static YourManager YourName { get; private set; }  //step 2

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
        NewInput = GetComponent<InputManager>(); //Using Input will duplicate with the old input system
        Audio = GetComponent<AudioManager>();
        // YourName = GetComponent<YourManager>();  //step 3

        _managerList.Add(Audio);
        _managerList.Add(NewInput);
        // _managerList.Add(YourName); //step 4

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
        //设置game status
        CurGameStatus = _status;

        //设置Time scale
        if (CurGameStatus == GameStatus.Paused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;

        //设置input action map
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

}
