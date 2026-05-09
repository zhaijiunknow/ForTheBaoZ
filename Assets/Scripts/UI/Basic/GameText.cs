using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class GameText : MonoBehaviour
{
    [SerializeField] TitleName title_name;
    [SerializeField] bool isTitle;
    [SerializeField] bool darkColor;
    Text text;

    void Awake()
    {
        text = gameObject.GetComponent<Text>();
    }

    void OnEnable()
    {
        ShowText();
    }

    public void ShowText()
    {
        if (text)
        {
            if (title_name != TitleName.None)
                text.text = GameManager.Data.TitleCSV.GetString(title_name.ToString(), GameManager.Data.CurLan);

            text.color = darkColor ? GameManager.Data.OptionData.colorType.dark : GameManager.Data.OptionData.colorType.light;
            int fontSize = GameManager.Data.SaveCSV.GetSettingsInt(OptionName.FontSize);
            text.fontSize = isTitle ? GameManager.Data.OptionData.GetFontSize(fontSize).titleSize : GameManager.Data.OptionData.GetFontSize(fontSize).subSize;
        }
    }
}
