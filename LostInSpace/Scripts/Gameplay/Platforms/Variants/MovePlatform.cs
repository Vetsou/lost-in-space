using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class MovePlatform(Vector2I direction) : IPlatform
{
	public VisualData VisualData { get; } = ResourceLoader.Load<VisualData>("uid://bt7fcql1et6em");
	public Vector2I Direction { get; } = direction;

	public void OnEnter(TileContext context) => context.LevelHandler.MovePlayer(Direction);

	public void OnExit(TileContext context)
	{
	}
}
