using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static GameManager;

public class RebindActionLable : MonoBehaviour
{
    public Text TextActionName;
    public Image ImageIcon;

    string action_name;

    void Awake()
    {
        action_name = TextActionName.text;
    }

    void OnEnable()
    {
        TextActionName.text = GameManager.Data.TitleCSV.GetString(action_name, GameManager.Data.CurLan);
    }

}
