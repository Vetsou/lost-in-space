using Godot;

public static class LevelExtensions
{
	public static Vector3 GridToWorldPosition(this Vector2I pos) =>
		new(pos.X * LevelConstants.SPACING, 0, pos.Y * LevelConstants.SPACING);
}
