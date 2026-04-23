using System.Collections.Generic;
using Godot;

public partial class LevelPlayer : Scene
{
	[Export] private Node PlatformContainer;
	[Export] private Godot.Collections.Dictionary<int, PackedScene> platformTypes;

	private static Dictionary<Vector2I, IPlatform> tileMap = [];

	[Export] private DummyPlayer player;
	private static int[,] grid = {
			{1, 0, 1, 1, 1, 1, 0, 2},
			{1, 1, 1, 1, 0, 1, 1, 1},
			{1, 1, 0, 1, 1, 0, 1, 0},
			{0, 1, 0, 1, 1, 0, 1, 0},
			{0, 1, 1, 1, 1, 0, 1, 1},
			{1, 1, 1, 0, 1, 1, 0, 1},
			{1, 0, 1, 1, 1, 1, 0, 1},
			{1, 1, 1, 0, 0, 1, 1, 1}
		}; //temporary
		   // 0 - empty, 1 - platform, 2 - goal
	private const float spacing = 1;
	private static float offsetX = (grid.GetLength(1) - 1) / 2.0f;
	private static float offsetY = (grid.GetLength(0) - 1) / 2.0f;

	public override void _Ready()
	{
		LoadLevel();
		player.Init(this);
	}

	public override void _Process(double delta)
	{
	}

	private void LoadLevel()
	{
		//this will be change when we decide how levels are stored
		offsetX = (grid.GetLength(1) - 1) / 2.0f;
		offsetY = (grid.GetLength(0) - 1) / 2.0f;

		for (int i = 0; i < grid.GetLength(0); i++)
		{
			for (int j = 0; j < grid.GetLength(1); j++)
			{
				if (grid[i, j] == 0)
				{
					continue;
				}

				PackedScene platform = platformTypes[grid[i, j]];
				Node3D instance = platform.Instantiate<Node3D>();

				instance.Position = new Vector3((j - offsetX) * spacing, 0, (i - offsetY) * spacing);
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

	public static Vector3 GridToWorld(Vector2I pos) => new Vector3((pos.X - offsetX) * spacing, 0, (pos.Y - offsetY) * spacing);

	//temporary - guess we should just make a UI for level completion
	public void Win()
	{
		ChangeScene(SceneId.MAIN_MENU);
	}
}
