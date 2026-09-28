using Godot;

namespace LostInSpace.Scripts.Gameplay.Platforms;

public enum MoveKind
{
	None,
	Walk,
	Teleport
}

public readonly record struct TileResult
{
	public static readonly TileResult Nothing = default;

	public MoveKind Kind { get; init; }
	public Vector2I Target { get; init; }
	public bool DestroySelf { get; init; }
	public bool RequestLevelComplete { get; init; }

	public static TileResult Walk(Vector2I direction) => new() { Kind = MoveKind.Walk, Target = direction };
	public static TileResult Teleport(Vector2I position) => new() { Kind = MoveKind.Teleport, Target = position };
	public static TileResult Destroy() => new() { DestroySelf = true };
	public static TileResult Complete() => new() { RequestLevelComplete = true };
}
