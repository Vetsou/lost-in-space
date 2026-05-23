using Godot;

public interface IPlatform
{
	public Point Point { get; set; }
	void OnEnter(TileContext context);
	void OnExit(TileContext context);
}
