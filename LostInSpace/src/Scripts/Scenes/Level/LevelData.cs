using System;

namespace LostInSpace.Scripts.Scenes.Level;

public struct LevelData
{
	public required ushort Width { get; init; }
	public required ushort Height { get; init; }
	public required ushort[,] Platforms { get; init; }
	public required ushort PlayerPositionX { get; init; }
	public required ushort PlayerPositionY { get; init; }

	public void Validate()
	{
		if (Platforms == null)
		{
			throw new Exception("Level platforms is null");
		}
		if (Platforms.GetLength(0) != Width || Platforms.GetLength(1) != Height)
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
	}
}
