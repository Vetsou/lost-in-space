using Godot;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class GoalPlatform : IPlatform
{
	public PlatformType Type => PlatformType.Goal;
	private bool _isOpen = false;
	public TileResult OnEnter(PlatformContext ctx) => TileResult.TryComplete();
	public TileResult OnExit(PlatformContext ctx) => TileResult.None;
	public Color GetInstanceCustom() => _isOpen ? VisualConstants.GoalPlatformOpenColor : VisualConstants.GoalPlatformClosedColor;
	public void Unlock() => _isOpen = true;
}
