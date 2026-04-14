using Godot;
using System;

public partial class MainMenu : Scene
{
	private void OnStartPressed()
    {
        EmitSignal(SignalName.RequestSceneChange, "LEVEL_SELECTOR");
    }

	private void OnExitPressed()
    {
        GetTree().Quit();
    }
}
