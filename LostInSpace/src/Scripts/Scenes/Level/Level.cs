using System.Collections.Generic;
using Godot;

public partial class Level : Scene
{
	[Export] private Node PlatformContainer;
	[Export] private Godot.Collections.Dictionary<int, PackedScene> platformTypes;
	[Export] private Player player;

	private static readonly Dictionary<Vector2I, IPlatform> tileMap = [];

	// TODO: Temporary, change when implementing level loading.
	//  0 - empty, 1 - platform, 2 - goal, 3 - breakable, 8 - slip
	// conveyors: 4 - up, 5 - down, 6 - left, 7 - right
	private static readonly int[,] grid = {
			{1, 0, 1, 8, 1, 1, 0, 2},
			{1, 1, 1, 1, 0, 1, 1, 1},
			{1, 3, 0, 1, 1, 0, 1, 0},
			{0, 1, 0, 4, 1, 0, 1, 0},
			{0, 1, 1, 1, 1, 0, 1, 1},
			{1, 6, 1, 0, 1, 1, 0, 1},
			{1, 0, 1, 1, 1, 1, 0, 1},
			{1, 1, 1, 0, 7, 1, 1, 1}
		};
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
		LoadLevel();
		player.Init(this);
	}

	public override void _Process(double delta)
	{
	}

	// TODO: Temporary, change when implementing level loading.
	private void LoadLevel()
	{
		PlatformFactory platformFactory = new PlatformFactory(platformTypes);
		for (int i = 0; i < GridHeight; i++)
		{
			for (int j = 0; j < GridWidth; j++)
			{
				if (grid[i, j] == 0)
				{
					continue;
				}
				Node3D instance = platformFactory.CreatePlatform(grid[i, j]);
				instance.Position = new Vector3((j - Offset.X) * spacing, 0, (i - Offset.Y) * spacing);
				PlatformContainer.AddChild(instance);

				if (instance is IPlatform tile)
				{
					var gridPos = new Vector2I(j, i);
					tileMap[gridPos] = tile;
				}
			}
		}
	}

	public static IPlatform GetTile(Vector2I pos) => tileMap.TryGetValue(pos, out IPlatform tile) ? tile : null;

	public void RemoveTile(IPlatform platform)
	{
		Vector2I tilePosition = default;
		bool foundTile = false;
		foreach (KeyValuePair <Vector2I, IPlatform> tile in tileMap)
		{
			if (ReferenceEquals(tile.Value, platform))
			{
				tilePosition = tile.Key;
				foundTile = true;
				break;
			}
		}
		if (!foundTile)
		{
			return;
		}
		tileMap.Remove(tilePosition);
		if (platform is Node node)
		{
			node.QueueFree();
		}
	}

	public static Vector3 GridToWorld(Vector2I pos) => new Vector3((pos.X - Offset.X) * spacing, 0, (pos.Y - Offset.Y) * spacing);

	// TODO: Temporary, should make win UI
	public void Win()
	{
		ChangeScene(SceneId.MainMenu);
	}

	private static int GridWidth => grid.GetLength(1);
	private static int GridHeight => grid.GetLength(0);
}
