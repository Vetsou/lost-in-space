public interface IPlatform
{
	public bool HasPoint { get; set; }
	void OnEnter(TileContext context);
	void OnExit(TileContext context);
}
