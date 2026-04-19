using System;
using System.Collections.Generic;
using Godot;

public enum ActionType
{
	MoveUpKey,
	MoveDownKey,
	MoveLeftKey,
	MoveRightKey
}

public partial class KeybindsPanel : Control
{
	[Export] private Button _moveUpButton;
	[Export] private Button _moveDownButton;
	[Export] private Button _moveLeftButton;
	[Export] private Button _moveRightButton;

	private ActionType? WaitingForAction { get; set; } = null;
	private Godot.Collections.Dictionary<ActionType, Button> _actionButtons;

	public override void _Ready()
	{
		_actionButtons = new()
		{
			{ ActionType.MoveUpKey, _moveUpButton },
			{ ActionType.MoveDownKey, _moveDownButton },
			{ ActionType.MoveLeftKey, _moveLeftButton },
			{ ActionType.MoveRightKey, _moveRightButton },
		};

		foreach (KeyValuePair<ActionType, Button> kvp in _actionButtons)
		{
			ActionType action = kvp.Key;
			Button button = kvp.Value;
			button.Pressed += () => StartRebind(action);
		}
	}

	private void StartRebind(ActionType action)
	{
		if (WaitingForAction.HasValue)
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
		// Save setting
		EndRebind();
	}

	private void EndRebind()
	{
		Button button = _actionButtons[(ActionType)WaitingForAction];
		button.Disabled = false;

		WaitingForAction = null;
		SetProcessInput(false);
	}
}
