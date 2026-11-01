using GdUnit4;
using GdUnit4.Asserts;
using LostInSpace.Scripts.Gameplay.Data;
using static GdUnit4.Assertions;

namespace LostInSpace.Tests.L0.Gameplay;

[TestSuite]
public class LevelDataTests
{
	[TestCase("")]
	[TestCase("    ")]
	[TestCase(null)]
	public void Level_WithoutLevelId_ShouldThrow(string levelId)
	{
		// Arrange
		LevelData levelData = CreateValidLevel() with { LevelId = levelId };

		// Act & Assert
		IExceptionAssert ex = AssertThrown(levelData.Validate);
		ex.StartsWithMessage("Level ID is null or empty");
	}

	[TestCase(1, 1)]
	[TestCase(1, 5)]
	[TestCase(5, 1)]
	public void Level_WithTooSmallDimensions_ShouldThrow(int width, int height)
	{
		// Arrange
		LevelData levelData = CreateValidLevel() with
		{
			Platforms = new ushort[height, width]
		};

		// Act & Assert
		IExceptionAssert ex = AssertThrown(levelData.Validate);
		ex.StartsWithMessage("Level needs to be at least 2x2 in size");
	}

	[TestCase(51, 6)]
	[TestCase(6, 51)]
	[TestCase(56, 61)]
	public void Level_WithTooBigDimensions_ShouldThrow(int width, int height)
	{
		// Arrange
		LevelData levelData = CreateValidLevel() with
		{
			Platforms = new ushort[height, width]
		};

		// Act & Assert
		IExceptionAssert ex = AssertThrown(levelData.Validate);
		ex.StartsWithMessage("Level cannot be bigger then 50x50");
	}

	[TestCase]
	public void Level_WithDimensionsNotMatchingPlatformsSize_ShouldThrow()
	{
		// Arrange
		LevelData levelData = CreateValidLevel() with
		{
			Width = 4,
			Height = 6,
			Platforms = new ushort[,]
			{
				{ 1, 1 },
				{ 1, 1 },
			}
		};

		// Act & Assert
		IExceptionAssert ex = AssertThrown(levelData.Validate);
		ex.StartsWithMessage("Level platforms dimensions don't match declared size");
	}

	[TestCase(3, 3)]
	[TestCase(7, 7)]
	[TestCase(-2, 2)]
	[TestCase(2, -2)]
	public void Level_WithPlayerPositionOutsideBounds_ShouldThrow(int posX, int posY)
	{
		// Arrange
		LevelData levelData = CreateValidLevel() with
		{
			PlayerPositionX = (ushort)posX,
			PlayerPositionY = (ushort)posY
		};

		// Act & Assert
		IExceptionAssert ex = AssertThrown(levelData.Validate);
		ex.StartsWithMessage("Player is placed outside the map");
	}

	[TestCase]
	public void Level_PlayerNeedsToBePlacedOnRegularPlatform()
	{
		// Arrange
		LevelData levelData = CreateValidLevel() with
		{
			PlayerPositionX = 0,
			PlayerPositionY = 0,
			Width = 2,
			Height = 2,
			Platforms = new ushort[,]
			{
				{ 3, 1,},
				{ 1, 1 },
			},
		};

		// Act & Assert
		IExceptionAssert ex = AssertThrown(levelData.Validate);
		ex.StartsWithMessage("Player is placed on platform other than regular platform [id=1]");
	}

	[TestCase]
	public void Level_CollectiblesArray_ShouldBeSameDimensionsAsPlatforms()
	{
		// Arrange
		LevelData levelData = CreateValidLevel() with
		{
			Width = 2,
			Height = 2,
			Platforms = new ushort[,]
			{
				{ 1, 1 },
				{ 1, 1 },
			},
			Collectibles = new CollectibleId[3, 3]
		};

		// Act & Assert
		IExceptionAssert ex = AssertThrown(levelData.Validate);
		ex.StartsWithMessage("Collectibles dimensions don't match");
	}

	private static LevelData CreateValidLevel() => new()
	{
		LevelId = "test",
		Width = 3,
		Height = 3,
		Platforms = new ushort[,]
		{
			{ 1, 1, 1 },
			{ 1, 1, 1 },
			{ 1, 1, 1 },
		},
		Collectibles = new CollectibleId[3, 3],
		PlayerPositionX = 0,
		PlayerPositionY = 0,
	};
}
