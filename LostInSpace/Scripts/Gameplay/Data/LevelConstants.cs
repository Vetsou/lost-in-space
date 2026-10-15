using Godot;

namespace LostInSpace.Scripts.Gameplay.Data;

public static class LevelConstants
{
	public const float PLATFORM_SPACING = 1.0f;
	public const float PLATFORM_RENDER_HEIGHT = 0.0f;
	public const float POINT_RENDER_HEIGHT = 0.5f;

	public static Vector3 GridToWorld(this Vector2I pos, float y = 0f) =>
		new(pos.X * PLATFORM_SPACING, y, pos.Y * PLATFORM_SPACING);
}
