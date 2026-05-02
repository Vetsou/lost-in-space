using Godot;

public partial class PointPlatform : Node3D, IPlatform
{
	[Export] public Node3D tilePath;

	public bool HasPoint { get; set; } = true;

	public void OnEnter(TileContext context)
	{
		context.Level.AddPoint(tilePath);
	}

	public void OnExit(TileContext context)
	{

	}
}
