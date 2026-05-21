using LostInSpace.Scripts.Level;

public static class TileData
{
	private const int HasPoint = 1 << 0;

	private const int TypeShift = 1;
	private const int TypeMask = 0b11110;

	private const int Empty = (int) PlatformType.Empty;
	private const int Regular = (int) PlatformType.Regular << TypeShift;
	private const int Goal = (int) PlatformType.Goal << TypeShift;

	public static int GetTileType(int tileValue) => (tileValue & TypeMask) >> TypeShift;
	public static bool HasPoints(int tileValue) => (tileValue & HasPoint) != 0;

	public static int CreateTile(int platformType, bool hasPoints)
	{
		int value = platformType << TypeShift;
		if (hasPoints)
		{
			value |= HasPoint;
		}
		return value;
	}

	public static int SetPoints(int tileValue, bool hasPoints)
	{
		if (hasPoints)
		{
			return tileValue | HasPoint;
		}

		return tileValue & ~HasPoint;
	}
}
