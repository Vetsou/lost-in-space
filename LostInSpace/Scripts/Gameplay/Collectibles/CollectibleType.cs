using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Collectibles;

public class CollectibleType(string name, VisualData visualData)
{
	public string Name { get; init; } = name;
	public VisualData VisualData { get; init; } = visualData;
}
