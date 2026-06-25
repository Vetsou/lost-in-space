using Godot;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class RegularPlatform(Vector2I pos) : Platform(pos)
{
	public override VisualData VisualData { get; } = ResourceLoader.Load<VisualData>("uid://b686csrmwec88");
}
