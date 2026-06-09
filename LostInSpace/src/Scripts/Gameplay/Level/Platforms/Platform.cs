using Godot;

public abstract class Platform
{
	public event Action<Vector2I> OnRemovalRequested;
	public abstract PlatformVisualData VisualData { get; }
	private Vector2I _position;

	public void SetPosition(Vector2I pos)
	{
		_position = pos;
		VisualData.Transform.Origin = pos.GridToWorldPosition();
	}

	public virtual void OnEnter(TileContext context) { }
	public virtual void OnExit(TileContext context) { }
	protected void RemovePlatform() => OnRemovalRequested?.Invoke(_position);
}
