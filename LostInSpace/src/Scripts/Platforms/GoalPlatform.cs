using Godot;

public partial class GoalPlatform : Node3D, IPlatform
{
	public bool HasPoint { get; set; }

	public void OnEnter(TileContext context)
	{
		context.Level.Win();
	}

	public void OnExit(TileContext context)
	{
		GD.Print("Goal Left");
	}
}
