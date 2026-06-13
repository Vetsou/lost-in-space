using GdUnit4;
using Godot;
using LostInSpace.Scripts.UI.Scenes.Settings;
using static GdUnit4.Assertions;

namespace LostInSpace.Tests.UI;

[TestSuite]
[RequireGodotRuntime]
public class SettingsMenuTests
{
	private ISceneRunner runner = null;
	private SettingsMenu settingsMenuScene = null;

	[Before]
	public void Before()
	{
		PackedScene settingsMenuData = ResourceLoader.Load<PackedScene>("res://Resources/Scenes/UI/Settings/SettingsMenu.tscn");
		Node settingsMenuNode = settingsMenuData.Instantiate();

		// Assign autoload objects
		var configManager = new ConfigManager();
		settingsMenuNode.AddChild(configManager);

		runner = ISceneRunner.Load(settingsMenuNode, true);
		settingsMenuScene = runner.Scene() as SettingsMenu
				?? throw new InvalidOperationException("Could not cast scene");
	}

	private async Task ChangePanel(string btnName)
	{
		Button btn = runner.FindChild(btnName) as Button
				?? throw new InvalidOperationException($"Could not find {btnName}");

		Vector2 pressPos = btn.GetGlobalRect().GetCenter();
		runner.SetMousePos(pressPos);
		runner.SimulateMouseButtonPressed(MouseButton.Left);
		await runner.AwaitInputProcessed();
	}

	private Button GetButtonByName(string btnName)
	{
		return runner.FindChild(btnName) as Button
				?? throw new InvalidOperationException($"Could not find {btnName} button");
	}

	[TestCase]
	public async Task KeybindsPanel_Rebind_GoldenFlow()
	{
		// Arrange
		await ChangePanel("KeybindsButton");
		Button btn = GetButtonByName("MoveUpButton");

		// Act - Start rebind
		Vector2 pressPos = btn.GetGlobalRect().GetCenter();
		runner.SetMousePos(pressPos);
		runner.SimulateMouseButtonPressed(MouseButton.Left);
		await runner.AwaitInputProcessed();

		// Assert - Should set correct state
		AssertThat(btn.Text).IsEqual("Press a key...");
		AssertThat(btn.Disabled).IsEqual(true);

		// Act - Set rebind button
		runner.SimulateKeyPressed(Key.G);
		await runner.AwaitInputProcessed();

		// Assert - Should be set to correct key
		AssertThat(btn.Text).IsEqual("G");
		AssertThat(btn.Disabled).IsEqual(false);
		string moveUpKey = ConfigManager.Instance.GetSetting<string>(SettingsMap.Section.KEYBINDS, SettingsMap.Keys.MOVE_UP, "K");
		AssertThat(moveUpKey).IsEqual("G");
	}

	[TestCase]
	public async Task KeybindsPanel_OnResetButtonPress_ShouldSetDefaultSettings()
	{
		// Arrange
		await ChangePanel("KeybindsButton");
		Button resetBtn = GetButtonByName("ResetButton");

		Button upBtn = GetButtonByName("MoveUpButton");
		Button downBtn = GetButtonByName("MoveDownButton");
		Button leftBtn = GetButtonByName("MoveLeftButton");
		Button rightBtn = GetButtonByName("MoveRightButton");

		ConfigManager.Instance.SetSetting(SettingsMap.Section.KEYBINDS, SettingsMap.Keys.MOVE_UP, "Z");
		ConfigManager.Instance.SetSetting(SettingsMap.Section.KEYBINDS, SettingsMap.Keys.MOVE_DOWN, "X");
		ConfigManager.Instance.SetSetting(SettingsMap.Section.KEYBINDS, SettingsMap.Keys.MOVE_LEFT, "C");
		ConfigManager.Instance.SetSetting(SettingsMap.Section.KEYBINDS, SettingsMap.Keys.MOVE_RIGHT, "V");

		upBtn.Text = "Z";
		downBtn.Text = "X";
		leftBtn.Text = "C";
		rightBtn.Text = "V";

		// Act
		Vector2 pressPos = resetBtn.GetGlobalRect().GetCenter();
		runner.SetMousePos(pressPos);
		runner.SimulateMouseButtonPressed(MouseButton.Left);
		await runner.AwaitInputProcessed();

		// Assert
		AssertThat(upBtn.Text).IsEqual("W");
		AssertThat(downBtn.Text).IsEqual("S");
		AssertThat(leftBtn.Text).IsEqual("A");
		AssertThat(rightBtn.Text).IsEqual("D");

		string moveUpKey = ConfigManager.Instance.GetSetting<string>(SettingsMap.Section.KEYBINDS, SettingsMap.Keys.MOVE_UP, "Z");
		string moveDownKey = ConfigManager.Instance.GetSetting<string>(SettingsMap.Section.KEYBINDS, SettingsMap.Keys.MOVE_DOWN, "X");
		string moveLeftKey = ConfigManager.Instance.GetSetting<string>(SettingsMap.Section.KEYBINDS, SettingsMap.Keys.MOVE_LEFT, "C");
		string moveRightKey = ConfigManager.Instance.GetSetting<string>(SettingsMap.Section.KEYBINDS, SettingsMap.Keys.MOVE_RIGHT, "V");

		AssertThat(moveUpKey).IsEqual("W");
		AssertThat(moveDownKey).IsEqual("S");
		AssertThat(moveLeftKey).IsEqual("A");
		AssertThat(moveRightKey).IsEqual("D");
	}
}
