using GdUnit4;
using GdUnit4.Constraints;
using Godot;
using LostInSpace.Scripts.UI;
using LostInSpace.Scripts.UI.Scenes.LevelSelector;
using static GdUnit4.Assertions;

namespace LostInSpace.Tests.UI;

[TestSuite]
[RequireGodotRuntime]
public class LevelSelectorTests
{
	private ISceneRunner runner = null!;
	private LevelSelector levelSelectorScene = null!;

	[Before]
	public void Before()
	{
		runner = ISceneRunner.Load("res://Resources/Scenes/UI/LevelSelector.tscn", true);
		levelSelectorScene = runner.Scene() as LevelSelector
				?? throw new InvalidOperationException("Could not cast scene");
	}

	[TestCase]
	public async Task LevelButton_OnButtonClick_ShouldSetNewLevelData()
	{
		// Arrange
		Button btn = runner.FindChild("Level_1") as Button
				?? throw new InvalidOperationException("Could not find Level_1 button");
		Label levelNameLabel = runner.FindChild("LevelName") as Label
				?? throw new InvalidOperationException("Could not find LevelName label");

		// Act
		Vector2 pressPos = btn.GetGlobalRect().GetCenter();
		runner.SetMousePos(pressPos);
		runner.SimulateMouseButtonPressed(MouseButton.Left);
		await runner.AwaitInputProcessed();

		// Assert
		AssertThat(levelNameLabel.Text).IsEqual("1");
	}

	[TestCase]
	public async Task ReturnButton_OnButtonClick_ShouldInvokeMainMenuSignal()
	{
		// Arrange
		Button btn = runner.FindChild("ReturnButton") as Button
				?? throw new InvalidOperationException("Could not find Return button");
		ISignalConstraint monitor = AssertSignal(levelSelectorScene).StartMonitoring();

		// Act
		Vector2 pressPos = btn.GetGlobalRect().GetCenter();
		runner.SetMousePos(pressPos);
		runner.SimulateMouseButtonPressed(MouseButton.Left);
		await runner.AwaitInputProcessed();

		// Assert
		await monitor.IsEmitted(LevelSelector.SignalName.RequestSceneChange, (int)SceneId.MainMenu).WithTimeout(60);
	}
}
