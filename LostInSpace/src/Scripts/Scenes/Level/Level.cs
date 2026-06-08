using System.Runtime.CompilerServices;
using Godot;
using Newtonsoft.Json;
using FileAccess = Godot.FileAccess;

public partial class Level : Scene
{
	public const float SPACING = 1;

	[Export] private PlatformRenderingServer platformRenderingServer;
	[Export] private Player player;

	private int _width;
	private int _height;
	private Platform[] _platforms;

	public override void _Process(double delta)
	{
	}

	public override void _ExitTree() => ClearLevel();

	public void LoadLevel(string levelFilePath)
	{
		LevelData levelData = JsonConvert.DeserializeObject<LevelData>(FileAccess.GetFileAsString(levelFilePath));
		levelData.Validate();

		_width = levelData.Width;
		_height = levelData.Height;
		_platforms = new Platform[_width * _height];

		for (int i = 0; i < _width; i++)
		{
			for (int j = 0; j < _height; j++)
			{
				if (levelData.Platforms[j, i] == 0)
				{
					continue;
				}

				Platform platform = PlatformRegistry.CreatePlatform(levelData.Platforms[j, i]);
				platform.OnRemovalRequested += RemovePlatform;

				var gridPos = new Vector2I(i, j);
				platform.SetPosition(gridPos);
				_platforms[GridToIndex(gridPos)] = platform;

				platformRenderingServer.RenderPlatform(platform);
			}
		}

		player.Init(this, new Vector2I(levelData.PlayerPositionX, levelData.PlayerPositionY));
	}

	public Platform GetTile(Vector2I pos)
	{
		int index = GridToIndex(pos);
		if (index < 0 || index >= _platforms.Length)
		{
			return null;
		}
		return _platforms[index];
	}

	public Vector3 GridToWorld(Vector2I pos) => new(pos.X * SPACING, 0, pos.Y * SPACING);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int GridToIndex(Vector2I pos) => pos.Y * _width + pos.X;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Vector2I IndexToGrid(int index) => new Vector2I(index % _width, index / _width);

	// TODO: Temporary, should make win UI
	public void Win()
	{
		ClearLevel();
		ChangeScene(SceneId.MainMenu);
	}

	public void RemovePlatform(Vector2I pos)
	{
		Platform platform = _platforms[GridToIndex(pos)];
		if (platform == null)
		{
			return;
		}

		platform.OnRemovalRequested -= RemovePlatform;
		platformRenderingServer.FreePlatform(platform);
		_platforms[GridToIndex(pos)] = null;
	}

	private void ClearLevel()
	{
		for (int i = 0; i < _platforms.Length; i++)
		{
			Platform platform = _platforms[i];
			if (platform == null)
			{
				continue;
			}

			platform.OnRemovalRequested -= RemovePlatform;
			_platforms[i] = null;
		}

		platformRenderingServer.ClearLevel();
	}

	public void UpdatePlatformShader(Platform instance, string param, Variant value) =>
		platformRenderingServer.UpdatePlatformShader(instance, param, value);
}
