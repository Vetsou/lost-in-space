using Godot;

public class RegularPlatform(Vector2I pos) : Platform(pos)
{
	public override PlatformVisualData VisualData { get; } = ResourceLoader.Load<PlatformVisualData>("uid://b686csrmwec88");
}
