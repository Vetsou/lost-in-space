using GdUnit4;
using GdUnit4.Constraints;
using Godot;
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

	[TestCase("PlayButton", SceneId.LevelSelector)]
    [TestCase("CreateLevelButton", SceneId.CreateLevel)]
    [TestCase("AchievementsButton", SceneId.Achievements)]
    [TestCase("SettingsButton", SceneId.SettingsMenu)]
    public async Task MainMenu_OnButtonClick_ShouldInvokeCorrectSignal(string btnName, SceneId sceneId)
    {
		// Arrange
        Button btn = runner.FindChild(btnName) as Button
                ?? throw new InvalidOperationException("Could not find PlayButton");
		ISignalConstraint monitor = AssertSignal(mainScene).StartMonitoring();

		// Act
		Vector2 pressPos = btn.GetGlobalRect().GetCenter();
		runner.SetMousePos(pressPos);
		runner.SimulateMouseButtonPressed(MouseButton.Left);
		await runner.AwaitInputProcessed();

		// Assert
		await monitor.IsEmitted(MainMenu.SignalName.RequestSceneChange, (int)sceneId).WithTimeout(100);
    }
}
