using Godot;

public static class PlatformRegistry
{
	// TODO: modify this to allow user generated content, store levels in JSONs and make this a dynamic dictionary based on loaded levels
	public static Platform CreatePlatform(int id) => id switch
	{
		1 => new RegularPlatform(),
		2 => new GoalPlatform(),
		3 => new BreakablePlatform(),
		4 => new MovePlatform(Vector2I.Left),
		5 => new MovePlatform(Vector2I.Right),
		6 => new MovePlatform(Vector2I.Down),
		7 => new MovePlatform(Vector2I.Up),
		_ => null
	};
}
