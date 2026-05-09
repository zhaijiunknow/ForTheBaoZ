using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;

public static class GameConfig
{
    public const int SettingsCol = 1;
}

public static class GameEvent
{
    //GameManager
    public static UnityEvent<int, int> ManagerProgress = new UnityEvent<int, int>();

    //Save
    public static UnityEvent DataSaved = new UnityEvent();

    //Rebind
    public static UnityEvent RebindingStarted = new UnityEvent();
    public static UnityEvent RebindingCompleted = new UnityEvent();

}
