using Godot;

public readonly struct TileContext
{
	public required Level Level { get; init; }
	public required Vector2I MoveDirection { get; init; }
}
