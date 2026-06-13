using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class GoalPlatform(Vector2I pos) : Platform(pos)
{
	public override PlatformVisualData VisualData { get; } = ResourceLoader.Load<PlatformVisualData>("uid://c7l8d1q81nlhq");
	public override void OnEnter(TileContext context) => context.LevelHandler.CompleteLevel();
	public override void OnExit(TileContext context) => GD.Print("Goal Left");
}
