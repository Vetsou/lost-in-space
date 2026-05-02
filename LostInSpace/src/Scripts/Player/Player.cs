using Godot;

public partial class Player : Node3D
{
	public Vector2I GridPosition { get; private set; } = Vector2I.Zero;
	private IPlatform currentTile;
	private Level level;

	public void Init(Level level)
	{
		this.level = level;
		Position = Level.GridToWorld(GridPosition);
		currentTile = Level.GetTile(GridPosition);
	}

	public override void _Input(InputEvent @event)
	{
		if (!TryGetDirection(@event, out Vector2I direction))
		{
			return;
		}

		Move(direction);
	}

	private bool TryGetDirection(InputEvent @event, out Vector2I direction)
	{
		if (@event.IsActionPressed(SettingsMap.Keys.MOVE_UP))
		{
			direction = Vector2I.Up;
			return true;
		}

		if (@event.IsActionPressed(SettingsMap.Keys.MOVE_DOWN))
		{
			direction = Vector2I.Down;
			return true;
		}

		if (@event.IsActionPressed(SettingsMap.Keys.MOVE_LEFT))
		{
			direction = Vector2I.Left;
			return true;
		}

		if (@event.IsActionPressed(SettingsMap.Keys.MOVE_RIGHT))
		{
			direction = Vector2I.Right;
			return true;
		}

		direction = default;
		return false;
	}

	private void Move(Vector2I direction)
	{
		IPlatform nextTile = Level.GetTile(GridPosition + direction);
		if (nextTile == null)
		{
			return;
		}

		GridPosition += direction;

		TileContext context = GetTileContext();

		currentTile?.OnExit(context);
		currentTile = nextTile;

		Position = Level.GridToWorld(GridPosition);
		currentTile?.OnEnter(context);
	}

	private TileContext GetTileContext()
	{
		return new TileContext
		{
			Level = level
		};
	}
}
