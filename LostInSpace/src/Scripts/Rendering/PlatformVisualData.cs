using Godot;

[GlobalClass]
public partial class PlatformVisualData : Resource
{
	// TODO: extend this to support more visuals (shaders, particles, skeleton animations, etc?)
	[Export] public Mesh mesh;
	[Export] public Material material;
	[Export] public Color albedo;
	public Transform3D transform = Transform3D.Identity;
}
