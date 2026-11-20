using Godot;

namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class BreakablePlatform(int health = 1) : IPlatform
{
	public PlatformType Type => PlatformType.Breakable;
	private int Health { get; set; } = health;
	public Vector2I BreakDirection { private get; set; } = default;
	public float BreakAmount { private get; set; } = 0;

	public TileResult OnEnter(PlatformContext ctx) => TileResult.None;
	public TileResult OnExit(PlatformContext ctx) =>
		--Health == 0 ? TileResult.Destroy() : TileResult.None;
	public Color GetInstanceCustom() => new(BreakDirection.X, BreakDirection.Y, BreakAmount, Health);
}
