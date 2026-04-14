using Godot;
using System.Threading.Tasks;

public partial class SceneManager : Node
{
	private Node sceneContainer;
	
	[Export] private Godot.Collections.Dictionary<string, Resource> sceneIds;

	public override void _Ready()
	{
		sceneContainer = GetNode("SceneContainer");

		if (sceneContainer == null)
		{
			GD.PushError("SceneContainer node not found!");
			GetTree().Quit();
			return;
		}

		#if TOOLS
		ValidateScenes();
		#endif

		_ = ChangeScene("MAIN_MENU");
	}

	// this will be async and await transitions when we need them
	private Task ChangeScene(string sceneId)
	{
		foreach (Node child in sceneContainer.GetChildren())
		{
			child.QueueFree();
		}

		if (!sceneIds.TryGetValue(sceneId, out Resource res))
		{
			GD.PushError($"Scene not found: {sceneId}");
			return Task.CompletedTask;
		}

		var newScene = (Scene)((PackedScene)res).Instantiate();
		newScene.RequestSceneChange += OnSceneChangeRequested;
		sceneContainer.AddChild(newScene);
		return Task.CompletedTask;
	}

#if TOOLS
	private void ValidateScenes()
	{
		foreach (string key in sceneIds.Keys)
		{
			Resource res = sceneIds[key];
			if (res is not PackedScene scene)
			{
				GD.PushError($"Resource for {key} is not a PackedScene");
				GetTree().Quit();
				return;
			}

			Node tempInstance = scene.Instantiate();

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

	private async void OnSceneChangeRequested(string sceneId)
	{
		await ChangeScene(sceneId);
	}
}
