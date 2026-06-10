using Godot;

public partial class PlayerData : Node3D
{
	public Vector2I GridPosition { get; private set; }

	public void SetPosition(Vector2I newPosition)
	{
		GridPosition = newPosition;
		Position = GridPosition.GridToWorldPosition();
	}
}
