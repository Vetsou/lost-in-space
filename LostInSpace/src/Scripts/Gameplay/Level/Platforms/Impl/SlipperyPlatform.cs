using Godot;

public partial class SlipperyPlatform(Vector2I pos) : Platform(pos)
{
	public override PlatformVisualData VisualData { get; } = ResourceLoader.Load<PlatformVisualData>("uid://dtyk1rdnwg8ui");
	public override void OnEnter(TileContext context) => context.Level.HandlePlayerMovement(context.MoveDirection);
}
