using Godot;

namespace LostInSpace.Scripts.Gameplay.Data;

public class PlayerData
{
	public Vector2I GridPosition { get; private set; }
	public Vector2I Facing { get; private set; } = Vector2I.Down;

	public void SetPosition(Vector2I newPosition, Vector2I direction)
	{
		GridPosition = newPosition;
		if (direction != Vector2I.Zero)
		{
			Facing = direction;
		}
	}

	public void SetPosition(Vector2I position) => GridPosition = position;
}
