using Godot;

public partial class VideoPanel : Control
{
	[Export]
	private CheckBox FullscreenToggle { get; set; }

	public override void _Ready() =>
		FullscreenToggle.ButtonPressed = (bool)SettingsManager.Instance.GetSetting(SettingsMap.Section.VIDEO, SettingsMap.Video.FULLSCREEN, false);

	private static void OnFullscreenToggled(bool toggledOn) =>
		SettingsManager.Instance.SetSetting(SettingsMap.Section.VIDEO, SettingsMap.Video.FULLSCREEN, toggledOn);
}
