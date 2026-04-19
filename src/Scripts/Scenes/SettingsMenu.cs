using System;
using Godot;

public enum SettingsTab
{
	Audio,
	Video,
	Keybinds,
}

public partial class SettingsMenu : Scene
{
	[Export] private Control _audioPanel;
	[Export] private Control _videoPanel;
	[Export] private Control _keybindsPanel;

	private Godot.Collections.Dictionary<SettingsTab, Control> _panels;

	public override void _Ready()
	{
		_panels = new Godot.Collections.Dictionary<SettingsTab, Control>
		{
			{ SettingsTab.Audio, _audioPanel },
			{ SettingsTab.Video, _videoPanel },
			{ SettingsTab.Keybinds, _keybindsPanel }
		};

		ChangePanel(SettingsTab.Video);
	}

	private void OnVideoCategoryPressed() => ChangePanel(SettingsTab.Video);
	private void OnAudioCategoryPressed() => ChangePanel(SettingsTab.Audio);
	private void OnKeybindsCategoryPressed() => ChangePanel(SettingsTab.Keybinds);

	private void ChangePanel(SettingsTab tab)
	{
		foreach (Control panel in _panels.Values)
		{
			panel.Visible = false;
		}

		_panels[tab].Visible = true;
	}

	private void OnBackPressed() => ChangeScene(SceneId.MAIN_MENU);
}
