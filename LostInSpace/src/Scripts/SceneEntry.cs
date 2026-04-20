using Godot;

[GlobalClass]
public partial class SceneEntry : Resource
{
	[Export] public SceneId id;
	[Export] public PackedScene scene;
}

