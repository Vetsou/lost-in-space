using GdUnit4;
using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using Newtonsoft.Json;
using static GdUnit4.Assertions;
using FileAccess = Godot.FileAccess;

namespace LostInSpace.Tests.L1.Smoke;

[TestSuite]
[RequireGodotRuntime]
public class LevelSceneTests
{
	private const string LevelsPath = "res://Resources/Levels/";

	[TestCase]
	public void AllLevelFiles_LoadsAndValidates_WithoutError()
	{
		AssertThat(DirAccess.DirExistsAbsolute(LevelsPath))
			.OverrideFailureMessage($"Levels directory not found: {LevelsPath}")
			.IsTrue();

		var levelFiles = DirAccess.GetFilesAt(LevelsPath)
			.Where(f => f.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
			.ToList();

		AssertThat(levelFiles.Count)
			.OverrideFailureMessage($"No .json level files found in {LevelsPath}")
			.IsGreater(0);

		foreach (string fileName in levelFiles)
		{
			string path = $"{LevelsPath}{fileName}";
			LevelData data = JsonConvert.DeserializeObject<LevelData>(FileAccess.GetFileAsString(path));

			AssertThat(data)
				.OverrideFailureMessage($"Failed to deserialize {path}")
				.IsNotNull();

			data.Validate();
		}
	}
}
