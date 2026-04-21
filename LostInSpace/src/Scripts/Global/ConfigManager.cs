using System;
using Godot;

public partial class ConfigManager : Node
{
	[Signal]
	public delegate void SettingChangedEventHandler(string section, string key, Variant value);

	public static ConfigManager Instance { get; private set; }

	private const string SETTINGS_PATH = "user://user_settings.ini";
	private readonly ConfigFile _config = new();
	private bool _isDirty = false;

	#region DefaultSettings
	private static readonly SettingEntry[] _defaultSettings =
	[
		new(SettingsMap.Section.AUDIO, SettingsMap.Audio.MASTER_VOLUME, 1.0f),

		new(SettingsMap.Section.VIDEO, SettingsMap.Video.FULLSCREEN, false),

		new(SettingsMap.Section.KEYBINDS, SettingsMap.Keys.MOVE_LEFT, "A"),
		new(SettingsMap.Section.KEYBINDS, SettingsMap.Keys.MOVE_RIGHT, "D"),
		new(SettingsMap.Section.KEYBINDS, SettingsMap.Keys.MOVE_UP, "W"),
		new(SettingsMap.Section.KEYBINDS, SettingsMap.Keys.MOVE_DOWN, "S")
	];
	#endregion

	#region SettingsCallbacks
	private static readonly System.Collections.Generic.Dictionary<(string, string), Action<Variant>> _applySettingMap = new()
	{
		[(SettingsMap.Section.AUDIO, SettingsMap.Audio.MASTER_VOLUME)] = v =>
			AudioServer.SetBusVolumeDb(0, Mathf.LinearToDb(v.As<float>())),

		[(SettingsMap.Section.VIDEO, SettingsMap.Video.FULLSCREEN)] = v =>
			DisplayServer.WindowSetMode(v.As<bool>()
				? DisplayServer.WindowMode.Fullscreen
				: DisplayServer.WindowMode.Windowed),
	};
	#endregion

	public override void _EnterTree()
	{
		Instance = this;
		LoadSettings();
		ApplySettings();
	}

	public override void _ExitTree()
	{
		SaveSettings();
		Instance = null;
	}

	public Variant GetSetting(string section, string key, Variant defaultValue = default) =>
		_config.GetValue(section, key, defaultValue);

	public void SetSetting(string section, string key, Variant value)
	{
		_config.SetValue(section, key, value);
		_isDirty = true;

		ApplySetting(section, key, value);
		EmitSignal(SignalName.SettingChanged, section, key, value);
	}

	public void ResetToDefault()
	{
		SetDefaultConfig();
		SaveSettings();
		ApplySettings();
	}

	public void SaveSettings()
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

	public void ApplySettings()
	{
		foreach (SettingEntry entry in _defaultSettings)
		{
			Variant value = GetSetting(entry.Section, entry.Key);
			ApplySetting(entry.Section, entry.Key, value);
		}
	}

	private void LoadSettings()
	{
		Error err = _config.Load(SETTINGS_PATH);

		if (err != Error.Ok)
		{
			SetDefaultConfig();
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

	private void SetDefaultConfig()
	{
		foreach (SettingEntry entry in _defaultSettings)
		{
			_config.SetValue(entry.Section, entry.Key, entry.DefaultValue);
			EmitSignal(SignalName.SettingChanged, entry.Section, entry.Key, entry.DefaultValue);
		}

		_isDirty = true;
	}

	private static void ApplySetting(string section, string key, Variant value)
	{
		if (_applySettingMap.TryGetValue((section, key), out Action<Variant> action))
		{
			action(value);
		}
		else if (section == SettingsMap.Section.KEYBINDS)
		{
			ApplyKeybindsSetting(key, value);
		}
		else
		{
			GD.PushWarning($"No callback for {section}:{key}");
		}
	}

	private static void ApplyKeybindsSetting(string action, Variant keyboardKey)
	{
		string keyString = keyboardKey.AsString();
		if (!Enum.TryParse(keyString, true, out Key keyCode))
		{
			GD.PushError($"Invalid key name: {keyString}");
			return;
		}

		if (!InputMap.HasAction(action))
		{
			InputMap.AddAction(action);
		}

		InputMap.ActionEraseEvents(action);
		var ev = new InputEventKey { Keycode = keyCode };
		InputMap.ActionAddEvent(action, ev);
	}

	private readonly struct SettingEntry(string section, string key, Variant value)
	{
		public readonly string Section = section;
		public readonly string Key = key;
		public readonly Variant DefaultValue = value;
	}
}
