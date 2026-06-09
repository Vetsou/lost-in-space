using Godot;

public class MovePlatform(Vector2I pos, Vector2I direction) : Platform(pos)
{
	public override PlatformVisualData VisualData { get; } = ResourceLoader.Load<PlatformVisualData>("uid://bt7fcql1et6em");
	public Vector2I Direction { get; } = direction;
	public override void OnEnter(TileContext context) => context.Level.HandlePlayerMovement(Direction);
}
