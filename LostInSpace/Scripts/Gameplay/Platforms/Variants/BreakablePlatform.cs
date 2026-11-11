using Godot;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class BreakablePlatform(int health = 1) : IPlatform
{
	public VisualData VisualData { get; } = ResourceLoader.Load<VisualData>("uid://cbne4iex327jv");
	private int _health = health;
	private int Health
	{
		get => _health;
		set
		{
			_health = value;
			BufferUpdatHelper.RequestBufferChange();
		}
	}
	private Vector2I _breakDirection = default;
	public Vector2I BreakDirection
	{
		private get => _breakDirection;
		set
		{
			_breakDirection = value;
			BufferUpdatHelper.RequestBufferChange();
		}
	}
	private float _breakAmount = 0;
	public float BreakAmount
	{
		private get => _breakAmount;
		set
		{
			_breakAmount = value;
			BufferUpdatHelper.RequestBufferChange();
		}
	}

	public TileResult OnEnter(PlatformContext ctx) => TileResult.None;
	public TileResult OnExit(PlatformContext ctx) =>
		--Health == 0 ? TileResult.Destroy() : TileResult.None;
	public Color GetInstanceCustom() => new(BreakDirection.X, BreakDirection.Y, BreakAmount, Health);
}
