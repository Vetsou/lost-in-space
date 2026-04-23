using Godot;

public partial class LevelSelector : Scene
{
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		// This is example input handling for testing. Remove it when implementing LevelSelector.
		if (Input.IsActionPressed(SettingsMap.Keys.MOVE_LEFT))
		{
			GD.Print("Moving left");
		}

		if (Input.IsActionPressed(SettingsMap.Keys.MOVE_RIGHT))
		{
			GD.Print("Moving right");
		}
	}
}
