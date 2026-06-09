using System;
using Godot;

public abstract class Platform
{
	public event Action<Vector2I> OnRemovalRequested;
	public abstract PlatformVisualData VisualData { get; }
	public Vector2I pos;

	public void SetPosition(Vector2I pos)
	{
		this.pos = pos;
		VisualData.transform.Origin = new Vector3(pos.X * Level.SPACING, 0, pos.Y * Level.SPACING);
	}

	public virtual void OnEnter(TileContext context) { }
	public virtual void OnExit(TileContext context) { }
	protected void RemovePlatform() => OnRemovalRequested?.Invoke(pos);
}
