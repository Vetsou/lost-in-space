using Godot;

namespace LostInSpace.Scripts.Rendering;

[GlobalClass]
public partial class VisualData : Resource
{
	[Export] public Mesh Mesh;
	[Export] public Material Material;
	[Export] public Color Albedo;
}
