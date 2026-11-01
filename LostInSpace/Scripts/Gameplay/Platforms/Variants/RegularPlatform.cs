namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class RegularPlatform : IPlatform
{
	public PlatformType Type => PlatformType.Regular;
	public TileResult OnEnter(PlatformContext ctx) => TileResult.None;
	public TileResult OnExit(PlatformContext ctx) => TileResult.None;
}
