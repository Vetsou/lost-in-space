using Godot;

public partial class Player : Node3D
{
	private Vector2I GridPosition { get; set; } = Vector2I.Zero;
	private Platform? _currentTile;
	private Level _level;

	public void Init(Level level, Vector2I gridPosition)
	{
		_level = level;
		GridPosition = gridPosition;
		Position = level.GridToWorld(GridPosition);
		_currentTile = level.GetTile(GridPosition);
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
		Platform? nextTile = _level.GetTile(GridPosition + direction);
		if (nextTile == null)
		{
			return;
		}

		GridPosition += direction;

		TileContext context = GetTileContext();

		_currentTile?.OnExit(context);
		_currentTile = nextTile;

		Position = _level.GridToWorld(GridPosition);
		_currentTile?.OnEnter(context);
	}

	private TileContext GetTileContext()
	{
		return new TileContext
		{
			Level = _level
		};
	}
}
