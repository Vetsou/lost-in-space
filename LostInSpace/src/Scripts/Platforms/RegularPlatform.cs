using Godot;

public class RegularPlatform : Platform
{
	public override PlatformVisualData VisualData { get; } = ResourceLoader.Load<PlatformVisualData>("res://src/Objects/Platforms/RegularPlatformVisualData.tres");
}
