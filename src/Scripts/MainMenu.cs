using Godot;
using System;

public partial class MainMenu : Scene
{
	private void OnStartPressed()
    {
        ChangeScene("LEVEL_SELECTOR");
    }

	private void OnExitPressed()
    {
        GetTree().Quit();
    }
}
