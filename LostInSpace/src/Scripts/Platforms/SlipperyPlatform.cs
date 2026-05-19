using Godot;
using System;

public partial class SlipperyPlatform : Node3D, IPlatform
{
	public void OnEnter(TileContext context)
	{
		context.Player.Move(context.MoveDirection);
	}

	public void OnExit(TileContext context)
	{
	}
}
