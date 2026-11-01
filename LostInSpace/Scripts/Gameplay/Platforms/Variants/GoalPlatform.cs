namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class GoalPlatform : IPlatform
{
	public PlatformType Type => PlatformType.Goal;
	public TileResult OnEnter(PlatformContext ctx) => TileResult.TryComplete();
	public TileResult OnExit(PlatformContext ctx) => TileResult.None;
}
