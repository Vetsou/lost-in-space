using GdUnit4;
using Godot;
using LostInSpace.Scripts.Gameplay.Platforms;
using LostInSpace.Scripts.Gameplay.Platforms.Variants;
using static GdUnit4.Assertions;

namespace LostInSpace.Tests.L0.Gameplay;

[TestSuite]
[RequireGodotRuntime]
public class LevelSelectorTests
{
	[TestCase]
	public void RegularPlatform_ShouldReturnCorrectActions()
	{
		// Arrange
		var platform = new RegularPlatform();
		var ctx = new PlatformContext();

		// Act
		TileResult resultOnEnter = platform.OnEnter(ctx);
		TileResult resultOnExit = platform.OnExit(ctx);

		// Assert
		AssertThat(resultOnEnter.EnterAction).IsEqual(EnterAction.None);
		AssertThat(resultOnExit.LeaveAction).IsEqual(LeaveAction.None);
	}

	[TestCase]
	public void GoalPlatform_ShouldReturnCorrectActions()
	{
		// Arrange
		var platform = new GoalPlatform();
		var ctx = new PlatformContext();

		// Act
		TileResult resultOnEnter = platform.OnEnter(ctx);
		TileResult resultOnExit = platform.OnExit(ctx);

		// Assert
		AssertThat(resultOnEnter.EnterAction).IsEqual(EnterAction.TryCompleteLevel);
		AssertThat(resultOnExit.LeaveAction).IsEqual(LeaveAction.None);
	}

	[TestCase]
	public void BreakablePlatfrom_OnEnter_ShouldDoNothing()
	{
		// Arrange
		var platform = new BreakablePlatform();
		var ctx = new PlatformContext();

		// Act
		TileResult result = platform.OnEnter(ctx);

		// Assert
		AssertThat(result.EnterAction).IsEqual(EnterAction.None);
	}

	[TestCase]
	public void DefaultBreakablePlatfrom_OnExit_ShouldDestorySelf()
	{
		// Arrange
		var platform = new BreakablePlatform();
		var ctx = new PlatformContext();

		// Act
		TileResult result = platform.OnExit(ctx);

		// Assert
		AssertThat(result.LeaveAction).IsEqual(LeaveAction.DestroySelf);
	}

	[TestCase(0, LeaveAction.None)]
	[TestCase(1, LeaveAction.None)]
	[TestCase(2, LeaveAction.None)]
	[TestCase(3, LeaveAction.DestroySelf)]
	public void DurableBreakablePlatfrom_OnExit_ShouldDestorySelfWhenZeroHealth(int moveCount, LeaveAction expectedAction)
	{
		// Arrange
		var platform = new BreakablePlatform(3);
		var ctx = new PlatformContext();

		// Act
		TileResult result = TileResult.None;
		for (int i = 0; i < moveCount; i++)
		{
			result = platform.OnExit(ctx);
		}

		// Assert
		AssertThat(result.LeaveAction).IsEqual(expectedAction);
	}

	[TestCase(0, 1)]
	[TestCase(0, -1)]
	[TestCase(1, 0)]
	[TestCase(-1, 0)]
	public void SlipperyPlatform_OnEnter_ShouldMoveInRightDirection(int playerMoveDirectionX, int playerMoveDirectionY)
	{
		// Arrange
		var enterDirection = new Vector2I(playerMoveDirectionX, playerMoveDirectionY);
		var platform = new SlipperyPlatform();
		var ctx = new PlatformContext
		{
			MoveDirection = enterDirection
		};

		// Act
		TileResult result = platform.OnEnter(ctx);

		// Assert
		AssertThat(result.EnterAction).IsEqual(EnterAction.None);
		AssertThat(result.MoveKind).IsEqual(MoveKind.ForcedMove);
		AssertThat(result.Target).IsEqual(enterDirection);
	}

	[TestCase(0, 1)]
	[TestCase(0, -1)]
	[TestCase(1, 0)]
	[TestCase(-1, 0)]
	public void MovePlatform_OnEnter_ShouldMoveInRightDirection(int moveX, int moveY)
	{
		// Arrange
		var direction = new Vector2I(moveX, moveY);
		var platform = new MovePlatform(direction);
		var ctx = new PlatformContext();

		// Act
		TileResult result = platform.OnEnter(ctx);

		// Assert
		AssertThat(result.EnterAction).IsEqual(EnterAction.None);
		AssertThat(result.MoveKind).IsEqual(MoveKind.ForcedMove);
		AssertThat(result.Target).IsEqual(direction);
	}

	[TestCase]
	public void TeleportPlatform_OnEnter_ShouldTeleport()
	{
		// Arrange
		var destination = new Vector2I(12, 23);
		var platform = new TeleporterPlatform(1);
		var ctx = new PlatformContext
		{
			TeleporterPartner = destination
		};

		// Act
		TileResult result = platform.OnEnter(ctx);

		// Assert
		AssertThat(result.EnterAction).IsEqual(EnterAction.None);
		AssertThat(result.MoveKind).IsEqual(MoveKind.Teleport);
		AssertThat(result.Target).IsEqual(destination);
	}
}
