using Godot;

public interface IPlatform
{
	PlatformVisualData VisualData { get; }

	void SetPosition(Vector3 pos);

	void OnEnter(TileContext context);
	void OnExit(TileContext context);

}
