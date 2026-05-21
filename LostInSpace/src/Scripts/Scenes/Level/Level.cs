using System.Collections.Generic;
using Godot;

public partial class Level : Scene
{
	[Export] private Node PlatformContainer;
	[Export] private Godot.Collections.Dictionary<int, PackedScene> platformTypes;
	[Export] private Player player;
	[Export] private LevelData levelData;

	private static readonly Dictionary<Vector2I, IPlatform> tileMap = [];
	private int pointCounter;

	private const float spacing = 1;
	private static Vector2 Offset
	{
		get
		{
			return new Vector2((GridWidth - 1) / 2.0f, (GridHeight - 1) / 2.0f);
		}
	}

	public override void _Ready()
	{
		player.Init(this);
	}

	public override void _Process(double delta)
	{
	}

	public static IPlatform GetTile(Vector2I pos) => tileMap.TryGetValue(pos, out IPlatform tile) ? tile : null;

	public static Vector3 GridToWorld(Vector2I pos) => new ((pos.X - Offset.X) * spacing, 0, (pos.Y - Offset.Y) * spacing);

	// TODO: Temporary, should make win UI
	public void Win()
	{
		if (pointCounter != 0)
		{
			return;
		}
		ChangeScene(SceneId.MainMenu);
	}

	public void AddPoint(Node3D tilePath)
	{
		IPlatform tile = GetTile(player.GridPosition);
		if (tile == null)
		{
			return;
		}

		if (tile.HasPoint)
		{
			pointCounter--;
			tilePath.QueueFree();
		}
		tile.HasPoint = false;
	}

	private static int GridWidth => grid.GetLength(1);
	private static int GridHeight => grid.GetLength(0);
}
