namespace LostInSpace.Scripts.UI.Scenes.MainMenu;

public partial class MainMenu : Scene
{
	private void OnMenuButtonPressed(SceneId id) => ChangeScene(id);
	private void OnExitPressed() => GetTree().Quit();
}
