using LostInSpace.Scripts.Gameplay.Platforms;
using LostInSpace.Scripts.Gameplay.Platforms.Variants;

namespace LostInSpace.Scripts.Gameplay.Data;

public readonly struct LevelData
{
	public required string LevelId { get; init; }
	public required ushort Width { get; init; }
	public required ushort Height { get; init; }
	public required ushort[,] Platforms { get; init; }
	public required ushort[,] Collectibles { get; init; }
	public required ushort PlayerPositionX { get; init; }
	public required ushort PlayerPositionY { get; init; }

	public void Validate()
	{
		if (string.IsNullOrWhiteSpace(LevelId))
		{
			throw new Exception("Level ID is null or empty");
		}

		if (Platforms == null)
		{
			throw new Exception("Level platforms is null");
		}

		if (Platforms.GetLength(1) != Width || Platforms.GetLength(0) != Height)
		{
			throw new Exception("Level platforms dimensions don't match");
		}

		if (PlayerPositionX >= Width || PlayerPositionY >= Height)
		{
			throw new Exception("Player position outside the map");
		}

		if (Platforms[PlayerPositionY, PlayerPositionX] != 1)
		{
			throw new Exception("Player is placed on platform other than regular platform [id=1]");
		}

		if (Collectibles == null)
		{
			throw new Exception("Collectibles are null");
		}

		if (Collectibles.GetLength(1) != Width || Collectibles.GetLength(0) != Height)
		{
			throw new Exception("Collectibles dimensions don't match");
		}

		var teleporterCounts = new Dictionary<byte, uint>();
		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				if (Collectibles[y, x] > 2)
				{
					throw new Exception(
						$"Invalid collectible id {Collectibles[y, x]} at position x={x}, y{y}, id should be between 0 and 2");
				}

				if (Collectibles[y, x] > 0 && Platforms[y, x] == 0)
				{
					throw new Exception($"Collectible at position x={x}, y={y}, has no platform underneath");
				}

				if (PlatformFactory.CreatePlatform(Platforms[y, x]) is TeleporterPlatform teleporter)
				{
					if (!teleporterCounts.TryAdd(teleporter.TeleportLinkId, 1))
					{
						teleporterCounts[teleporter.TeleportLinkId]++;
					}
				}
			}
		}

		foreach (KeyValuePair<byte, uint> teleporter in teleporterCounts)
		{
			if (teleporter.Value != 2)
			{
				throw new Exception(
					$"Invalid teleporter platform count {teleporter.Value} for teleport link {teleporter.Key}, it can only equal 0 or 2");
			}
		}
	}
}
