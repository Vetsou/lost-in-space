using Godot;

public partial class AudioPanel : Control
{
	[Export]
	private HSlider MasterVolumeSlider { get; set; }

	public override void _Ready()
	{
		MasterVolumeSlider.Value = (double)SettingsManager.Instance.GetSetting(SettingsMap.Section.AUDIO, SettingsMap.Audio.MASTER_VOLUME);
		SettingsManager.Instance.Connect(SettingsManager.SignalName.SettingChanged, Callable.From<string, string, Variant>(UpdateAudioSettings));
	}

	private static void OnMasterVolumeChanged(float value) =>
		SettingsManager.Instance.SetSetting(SettingsMap.Section.AUDIO, SettingsMap.Audio.MASTER_VOLUME, value);

	private void UpdateAudioSettings(string section, string _, Variant value)
	{
		if (section != SettingsMap.Section.AUDIO)
		{
			return;
		}

		MasterVolumeSlider.Value = value.As<double>();
	}
}
