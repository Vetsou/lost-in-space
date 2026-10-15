using Godot;

namespace LostInSpace.Scripts.Gameplay.Platforms;

public readonly record struct PlatformContext
{
	public Vector2I MoveDirection { get; init; }
	public Vector2I? TeleporterPartner { get; init; }
}
