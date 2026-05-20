using Godot;

public class GoalPlatform : Platform
{
	public override PlatformVisualData VisualData { get; } = ResourceLoader.Load<PlatformVisualData>("res://src/Objects/Platforms/GoalPlatformVisualData.tres");

	public override void OnEnter(TileContext context)
	{
		context.Level.Win();
	}

	public override void OnExit(TileContext context)
	{
		GD.Print("Goal Left");
	}
}
