using Godot;

namespace LostInSpace.Scripts.UI.Scenes.Settings;

public enum Languagues
{
	en = 0,
	pl = 1
}

public partial class GeneralPanel : Control
{
	[Export]
	private OptionButton LanguageSelector { get; set; }

	public override void _Ready()
	{
		LanguageSelector.Selected = ConfigManager.Instance.GetSetting<int>(SettingsMap.Section.GENERAL, SettingsMap.General.LANGUAGE, 0);
		ConfigManager.Instance.Connect(ConfigManager.SignalName.SettingChanged, Callable.From<string, string, Variant>(UpdateGeneralSettings));
	}

	private static void OnLanguageChanged(int langId) =>
		ConfigManager.Instance.SetSetting(SettingsMap.Section.GENERAL, SettingsMap.General.LANGUAGE, langId);

	private void UpdateGeneralSettings(string section, string key, Variant value)
	{
		if (key != SettingsMap.Section.GENERAL)
		{
			return;
		}

		switch (key)
		{
			case SettingsMap.General.LANGUAGE:
				LanguageSelector.Selected = value.As<int>();
				break;
		}
	}
}
