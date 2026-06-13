using GdUnit4;
using GdUnit4.Constraints;
using LostInSpace.Scripts.UI;
using LostInSpace.Scripts.UI.Scenes.MainMenu;
using static GdUnit4.Assertions;

namespace LostInSpace.Tests.UI;

[TestSuite]
[RequireGodotRuntime]
public class MainMenuTests
{
	private ISceneRunner runner = null!;
    private MainMenu mainScene = null!;

	[Before]
    public void Before()
    {
        runner = ISceneRunner.Load("res://Resources/Scenes/UI/MainMenu.tscn", true);
        mainScene = runner.Scene() as MainMenu
				?? throw new InvalidOperationException("Could not cast scene");
    }

	[TestCase(SceneId.LevelSelector)]
    public async Task MainMenu_OnButtonClick_ShouldInvokeCorrectSignal(SceneId changeToSceneId)
    {
		// Arrange
		ISignalConstraint monitor = AssertSignal(mainScene).StartMonitoring();

        // Act
        mainScene.OnMenuButtonPressed(changeToSceneId);

		// Assert
        await monitor.IsEmitted(MainMenu.SignalName.RequestSceneChange, 1).WithTimeout(100);
    }
}
