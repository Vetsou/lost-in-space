using LostInSpace.Scripts.Gameplay.Platforms;
using LostInSpace.Scripts.Gameplay.Platforms.Variants;
using Newtonsoft.Json;

namespace LostInSpace.Scripts.Gameplay.Data;

public readonly record struct LevelData
{
	private const ushort MIN_LEVEL_SIZE = 2;
	private const ushort MAX_LEVEL_SIZE = 50;

	[JsonProperty(Required = Required.Always)]
	public required string LevelId { get; init; }

	[JsonProperty(Required = Required.Always)]
	public required ushort Width { get; init; }

	[JsonProperty(Required = Required.Always)]
	public required ushort Height { get; init; }

	[JsonProperty(Required = Required.Always)]
	public required ushort[,] Platforms { get; init; }

	[JsonProperty(Required = Required.Always)]
	public required CollectibleId[,] Collectibles { get; init; }

	[JsonProperty(Required = Required.Always)]
	public required ushort PlayerPositionX { get; init; }

	[JsonProperty(Required = Required.Always)]
	public required ushort PlayerPositionY { get; init; }

	public void Validate()
	{
		if (string.IsNullOrWhiteSpace(LevelId))
		{
			throw new Exception("Level ID is null or empty");
		}

		ValidateLevelBounds();
		ValidatePlayerPlacement();

		if (Collectibles.GetLength(1) != Width || Collectibles.GetLength(0) != Height)
		{
			throw new Exception("Collectibles dimensions don't match");
		}

		var teleporterCounts = new Dictionary<byte, uint>();
		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				if (!Enum.IsDefined(Collectibles[y, x]))
				{
					throw new Exception(
						$"Invalid collectible id {Collectibles[y, x]} at position x={x}, y={y}");
				}

				if (Collectibles[y, x] != CollectibleId.None && Platforms[y, x] == 0)
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

	private void ValidateLevelBounds()
	{
		if (Platforms.GetLength(1) < MIN_LEVEL_SIZE || Platforms.GetLength(0) < MIN_LEVEL_SIZE)
		{
			throw new Exception("Level needs to be at least 2x2 in size");
		}

		if (Platforms.GetLength(1) > MAX_LEVEL_SIZE || Platforms.GetLength(0) > MAX_LEVEL_SIZE)
		{
			throw new Exception("Level cannot be bigger then 50x50");
		}

		if (Platforms.GetLength(1) != Width || Platforms.GetLength(0) != Height)
		{
			throw new Exception("Level platforms dimensions don't match declared size");
		}
	}

	private void ValidatePlayerPlacement()
	{
		if (PlayerPositionX >= Width || PlayerPositionY >= Height)
		{
			throw new Exception("Player is placed outside the map");
		}

		if (Platforms[PlayerPositionY, PlayerPositionX] != 1)
		{
			throw new Exception("Player is placed on platform other than regular platform [id=1]");
		}
	}
}
