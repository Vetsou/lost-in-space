using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class SlipperyPlatform : IPlatform
{
	public VisualData VisualData { get; } = ResourceLoader.Load<VisualData>("uid://dtyk1rdnwg8ui");
	public void OnEnter(TileContext context) => context.LevelHandler.MovePlayer(context.MoveDirection);

	public void OnExit(TileContext context)
	{
	}
}
