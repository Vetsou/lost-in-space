using Godot;

public class RegularPlatform : Platform
{
	public override PlatformVisualData VisualData { get; } = ResourceLoader.Load<PlatformVisualData>("uid://b686csrmwec88");
}
