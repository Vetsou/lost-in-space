using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Gameplay.Player;
using LostInSpace.Scripts.Gameplay.Systems;
using LostInSpace.Scripts.UI;
using Newtonsoft.Json;
using FileAccess = Godot.FileAccess;

namespace LostInSpace.Scripts.Gameplay;

public partial class Level : Scene
{
	[Export] private LevelRenderer _renderer;
	[Export] private PlayerView _playerView;
	[Export] private LevelHud _hud;

	private string _currentLevelPath;

	private LevelState _state;
	private PlayerData _playerData;
	private CollectibleSystem _collectibles;
	private LevelProgression _progression;
	private MovementSystem _movement;

	#region HUD signals
	[Signal] public delegate void LevelLoadedEventHandler(string levelId);
	[Signal] public delegate void StepsChangedEventHandler(int steps);
	[Signal] public delegate void BestStepsChangedEventHandler(int best);
	[Signal] public delegate void PrimaryPointsChangedEventHandler(int c, int t);
	[Signal] public delegate void OptionalPointsChangedEventHandler(int c, int t);
	#endregion

	public override void _Ready()
	{
		_playerData = new PlayerData();
		_hud.Bind(this);
	}

	public override void _ExitTree()
	{
		_hud?.Unbind();
		_renderer.Clear();
	}

	public override void _Input(InputEvent @event)
	{
		if (_movement != null && InputReader.TryReadDirection(@event, out Vector2I dir))
		{
			_movement.TryMove(dir);
		}
	}

	public void LoadLevel(string levelFilePath)
	{
		_currentLevelPath = levelFilePath;
		_renderer.Clear();

		LevelData data = JsonConvert.DeserializeObject<LevelData>(FileAccess.GetFileAsString(levelFilePath));
		data.Validate();

		_state = new LevelState();
		_state.Initialize(data);

		_collectibles = new CollectibleSystem(_state, _renderer);
		_progression = new LevelProgression { LevelId = _state.LevelId };

		_movement = new MovementSystem(
			_state, _playerData, _playerView,
			_collectibles, _progression, _renderer);

		_renderer.RenderState(_state);

		_playerData.SetPosition(_state.PlayerStart);
		_playerView.SetTo(_state.PlayerStart);

		EmitSignal(SignalName.LevelLoaded, _state.LevelId);
		EmitSignal(SignalName.StepsChanged, 0);
		EmitSignal(SignalName.BestStepsChanged, 0);
		EmitSignal(SignalName.PrimaryPointsChanged, 0, _state.PrimaryPointsTotal);
		EmitSignal(SignalName.OptionalPointsChanged, 0, _state.OptionalPointsTotal);
	}

	public void ResetLevel()
	{
		if (_currentLevelPath != null)
		{
			LoadLevel(_currentLevelPath);
		}
	}
}
