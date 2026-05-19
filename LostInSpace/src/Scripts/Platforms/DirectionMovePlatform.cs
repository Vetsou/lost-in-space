using Godot;
using System;

public enum MoveDirection
{
	Up,
	Down,
	Left,
	Right
}

public partial class DirectionMovePlatform : Node3D, IPlatform
{
	[Export]
	public MoveDirection Direction { get; set; } = MoveDirection.Right;
	
	public void OnEnter(TileContext context)
	{
		context.Player.Move(GetDirectionVector());
	}

	public void OnExit(TileContext context)
	{
	}
	
	private Vector2I GetDirectionVector()
	{
		return Direction switch
		{
			MoveDirection.Up => Vector2I.Up,
			MoveDirection.Down => Vector2I.Down,
			MoveDirection.Left => Vector2I.Left,
			MoveDirection.Right => Vector2I.Right,
			_ => Vector2I.Zero
		};
	}
}
