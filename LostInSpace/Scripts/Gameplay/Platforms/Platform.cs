using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms;

public abstract class Platform(Vector2I position)
{
	public abstract PlatformVisualData VisualData { get; }
	public Vector2I Position { get; } = position;

	public virtual void OnEnter(TileContext context) { }
	public virtual void OnExit(TileContext context) { }
}
