using Godot;

public class RegularPlatform : IPlatform
{
	public PlatformVisualData VisualData { get; } = ResourceLoader.Load<PlatformVisualData>("res://src/Objects/Platforms/RegularPlatformVisualData.tres");

	public void OnEnter(TileContext context)
	{

	}

	public void OnExit(TileContext context)
	{

	}

	public void SetPosition(Vector3 pos) => VisualData.transform.Origin = pos;
}
