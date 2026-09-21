using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class TeleporterPlatform(byte teleportLinkId) : IPlatform
{
	public byte TeleportLinkId => teleportLinkId;
	public VisualData VisualData { get; } = ResourceLoader.Load<VisualData>("uid://dva7a0u42vhvn");
	private bool _isDisabled = false;

	public void OnEnter(TileContext context)
	{
		if (_isDisabled)
		{
			return;
		}

		(Vector2I a, Vector2I b) positions = context.LevelHandler.GetTeleporterLinkPositions(TeleportLinkId);
		if (positions.a != context.Position && positions.b != context.Position)
		{
			throw new Exception("Invalid teleporter position");
		}

		Vector2I targetPosition = positions.a == context.Position ? positions.b : positions.a;

		if (context.LevelHandler.GetPlatform(targetPosition) is TeleporterPlatform platform)
		{
			platform._isDisabled = true;
		}
		else
		{
			throw new Exception($"Platform at teleport link position must be of type {nameof(TeleporterPlatform)}");
		}

		context.LevelHandler.MovePlayer(targetPosition - context.Position);
	}

	public void OnExit(TileContext context) => _isDisabled = false;
}
