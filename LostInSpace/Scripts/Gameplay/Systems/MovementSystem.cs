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

	private bool DoStep(Vector2I direction, MoveKind moveKind)
	{
		Vector2I next = moveKind == MoveKind.Teleport
			? direction
			: _player.GridPosition + direction;

		if (_state.GetPlatform(next) == null)
		{
			return false;
		}

		Vector2I from = _player.GridPosition;
		TileResult exitResult = _state.GetPlatform(from)?.OnExit(BuildPlatformContext(from, direction))
			?? TileResult.None;
		HandleOnLeaveResult(exitResult, from, direction);

		switch (moveKind)
		{
			case MoveKind.Walk:
			case MoveKind.ForcedMove:
				_player.WalkTo(next, direction);
				break;
			case MoveKind.Teleport:
				_player.TeleportTo(next);
				break;
		}

		_levelSession.TryPickUp(next);
		_levelSession.RecordStep(moveKind);
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

			switch (result.MoveKind)
			{
				case MoveKind.Walk:
				case MoveKind.ForcedMove:
					if (!DoStep(result.Target, result.MoveKind))
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
		switch (result.EnterAction)
		{
			case EnterAction.TryCompleteLevel:
				_levelSession.TryCompleteLevel();
				return true;

			default:
			case EnterAction.None:
				return false;
		}
	}

	private void HandleOnLeaveResult(TileResult result, Vector2I leavingFrom, Vector2I direction)
	{
		switch (result.LeaveAction)
		{
			case LeaveAction.DemageSelf:
				_renderer.SetPlatformData(leavingFrom, result.Data);
				break;
			case LeaveAction.DestroySelf:
				_renderer.AnimateDestroy(leavingFrom, direction);
				_state.RemovePlatform(leavingFrom);
				break;
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
			TeleporterPartner = partner
		};
	}
}
