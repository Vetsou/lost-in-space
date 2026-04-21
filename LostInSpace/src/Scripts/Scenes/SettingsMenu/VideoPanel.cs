using Godot;

public partial class VideoPanel : Control
{
	[Export]
	private CheckBox FullscreenToggle { get; set; }

	public override void _Ready()
	{
		FullscreenToggle.ButtonPressed = (bool)SettingsManager.Instance.GetSetting(SettingsMap.Section.VIDEO, SettingsMap.Video.FULLSCREEN, false);
		SettingsManager.Instance.Connect(SettingsManager.SignalName.SettingChanged, Callable.From<string, string, Variant>(UpdateVideoSettings));
	}

	private static void OnFullscreenToggled(bool toggledOn) =>
		SettingsManager.Instance.SetSetting(SettingsMap.Section.VIDEO, SettingsMap.Video.FULLSCREEN, toggledOn);

	private void UpdateVideoSettings(string section, string _, Variant value)
	{
		if (section != SettingsMap.Section.VIDEO)
		{
			return;
		}

		FullscreenToggle.ButtonPressed = value.As<bool>();
	}
}
