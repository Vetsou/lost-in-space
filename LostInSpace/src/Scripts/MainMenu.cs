using System;
using Godot;

public partial class MainMenu : Scene
{
	private void OnStartPressed()
	{
		//ChangeScene(SceneId.LEVEL_SELECTOR);
		ChangeScene(SceneId.LEVEL_PLAYER);
	}

	private void OnExitPressed()
	{
		GetTree().Quit();
	}
}
