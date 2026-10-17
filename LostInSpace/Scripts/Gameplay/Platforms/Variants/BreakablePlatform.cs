using Godot;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class BreakablePlatform(int health = 1) : IPlatform
{
	public VisualData VisualData { get; } = ResourceLoader.Load<VisualData>("uid://cbne4iex327jv");
	public Color InitialCustomData { get; } = default;
	private int Health { get; set; } = health;

	public TileResult OnEnter(PlatformContext ctx) => TileResult.None;
	public TileResult OnExit(PlatformContext ctx) =>
		--Health == 0 ? TileResult.Destroy() : TileResult.None;
}
