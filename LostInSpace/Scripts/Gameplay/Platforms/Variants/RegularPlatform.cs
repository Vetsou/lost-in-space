using Godot;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class RegularPlatform : IPlatform
{
	public VisualData VisualData { get; } = ResourceLoader.Load<VisualData>("uid://b686csrmwec88");
	public TileResult OnEnter(PlatformContext ctx) => TileResult.Nothing;
	public TileResult OnExit(PlatformContext ctx) => TileResult.Nothing;
}
