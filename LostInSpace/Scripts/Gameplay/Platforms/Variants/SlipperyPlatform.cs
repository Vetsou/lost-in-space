using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public partial class SlipperyPlatform(Vector2I pos) : Platform(pos)
{
	public override VisualData VisualData { get; } = ResourceLoader.Load<VisualData>("uid://dtyk1rdnwg8ui");
	public override void OnEnter(TileContext context) => context.LevelHandler.MovePlayer(context.MoveDirection);
}
