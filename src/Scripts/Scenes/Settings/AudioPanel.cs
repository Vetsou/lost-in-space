using Godot;
using System;

public partial class AudioPanel : Control
{
	public override void _Ready()
	{
	}

	private void OnMasterVolumeChanged(float value)
	{
		// SettingsManager.ChangeVolume();
	}
}
