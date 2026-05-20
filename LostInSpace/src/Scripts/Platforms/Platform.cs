using Godot;

public abstract class Platform
{
	public abstract PlatformVisualData VisualData { get; }

	public void SetPosition(Vector3 pos) => VisualData.transform.Origin = pos;

	public virtual void OnEnter(TileContext context) {}
	public virtual void OnExit(TileContext context) {}
	void RemovePlatform(TileContext context)
	{
		
	}
}
