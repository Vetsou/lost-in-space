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
	[TestCase]
	public void AllLevelFiles_LoadsAndValidates_WithoutError()
	{
		foreach (string fileName in DirAccess.GetFilesAt("res://Resources/Levels/"))
		{
			// Arrange
			string path = $"res://Resources/Levels/{fileName}";

			// Act & Assert
			LevelData data = JsonConvert.DeserializeObject<LevelData>(FileAccess.GetFileAsString(path));
			AssertThat(data).IsNotNull();
			data.Validate();
		}
	}
}
