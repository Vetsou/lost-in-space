using Godot;
using LostInSpace.Scripts.Gameplay.Collectibles;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay;

public partial class LevelRenderer : Node
{
	[Export] private LevelRenderingServer _server;

	public void RenderState(LevelState state)
	{
		_server.SetBatchSize(state.MapSize.X * state.MapSize.Y);

		for (int x = 0; x < state.MapSize.X; x++)
		{
			for (int y = 0; y < state.MapSize.Y; y++)
			{
				var pos = new Vector2I(x, y);

				if (state.GetPlatform(pos) is { } platform)
				{
					_server.RenderPlatform(pos, platform.VisualData);
				}

				if (state.HasPrimaryPoint(pos))
				{
					_server.RenderCollectible(pos, CollectibleData.PrimaryCollectibleVisualData);
				}
				else if (state.HasOptionalPoint(pos))
				{
					_server.RenderCollectible(pos, CollectibleData.OptionalCollectibleVisualData);
				}
			}
		}
	}

	public void FreePlatform(Vector2I pos) => _server.FreePlatform(pos);
	public void FreeCollectible(Vector2I pos) => _server.FreeCollectible(pos);
	public void Clear() => _server.ClearLevel();
}
