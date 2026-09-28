using Godot;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class MovePlatform(Vector2I direction) : IPlatform
{
	public VisualData VisualData { get; } = ResourceLoader.Load<VisualData>("uid://bt7fcql1et6em");
	public TileResult OnEnter(PlatformContext ctx) => TileResult.Walk(direction);
	public TileResult OnExit(PlatformContext ctx) => TileResult.Nothing;
}
