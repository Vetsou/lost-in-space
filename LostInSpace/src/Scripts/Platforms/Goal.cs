using Godot;

public partial class Goal : Node3D, IPlatform
{
	public void OnEnter(TileContext context)
	{
		context.Level.Win();
	}

	public void OnExit(TileContext context)
	{
		GD.Print("Goal Left");
	}
}
