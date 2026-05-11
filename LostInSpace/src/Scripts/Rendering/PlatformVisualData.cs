using Godot;

[GlobalClass]
public partial class PlatformVisualData : Resource
{
	// TODO: extend this to support more visuals (shaders, particles, skeleton animations, etc?)
	[Export] public Mesh mesh;
	public Transform3D transform = Transform3D.Identity;
}
