using Godot;
using System;

public static class SettingsSections
{
	public const string AUDIO = "Audio";
	public const string VIDEO = "Video";
	public const string KEYBINDS = "Keybindings";
}

public static class SettingsKeys
{
	public static class Audio
	{
		public const string MASTER_VOLUME = "master_volume";
	}

	public static class Video
	{
		public const string FULLSCREEN = "fullscreen";
	}

	public static class Keys
	{
		public const string MOVE_LEFT = "move_left";
		public const string MOVE_RIGHT = "move_right";
		public const string MOVE_UP = "move_up";
		public const string MOVE_DOWN = "move_down";
	}
}

public partial class SettingsManager : Node
{
	private const string SETTINGS_PATH = "user://user_settings.ini";

	[Signal]
	public delegate void SettingChangedEventHandler(string section, string key, Variant value);

	private readonly ConfigFile _config = new();
	private bool _isDirty = false;

	private readonly SettingEntry[] _defaultSettings =
	[
		new(SettingsSections.AUDIO, SettingsKeys.Audio.MASTER_VOLUME, 1.0f),

		new(SettingsSections.VIDEO, SettingsKeys.Video.FULLSCREEN, true),

		new(SettingsSections.KEYBINDS, SettingsKeys.Keys.MOVE_LEFT, "A"),
		new(SettingsSections.KEYBINDS, SettingsKeys.Keys.MOVE_RIGHT, "D"),
		new(SettingsSections.KEYBINDS, SettingsKeys.Keys.MOVE_UP, "W"),
		new(SettingsSections.KEYBINDS, SettingsKeys.Keys.MOVE_DOWN, "S")
	];

	public override void _Ready()
	{
		LoadSettings();
		ApplySettings();
	}

	public Variant GetSetting(string section, string key, Variant defaultValue = default)
	{
		if (_config.HasSectionKey(section, key))
		{
			return _config.GetValue(section, key);
		}

		if (!defaultValue.VariantType.Equals(Variant.Type.Nil))
		{
			return defaultValue;
		}

		return Variant.CreateFrom<string>(null);
	}

	public void SetSetting(string section, string key, Variant value)
	{
		_config.SetValue(section, key, value);
		_isDirty = true;

		EmitSignal(SignalName.SettingChanged, section, key, value);
		ApplySetting(section, key, value);
	}

	private void LoadSettings()
	{
		Error err = _config.Load(SETTINGS_PATH);

		if (err != Error.Ok)
		{
			CreateDefaultConfig();
			SaveSettings();
		}
		else
		{
			ValidateDefaultConfig();
			SaveSettings();
		}
	}

	private void ValidateDefaultConfig()
	{
		foreach (SettingEntry entry in _defaultSettings)
		{
			if (!_config.HasSectionKey(entry.Section, entry.Key))
			{
				_config.SetValue(entry.Section, entry.Key, entry.DefaultValue);
				_isDirty = true;
			}
		}
	}

	private void CreateDefaultConfig()
	{
		foreach (SettingEntry entry in _defaultSettings)
		{
			_config.SetValue(entry.Section, entry.Key, entry.DefaultValue);
		}

		_isDirty = true;
	}

	private void SaveSettings()
	{
		if (!_isDirty)
		{
			return;
		}

		Error err = _config.Save(SETTINGS_PATH);

		if (err != Error.Ok)
		{
			GD.PushError("Failed to save user settings");
		}

		_isDirty = false;
	}

	private void ApplySettings()
	{
		foreach (SettingEntry entry in _defaultSettings)
		{
			Variant value = GetSetting(entry.Section, entry.Key);
			ApplySetting(entry.Section, entry.Key, value);
		}
	}

	private static void ApplySetting(string section, string key, Variant value)
	{
		switch (section)
		{
			case SettingsSections.AUDIO:
				ApplyAudioSetting(key, value);
				break;

			case SettingsSections.VIDEO:
				ApplyVideoSetting(key, value);
				break;

			case SettingsSections.KEYBINDS:
				ApplyKeybindsSetting(key, value);
				break;
		}
	}

	private static void ApplyAudioSetting(string key, Variant value)
	{
		switch (key)
		{
			case SettingsKeys.Audio.MASTER_VOLUME:
				float volume = value.As<float>();
				AudioServer.SetBusVolumeDb(0, Mathf.LinearToDb(volume));
				break;
		}
	}

	private static void ApplyVideoSetting(string key, Variant setting)
	{
		switch (key)
		{
			case SettingsKeys.Video.FULLSCREEN:
				DisplayServer.WindowSetMode(
					setting.As<bool>()
						? DisplayServer.WindowMode.Fullscreen
						: DisplayServer.WindowMode.Windowed
				);
				break;
		}
	}

	private static void ApplyKeybindsSetting(string action, Variant keyboardKey)
	{
		string keyString = keyboardKey.AsString();

		if (!InputMap.HasAction(action))
		{
			InputMap.AddAction(action);
		}

		InputMap.ActionEraseEvents(action);
		var ev = new InputEventKey
		{
			Keycode = OS.FindKeycodeFromString(keyString)
		};

		InputMap.ActionAddEvent(action, ev);
	}

	private struct SettingEntry(string section, string key, Variant value)
	{
		public string Section = section;
		public string Key = key;
		public Variant DefaultValue = value;
	}
}
