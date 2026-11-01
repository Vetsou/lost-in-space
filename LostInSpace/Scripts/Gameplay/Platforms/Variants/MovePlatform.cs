using Godot;

namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class MovePlatform(Vector2I direction) : IPlatform
{
	public PlatformType Type => PlatformType.Move;
	public Vector2I Direction => direction;
	public TileResult OnEnter(PlatformContext ctx) => TileResult.ForcedMove(direction);
	public TileResult OnExit(PlatformContext ctx) => TileResult.None;
}
