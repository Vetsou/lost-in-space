using System.Runtime.CompilerServices;
using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Gameplay.Platforms;
using LostInSpace.Scripts.Gameplay.Systems;
using LostInSpace.Scripts.Rendering;
using LostInSpace.Scripts.UI;
using Newtonsoft.Json;
using FileAccess = Godot.FileAccess;

namespace LostInSpace.Scripts.Gameplay;

public partial class Level : Scene, ILevelHandler
{
	[Export] private LevelRenderingServer _levelRenderingServer;
	[Export] private PlayerData _player;

	private MovementSystem MovementSystem { get; set; }

	private Vector2I MapSize { get; set; }
	private Platform[] _platforms;

	public override void _Ready() => MovementSystem = new MovementSystem(this);
	public override void _ExitTree() => ClearLevel();
	public override void _Input(InputEvent @event) => MovementSystem.HandlePlayerMovement(_player, @event);

	public override void _Process(double delta)
	{
		if (Engine.GetFramesDrawn() % 20 == 0)
		{
			GD.Print("FPS: ", Performance.GetMonitor(Performance.Monitor.TimeFps));
			GD.Print("Memory static: ", Performance.GetMonitor(Performance.Monitor.MemoryStatic));
			GD.Print("Draw Calls: ", Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame));
			GD.Print("Video Mem: ", Performance.GetMonitor(Performance.Monitor.RenderVideoMemUsed));
		}
	}

	public void LoadLevel(string levelFilePath)
	{
		LevelData levelData = JsonConvert.DeserializeObject<LevelData>(FileAccess.GetFileAsString(levelFilePath));
		levelData.Validate();

		MapSize = new Vector2I(levelData.Width, levelData.Height);
		_levelRenderingServer.SetBatchSize(MapSize.X * MapSize.Y);
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
				_levelRenderingServer.RenderPlatform(platform);
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

	public void CompleteLevel()
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

		_levelRenderingServer.FreePlatform(platform);
		_platforms[GridToIndex(pos)] = null;
	}

	public void MovePlayer(Vector2I direction) => MovementSystem.MovePlayer(_player, direction);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int GridToIndex(Vector2I pos) => pos.Y * MapSize.X + pos.X;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Vector2I IndexToGrid(int index) => new(index % MapSize.X, index / MapSize.X);

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

		_levelRenderingServer.ClearLevel();
	}

	public void UpdatePlatformColor(Platform instance, Color color) => _levelRenderingServer.UpdatePlatformColor(instance, color);

	public void UpdatePlatformRenderData(Platform instance, Color color) => _levelRenderingServer.UpdatePlatformData(instance, color);
}
