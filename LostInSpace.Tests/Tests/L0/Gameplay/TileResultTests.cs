using GdUnit4;
using Godot;
using LostInSpace.Scripts.Gameplay.Platforms;
using static GdUnit4.Assertions;

namespace LostInSpace.Tests.L0.Gameplay;

[TestSuite]
public class TileResultTests
{
	[TestCase(0, 1)]
	[TestCase(0, -1)]
	[TestCase(1, 0)]
	[TestCase(-1, 0)]
	public void ForcedMove_HasCorrectKindAndTarget(int dirX, int dirY)
	{
		// Arrange
		var direction = new Vector2I(dirX, dirY);
		var result = TileResult.ForcedMove(direction);

		// Assert
		AssertThat(result.MoveKind).IsEqual(MoveKind.ForcedMove);
		AssertThat(result.Target).IsEqual(direction);
		AssertThat(result.EnterAction).IsEqual(EnterAction.None);
		AssertThat(result.LeaveAction).IsEqual(LeaveAction.None);
	}

	[TestCase(0, 0)]
	[TestCase(12, 23)]
	[TestCase(49, 49)]
	public void Teleport_HasCorrectKindAndTarget(int posX, int posY)
	{
		// Arrange
		var position = new Vector2I(posX, posY);
		var result = TileResult.Teleport(position);

		// Assert
		AssertThat(result.MoveKind).IsEqual(MoveKind.Teleport);
		AssertThat(result.Target).IsEqual(position);
		AssertThat(result.EnterAction).IsEqual(EnterAction.None);
		AssertThat(result.LeaveAction).IsEqual(LeaveAction.None);
	}

	[TestCase]
	public void Destroy_HasCorrectLeaveAction()
	{
		// Arrange
		var result = TileResult.Destroy();

		// Assert
		AssertThat(result.LeaveAction).IsEqual(LeaveAction.DestroySelf);
		AssertThat(result.MoveKind).IsEqual(MoveKind.None);
		AssertThat(result.EnterAction).IsEqual(EnterAction.None);
		AssertThat(result.Target).IsEqual(Vector2I.Zero);
	}

	[TestCase]
	public void TryComplete_HasCorrectEnterAction()
	{
		// Arrange
		var result = TileResult.TryComplete();

		// Assert
		AssertThat(result.EnterAction).IsEqual(EnterAction.TryCompleteLevel);
		AssertThat(result.MoveKind).IsEqual(MoveKind.None);
		AssertThat(result.LeaveAction).IsEqual(LeaveAction.None);
		AssertThat(result.Target).IsEqual(Vector2I.Zero);
	}

	[TestCase]
	public void None_HasAllDefaultValues()
	{
		// Arrange
		TileResult result = TileResult.None;

		// Assert
		AssertThat(result.MoveKind).IsEqual(MoveKind.None);
		AssertThat(result.EnterAction).IsEqual(EnterAction.None);
		AssertThat(result.LeaveAction).IsEqual(LeaveAction.None);
		AssertThat(result.Target).IsEqual(Vector2I.Zero);
	}
}
