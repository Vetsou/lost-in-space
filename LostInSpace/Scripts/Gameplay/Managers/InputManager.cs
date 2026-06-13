using Godot;
using LostInSpace.Scripts.UI.Scenes.Settings;

namespace LostInSpace.Scripts.Gameplay.Managers;

public static class InputManager
{
	public static bool GetPlayerMovementDirection(InputEvent @event, out Vector2I direction)
	{
		if (@event.IsActionPressed(SettingsMap.Keys.MOVE_UP))
		{
			direction = Vector2I.Up;
			return true;
		}

		if (@event.IsActionPressed(SettingsMap.Keys.MOVE_DOWN))
		{
			direction = Vector2I.Down;
			return true;
		}

		if (@event.IsActionPressed(SettingsMap.Keys.MOVE_LEFT))
		{
			direction = Vector2I.Left;
			return true;
		}

		if (@event.IsActionPressed(SettingsMap.Keys.MOVE_RIGHT))
		{
			direction = Vector2I.Right;
			return true;
		}

		direction = default;
		return false;
	}
}
