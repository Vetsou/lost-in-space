using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Gameplay.Platforms;
using LostInSpace.Scripts.Gameplay.Platforms.Variants;
using LostInSpace.Scripts.Gameplay.Player;

namespace LostInSpace.Scripts.Gameplay.Systems;

public class MovementSystem(
	LevelState state,
	IPlayerView player,
	CollectibleSystem collectibles,
	LevelProgression progression,
	LevelRenderer renderer)
{
	private const int MaxChain = 64;

	private readonly LevelState _state = state;
	private readonly IPlayerView _player = player;
	private readonly CollectibleSystem _collectibles = collectibles;
	private readonly LevelProgression _progression = progression;
	private readonly LevelRenderer _renderer = renderer;

	public bool TryMove(Vector2I direction)
	{
		if (!DoStep(direction, MoveKind.Walk))
		{
			return false;
		}

		RunTileChain(direction);
		return true;
	}

	private bool DoStep(Vector2I direction, MoveKind kind)
	{
		Vector2I next = kind == MoveKind.Walk
			? _player.GridPosition + direction
			: direction;

		if (kind == MoveKind.Walk && _state.GetPlatform(next) == null)
		{
			return false;
		}

		Vector2I from = _player.GridPosition;
		TileResult exitResult = _state.GetPlatform(from).OnExit(BuildContext(from, direction));
		if (exitResult.DestroySelf)
		{
			_state.RemovePlatform(from);
			_renderer.RemovePlatform(from);
		}

		switch (kind)
		{
			case MoveKind.Walk:
				_player.WalkTo(next, direction);
				break;
			case MoveKind.Teleport:
				_player.TeleportTo(next, direction);
				break;
		}

		_collectibles.TryPickUp(next);
		_progression.IncrementStep();
		return true;
	}

	private void RunTileChain(Vector2I direction)
	{
		for (int i = 0; i < MaxChain; i++)
		{
			// Get curr platform
			Vector2I here = _player.GridPosition;

			IPlatform platform = _state.GetPlatform(here);
			if (platform == null)
			{
				return;
			}

			TileResult result = platform.OnEnter(BuildContext(here, direction));
			if (result.RequestLevelComplete)
			{
				_progression.RegisterCompletion();
				return;
			}

			switch (result.Kind)
			{
				case MoveKind.Walk:
					if (!DoStep(result.Target, MoveKind.Walk))
					{
						return;
					}

					direction = result.Target;
					continue;
				case MoveKind.Teleport:
					if (_state.GetPlatform(result.Target) is TeleporterPlatform dest)
					{
						dest.IsDisabled = true;
					}

					DoStep(result.Target, MoveKind.Teleport);
					return;
			}

			return;
		}

		throw new InvalidOperationException("Movement chain exceeded safe limit (64 moves)");
	}

	private PlatformContext BuildContext(Vector2I position, Vector2I direction)
	{
		Vector2I? partner = null;

		if (_state.GetPlatform(position) is TeleporterPlatform tp)
		{
			(Vector2I a, Vector2I b) = _state.GetTeleporterLinkPositions(tp.TeleportLinkId);
			partner = a == position ? b : a;
		}

		return new PlatformContext
		{
			Position = position,
			MoveDirection = direction,
			AllRequiredPointsCollected = _collectibles.AllPrimaryCollected,
			TeleporterPartner = partner
		};
	}
}
