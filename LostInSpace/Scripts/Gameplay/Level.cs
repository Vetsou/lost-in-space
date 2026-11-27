using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Gameplay.Player;
using LostInSpace.Scripts.Gameplay.Systems;
using LostInSpace.Scripts.UI;
using LostInSpace.Scripts.UI.Scenes.PauseMenu;
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
	private LevelSession _levelSession;
	private PauseMenu _pauseMenu;
	private MovementSystem _movement;
	private StringName _pause = "ui_cancel";

	#region HUD signals
	[Signal] public delegate void LevelLoadedEventHandler(string levelId);
	[Signal] public delegate void StepsChangedEventHandler(int steps);
	[Signal] public delegate void BestStepsChangedEventHandler(int best);
	[Signal] public delegate void PrimaryPointsChangedEventHandler(int c, int t);
	[Signal] public delegate void OptionalPointsChangedEventHandler(int c, int t);
	#endregion

	public override void _Ready()
	{
		_levelSession = new LevelSession(_renderer);
		_pauseMenu = GetNode<PauseMenu>("PauseMenu");

		_levelSession.PrimaryChanged += OnPrimaryChanged;
		_levelSession.OptionalChanged += OnOptionalChanged;
		_levelSession.StepsChanged += OnStepsChanged;
		_levelSession.BestStepsChanged += OnBestStepsChanged;
		_levelSession.LevelCompleted += OnLevelCompleted;

		_hud.Bind(this);
	}

	public override void _ExitTree()
	{
		_levelSession.PrimaryChanged -= OnPrimaryChanged;
		_levelSession.OptionalChanged -= OnOptionalChanged;
		_levelSession.StepsChanged -= OnStepsChanged;
		_levelSession.BestStepsChanged -= OnBestStepsChanged;
		_levelSession.LevelCompleted -= OnLevelCompleted;

		_hud.Unbind();
		_renderer.Clear();
	}

	public override void _Input(InputEvent @event)
	{
		if (_movement != null && InputReader.TryReadDirection(@event, out Vector2I dir))
		{
			_movement.TryMove(dir);
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event.IsActionPressed(_pause))
		{
			PauseLevel();
		}
	}

	public override void _Notification(int what)
	{
		switch(what)
		{
			case (int)NotificationPaused:
				_hud.Visible = false;
				Engine.TimeScale = 0;
			break;
			case (int)NotificationUnpaused:
				_hud.Visible = true;
				Engine.TimeScale = 1;
			break;
			default:
			break;
		}
	}

	public void LoadLevel(string levelFilePath)
	{
		_currentLevelPath = levelFilePath;

		LevelData data = JsonConvert.DeserializeObject<LevelData>(FileAccess.GetFileAsString(levelFilePath));
		data.Validate();

		_state = new LevelState();
		_state.Initialize(data);

		_levelSession.StartSession(_state);
		_movement = new MovementSystem(_state, _playerView, _levelSession, _renderer);

		_renderer.RenderLevel(_state);
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

	public void PauseLevel()
	{
		_pauseMenu.TooglePause();
	}

	private void OnPrimaryChanged()
	{
		EmitSignal(SignalName.PrimaryPointsChanged,
			_levelSession.PrimaryCurrent, _state.PrimaryPointsTotal);

		if (_levelSession.AllPrimaryCollected)
		{
			_renderer.UnlockExits(_state);
		}
	}

	private void OnOptionalChanged() =>
		EmitSignal(SignalName.OptionalPointsChanged,
			_levelSession.OptionalCurrent, _state.OptionalPointsTotal);

	private void OnStepsChanged(ushort steps) =>
		EmitSignal(SignalName.StepsChanged, (int)steps);

	private void OnBestStepsChanged(ushort best) =>
		EmitSignal(SignalName.BestStepsChanged, (int)best);

	private void OnLevelCompleted()
	{
		_hud.Unbind();
		_renderer.Clear();
		ChangeScene(SceneId.LevelSelector);
	}

	public void ChangeSceneExt(SceneId sceneId)
	{
		ChangeScene(sceneId);
	}
}
