using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Gameplay.Platforms;
using LostInSpace.Scripts.UI.Scenes.Settings;

namespace LostInSpace.Scripts.Gameplay.Systems;

public class MovementSystem(ILevelHandler level)
{
	private ILevelHandler CurrentLevel { get; init; } = level;

	public void HandlePlayerMovement(PlayerData player, InputEvent @event)
	{
		if (!GetPlayerMovementDirection(@event, out Vector2I direction))
		{
			return;
		}

		MovePlayer(player, direction);
	}

	public void MovePlayer(PlayerData player, Vector2I direction)
	{
		IPlatform nextTile = CurrentLevel.GetPlatform(player.GridPosition + direction);

		if (nextTile == null)
		{
			return;
		}

		IPlatform currPlatform = CurrentLevel.GetPlatform(player.GridPosition);
		currPlatform.OnExit(CreateTileContext(player.GridPosition, direction));

		player.SetPosition(player.GridPosition + direction);
		CurrentLevel.PickUpCollectible(player.GridPosition);

		IPlatform newPlatform = CurrentLevel.GetPlatform(player.GridPosition);
		newPlatform?.OnEnter(CreateTileContext(player.GridPosition, direction));
	}

	private static bool GetPlayerMovementDirection(InputEvent @event, out Vector2I direction)
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

	private TileContext CreateTileContext(Vector2I position, Vector2I direction) => new()
	{
		LevelHandler = CurrentLevel,
		MoveDirection = direction,
		Position = position
	};
}
