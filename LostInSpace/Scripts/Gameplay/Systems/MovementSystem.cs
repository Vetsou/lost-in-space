using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Gameplay.Platforms;
using LostInSpace.Scripts.Gameplay.Platforms.Variants;
using LostInSpace.Scripts.Gameplay.Player;

namespace LostInSpace.Scripts.Gameplay.Systems;

public class MovementSystem(
	LevelState state,
	IPlayerView player,
	LevelSession levelSession,
	LevelRenderer renderer)
{
	private const int MaxChain = 64;

	private readonly LevelState _state = state;
	private readonly IPlayerView _player = player;
	private readonly LevelSession _levelSession = levelSession;
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
		TileResult exitResult = _state.GetPlatform(from)?.OnExit(BuildPlatformContext(from, direction))
			?? TileResult.None;
		HandleOnLeaveResult(exitResult, from);

		switch (kind)
		{
			case MoveKind.Walk:
				_player.WalkTo(next, direction);
				break;
			case MoveKind.Teleport:
				_player.TeleportTo(next, direction);
				break;
		}

		_levelSession.TryPickUp(next);
		_levelSession.RecordStep();
		return true;
	}

	private void RunTileChain(Vector2I direction)
	{
		for (int i = 0; i < MaxChain; i++)
		{
			Vector2I here = _player.GridPosition;
			IPlatform platform = _state.GetPlatform(here);
			if (platform == null)
			{
				return;
			}

			TileResult result = platform.OnEnter(BuildPlatformContext(here, direction));
			if (HandleOnEnterResult(result))
			{
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
					DoStep(result.Target, MoveKind.Teleport);
					return;
			}
			return;
		}

		throw new InvalidOperationException("Movement chain exceeded safe limit (64 moves)");
	}

	private bool HandleOnEnterResult(TileResult result)
	{
		if (result.RequestLevelComplete)
		{
			_levelSession.CompleteLevel();
			return true;
		}

		return false;
	}

	private void HandleOnLeaveResult(TileResult result, Vector2I leavingFrom)
	{
		if (result.DestroySelf)
		{
			_state.RemovePlatform(leavingFrom);
			_renderer.RemovePlatform(leavingFrom);
		}
	}

	private PlatformContext BuildPlatformContext(Vector2I position, Vector2I direction)
	{
		Vector2I? partner = null;

		if (_state.GetPlatform(position) is TeleporterPlatform tp)
		{
			(Vector2I a, Vector2I b) = _state.GetTeleporterLinkPositions(tp.TeleportLinkId);
			partner = a == position ? b : a;
		}

		return new PlatformContext
		{
			MoveDirection = direction,
			AllRequiredPointsCollected = _levelSession.AllPrimaryCollected,
			TeleporterPartner = partner
		};
	}
}
