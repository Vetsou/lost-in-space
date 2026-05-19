using Godot;
using System;

public partial class OneTimePlatform : Node3D, IPlatform
{
	public void OnEnter(TileContext context)
	{
	}

	public void OnExit(TileContext context)
	{
		context.Level.RemoveTile(this);
	}
}
