using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputManager : MonoBehaviour, IGameManager
{
    public ManagerStatus Status { get; private set; }

    private PlayerInput playerInput;
    public InputData_SO InputData;

    //如果你需要添加新的action引用，遵循注释(step 1-4)添加即可
    #region Player map
    public bool OpenMenu { get; private set; }
    public Vector2 Movement { get; private set; }
    public bool Jump { get; private set; }
    // public bool YourAction { get; private set; } //step 1

    private InputAction openMenuAction;
    private InputAction moveAction;
    private InputAction jumpAction;
    // private InputAction yourAction; //step 2
    #endregion

    #region UI map
    public Vector2 Navigate { get; private set; }
    public bool UIRight { get; private set; }
    public bool UILeft { get; private set; }
    public bool Cancel { get; private set; }
    public bool Click { get; private set; }

    private InputAction navigateAction;
    private InputAction clickAction;
    private InputAction cancelAction;
    #endregion

    //Get Attributes
    public InputType CurInputType { get; private set; }
    private InputAction anyGamePad, anyGamePadUI, anyKey, anyKeyUI;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        AddAnyKeyEvent();
    }

    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
        RemoveAnyKeyEvent();
    }

    public void Startup()
    {
        LoadBindings();
        InitInputAction();
        Status = ManagerStatus.Started;
    }

    void Update()
    {
        //这里用的是每帧检测，也可以用事件的形式，参考AddAnyKeyEvent()，事件效率更好，看个人习惯和项目规模
        UpdateInputAction();
    }

    void InitInputAction()
    {
        //Player map
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        openMenuAction = playerInput.actions["OpenMenu"];
        // yourAction = playerInput.actions["YourAction"]; //step 3

        //UI map
        cancelAction = playerInput.actions["Cancel"];
        clickAction = playerInput.actions["Click"];
        navigateAction = playerInput.actions.FindActionMap("UI").FindAction("Navigate");
    }

    void UpdateInputAction()
    {
        //Player map
        Movement = moveAction.ReadValue<Vector2>();
        Jump = jumpAction.WasPressedThisFrame();
        OpenMenu = openMenuAction.WasPressedThisFrame();
        // YourAction = yourAction.WasPressedThisFrame(); //step 4

        //UI map
        Cancel = cancelAction.WasPressedThisFrame();
        Navigate = navigateAction.ReadValue<Vector2>();
        UIRight = navigateAction.WasPressedThisFrame() && Navigate.x > 0;
        UILeft = navigateAction.WasPressedThisFrame() && Navigate.x < 0;
        Click = clickAction.WasPressedThisFrame();
    }

    //当CurGameStatus != GameStatus.Started，即UI页面时，调用UI map -> GameManger.cs/SetGameStatus(）
    public void SwitchToUI()
    {
        playerInput.SwitchCurrentActionMap("UI");
    }

    //当CurGameStatus == GameStatus.Started，即游玩时，调用Player map -> GameManger.cs/SetGameStatus(）
    public void SwitchToPlayer()
    {
        playerInput.SwitchCurrentActionMap("Player");
    }

    //保存按键绑定
    public void SaveBindings()
    {
        var rebinds = playerInput.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("player_input", rebinds);
    }

    //读取按键绑定
    public void LoadBindings()
    {
        var rebinds = PlayerPrefs.GetString("player_input");
        if (!string.IsNullOrEmpty(rebinds))
        {
            playerInput.actions.LoadBindingOverridesFromJson(rebinds);
        }
    }

    //重置按键绑定
    public void ResetControlSchemeBinding(string _targetControlScheme)
    {
        foreach (InputActionMap map in playerInput.actions.actionMaps)
        {
            foreach (InputAction action in map.actions)
            {
                action.RemoveBindingOverride(InputBinding.MaskByGroup(_targetControlScheme));
            }
        }
    }

    //获取按键icon
    public Sprite GetBindingIcon(string _actionName)
    {
        string controlScheme = CurInputType == InputType.Keyboard ? "Keyboard&Mouse" : "Gamepad";
        int bindingID = playerInput.actions[_actionName].GetBindingIndex(InputBinding.MaskByGroup(controlScheme));
        string path = playerInput.actions[_actionName].bindings[bindingID].effectivePath;
        return InputData.GetIconSprite(path, CurInputType);
    }

    public Sprite GetBindingIconForMove()
    {
        if (CurInputType == InputType.Keyboard)
            return InputData.MoveKeyboard;
        else
            return InputData.MoveGamePad;
    }

    public Sprite GetBindingIconForNavigate()
    {
        if (CurInputType == InputType.Keyboard)
            return InputData.NaviKeyboard;
        else
            return InputData.NaviGamePad;
    }

    #region InputType switch
    //当在<游戏运行过程>中连接手柄，或断开手柄，会改变CurInputType，改变按键提示
    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad)
        {
            if (change == InputDeviceChange.Added)
            {
                Debug.Log("Gamepad connected: " + device.name);
                if (device.name.Contains("DualShock") || device.name.Contains("DualSense"))
                    SetInputType(InputType.PS);
                else
                    SetInputType(InputType.Xbox);
            }
            else if (change == InputDeviceChange.Removed)
            {
                Debug.Log("Gamepad disconnected: " + device.name);
                SetInputType(InputType.Keyboard);
            }
        }
    }

    //当在<游戏运行过程>中按下键盘/手柄，会改变CurInputType，改变按键提示
    void AddAnyKeyEvent()
    {
        anyGamePad = playerInput.actions["AnyGamepad"];
        anyGamePadUI = playerInput.actions["AnyGamepadUI"];
        anyKey = playerInput.actions["AnyKey"];
        anyKeyUI = playerInput.actions["AnyKeyUI"];

        anyGamePad.performed += ctx => SwitchToGamePad();
        anyGamePadUI.performed += ctx => SwitchToGamePad();
        anyKey.performed += ctx => SwitchToKeyboard();
        anyKeyUI.performed += ctx => SwitchToKeyboard();
    }

    void RemoveAnyKeyEvent()
    {
        anyGamePad.performed -= ctx => SwitchToGamePad();
        anyGamePadUI.performed -= ctx => SwitchToGamePad();
        anyKey.performed -= ctx => SwitchToKeyboard();
        anyKeyUI.performed -= ctx => SwitchToKeyboard();
    }

    void SwitchToKeyboard()
    {
        if (CurInputType != InputType.Keyboard)
        {
            SetInputType(InputType.Keyboard);
        }
    }

    void SwitchToGamePad()
    {
        if (CurInputType == InputType.Keyboard)
        {
            if (Gamepad.current.device.name.Contains("DualShock") || Gamepad.current.device.name.Contains("DualSense"))
                SetInputType(InputType.PS);
            else
                SetInputType(InputType.Xbox);
        }
    }

    void SetInputType(InputType _type)
    {
        CurInputType = _type;
    }

    #endregion








}