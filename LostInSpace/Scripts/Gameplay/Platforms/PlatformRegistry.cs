using Godot;

public static class PlatformRegistry
{
	public static Platform CreatePlatform(int id, Vector2I gridPosition) => id switch
	{
		1 => new RegularPlatform(gridPosition),
		2 => new GoalPlatform(gridPosition),
		3 => new BreakablePlatform(gridPosition),
		4 => new MovePlatform(gridPosition, Vector2I.Left),
		5 => new MovePlatform(gridPosition, Vector2I.Right),
		6 => new MovePlatform(gridPosition, Vector2I.Down),
		7 => new MovePlatform(gridPosition, Vector2I.Up),
		8 => new SlipperyPlatform(gridPosition),
		_ => null
	};
}
