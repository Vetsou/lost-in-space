using Godot;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Collectibles;

public static class CollectibleFactory
{
	private static readonly Dictionary<Collectible, CollectibleType> _collectibleTypes = new()
	{
		{ Collectible.Point, new CollectibleType("Point", ResourceLoader.Load<VisualData>("uid://dkssxmi6nh8h0")) }
	};

	public static CollectibleType GetCollectibleType(Collectible id)
	{
		if (_collectibleTypes.TryGetValue(id, out CollectibleType collectibleType))
		{
			return collectibleType;
		}
		else
		{
			throw new Exception($"Collectible Type for ID {id} does not exist.");
		}
	}
}
