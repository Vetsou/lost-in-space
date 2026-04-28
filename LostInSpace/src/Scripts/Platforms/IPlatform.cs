public interface IPlatform
{
	void OnEnter(TileContext context);
	void OnExit(TileContext context);
	public bool HasPoint {get; set;}
}
