using Godot;

public partial class Scene : Node
{
	[Signal] public delegate void RequestSceneChangeEventHandler(SceneId sceneId);
	protected void ChangeScene(SceneId sceneId) => EmitSignal(SignalName.RequestSceneChange, (int)sceneId);
}
