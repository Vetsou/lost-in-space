using Godot;

namespace LostInSpace.Scripts.Gameplay.Player;

public interface IPlayerView
{
	void SetTo(Vector2I gridPos);
	void WalkTo(Vector2I gridPos, Vector2I direction);
	void TeleportTo(Vector2I gridPos, Vector2I direction);
}
