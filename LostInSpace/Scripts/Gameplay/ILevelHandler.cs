using Godot;
using LostInSpace.Scripts.Gameplay.Platforms;
using LostInSpace.Scripts.Gameplay.Systems;

namespace LostInSpace.Scripts.Gameplay;

public interface ILevelHandler
{
	public ushort PrimaryPointsTotalCount { get; }
	public ushort PrimaryPointsCurrentCount { get; }
	public ushort OptionalPointsTotalCount { get; }
	public ushort OptionalPointsCurrentCount { get; }

	public void CompleteLevel();
	public void RemovePlatform(Vector2I pos);
	public MovementSystem MovementSystem { get; }
	public (Vector2I a, Vector2I b) GetTeleporterLinkPositions(byte teleporterLinkId);
	public IPlatform GetPlatform(Vector2I pos);
	public void PickUpCollectible(Vector2I pos);
}
