using Godot;

public partial class DummyPlayer : Node3D
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
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			Vector2I direction;
			if (keyEvent.Keycode == Key.W)
			{
				direction = Vector2I.Up;
			}
			else if (keyEvent.Keycode == Key.S)
			{
				direction = Vector2I.Down;
			}
			else if (keyEvent.Keycode == Key.A)
			{
				direction = Vector2I.Left;
			}
			else if (keyEvent.Keycode == Key.D)
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
}
