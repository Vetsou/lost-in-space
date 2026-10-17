using Godot;

namespace LostInSpace.Scripts.Gameplay.Platforms;

public enum MoveKind
{
	None,
	Walk,
	ForcedMove,
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
	DemageSelf,
	DestroySelf
}

public readonly record struct TileResult
{
	public static readonly TileResult None = default;

	public MoveKind MoveKind { get; init; }
	public EnterAction EnterAction { get; init; }
	public LeaveAction LeaveAction { get; init; }
	public Vector2I Target { get; init; }
	public Color Data { get; init; }

	public static TileResult ForcedMove(Vector2I direction) => new() { MoveKind = MoveKind.ForcedMove, Target = direction };
	public static TileResult Teleport(Vector2I position) => new() { MoveKind = MoveKind.Teleport, Target = position };
	public static TileResult Demage(Color data) => new() { LeaveAction = LeaveAction.DemageSelf, Data = data };
	public static TileResult Destroy() => new() { LeaveAction = LeaveAction.DestroySelf };
	public static TileResult TryComplete() => new() { EnterAction = EnterAction.TryCompleteLevel };
}
