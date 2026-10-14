using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay;

public partial class LevelRenderer : Node3D
{
	private static VisualData PrimaryCollectibleVisualData { get; } = ResourceLoader.Load<VisualData>("uid://dkssxmi6nh8h0");
	private static VisualData OptionalCollectibleVisualData { get; } = ResourceLoader.Load<VisualData>("uid://dai8uv2vxng4m");

	private LevelRenderingServer _server;

	public override void _Ready() => _server = new LevelRenderingServer(GetWorld3D());

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

				VisualData visual = state.GetCollectible(pos) switch
				{
					CollectibleId.Primary => PrimaryCollectibleVisualData,
					CollectibleId.Optional => OptionalCollectibleVisualData,
					_ => null,
				};

				if (visual != null)
				{
					_server.Render(RenderLayer.Collectibles, pos, visual);
				}
			}
		}
	}

	public void RemovePlatform(Vector2I pos) => _server.Remove(RenderLayer.Platforms, pos);
	public void RemoveCollectible(Vector2I pos) => _server.Remove(RenderLayer.Collectibles, pos);
	public void Clear() => _server.ClearLevel();
}
