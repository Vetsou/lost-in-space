using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Gameplay.Platforms;
using LostInSpace.Scripts.UI.Scenes.Settings;

namespace LostInSpace.Scripts.Gameplay.Systems;

public class MovementSystem(ILevelHandler level, PlayerData player)
{
	private ILevelHandler CurrentLevel { get; init; } = level;
	private PlayerData Player { get; init; } = player;

	public bool HandlePlayerMovement(InputEvent @event)
	{
		if (!GetPlayerMovementDirection(@event, out Vector2I direction))
		{
			return false;
		}

		return TryMovePlayer(direction);
	}

	public bool TryMovePlayer(Vector2I direction)
	{
		IPlatform nextTile = CurrentLevel.GetPlatform(Player.GridPosition + direction);

		if (nextTile == null)
		{
			return false;
		}

		IPlatform currPlatform = CurrentLevel.GetPlatform(Player.GridPosition);
		currPlatform.OnExit(CreateTileContext(direction));

		Player.SetPosition(Player.GridPosition + direction);
		CurrentLevel.PickUpCollectible(Player.GridPosition);

		IPlatform newPlatform = CurrentLevel.GetPlatform(Player.GridPosition);
		newPlatform?.OnEnter(CreateTileContext(direction));

		return true;
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

	private TileContext CreateTileContext(Vector2I direction) => new()
	{
		LevelHandler = CurrentLevel,
		MoveDirection = direction,
		Position = Player.GridPosition
	};
}
