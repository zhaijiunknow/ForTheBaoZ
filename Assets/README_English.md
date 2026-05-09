## NOTE
This menu toolkit is designed for small or new projects. Please back up your project before making any changes.

## Features
This menu toolkit can save you time on the following tasks:
1. Language switching: Easily switch between multiple languages.
2. Key binding: Customize key bindings for player controls.
3. Input hint: Display input hints dynamically.
4. Volume control: Adjust audio settings for the game.
5. Font style: Change font styles for UI elements.
6. Game pause: Implement a pause menu for the game.
7. Simple save and load: Save and load game progress.
8. Game preset: Set up default game configurations.
9. UI navigation: Navigate through menus with ease.

## Version
- Unity version: 2022.3.40f1c1
- Input System: 1.7 (Installing Input System 1.7 is necessary.)

## Getting Started
1. Install the Input System (via Package Manager > Unity Registry).
2. Import the `MenuToolkit` package and add all scenes to Build Settings (File > Build Settings).
3. Run the project from the `StartUp` scene.

## Attention
1. Since the `GameManager` singleton is located in the `StartUp` scene, you must always run the project from this scene.
2. Do not remove any enums in `EnumUI.cs`, as this will break the selection sequence in the Inspector.
3. Remove `GetSaveFileLocation()` method in `DataManager.cs` before publishing the game.

## Resources
- Input Icons: Input Prompts (1.1) created/distributed by Kenney (www.kenney.nl). Refer to the License in the Asset/Resources/Texture/Input folder.

## References
- Input System Built-in Samples: Includes In-game Hint and Rebinding UI.
- Game Framework: Based on Unity Action 5 for `GameManager.cs` and `AudioManager.cs`.

## License
- MenuToolkit is modified and created by MoonWhiteStudio.
- License: Creative Commons Zero (CC0).
- Website: https://www.bilibili.com/video/BV15Z421Y7ha/


### How to customize buttons
1. Change ButtonNormal, ButtonOption, BindingItem prefabs
2. Change OnFocused(), OnUnfocused() in GameButton.cs, ButtonOption.cs


### How to set the first selected panel or button
1.UI Controller / firstSelectedPanel 
2.GamePanel / firstSelectedButton 
3.GameSubPanel / firstSelectedButton 
-> Attention:If you are setting firstSelectedButton for SubpanelGamepad or SubpanelKeyboard, you need to select ButtonBinding under the BindingItem, but not the BindingItem.
