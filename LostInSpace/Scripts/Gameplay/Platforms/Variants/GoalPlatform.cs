using Godot;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class GoalPlatform : IPlatform
{
	public VisualData VisualData { get; } = ResourceLoader.Load<VisualData>("uid://c7l8d1q81nlhq");
	public TileResult OnEnter(PlatformContext ctx) =>
		ctx.AllRequiredPointsCollected ? TileResult.Complete() : TileResult.Nothing;
	public TileResult OnExit(PlatformContext ctx) => TileResult.Nothing;
}
