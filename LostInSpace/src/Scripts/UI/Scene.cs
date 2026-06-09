using Godot;

public partial class Scene : Node
{
	[Signal]
	public delegate void RequestSceneChangeEventHandler(SceneId sceneId);
	[Signal]
	public delegate void RequestLevelSceneChangeEventHandler(string levelFilePath);

	protected void ChangeScene(SceneId sceneId) => EmitSignal(SignalName.RequestSceneChange, (int)sceneId);

	protected void ChangeLevelScene(string levelFilePath) => EmitSignal(SignalName.RequestLevelSceneChange, levelFilePath);
}
