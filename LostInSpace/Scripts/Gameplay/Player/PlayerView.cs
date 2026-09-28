using Godot;

namespace LostInSpace.Scripts.Gameplay.Player;

public partial class PlayerView : Node3D, IPlayerView
{
	[Export] private Node3D _model;

	public void SetTo(Vector2I gridPos)
	{
		Vector3 newPos = gridPos.GridToWorldPosition();
		Position = newPos;
	}

	public void WalkTo(Vector2I gridPos, Vector2I direction)
	{
		Vector3 newPos = gridPos.GridToWorldPosition();
		Position = newPos;
		Face(direction);
	}

	public void TeleportTo(Vector2I gridPos, Vector2I direction)
	{
		Vector3 newPos = gridPos.GridToWorldPosition();
		Position = newPos;
		Face(direction);
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
