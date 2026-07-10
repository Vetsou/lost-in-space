using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class RegularPlatform : IPlatform
{
	public VisualData VisualData { get; } = ResourceLoader.Load<VisualData>("uid://b686csrmwec88");

	public void OnEnter(TileContext context)
	{

	}

	public void OnExit(TileContext context)
	{

	}
}
