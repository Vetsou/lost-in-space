using Godot;

public class MovePlatform(Vector2I direction) : Platform
{
	public Vector2I Direction { get; } = direction;
	public override PlatformVisualData VisualData { get; } = ResourceLoader.Load<PlatformVisualData>("uid://c7l8d1q81nlhq");
	public override void OnEnter(TileContext context) => context.Player.Move(Direction);
}
