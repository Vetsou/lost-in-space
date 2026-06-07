using Godot;

public partial class SlipperyPlatform : Platform
{
	public override PlatformVisualData VisualData { get; } = ResourceLoader.Load<PlatformVisualData>("uid://dtyk1rdnwg8ui");

	public override void OnEnter(TileContext context) => context.Player.Move(context.MoveDirection);
}
