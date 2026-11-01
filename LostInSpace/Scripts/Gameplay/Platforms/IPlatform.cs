using Godot;

namespace LostInSpace.Scripts.Gameplay.Platforms;

public interface IPlatform
{
	PlatformType Type { get; }
	Vector2I Direction => Vector2I.Right;

	TileResult OnEnter(PlatformContext ctx);
	TileResult OnExit(PlatformContext ctx);
}
