using Godot;

namespace LostInSpace.Scripts.Rendering;

[GlobalClass]
public partial class PlatformVisualData : Resource
{
	[Export] public Mesh Mesh;
	[Export] public Material Material;
	[Export] public Color Albedo;
}
