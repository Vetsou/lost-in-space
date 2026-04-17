using System.Threading.Tasks;
using Godot;

public partial class SceneManager : Node
{
	[Export] private Node sceneContainer;

	[Export] private Godot.Collections.Dictionary<SceneId, PackedScene> sceneIds;

	public override void _Ready()
	{
#if TOOLS
		ValidateScenes();
#endif

		ChangeScene(SceneId.MAIN_MENU);
	}

	// this will be async and await transitions when we need them
	private Task ChangeScene(SceneId sceneId)
	{
		foreach (Node child in sceneContainer.GetChildren())
		{
			child.QueueFree();
		}

		var newScene = (Scene)sceneIds[sceneId].Instantiate();
		//newScene.RequestSceneChange += OnSceneChangeRequested;
		newScene.Connect(Scene.SignalName.RequestSceneChange, Callable.From<SceneId>(OnSceneChangeRequested));
		sceneContainer.AddChild(newScene);
		return Task.CompletedTask;
	}

#if TOOLS
	private void ValidateScenes()
	{
		foreach (SceneId key in sceneIds.Keys)
		{
			if (!sceneIds.TryGetValue(key, out PackedScene scene))
			{
				GD.PushError($"Scene '{key}' not found");
				GetTree().Quit();
				return;
			}

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

	private async void OnSceneChangeRequested(SceneId sceneId)
	{
		await ChangeScene(sceneId);
	}
}
