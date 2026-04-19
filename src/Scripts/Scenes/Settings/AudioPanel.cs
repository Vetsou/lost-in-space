using Godot;

public partial class AudioPanel : Control
{
	[Export]
	private HSlider MasterVolumeSlider { get; set; }

	public override void _Ready() =>
		MasterVolumeSlider.Value = (double)SettingsManager.Instance.GetSetting(SettingsMap.Section.AUDIO, SettingsMap.Audio.MASTER_VOLUME);

	private static void OnMasterVolumeChanged(float value) =>
		SettingsManager.Instance.SetSetting(SettingsMap.Section.AUDIO, SettingsMap.Audio.MASTER_VOLUME, value);
}
