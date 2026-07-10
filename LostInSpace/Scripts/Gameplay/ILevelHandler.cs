using Godot;
using LostInSpace.Scripts.Gameplay.Collectibles;
using LostInSpace.Scripts.Gameplay.Platforms;

namespace LostInSpace.Scripts.Gameplay;

public interface ILevelHandler
{
	public void CompleteLevel();
	public void RemovePlatform(Vector2I pos);
	public void MovePlayer(Vector2I direction);
	public IPlatform GetPlatform(Vector2I pos);
	public void PickUpCollectible(Vector2I pos);
	public ushort GetCollectiblePickedUpCount(Collectible collectible);
	public ushort GetCollectibleStartCount(Collectible collectible);
}
