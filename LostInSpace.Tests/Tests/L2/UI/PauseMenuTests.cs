
using GdUnit4;
using GdUnit4.Constraints;
using Godot;
using LostInSpace.Scripts.Gameplay;
using LostInSpace.Scripts.UI;
using LostInSpace.Scripts.UI.Scenes.PauseMenu;
using LostInSpace.Scripts.UI.Scenes.Settings;
using static GdUnit4.Assertions;

namespace LostInSpace.Tests.L2.UI;

[TestSuite]
[RequireGodotRuntime]
public class PauseMenuTests
{
    private ISceneRunner runner = null!;
    private Level level = null!;
    private PauseMenu pauseMenu = null!;
    private SettingsMenu settingsMenu = null!;

   [Before]
    public void Before()
    {

        runner = ISceneRunner.Load(
            "res://Resources/Scenes/Gameplay/Level.tscn", true);

        level = runner.Scene() as Level
            ?? throw new InvalidOperationException("Could not cast scene");

        pauseMenu = runner.FindChild("PauseMenu") as PauseMenu
            ?? throw new InvalidOperationException("Could not find PauseMenu");

        pauseMenu.GetTree().Paused = false;

    }

    [After]
    public void After()
    {
        if (GodotObject.IsInstanceValid(settingsMenu))
        {
            settingsMenu.Free();
        }

        if (GodotObject.IsInstanceValid(pauseMenu))
        {
            SceneTree tree = pauseMenu.GetTree();
            tree.Paused = false;
            pauseMenu.Hide();
        }
    }

    [TestCase]
    public void PauseMenu_OnReady_ShouldBeHidden()
    {
        // Assert
        AssertBool(pauseMenu.Visible).IsFalse();
        AssertBool(pauseMenu.GetTree().Paused).IsFalse();
    }

    [TestCase]
    public void PauseMenu_OnTogglePause_ShouldPauseGameAndShowMenu()
    {
        // Act
        level.PauseLevel();

        // Assert
        AssertBool(pauseMenu.GetTree().Paused).IsTrue();
        AssertBool(pauseMenu.Visible).IsTrue();
    }

    [TestCase]
    public void PauseMenu_OnTogglePauseTwice_ShouldResumeGameAndHideMenu()
    {
        // Arrange
        SceneTree tree = pauseMenu.GetTree();
        tree.Paused = false;
        pauseMenu.Hide();

        // Act
        level.PauseLevel();

        AssertBool(tree.Paused).IsTrue();
        AssertBool(pauseMenu.Visible).IsTrue();

        pauseMenu.TooglePause();

        // Assert
        AssertBool(tree.Paused).IsFalse();
        AssertBool(pauseMenu.Visible).IsFalse();
    }
    [TestCase]
    public async Task PauseMenu_OnResumeButtonClick_ShouldResumeGame()
    {
        // Arrange
        level.PauseLevel();

        Button btn = runner.FindChild("ResumeButton") as Button
            ?? throw new InvalidOperationException("Could not find ResumeButton");

        // Act
        Vector2 pressPos = btn.GetGlobalRect().GetCenter();
        runner.SetMousePos(pressPos);
        runner.SimulateMouseButtonPressed(MouseButton.Left);
        await runner.AwaitInputProcessed();

        // Assert
        AssertBool(pauseMenu.GetTree().Paused).IsFalse();
        AssertBool(pauseMenu.Visible).IsFalse();
    }

    [TestCase("LevelSelectButton", SceneId.LevelSelector)]
    [TestCase("ReturnToMenuButton", SceneId.MainMenu)]
    public async Task PauseMenu_OnExitButtonClick_ShouldInvokeCorrectSignal(
        string btnName, SceneId sceneId)
    {
        // Arrange
        level.PauseLevel();

        Button btn = runner.FindChild(btnName) as Button
            ?? throw new InvalidOperationException($"Could not find {btnName}");

        ISignalConstraint monitor = AssertSignal(level).StartMonitoring();

        // Act
        Vector2 pressPos = btn.GetGlobalRect().GetCenter();
        runner.SetMousePos(pressPos);
        runner.SimulateMouseButtonPressed(MouseButton.Left);
        await runner.AwaitInputProcessed();

        // Assert
        await monitor.IsEmitted(
            Scene.SignalName.RequestSceneChange,
            (int)sceneId
        ).WithTimeout(60);

        AssertBool(pauseMenu.GetTree().Paused).IsFalse();
        AssertBool(pauseMenu.Visible).IsFalse();
    }

    [TestCase]
    public async Task PauseMenu_OnRestartButtonClick_ShouldResumeGame()
    {
        // Arrange
        SceneTree tree = pauseMenu.GetTree();
        tree.Paused = false;
        pauseMenu.Hide();

        level.PauseLevel();

        Button btn = runner.FindChild("RestartButton") as Button
            ?? throw new InvalidOperationException("Could not find RestartButton");

        // Act
        Vector2 pressPos = btn.GetGlobalRect().GetCenter();
        runner.SetMousePos(pressPos);
        runner.SimulateMouseButtonPressed(MouseButton.Left);
        await runner.AwaitInputProcessed();

        // Assert
        AssertBool(tree.Paused).IsFalse();
        AssertBool(pauseMenu.Visible).IsFalse();
    }

    [TestCase]
    public async Task PauseMenu_OnSettingsButtonClick_ShouldOpenSettingsMenu()
    {
        // Arrange
        SceneTree tree = pauseMenu.GetTree();
        tree.Paused = false;
        pauseMenu.Hide();

        level.PauseLevel();

        Button btn = runner.FindChild("SettingsButton") as Button
            ?? throw new InvalidOperationException("Could not find SettingsButton");

        // Act
        Vector2 pressPos = btn.GetGlobalRect().GetCenter();
        runner.SetMousePos(pressPos);
        runner.SimulateMouseButtonPressed(MouseButton.Left);
        await runner.AwaitInputProcessed();

        // Assert
        settingsMenu = tree.Root.FindChild("SettingsMenu", true, false) as SettingsMenu
            ?? throw new InvalidOperationException("Could not find SettingsMenu");

        AssertBool(settingsMenu.OpenedFromPause).IsTrue();
        AssertBool(settingsMenu.ProcessMode == Node.ProcessModeEnum.WhenPaused).IsTrue();
        AssertBool(pauseMenu.GetNode<CenterContainer>("Foreground").Visible).IsFalse();
    }

    [TestCase]
    public async Task PauseMenu_OnSettingsClosed_ShouldShowForeground()
    {
        // Arrange
        SceneTree tree = pauseMenu.GetTree();
        tree.Paused = false;
        pauseMenu.Hide();

        level.PauseLevel();

        Button btn = runner.FindChild("SettingsButton") as Button
            ?? throw new InvalidOperationException("Could not find SettingsButton");

        Vector2 pressPos = btn.GetGlobalRect().GetCenter();
        runner.SetMousePos(pressPos);
        runner.SimulateMouseButtonPressed(MouseButton.Left);
        await runner.AwaitInputProcessed();

        settingsMenu = tree.Root.FindChild("SettingsMenu", true, false) as SettingsMenu
            ?? throw new InvalidOperationException("Could not find SettingsMenu");

       // Act
        settingsMenu.Free();
        await runner.AwaitInputProcessed();


       // Assert
        AssertBool(pauseMenu.GetNode<CenterContainer>("Foreground").Visible).IsTrue();
        AssertBool(tree.Paused).IsTrue();
    }
}
