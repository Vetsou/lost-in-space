using Godot;

public partial class Scene : Node
{
	[Signal] public delegate void RequestSceneChangeEventHandler(string sceneId);

	protected void ChangeScene(string sceneId)
	{
		EmitSignal(SignalName.RequestSceneChange, sceneId);
	}
}
