using Godot;

public partial class PointPlatform : Node3D, IPlatform
{
	[Export] public Node3D tilePath;

	public void OnEnter(TileContext context)
	{
		if (HasPoint)
		{
			tilePath.QueueFree();
		}
		context.Level.AddPoint();
	}

	public void OnExit(TileContext context)
	{

	}

	public bool HasPoint { get; set; } = true;
}
