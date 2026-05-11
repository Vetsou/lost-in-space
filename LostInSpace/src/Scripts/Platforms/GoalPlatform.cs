using Godot;

public class GoalPlatform : IPlatform
{
	public PlatformVisualData VisualData { get; } = ResourceLoader.Load<PlatformVisualData>("res://src/Objects/Platforms/GoalPlatformVisualData.tres");

	public void OnEnter(TileContext context)
	{
		context.Level.Win();
	}

	public void OnExit(TileContext context)
	{
		GD.Print("Goal Left");
	}

	public void SetPosition(Vector3 pos) => VisualData.transform.Origin = pos;
}
