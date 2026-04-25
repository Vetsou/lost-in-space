using Godot;

public enum SettingsTab
{
	Audio,
	Video,
	Keybinds,
}

public partial class SettingsMenu : Scene
{
	[Export] private AudioPanel _audioPanel;
	[Export] private VideoPanel _videoPanel;
	[Export] private KeybindsPanel _keybindsPanel;

	[Export]
	private Godot.Collections.Array<Button> _settingsButtons;

	private Godot.Collections.Dictionary<SettingsTab, Control> _panels;

	public override void _Ready()
	{
		_panels = new Godot.Collections.Dictionary<SettingsTab, Control>
		{
			{ SettingsTab.Audio, _audioPanel },
			{ SettingsTab.Video, _videoPanel },
			{ SettingsTab.Keybinds, _keybindsPanel }
		};

		_keybindsPanel.Connect(KeybindsPanel.SignalName.RebindStateChanged, Callable.From<bool>(OnRebindStateChanged));
		ChangePanel(SettingsTab.Video);
	}

	private void OnVideoCategoryPressed() => ChangePanel(SettingsTab.Video);
	private void OnAudioCategoryPressed() => ChangePanel(SettingsTab.Audio);
	private void OnKeybindsCategoryPressed() => ChangePanel(SettingsTab.Keybinds);
	private static void OnResetButtonPressed() => ConfigManager.Instance.ResetToDefault();

	private void ChangePanel(SettingsTab tab)
	{
		foreach (Control panel in _panels.Values)
		{
			panel.Visible = false;
		}

		_panels[tab].Visible = true;
	}

	private void OnBackPressed()
	{
		ConfigManager.Instance.SaveSettings();
		ConfigManager.Instance.ApplySettings();
		ChangeScene(SceneId.MainMenu);
	}

	private void OnRebindStateChanged(bool active)
	{
		foreach (Button btn in _settingsButtons)
		{
			btn.Disabled = active;
		}
	}
}
