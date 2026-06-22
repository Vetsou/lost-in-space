using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms;

public abstract class Platform
{
	public abstract PlatformVisualData VisualData { get; }
	protected Vector2I Position { get; }

	public Platform(Vector2I position)
	{
		Position = position;
		VisualData.Transform.Origin = position.GridToWorldPosition();
	}

	public virtual void OnEnter(TileContext context) { }
	public virtual void OnExit(TileContext context) { }
}
