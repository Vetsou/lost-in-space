using Godot;

namespace LostInSpace.Scripts.UI.Scenes.Settings;

public enum Languagues
{
	en = 0,
	pl = 1
}

public partial class VideoPanel : Control
{
	[Export]
	private CheckBox FullscreenToggle { get; set; }
	[Export]
	private OptionButton LanguageSelector { get; set; }

	public override void _Ready()
	{
		FullscreenToggle.ButtonPressed = ConfigManager.Instance.GetSetting<bool>(SettingsMap.Section.VIDEO, SettingsMap.Video.FULLSCREEN, false);
		LanguageSelector.Selected = ConfigManager.Instance.GetSetting<int>(SettingsMap.Section.VIDEO, SettingsMap.Video.LANGUAGE, 0);
		ConfigManager.Instance.Connect(ConfigManager.SignalName.SettingChanged, Callable.From<string, string, Variant>(UpdateVideoSettings));
	}

	private static void OnFullscreenToggled(bool toggledOn) =>
		ConfigManager.Instance.SetSetting(SettingsMap.Section.VIDEO, SettingsMap.Video.FULLSCREEN, toggledOn);

	private static void OnLanguageChanged(int langId) =>
		ConfigManager.Instance.SetSetting(SettingsMap.Section.VIDEO, SettingsMap.Video.LANGUAGE, langId);

	private void UpdateVideoSettings(string section, string key, Variant value)
	{
		if (key == SettingsMap.Video.FULLSCREEN)
		{
			FullscreenToggle.ButtonPressed = value.As<bool>();
		}

	}
}
