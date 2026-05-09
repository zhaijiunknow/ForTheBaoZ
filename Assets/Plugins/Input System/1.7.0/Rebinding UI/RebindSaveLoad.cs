using UnityEngine;
using UnityEngine.InputSystem;

public class RebindSaveLoad : MonoBehaviour
{
    public InputActionAsset actions;


    public void OnEnable()
    {
        print("rebind enabled");

        var rebinds = PlayerPrefs.GetString("player_input");
        if (!string.IsNullOrEmpty(rebinds))
        {
            actions.LoadBindingOverridesFromJson(rebinds);
        }
    }

    public void OnDisable()
    {
        print("rebind disabled");
        var rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("player_input", rebinds);
    }

}
