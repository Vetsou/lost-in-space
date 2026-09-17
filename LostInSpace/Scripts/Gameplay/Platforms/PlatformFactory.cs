using Godot;
using LostInSpace.Scripts.Gameplay.Platforms.Variants;

namespace LostInSpace.Scripts.Gameplay.Platforms;

public static class PlatformFactory
{
	public static IPlatform CreatePlatform(int id) => id switch
	{
		1 => new RegularPlatform(),
		2 => new GoalPlatform(),
		3 => new BreakablePlatform(),
		4 => new MovePlatform(Vector2I.Left),
		5 => new MovePlatform(Vector2I.Right),
		6 => new MovePlatform(Vector2I.Down),
		7 => new MovePlatform(Vector2I.Up),
		8 => new SlipperyPlatform(),
		9 => new TeleporterPlatform(0),
		10 => new TeleporterPlatform(1),
		11 => new TeleporterPlatform(2),
		12 => new TeleporterPlatform(3),
		_ => null
	};
}
