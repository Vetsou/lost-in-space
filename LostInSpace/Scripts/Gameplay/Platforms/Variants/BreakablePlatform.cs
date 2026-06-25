using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class BreakablePlatform(Vector2I pos, int health = 1) : Platform(pos)
{
	public override VisualData VisualData { get; } = ResourceLoader.Load<VisualData>("uid://cbne4iex327jv");
	private int Health { get; set; } = health;

	public override void OnExit(TileContext context)
	{
		Health--;
		if (Health == 0)
		{
			context.LevelHandler.RemovePlatform(Position);
		}
	}
}
