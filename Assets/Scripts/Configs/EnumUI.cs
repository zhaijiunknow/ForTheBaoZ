//Attention: Can not remove any existing name cause that will break the select sequence in the Inspector

public enum PanelName
{
    None, PanelSample, PanelPreset, PanelMenu, PanelOption, PanelCredits, PanelPause
}

public enum SubPanelName
{
    None, General, Graphics, Sound, Keyboard, Gamepad
}

public enum TitleName
{
    None, StartGame, Option, Credits, Quit, CloseSubPanel, ClosePanel, Continue, GoTitle, Keyboard, Gamepad, ResetInput, Wait, Language, CompletePreset,
    GameTitle, General, Graphics, Sound, Test, Saved
}

public enum OptionName
{
    None, PresetProgress, LevelProgress, Lan, Vibration, Contrast, FontSize, MasterVolume, Music, SoundEffect, Fullscreen
}
