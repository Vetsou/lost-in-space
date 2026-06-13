namespace LostInSpace.Scripts.UI.Scenes.MainMenu;

public partial class MainMenu : Scene
{
	public void OnMenuButtonPressed(SceneId id) => ChangeScene(id);
	public void OnExitPressed() => GetTree().Quit();
}
