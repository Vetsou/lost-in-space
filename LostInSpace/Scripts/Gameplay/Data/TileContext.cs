using Godot;

namespace LostInSpace.Scripts.Gameplay.Data;

public readonly struct TileContext
{
	public required ILevelHandler LevelHandler { get; init; }
	public required Vector2I MoveDirection { get; init; }
}
