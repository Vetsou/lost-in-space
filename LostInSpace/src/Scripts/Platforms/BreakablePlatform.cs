using Godot;

public class BreakablePlatform(int health = 1) : Platform
{
	private int Health { get; set; } = health;
	public override PlatformVisualData VisualData { get; } = ResourceLoader.Load<PlatformVisualData>("uid://c7l8d1q81nlhq");

    public override void OnExit(TileContext context)
    {
		Health--;
		if (Health == 0)
        {
			RemovePlatform();
		}
	}
}
