using Godot;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class SlipperyPlatform : IPlatform
{
	public VisualData VisualData { get; } = ResourceLoader.Load<VisualData>("uid://dtyk1rdnwg8ui");
	public TileResult OnEnter(PlatformContext ctx) => TileResult.Walk(ctx.MoveDirection);
	public TileResult OnExit(PlatformContext ctx) => TileResult.Nothing;
}
