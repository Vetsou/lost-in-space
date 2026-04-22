using Godot;

public interface IPlatform
{
	void OnEnter(TileContext context);
	void OnExit(TileContext context);
}