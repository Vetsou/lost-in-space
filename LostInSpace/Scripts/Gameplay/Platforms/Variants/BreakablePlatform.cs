namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class BreakablePlatform(int health = 1) : IPlatform
{
	public PlatformType Type => PlatformType.Breakable;
	private int Health { get; set; } = health;

	public TileResult OnEnter(PlatformContext ctx) => TileResult.None;
	public TileResult OnExit(PlatformContext ctx) =>
		--Health == 0 ? TileResult.Destroy() : TileResult.None;
}
