using Godot;

public partial class Goal : Node3D, IPlatform
{
	public override PlatformVisualData VisualData { get; } = ResourceLoader.Load<PlatformVisualData>("res://src/Objects/Platforms/GoalPlatformVisualData.tres");
	public Point Point { get; set; }

	public override void OnEnter(TileContext context)
	{
		context.Level.Win();
	}

	public override void OnExit(TileContext context)
	{
		GD.Print("Goal Left");
	}
}
