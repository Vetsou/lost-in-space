using Godot;

public partial class Scene : Node
{
	[Signal] public delegate void RequestSceneChangeEventHandler(string sceneId);
}
