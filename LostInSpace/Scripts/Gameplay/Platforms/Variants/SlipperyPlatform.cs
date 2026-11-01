namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class SlipperyPlatform : IPlatform
{
	public PlatformType Type => PlatformType.Slippery;
	public TileResult OnEnter(PlatformContext ctx) => TileResult.ForcedMove(ctx.MoveDirection);
	public TileResult OnExit(PlatformContext ctx) => TileResult.None;
}
