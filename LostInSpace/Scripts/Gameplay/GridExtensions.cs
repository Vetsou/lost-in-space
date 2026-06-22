using Godot;
using LostInSpace.Scripts.Gameplay.Data;

namespace LostInSpace.Scripts.Gameplay;

public static class GridExtensions
{
	public static Vector3 GridToWorldPosition(this Vector2I pos) =>
		new(pos.X * LevelConstants.SPACING, 0, pos.Y * LevelConstants.SPACING);
}
