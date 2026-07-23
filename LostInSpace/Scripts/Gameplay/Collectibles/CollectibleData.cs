using Godot;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Collectibles;

public static class CollectibleData
{
	public static VisualData PrimaryCollectibleVisualData { get; } = ResourceLoader.Load<VisualData>("uid://dkssxmi6nh8h0");
	public static VisualData OptionalCollectibleVisualData { get; } = ResourceLoader.Load<VisualData>("uid://dai8uv2vxng4m");
}
