using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YourManager : MonoBehaviour, IGameManager
{
    public ManagerStatus Status { get; private set; }

    public void Startup()
    {
        //在这里添加Manager所需初始化的数据和方法

        Status = ManagerStatus.Started;
    }
}
