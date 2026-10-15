using Godot;

namespace LostInSpace.Scripts.Gameplay.Platforms;

public enum MoveKind
{
	None,
	Walk,
	Teleport
}

public enum EnterAction
{
	None,
	TryCompleteLevel
}

public enum LeaveAction
{
	None,
	DestroySelf
}

public readonly record struct TileResult
{
	public static readonly TileResult None = default;

	public MoveKind MoveKind { get; init; }
	public EnterAction EnterAction { get; init; }
	public LeaveAction LeaveAction { get; init; }
	public Vector2I Target { get; init; }

	public static TileResult Walk(Vector2I direction) => new() { MoveKind = MoveKind.Walk, Target = direction };
	public static TileResult Teleport(Vector2I position) => new() { MoveKind = MoveKind.Teleport, Target = position };
	public static TileResult Destroy() => new() { LeaveAction = LeaveAction.DestroySelf };
	public static TileResult TryComplete() => new() { EnterAction = EnterAction.TryCompleteLevel };
}
