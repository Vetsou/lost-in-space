using System.Runtime.CompilerServices;
using Godot;
using LostInSpace.Scripts.Gameplay.Collectibles;
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
	private IPlatform[] _platforms;
	private bool[] _primaryPoints;
	private bool[] _optionalPoints;

	public ushort PrimaryPointsTotalCount { get; private set; }
	public ushort PrimaryPointsCurrentCount { get; private set; }
	public ushort OptionalPointsTotalCount { get; private set; }
	public ushort OptionalPointsCurrentCount { get; private set; }

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
		_platforms = new IPlatform[MapSize.X * MapSize.Y];
		_primaryPoints = new bool[MapSize.X * MapSize.Y];
		_optionalPoints = new bool[MapSize.X * MapSize.Y];
		PrimaryPointsCurrentCount = 0;
		PrimaryPointsTotalCount = 0;
		OptionalPointsCurrentCount = 0;
		OptionalPointsTotalCount = 0;

		for (int i = 0; i < MapSize.X; i++)
		{
			for (int j = 0; j < MapSize.Y; j++)
			{
				var gridPos = new Vector2I(i, j);
				if (levelData.Platforms[j, i] != 0)
				{
					IPlatform platform = PlatformFactory.CreatePlatform(levelData.Platforms[j, i]);
					_platforms[GridToIndex(gridPos)] = platform;
					_levelRenderingServer.RenderPlatform(gridPos, platform.VisualData);
				}

				bool primaryPoint = levelData.Collectibles[j, i] == 1;
				_primaryPoints[GridToIndex(gridPos)] = primaryPoint;
				if (primaryPoint)
				{
					PrimaryPointsTotalCount++;
					_levelRenderingServer.RenderCollectible(gridPos, CollectibleData.PrimaryCollectibleVisualData);
				}

				bool optionalPoint = levelData.Collectibles[j, i] == 2;
				_optionalPoints[GridToIndex(gridPos)] = optionalPoint;
				if (optionalPoint)
				{
					OptionalPointsTotalCount++;
					_levelRenderingServer.RenderCollectible(gridPos, CollectibleData.OptionalCollectibleVisualData);
				}
			}
		}

		_player.SetPosition(new Vector2I(levelData.PlayerPositionX, levelData.PlayerPositionY));
	}

	private bool IsPositionValid(Vector2I pos) => pos.X >= 0 && pos.Y >= 0 && pos.X < MapSize.X && pos.Y < MapSize.Y;

	public IPlatform GetPlatform(Vector2I pos) => IsPositionValid(pos) ? _platforms[GridToIndex(pos)] : null;
	private bool GetPrimaryPoint(Vector2I pos) => IsPositionValid(pos) && _primaryPoints[GridToIndex(pos)];
	private bool GetOptionalPoint(Vector2I pos) => IsPositionValid(pos) && _optionalPoints[GridToIndex(pos)];

	public void PickUpCollectible(Vector2I pos)
	{
		if (GetPrimaryPoint(pos))
		{
			PrimaryPointsCurrentCount++;
			_levelRenderingServer.FreeCollectible(pos);
			_primaryPoints[GridToIndex(pos)] = false;
		}

		if (GetOptionalPoint(pos))
		{
			OptionalPointsCurrentCount++;
			_levelRenderingServer.FreeCollectible(pos);
			_optionalPoints[GridToIndex(pos)] = false;
		}
	}

	public void CompleteLevel()
	{
		ClearLevel();
		ChangeScene(SceneId.MainMenu);
	}

	public void RemovePlatform(Vector2I pos)
	{
		IPlatform platform = GetPlatform(pos);
		if (platform == null)
		{
			return;
		}

		_levelRenderingServer.FreePlatform(pos);
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
			_platforms[i] = null;
			_primaryPoints[i] = false;
			_optionalPoints[i] = false;
		}

		_levelRenderingServer.ClearLevel();
	}

	public void UpdatePlatformColor(Vector2I pos, Color color) => _levelRenderingServer.UpdatePlatformColor(pos, color);

	public void UpdatePlatformRenderData(Vector2I pos, Color color) => _levelRenderingServer.UpdatePlatformData(pos, color);
}
