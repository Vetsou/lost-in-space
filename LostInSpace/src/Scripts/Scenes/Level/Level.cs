using System.Collections.Generic;
using Godot;

public partial class Level : Scene
{
	[Export] private PlatformRenderingServer platformRenderingServer;
	[Export] private Player player;

	private static readonly Dictionary<Vector2I, Platform> tileMap = [];

	// TODO: Temporary, change when implementing level loading.
	//  0 - empty, 1 - platform, 2 - goal
	private static readonly int[,] grid = {
			{1, 0, 1, 1, 1, 1, 0, 2},
			{1, 1, 1, 1, 0, 1, 1, 1},
			{1, 1, 0, 1, 1, 0, 1, 0},
			{0, 1, 0, 1, 1, 0, 1, 0},
			{0, 1, 1, 1, 1, 0, 1, 1},
			{1, 1, 1, 0, 1, 1, 0, 1},
			{1, 0, 1, 1, 1, 1, 0, 1},
			{1, 1, 1, 0, 0, 1, 1, 1}
		};
	public const float spacing = 1;
	public static Vector2 Offset
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
		for (int i = 0; i < GridHeight; i++)
		{
			for (int j = 0; j < GridWidth; j++)
			{
				if (grid[i, j] == 0)
				{
					continue;
				}

				Platform platform = PlatformRegistry.CreatePlaform(grid[i, j]);

				var gridPos = new Vector2I(j, i);
				platform.SetPosition(gridPos);
				tileMap[gridPos] = platform;

				platformRenderingServer.RenderPlatform(platform);
			}
		}
	}

	public static Platform GetTile(Vector2I pos) => tileMap.TryGetValue(pos, out Platform tile) ? tile : null;

	public static Vector3 GridToWorld(Vector2I pos) => new Vector3((pos.X - Offset.X) * spacing, 0, (pos.Y - Offset.Y) * spacing);

	// TODO: Temporary, should make win UI
	public void Win()
	{
		platformRenderingServer.ClearLevel();
		ChangeScene(SceneId.MainMenu);
	}

	public void RemovePlatform(Vector2I pos)
	{
		platformRenderingServer.FreePlatform(tileMap[pos]);
		tileMap.Remove(pos);
	}

	public void UpdatePlatformShader(Platform instance, string param, Variant value) => platformRenderingServer.UpdatePlatformShader(instance, param, value);

	private static int GridWidth => grid.GetLength(1);
	private static int GridHeight => grid.GetLength(0);
}
