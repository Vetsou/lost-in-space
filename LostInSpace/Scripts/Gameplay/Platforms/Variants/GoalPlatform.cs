using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class GoalPlatform : IPlatform
{
	public VisualData VisualData { get; } = ResourceLoader.Load<VisualData>("uid://c7l8d1q81nlhq");
	public void OnEnter(TileContext context) => context.LevelHandler.CompleteLevel();
	public void OnExit(TileContext context) => GD.Print("Goal Left");
}
