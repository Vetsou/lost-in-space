using System;
using Godot;

public partial class MainMenu : Scene
{
	private void OnStartPressed()
	{
		ChangeScene(SceneId.LEVEL_SELECTOR);
	}

	private void OnExitPressed()
	{
		GetTree().Quit();
	}
}
