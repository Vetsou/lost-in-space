using Godot;

namespace LostInSpace.Scripts.UI.Scenes.LevelSelector;

public partial class LevelSelector : Scene
{
	private const string LEVELS_PATH = "res://Levels/";
	[Export] private Container _buttonsContainer;
	[Export] private Label _levelNameLabel;
	private string _selectedLevelFileName;

	public override void _Ready()
	{
		string[] files = DirAccess.GetFilesAt(LEVELS_PATH)
			.Where(f => f.EndsWith(".json"))
			.Select(f => f.TrimSuffix(".json"))
			.ToArray();

		if (files.Length == 0)
		{
			throw new Exception("No valid level exists");
		}

		foreach (string file in files)
		{
			_buttonsContainer.AddChild(new LevelButton(file, SelectLevel));
		}

		SelectLevel(files.FirstOrDefault());
	}

	private void SelectLevel(string levelFileName)
	{
		_levelNameLabel.Text = levelFileName;
		_selectedLevelFileName = levelFileName;
	}

	private void OnPlayButtonPressed() => ChangeLevelScene(LEVELS_PATH + _selectedLevelFileName + ".json");
	private void OnReturnButtonPressed() => ChangeScene(SceneId.MainMenu);
}
