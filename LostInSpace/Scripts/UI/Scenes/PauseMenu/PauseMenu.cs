using Godot;
using LostInSpace.Scripts.Gameplay;
using LostInSpace.Scripts.UI.Scenes.Settings;
namespace LostInSpace.Scripts.UI.Scenes.PauseMenu;

public partial class PauseMenu : Control
{
	private StringName _pause = "ui_cancel";
	private Level _level;
	private CenterContainer _foreground;
	private readonly PackedScene _optionsScene = GD.Load<PackedScene>("res://Resources/Scenes/UI/Settings/SettingsMenu.tscn");

	public override void _Ready()
	{
		_level = GetParent<Level>();
		_foreground = GetNode<CenterContainer>("Foreground");
		Hide();
	}

	public override void _ExitTree()
	{
		_level = null;
		_foreground = null;
	}


	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event.IsActionPressed(_pause))
		{
			ResumeButtonPressed();
			GetViewport().SetInputAsHandled(); 
		}
	}

	public void TooglePause()
	{
		bool isPaused = GetTree().Paused;
		GetTree().Paused = !isPaused;
		Visible = !isPaused;
		
	}

	private void ResumeButtonPressed()
	{
		TooglePause();
	}

	private void RestartButtonPressed()
	{
		TooglePause();
		_level.ResetLevel();
	}

	private void ExitSceneButtonPressed(int buttonDistinguisher)
	{
		TooglePause();
		switch(buttonDistinguisher)
		{
			case 0:
				_level.ChangeSceneExt(SceneId.LevelSelector);
			break;
			case 1:
				_level.ChangeSceneExt(SceneId.MainMenu);
			break;
			default:
			break;
		}
		
	}
	private void OnOptionsButtonPressed()
	{
		SettingsMenu optionsInstance = _optionsScene.Instantiate<SettingsMenu>();
		optionsInstance.ProcessMode = ProcessModeEnum.WhenPaused;
		optionsInstance.OpenedFromPause = true;
		optionsInstance.TreeExited += OnOptionsClosed;
		_foreground.Hide();
		GetTree().Root.AddChild(optionsInstance);
	}

	private void OnOptionsClosed()
	{
		if (IsInstanceValid(this))
		{
			_foreground.Show();
		}
	}
}
