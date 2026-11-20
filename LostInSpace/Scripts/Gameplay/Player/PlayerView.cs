using Godot;
using LostInSpace.Scripts.Gameplay.Data;

namespace LostInSpace.Scripts.Gameplay.Player;

public partial class PlayerView : Node3D, IPlayerView
{
	[Export] private Node3D _model;

	public Vector2I GridPosition { get; private set; }

	public void SetTo(Vector2I gridPos)
	{
		GridPosition = gridPos;
		Position = gridPos.GridToWorld();
	}

	public void WalkTo(Vector2I gridPos, Vector2I direction)
	{
		GridPosition = gridPos;
		Position = gridPos.GridToWorld();
		Face(direction);
	}

	public void TeleportTo(Vector2I gridPos)
	{
		GridPosition = gridPos;
		Position = gridPos.GridToWorld();
	}

	private void Face(Vector2I dir)
	{
		if (dir == Vector2I.Zero)
		{
			return;
		}

		float angle = Mathf.Atan2(dir.X, dir.Y);
		_model.Rotation = new Vector3(0, angle, 0);
	}
}
