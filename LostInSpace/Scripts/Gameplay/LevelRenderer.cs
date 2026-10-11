using Godot;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay;

public partial class LevelRenderer : Node
{
	[Export] private LevelRenderingServer _server;
	private static VisualData PrimaryCollectibleVisualData { get; } = ResourceLoader.Load<VisualData>("uid://dkssxmi6nh8h0");
	private static VisualData OptionalCollectibleVisualData { get; } = ResourceLoader.Load<VisualData>("uid://dai8uv2vxng4m");

	public void RenderLevel(LevelState state)
	{
		_server.InitLevel(state.MapSize);

		for (int x = 0; x < state.MapSize.X; x++)
		{
			for (int y = 0; y < state.MapSize.Y; y++)
			{
				var pos = new Vector2I(x, y);

				if (state.GetPlatform(pos) is { } platform)
				{
					_server.Render(RenderLayer.Platforms, pos, platform.VisualData);
				}

				if (state.HasPrimaryPoint(pos))
				{
					_server.Render(RenderLayer.Collectibles, pos, PrimaryCollectibleVisualData);
				}
				else if (state.HasOptionalPoint(pos))
				{
					_server.Render(RenderLayer.Collectibles, pos, OptionalCollectibleVisualData);
				}
			}
		}
	}

	public void RemovePlatform(Vector2I pos) => _server.Remove(RenderLayer.Platforms, pos);
	public void RemoveCollectible(Vector2I pos) => _server.Remove(RenderLayer.Collectibles, pos);
	public void Clear() => _server.ClearLevel();
}
