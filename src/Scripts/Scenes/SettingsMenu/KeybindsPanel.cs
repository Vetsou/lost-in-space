using System.Collections.Generic;
using Godot;

public partial class KeybindsPanel : Control
{
	[Export] private Button _moveUpButton;
	[Export] private Button _moveDownButton;
	[Export] private Button _moveLeftButton;
	[Export] private Button _moveRightButton;

	private string WaitingForAction { get; set; } = null;
	private Godot.Collections.Dictionary<string, Button> _actionButtons;

	public override void _Ready()
	{
		_actionButtons = new()
		{
			{ SettingsMap.Keys.MOVE_UP, _moveUpButton },
			{ SettingsMap.Keys.MOVE_DOWN, _moveDownButton },
			{ SettingsMap.Keys.MOVE_LEFT, _moveLeftButton },
			{ SettingsMap.Keys.MOVE_RIGHT, _moveRightButton },
		};

		foreach (KeyValuePair<string, Button> kvp in _actionButtons)
		{
			string action = kvp.Key;
			Button button = kvp.Value;

			button.Text = (string)SettingsManager.Instance.GetSetting(SettingsMap.Section.KEYBINDS, action);
			button.Pressed += () => StartRebind(action);
		}

		SettingsManager.Instance.Connect(SettingsManager.SignalName.SettingChanged, Callable.From<string, string, Variant>(UpdateButtonText));
	}

	private void StartRebind(string action)
	{
		if (!string.IsNullOrEmpty(WaitingForAction))
		{
			return;
		}

		WaitingForAction = action;

		Button actionButton = _actionButtons[action];
		actionButton.Text = "Press a key...";
		actionButton.Disabled = true;

		SetProcessInput(true);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (keyEvent.Keycode == Key.Escape)
			{
				EndRebind();
				return;
			}

			FinishRebind(keyEvent);
		}
	}

	private void FinishRebind(InputEventKey keyEvent)
	{
		string keyName = OS.GetKeycodeString(keyEvent.Keycode);
		SettingsManager.Instance.SetSetting(SettingsMap.Section.KEYBINDS, WaitingForAction, keyName);
		EndRebind();
	}

	private void EndRebind()
	{
		Button button = _actionButtons[WaitingForAction];
		button.Disabled = false;

		WaitingForAction = null;
		SetProcessInput(false);
	}

	private void UpdateButtonText(string section, string key, Variant value)
	{
		if (section != SettingsMap.Section.KEYBINDS)
		{
			return;
		}

		if (!_actionButtons.TryGetValue(key, out Button button))
		{
			return;
		}

		button.Text = value.As<string>();
	}
}
