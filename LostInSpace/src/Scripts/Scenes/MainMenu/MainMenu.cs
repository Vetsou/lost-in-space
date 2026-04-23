public partial class MainMenu : Scene
{
	private void OnStartPressed()
	{
		//ChangeScene(SceneId.LEVEL_SELECTOR);
		ChangeScene(SceneId.LEVEL_PLAYER);
	}

	private void OnSettingsPressed()
	{
		ChangeScene(SceneId.SETTINGS_MENU);
	}

	private void OnExitPressed()
	{
		GetTree().Quit();
	}
}
