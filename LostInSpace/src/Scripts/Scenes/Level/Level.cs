using Godot;

public partial class Level : Scene
{
	[Export] private Node PlatformContainer;
	[Export] private Player Player;
	[Export] private PackedScene PointScene;
	[Export] public string LevelPath;
	[Export] private PlatformRenderingServer platformRenderingServer;

	private static readonly Dictionary<Vector2I, Platform> tileMap = [];

	public override void _Ready()
	{
		LevelLoader levelLoader = new(PointScene);
		levelLoader.LoadPlatformScenes(); // TODO move this to a game manager or smth
		levelLoader.LoadLevel(LevelPath, PlatformContainer);
		Player.Init(this);
	}

	public static IPlatform GetTile(Vector2I pos) => LevelLoader.tileMap.TryGetValue(pos, out IPlatform tile) ? tile : null;

	public static Vector3 GridToWorld(Vector2I pos) => new ((pos.X - LevelLoader.offset.X) * PlatformData.Spacing, 0, (pos.Y - LevelLoader.offset.Y) * PlatformData.Spacing);

	// TODO: Temporary, should make win UI
	public void Win()
	{
		ClearLevel();
		ChangeScene(SceneId.MainMenu);
	}

	public void RemovePlatform(Vector2I pos)
	{
		tileMap[pos].OnRemovalRequested -= RemovePlatform;
		platformRenderingServer.FreePlatform(tileMap[pos]);
		tileMap.Remove(pos);
	}

	private void ClearLevel()
	{
		foreach (Platform platform in tileMap.Values)
		{
			platform.OnRemovalRequested -= RemovePlatform;
		}

		platformRenderingServer.ClearLevel();
		tileMap.Clear();
	}

	public void UpdatePlatformShader(Platform instance, string param, Variant value) => platformRenderingServer.UpdatePlatformShader(instance, param, value);

	private static int GridWidth => grid.GetLength(1);
	private static int GridHeight => grid.GetLength(0);
}
