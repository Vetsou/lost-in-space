using Godot;

public partial class Level : Scene
{
	[Export] private Node PlatformContainer;
	[Export] private Player Player;
	[Export] private PackedScene PointScene;
	[Export] public string LevelPath;

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
		ChangeScene(SceneId.MainMenu);
	}
}
