using Godot;

public partial class Regular : Node3D, IPlatform
{
	public Point Point { get; set; }

	public void OnEnter(TileContext context)
	{
		Point?.OnCollect();
	}

	public void OnExit(TileContext context)
	{

	}
}
