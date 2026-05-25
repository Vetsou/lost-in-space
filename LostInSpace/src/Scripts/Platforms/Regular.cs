using Godot;

public partial class Regular : Node3D, IPlatform
{
	public override PlatformVisualData VisualData { get; } = ResourceLoader.Load<PlatformVisualData>("res://src/Objects/Platforms/RegularPlatformVisualData.tres");

	public Point Point { get; set; }

	public void OnEnter(TileContext context)
	{
		Point?.OnCollect();
	}

	public void OnExit(TileContext context)
	{

	}
}