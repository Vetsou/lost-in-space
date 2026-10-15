using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms;

public interface IPlatform
{
	VisualData VisualData { get; }
	TileResult OnEnter(PlatformContext ctx);
	TileResult OnExit(PlatformContext ctx);
}
