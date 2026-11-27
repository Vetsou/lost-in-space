using Godot;

namespace LostInSpace.Scripts.UI.Scenes.Settings;

public enum SettingsTab
{
	General,
	Audio,
	Video,
	Keybinds,
}

public partial class SettingsMenu : Scene
{
	[Export] private GeneralPanel _generalPanel;
	[Export] private AudioPanel _audioPanel;
	[Export] private VideoPanel _videoPanel;
	[Export] private KeybindsPanel _keybindsPanel;

	[Export]
	private Godot.Collections.Array<Button> _settingsButtons;

	private Godot.Collections.Dictionary<SettingsTab, Control> _panels;
	private StringName _pause = "ui_cancel";
	public bool OpenedFromPause { get; set; } = false;

	public override void _Ready()
	{
		_panels = new Godot.Collections.Dictionary<SettingsTab, Control>
		{
			{ SettingsTab.General, _generalPanel},
			{ SettingsTab.Audio, _audioPanel },
			{ SettingsTab.Video, _videoPanel },
			{ SettingsTab.Keybinds, _keybindsPanel }
		};

		_keybindsPanel.Connect(KeybindsPanel.SignalName.RebindStateChanged, Callable.From<bool>(OnRebindStateChanged));
		ChangePanel(SettingsTab.General);
	}

	private void OnGeneralCategoryPressed() => ChangePanel(SettingsTab.General);
	private void OnVideoCategoryPressed() => ChangePanel(SettingsTab.Video);
	private void OnAudioCategoryPressed() => ChangePanel(SettingsTab.Audio);
	private void OnKeybindsCategoryPressed() => ChangePanel(SettingsTab.Keybinds);
	private static void OnResetButtonPressed() => ConfigManager.Instance.ResetToDefault();

	public override void _UnhandledInput(InputEvent @event)
	{
		if(OpenedFromPause && @event.IsActionPressed(_pause))
		{
			OnBackPressed();
			GetViewport().SetInputAsHandled(); 
		}
	}

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
		if(OpenedFromPause)
		{
			QueueFree();
		}
		else
		{
			ChangeScene(SceneId.MainMenu);
		}
		
	}

	private void OnRebindStateChanged(bool active)
	{
		foreach (Button btn in _settingsButtons)
		{
			btn.Disabled = active;
		}
	}
}
