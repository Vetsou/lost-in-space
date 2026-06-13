using Godot;
using LostInSpace.Scripts.Gameplay.Platforms;

namespace LostInSpace.Scripts.Gameplay;

public interface ILevelHandler
{
	public void CompleteLevel();
	public void RemovePlatform(Vector2I pos);
	public void MovePlayer(Vector2I direction);
	public Platform GetTile(Vector2I pos);
}
