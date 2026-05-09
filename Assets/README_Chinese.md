## 前言
本工具适用于小型或全新项目。使用前请先备份你的项目。

## 功能
本工具包含以下功能：
1. 多语言切换
2. 按键绑定（键盘/手柄）
3. 按键提示（键盘/手柄）
4. 音量控制
5. 字体样式
6. 游戏暂停
7. 简单保存读取
8. 游戏预设
9. UI导航

## 版本
- Unity 版本：2022.3.40f1c1  
- Input System 1.7  

## 如何使用
1. 安装 Input System（Package Manager > Unity Registry）
2. 导入 `MenuToolkit` 包，将所有场景添加到 Build Settings（File > Build Settings）
3. 从 `StartUp` 场景运行项目

## 注意事项
1. 由于 `GameManager` 是一个跨场景单例，必须从 `StartUp` 场景运行游戏
2. 不要删除 `EnumUI.cs` 中的枚举，否则会打乱 Inspector 面板中的选项顺序
3. 运行游戏后，按下等号键可快速定位存档位置（仅用于测试）。发布游戏前，请删除 `DataManager.cs` 中的 `GetSaveFileLocation()` 方法

## 使用素材
- Input Icons: Input Prompts (1.1) created/distributed by Kenney (www.kenney.nl) 许可证详见 `Asset/Resources/Texture/Input` 文件夹的License文件

## 参考代码
- 按键绑定和按键提示代码修改自Input System自带案例（In-game Hint 和 Rebinding UI）
- 游戏框架代码`GameManager.cs` 和 `AudioManager.cs` 参考了《Unity Action 5》中的游戏框架写法

## License
- MenuToolkit 由 MoonWhiteStudio 创建  
- License：Creative Commons Zero (CC0) 
- MoonWhiteStudio Website：https://www.bilibili.com/video/BV15Z421Y7ha/

## 教程
### 添加新的Option选项（如语言>法语）
1. 添加Option选项枚举：Scripts/Configs/enumSettings.cs中添加新的Language枚举变量，例如French
2. 添加选项的语言翻译：StreamingAssets/options.csv语言下方添加新行，并将“end”移到新增语言前
3. 为所有存在翻译的csv文件添加新的一列，添加对应语言翻译
4. 测试前，把旧的存档删除（根据DataManager.cs / GetSaveFileLocation()找到save.csv，删除即可）

### 添加新的Option （如全屏）
数据部分
1. 添加OptionName枚举：在Scripts/Configs/enumUI.cs文件中添加新的OptionName
2. 添加Option选项枚举：在Scripts/Configs/enumSettings.cs文件中添加，如为已存在枚举（如On/Off）可跳过此步骤
3. 添加名称及选项的语言翻译：在StreamingAssets/options.csv中，仿照已有格式，添加对应的OptionName和选项
4. 添加存档模板的选项栏：StreamingAssets/defaultSave.csv，仿照已有格式添加

方法部分
如果设定需要立刻生效（如音量，全屏），在ButtonOption.cs / ChangeOption()的switch方法中添加新的OptionName case
如设定不是立刻生效（如震动，字体大小），可跳过此步骤

Unity部分
1. 打开SubpanelGraphics.prefab，添加ButtonOption.prefab，或拷贝一个现有的ButtonOption
2. 选中ButtonOption，在Inspector面板中选择新增的OptionName（如OptionName.Fullscreen）
3. 测试前，把旧的存档删除（根据DataManager.cs / GetSaveFileLocation()找到存档，删除save.csv即可）


### 添加新的按键/按键绑定/按键提示
新的按键
1. 配置按键：InputController配置表中添加按键并保存（键盘按键勾选Keyboard&Mouse，手柄按键勾选Gamepad），避免重名
2. 添加引用：InputManager.cs中按注释添加按键的引用
3. 添加翻译：在StreamingAssets/title.csv中添加按键的各个语言翻译
实际使用时，调用GameManager.NewInput.xxAction即可

按键绑定
1. 在Resources/Prefab文件夹找到SubPanelKeyboard prefab 和SubpanelGamepad prefab 
2. 添加新的BindingItem，配置对应的Action和Binding

按键提示
1. 添加图标
本工具已经在InputData文件中配置了键盘a-z，1-9，以及PS / Xbox手柄的图标；如果该按键未配置过图标，只需打开InputData_SO，遵循注释添加图标引用；回到Unity找到InputData文件，挂上对应图标即可
2. 添加提示
在所需的GamePanel中添加InputItemGroup prefab，在InputItem选择对应按键即可完成按键提示
（注意InputItemGroup需要放在任意GamePanel中）


###  添加新的GamePanel/GameButton
数据部分
1. 添加PanelName：在Scripts/Configs/EnumUI.cs添加新的PanelName
2. 添加Button的TitleName(如需要Button)：在Scripts/Configs/EnumUI.cs添加新的TitleName
3. 添加翻译：在StreamingAssets/title.csv中添加TitleName的各个语言翻译

Unity部分
1. 添加Panel：Resources/Prefab/Panel路径下，复制一个GamePanel prefab，修改PanelName，选择firstSelectedButton（可留空），拖入场景的某个Canvas。
- 注意不可在场景中直接复制其他GamePanel。
- 如须打开场景时便显示该Panel，设置UI Controller的firstSelectedPanel（UI Controller在场景中的GameController对象上）

2. 添加Button（用于打开Panel）：在另一个Panel中，拖入一个ButtonNormal prefab，在Inspector面板中修改TitleName

方法部分
1. 添加按钮方法：双击按钮上挂载的GameButton脚本，设置按钮点击事件，点击后打开Panel
    case TitleName.XX:
      UIController.instance.OpenPanel(PanelName.XX);
      break;


### 自定义按钮样式
ButtonNormal, ButtonOption, BindingItem是所有按钮相关的预制体，可以统一修改其悬浮效果
GameButton.cs, ButtonOption.cs里的OnFocused(), OnUnfocused()方法可以设置文字颜色，添加音效等


### 如何设置聚焦按钮
1.UI Controller / firstSelectedPanel 设置场景默认显示GamePanel，可留空，留空则隐藏所有GamePanel
2.GamePanel / firstSelectedButton 设置GamePanel默认聚焦按钮，可留空
3.GameSubPanel / firstSelectedButton 设置GameSubPanel默认聚焦按钮，可留空
-> 特别注意，如果是为SubpanelGamepad和SubpanelKeyboard添加firstSelectedButton，需要挂载的是BindingItem子级的ButtonBinding，而不是BindingItem


### 如何保存
返回游戏标题时：GameButton.cs / GameButtonOnClick() / case TitleName.GoTitle: 
设置修改完成，关闭SubPanel时：UIController.cs / CloseSubPanel() / GameManager.Data.SaveGame();


### 如何设置游戏状态
GameButton.cs / SetGameStatus()
暂停游戏时，时间停止，切换为UI的Input Map（此时角色移动跳跃将不会生效）
开始游戏时，时间运动，切换为Player的Input Map

### 更新日志
202604 
将手柄键盘模式切换的按键检测从每帧触发改为事件触发
为DataCSV.cs的GetString添加了Trim方法，自动移除单元格中可能存在的首尾空格

