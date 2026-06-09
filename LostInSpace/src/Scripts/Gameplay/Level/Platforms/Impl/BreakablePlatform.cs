using Godot;

public class BreakablePlatform(Vector2I pos, int health = 1) : Platform(pos)
{
	public override PlatformVisualData VisualData { get; } = ResourceLoader.Load<PlatformVisualData>("uid://cbne4iex327jv");
	private int Health { get; set; } = health;

	public override void OnExit(TileContext context)
	{
		Health--;
		if (Health == 0)
		{
			context.Level.RemovePlatform(Position);
		}
	}
}
