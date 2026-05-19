using Godot;
using System;

public partial class PlatformFactory : RefCounted
{
	private readonly Godot.Collections.Dictionary<int, PackedScene> platformTypes;
	public PlatformFactory(Godot.Collections.Dictionary<int, PackedScene> platformTypes)
	{
		this.platformTypes = platformTypes;
	}

	public Node3D CreatePlatform(int platformId)
	{
		if (!platformTypes.TryGetValue(GetPlatformSceneId(platformId), out PackedScene platformScene))
		{
			throw new System.ArgumentException($"Unknown platform id: {platformId}");
		}
		Node3D instance = platformScene.Instantiate<Node3D>();
		if (instance is DirectionMovePlatform directionMovePlatform)
		{
			directionMovePlatform.Direction = GetDirection(platformId);
		}
		return instance;
	}

	private int GetPlatformSceneId(int platformId)
	{
		return platformId switch
		{
			4 => 4,
			5 => 4,
			6 => 4,
			7 => 4,
			_ => platformId
		};
	}

	private MoveDirection GetDirection(int platformId)
	{
		return platformId switch
		{
			4 => MoveDirection.Up,
			5 => MoveDirection.Down,
			6 => MoveDirection.Left,
			7 => MoveDirection.Right,
			_ => MoveDirection.Right
		};
	}
}
