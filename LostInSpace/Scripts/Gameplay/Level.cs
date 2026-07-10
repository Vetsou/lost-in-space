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
	private Collectible[] _collectibles;
	private ushort[] _pickedUpCollectibleCounts = new ushort[(int)Collectible.Count];
	private ushort[] _startCollectibleCounts = new ushort[(int)Collectible.Count];

	public override void _Ready() => MovementSystem = new MovementSystem(this);
	public override void _ExitTree() => ClearLevel();
	public override void _Input(InputEvent @event) => MovementSystem.HandlePlayerMovement(_player, @event);

	public override void _Process(double delta)
	{
		// if (Engine.GetFramesDrawn() % 20 == 0)
		// {
		// 	GD.Print("FPS: ", Performance.GetMonitor(Performance.Monitor.TimeFps));
		// 	GD.Print("Memory static: ", Performance.GetMonitor(Performance.Monitor.MemoryStatic));
		// 	GD.Print("Draw Calls: ", Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame));
		// 	GD.Print("Video Mem: ", Performance.GetMonitor(Performance.Monitor.RenderVideoMemUsed));
		// }
	}

	public void LoadLevel(string levelFilePath)
	{
		LevelData levelData = JsonConvert.DeserializeObject<LevelData>(FileAccess.GetFileAsString(levelFilePath));
		levelData.Validate();

		MapSize = new Vector2I(levelData.Width, levelData.Height);
		_levelRenderingServer.SetBatchSize(MapSize.X * MapSize.Y);
		_platforms = new IPlatform[MapSize.X * MapSize.Y];
		_collectibles = new Collectible[MapSize.X * MapSize.Y];

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

				var collectible = (Collectible)levelData.Collectibles[j, i];
				_collectibles[GridToIndex(gridPos)] = collectible;
				_startCollectibleCounts[(ushort)collectible]++;
				if (collectible != Collectible.None)
				{
					_levelRenderingServer.RenderCollectible(gridPos, collectible);
				}
			}
		}

		_player.SetPosition(new Vector2I(levelData.PlayerPositionX, levelData.PlayerPositionY));
	}

	private bool IsPositionValid(Vector2I pos) => pos.X >= 0 && pos.Y >= 0 && pos.X < MapSize.X && pos.Y < MapSize.Y;

	public IPlatform GetPlatform(Vector2I pos) => IsPositionValid(pos) ? _platforms[GridToIndex(pos)] : null;
	private Collectible GetCollectible(Vector2I pos) => IsPositionValid(pos) ? _collectibles[GridToIndex(pos)] : Collectible.None;

	public void PickUpCollectible(Vector2I pos)
	{
		Collectible collectible = GetCollectible(pos);
		if (collectible >= Collectible.Count)
		{
			throw new Exception($"Invalid collectible id {(int)collectible} at position x={pos.X}, y={pos.Y}");
		}
		if (collectible <= Collectible.None)
		{
			return;
		}

		_pickedUpCollectibleCounts[(int)collectible]++;
		_levelRenderingServer.FreeCollectible(pos);
		_collectibles[GridToIndex(pos)] = Collectible.None;
	}

	public ushort GetCollectiblePickedUpCount(Collectible collectible)
	{
		if (collectible <= Collectible.None || collectible >= Collectible.Count)
		{
			throw new Exception($"Invalid collectible id {(int)collectible}");
		}

		return _pickedUpCollectibleCounts[(int)collectible];
	}

	public ushort GetCollectibleStartCount(Collectible collectible)
	{
		if (collectible <= Collectible.None || collectible >= Collectible.Count)
		{
			throw new Exception($"Invalid collectible id {(int)collectible}");
		}

		return _startCollectibleCounts[(int)collectible];
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
			_collectibles[i] = Collectible.None;
		}

		_levelRenderingServer.ClearLevel();
	}

	public void UpdatePlatformColor(Vector2I pos, Color color) => _levelRenderingServer.UpdatePlatformColor(pos, color);

	public void UpdatePlatformRenderData(Vector2I pos, Color color) => _levelRenderingServer.UpdatePlatformData(pos, color);
}
