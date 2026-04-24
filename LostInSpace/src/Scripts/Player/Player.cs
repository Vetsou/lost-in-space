using Godot;

public partial class Player : Node3D
{
	private Vector2I position = Vector2I.Zero;
	private IPlatform currentTile;
	private LevelPlayer level;

	public void Init(LevelPlayer level)
	{
		this.level = level;
		Position = LevelPlayer.GridToWorld(position);
		currentTile = LevelPlayer.GetTile(position);
	}

	public override void _Input(InputEvent @event)
	{
		Vector2I direction;
		if (@event.IsActionPressed(SettingsMap.Keys.MOVE_UP))
		{
			direction = Vector2I.Up;
		}
		else if (@event.IsActionPressed(SettingsMap.Keys.MOVE_DOWN))
		{
			direction = Vector2I.Down;
		}
		else if (@event.IsActionPressed(SettingsMap.Keys.MOVE_LEFT))
		{
			direction = Vector2I.Left;
		}
		else if (@event.IsActionPressed(SettingsMap.Keys.MOVE_RIGHT))
		{
			direction = Vector2I.Right;
		}
		else
		{
			return;
		}

		IPlatform nextTile = LevelPlayer.GetTile(position + direction);
		if (nextTile == null)
		{
			return;
		}

		position += direction;

		var context = new TileContext
		{
			Level = level
		};

		currentTile?.OnExit(context);
		currentTile = nextTile;

		Position = LevelPlayer.GridToWorld(position);
		currentTile?.OnEnter(context);
	}
}
