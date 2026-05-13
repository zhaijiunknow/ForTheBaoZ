using UnityEngine;
using UnityEngine.UI;
using static GameManager;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class GameButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IUpdateSelectedHandler
{
    public TitleName title_name;
    public int buttonID { get; private set; }

    protected Button button;
    protected Text[] texts;
    protected Image[] images;
    protected Image imageIcon, imageBackground;
    protected CanvasGroup canvasGroup;

    bool focused, updateSelected;

    void Awake()
    {
        InitButton();
    }

    void OnEnable()
    {
        ShowButton();
        button.onClick.AddListener(GameButtonOnClick);
    }

    void OnDisable()
    {
        focused = false;
        updateSelected = false;

        button.onClick.RemoveAllListeners();
    }

    public void InitButton()
    {
        button = GetComponent<Button>();
        images = GetComponentsInChildren<Image>(true);
        imageBackground = images[0]; // Assuming the fisrt image is the button background
        imageIcon = images.Length > 1 ? images[1] : null; // Assuming the second image is the icon
        texts = GetComponentsInChildren<Text>(true);
        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    public void ShowButton()
    {
        if (texts.Length > 0 && title_name != TitleName.None)
            texts[0].text = GameManager.Data.TitleCSV.GetString(title_name.ToString(), GameManager.Data.CurLan);

        SetTextStyle(false);
        SetIconColor(false);
    }

    void GameButtonOnClick()
    {
        switch (title_name)
        {
            //for buttons
            case TitleName.StartGame:
                GameManager.StartNewRun();
                break;
            case TitleName.Quit:
                Application.Quit();
                break;
            case TitleName.Continue:
                if (UIController.instance != null && UIController.instance.CurPanel == PanelName.PanelPause)
                    UIController.instance.ContinueGame();
                else
                    GameManager.ResumeSavedRun();
                break;
            case TitleName.GoTitle:
                GameManager.SaveCurrentRun();
                SetGameStatus(GameStatus.Loaded);
                GameManager.SwitchScene(SceneName.Menu);
                break;
            case TitleName.ResetInput:
                if (UIController.instance.CurSubPanel == SubPanelName.Keyboard)
                    GameManager.NewInput.ResetControlSchemeBinding("Keyboard&Mouse");
                if (UIController.instance.CurSubPanel == SubPanelName.Gamepad)
                    GameManager.NewInput.ResetControlSchemeBinding("Gamepad");
                break;
            case TitleName.CompletePreset:
                GameManager.Data.ChangeSaveData(OptionName.PresetProgress, (int)DataStatus.HasPresetData);
                GameManager.Data.SaveGame();
                GameManager.SwitchScene(SceneName.Menu);
                break;
            default:
                break;

            //for panels
            case TitleName.Option:
                UIController.instance.OpenPanel(PanelName.PanelOption);
                break;
            case TitleName.Credits:
                UIController.instance.OpenPanel(PanelName.PanelCredits);
                break;
            case TitleName.ClosePanel:
                UIController.instance.CloseCurPanel();
                break;

            //for subpanels
            case TitleName.General:
                UIController.instance.OpenSubPanel(SubPanelName.General);
                break;
            case TitleName.Graphics:
                UIController.instance.OpenSubPanel(SubPanelName.Graphics);
                break;
            case TitleName.Sound:
                UIController.instance.OpenSubPanel(SubPanelName.Sound);
                break;
            case TitleName.Keyboard:
                UIController.instance.OpenSubPanel(SubPanelName.Keyboard);
                break;
            case TitleName.Gamepad:
                UIController.instance.OpenSubPanel(SubPanelName.Gamepad);
                break;
            case TitleName.CloseSubPanel:
                UIController.instance.CloseSubPanel();
                break;
        }
    }

    public void SetButtonID(int m_id)
    {
        buttonID = m_id;
    }

    public void SetInteractable(bool interact)
    {
        button.interactable = interact;
        canvasGroup.alpha = interact ? 1f : 0.2f;
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnFocused();
        PlayButtonSFX();
        UIController.instance.SetCurBtnID(buttonID);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        updateSelected = false;
        OnUnfocused();
    }

    //OnUpdateSelected 虽然只调用一次，但不能删掉，用于补全按钮在某些情况下不会触发OnSelect的情况
    public void OnUpdateSelected(BaseEventData eventData)
    {
        if (updateSelected)
            return;
        updateSelected = true;
        OnFocused();
        UIController.instance.SetCurBtnID(buttonID);
    }

    #region button style
    public void OnFocused()
    {
        focused = true;
        SetTextStyle(focused);
        SetIconColor(focused);
    }

    public void OnUnfocused()
    {
        focused = false;
        SetTextStyle(focused);
        SetIconColor(focused);
    }

    protected void SetIconColor(bool focused)
    {
        if (imageIcon)
            imageIcon.color = focused ? GameManager.Data.OptionData.colorType.dark : GameManager.Data.OptionData.colorType.light;
    }

    protected void SetTextStyle(bool focused)
    {
        foreach (var txt in texts)
        {
            txt.color = focused ? GameManager.Data.OptionData.colorType.dark : GameManager.Data.OptionData.colorType.light;
            int curFontSize = GameManager.Data.SaveCSV.GetSettingsInt(OptionName.FontSize);
            txt.fontSize = GameManager.Data.OptionData.GetFontSize(curFontSize).subSize;
        }
    }

    void PlayButtonSFX()
    {
        if (title_name == TitleName.ClosePanel || title_name == TitleName.CloseSubPanel || title_name == TitleName.Quit)
        {
            // GameManager.Audio.PlaySound(1); //尚未添加音效文件，打开Unity，找到Scripts/ScriptableObj/AudioData添加音效
        }
        else
        {
            // GameManager.Audio.PlaySound(0); //尚未添加音效文件，打开Unity，找到Scripts/ScriptableObj/AudioData添加音效
        }
    }
    #endregion button style 
}


