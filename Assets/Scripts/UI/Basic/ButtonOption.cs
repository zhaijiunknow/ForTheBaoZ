using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static GameManager;
using System.Collections.Generic;

public class ButtonOption : MonoBehaviour, ISelectHandler, IDeselectHandler, IUpdateSelectedHandler
{
    public OptionName optionName;
    public Text textOptionTitle;
    public Text textOptionContent;
    Text[] texts;
    Slider slider;
    Button button;

    bool inited, focused, updateSelected;

    //option value
    OptionContent optionContent;
    List<string> optionContentNames;
    int curValue, optionMinValue, optionMaxValue, optionAmount;

    void Awake()
    {
        InitBtnOption();
    }

    void Update()
    {
        if (NewInput.UIRight)
            NextOption();
        else if (NewInput.UILeft)
            PreviousOption();
    }

    void OnEnable()
    {
        if (slider)
            slider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
        else
            button.onClick.AddListener(delegate () { OnButtonClick(); });
        ShowBtnOption();
        inited = true;
    }

    void OnDisable()
    {
        updateSelected = false;
        focused = false;
        if (button)
            button.onClick.RemoveAllListeners();
        if (slider)
            slider.onValueChanged.RemoveAllListeners();
    }

    public void InitBtnOption()
    {
        optionContent = Data.OptionBundle.GetOptionContent(optionName);
        curValue = Data.SaveCSV.GetSettingsInt(optionName);

        texts = GetComponentsInChildren<Text>(true);
        slider = GetComponentInChildren<Slider>();
        button = GetComponentInChildren<Button>();

        //设置选项的最大值和最小值
        if (slider)
        {
            switch (optionName)
            {
                case OptionName.MasterVolume:
                case OptionName.Music:
                case OptionName.SoundEffect:
                    optionMinValue = 0;
                    optionMaxValue = Audio.MaxVolume;
                    slider.maxValue = optionMaxValue;
                    break;
                default:
                    break;
            }
        }
        else
        {
            optionAmount = optionContent.GetOptionValuesLength();
            optionMinValue = optionName == OptionName.Lan ? 1 : 0; //如果是语言，其最小值为1(English)，其他情况为0
            optionMaxValue = optionAmount - 1;
        }
    }

    public void ShowBtnOption()
    {
        if (slider)
            UpdateSliderValue();
        else
        {
            optionContentNames = optionContent.GetOptionValuesByLan(Data.CurLan);
            UpdateSelectValue();
        }
        textOptionTitle.text = Data.OptionCSV.GetString(optionName.ToString(), Data.CurLan);
    }

    void PreviousOption()
    {
        if (!UIController.instance.IsCurOption(optionName))
            return;

        if (slider)
        {
            curValue = curValue - 1 < optionMinValue ? optionMinValue : curValue - 1;
            UpdateSliderValue(); //ChangeOption()会发生在OnSliderValueChanged()中，此处不必添加，这是slider特性
        }
        else
        {
            curValue = curValue - 1 < optionMinValue ? optionMaxValue : curValue - 1;
            UpdateSelectValue();
            ChangeOption();
        }
    }

    void NextOption()
    {
        if (!UIController.instance.IsCurOption(optionName))
            return;


        if (slider)
        {
            curValue = curValue + 1 > optionMaxValue ? optionMaxValue : curValue + 1;
            UpdateSliderValue();  //ChangeOption()会发生在OnSliderValueChanged()中，此处不必添加，这是slider特性
        }
        else
        {
            curValue = curValue + 1 > optionMaxValue ? optionMinValue : curValue + 1;
            UpdateSelectValue();
            ChangeOption();
        }
    }

    void UpdateSliderValue()
    {
        slider.value = curValue;
        textOptionContent.text = "[ " + curValue.ToString() + " ]";
    }

    void UpdateSelectValue()
    {
        textOptionContent.text = "[ " + optionContentNames[curValue] + " ]";
    }

    void ChangeOption()
    {
        PlaySFX();

        //修改数据，尚未保存，关闭界面时才会保存 -> UIController.cs / CloseSubPanel()
        Data.ChangeSaveData(optionName, curValue);

        //如须立刻生效，比如全屏，音量等，将方法放在此处；如非立刻生效，跳过此步骤
        switch (optionName)
        {
            case OptionName.MasterVolume:
            case OptionName.Music:
            case OptionName.SoundEffect:
                Audio.SetVolume(optionName, curValue);
                break;
            case OptionName.Fullscreen:
                Data.SwitchFullscreen(curValue);
                break;
            default:
                break;
        }
    }

    //Event
    public void OnSliderValueChanged()
    {
        //暂时解决调整handler失焦的问题
        if (!UIController.instance.IsCurOption(optionName))
        {
            OnFocused();
            UIController.instance.SetCurOption(optionName);
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
        curValue = (int)slider.value;
        textOptionContent.text = "[ " + curValue + " ]";
        ChangeOption();
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnFocused();
        UIController.instance.SetCurOption(optionName);
        PlaySFX();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        updateSelected = false;
        OnUnfocused();
    }

    //OnUpdateSelected 虽然只调用一次，但不能删掉，用于补全按钮在某些情况下不会触发OnSelect事件的情况
    public void OnUpdateSelected(BaseEventData eventData)
    {
        if (updateSelected)
            return;
        updateSelected = true;
        OnFocused();
        UIController.instance.SetCurOption(optionName);
    }

    void OnButtonClick()
    {
        NextOption();
    }

    #region button style
    public void OnFocused()
    {
        focused = true;
        SetTextStyle(focused);
    }

    public void OnUnfocused()
    {
        focused = false;
        SetTextStyle(focused);
    }

    protected void SetTextStyle(bool isFocused)
    {
        foreach (var txt in texts)
        {
            txt.color = isFocused ? Data.OptionData.colorType.dark : Data.OptionData.colorType.light;
        }
    }

    void PlaySFX()
    {
        if (inited)
        {
            //Audio.PlaySound(0); //尚未添加音效文件，打开Unity，找到Scripts/ScriptableObj/AudioData添加音效
        }
    }

    #endregion button style 
}
