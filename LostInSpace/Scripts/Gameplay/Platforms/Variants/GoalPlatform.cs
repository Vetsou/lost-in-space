using Godot;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class GoalPlatform : IPlatform
{
	private bool _isOpen = false;
	public VisualData VisualData { get; } = ResourceLoader.Load<VisualData>("uid://c7l8d1q81nlhq");
	public TileResult OnEnter(PlatformContext ctx) => TileResult.TryComplete();
	public TileResult OnExit(PlatformContext ctx) => TileResult.None;
	public Color GetInstanceCustom() => _isOpen ? VisualConstants.GoalPlatformOpenColor : VisualConstants.GoalPlatformClosedColor;
	public void Unlock() => _isOpen = true;
}
