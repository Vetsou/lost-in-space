using Godot;

public class TileContext
{
	public required Level Level { get; set; }
	//this can be used to pass movedirection etc. into the tile
	//yeah thank you very much, actually usefull comment ;D
	public required Player Player { get; set; }
	public Vector2I MoveDirection { get; set; }
}
