using Godot;
using LostInSpace.Scripts.Gameplay;

namespace LostInSpace.Scripts.UI;

public partial class SceneManager : Node
{
	[Export] private Node sceneContainer;

	[Export] private Godot.Collections.Dictionary<SceneId, PackedScene> sceneIds;

	public override void _Ready()
	{
#if TOOLS
		ValidateScenes();
#endif

		ChangeScene(SceneId.MainMenu);
	}

	private void ChangeScene(SceneId sceneId) => ChangeSceneInternal(sceneId);

	private void ChangeLevelScene(string levelFilePath)
	{
		var level = (Level)ChangeSceneInternal(SceneId.Level);
		level.LoadLevel(levelFilePath);
	}

	private Scene ChangeSceneInternal(SceneId sceneId)
	{
		foreach (Node child in sceneContainer.GetChildren())
		{
			child.QueueFree();
		}

		var newScene = (Scene)sceneIds[sceneId].Instantiate();
		newScene.Connect(Scene.SignalName.RequestSceneChange, Callable.From<SceneId>(OnSceneChangeRequested));
		newScene.Connect(Scene.SignalName.RequestLevelSceneChange, Callable.From<string>(OnLevelSceneChangeRequested));
		sceneContainer.AddChild(newScene);
		return newScene;
	}

#if TOOLS
	private void ValidateScenes()
	{
		foreach (SceneId key in sceneIds.Keys)
		{
			Node tempInstance = sceneIds[key].Instantiate();

			if (tempInstance is not Scene)
			{
				GD.PushError($"Scene '{key}' root does not inherit from Scene base class");
				tempInstance.QueueFree();
				GetTree().Quit();
				return;
			}

			tempInstance.QueueFree();
		}
	}
#endif

	private void OnSceneChangeRequested(SceneId sceneId) => ChangeScene(sceneId);
	private void OnLevelSceneChangeRequested(string levelFilePath) => ChangeLevelScene(levelFilePath);
}
