using Godot;

public partial class LevelSelector : Scene
{
	private void OnPlayButtonPressed() => ChangeScene(SceneId.Level);
	private void OnReturnButtonPressed() => ChangeScene(SceneId.MainMenu);
}
