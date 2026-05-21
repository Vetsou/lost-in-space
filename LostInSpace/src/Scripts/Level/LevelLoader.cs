using Godot;

public partial class LevelLoader : Node
{
	[Export] public TileMap TileMap;
	[Export] public PackedScene PointScene;
	[Export] public string LevelPath;
}
