using Godot;

public class MovePlatform(Vector2I direction) : Platform
{
	public Vector2I Direction { get; } = direction;
	public override PlatformVisualData VisualData { get; } = ResourceLoader.Load<PlatformVisualData>("uid://bt7fcql1et6em");
	public override void OnEnter(TileContext context) => context.Player.Move(Direction);
}
