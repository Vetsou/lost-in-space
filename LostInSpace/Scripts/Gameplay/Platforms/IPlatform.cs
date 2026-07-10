using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms;

public interface IPlatform
{
	VisualData VisualData { get; }

	void OnEnter(TileContext context);
	void OnExit(TileContext context);
}
