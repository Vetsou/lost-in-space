using System.Runtime.CompilerServices;
using LostInSpace.Scripts.Gameplay.Data;
using Godot;
using Newtonsoft.Json;
using FileAccess = Godot.FileAccess;
using LostInSpace.Scripts.Gameplay.Platforms;
using LostInSpace.Scripts.Gameplay.Managers;
using LostInSpace.Scripts.Rendering;
using LostInSpace.Scripts.UI;

namespace LostInSpace.Scripts.Gameplay;

public partial class Level : Scene
{
	[Export] private PlatformRenderingServer _platformRenderingServer;
	[Export] private PlayerData _player;

	private Vector2I MapSize { get; set; }
	private Platform[] _platforms;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int GridToIndex(Vector2I pos) => pos.Y * MapSize.X + pos.X;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Vector2I IndexToGrid(int index) => new(index % MapSize.X, index / MapSize.X);

	public override void _ExitTree() => ClearLevel();

	public override void _Input(InputEvent @event)
	{
		if (!InputManager.GetPlayerMovementDirection(@event, out Vector2I direction))
		{
			return;
		}

		HandlePlayerMovement(direction);
	}

	public void HandlePlayerMovement(Vector2I direction)
	{
		Platform nextTile = GetTile(_player.GridPosition + direction);
		if (nextTile == null)
		{
			return;
		}

		var context = new TileContext
		{
			Level = this,
			MoveDirection = direction
		};

		Platform currPlatform = GetTile(_player.GridPosition);
		currPlatform.OnExit(context);

		_player.SetPosition(_player.GridPosition + direction);

		Platform newPlatform = GetTile(_player.GridPosition);
		newPlatform?.OnEnter(context);
	}

	public void LoadLevel(string levelFilePath)
	{
		LevelData levelData = JsonConvert.DeserializeObject<LevelData>(FileAccess.GetFileAsString(levelFilePath));
		levelData.Validate();

		MapSize = new Vector2I(levelData.Width, levelData.Height);
		_platforms = new Platform[MapSize.X * MapSize.Y];

		for (int i = 0; i < MapSize.X; i++)
		{
			for (int j = 0; j < MapSize.Y; j++)
			{
				if (levelData.Platforms[j, i] == 0)
				{
					continue;
				}

				var gridPos = new Vector2I(i, j);

				Platform platform = PlatformRegistry.CreatePlatform(levelData.Platforms[j, i], gridPos);
				_platforms[GridToIndex(gridPos)] = platform;
				_platformRenderingServer.RenderPlatform(platform);
			}
		}

		_player.SetPosition(new Vector2I(levelData.PlayerPositionX, levelData.PlayerPositionY));
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

		_platformRenderingServer.FreePlatform(platform);
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

			_platforms[i] = null;
		}

		_platformRenderingServer.ClearLevel();
	}
}
