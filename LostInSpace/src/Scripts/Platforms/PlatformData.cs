using Godot;

public static class PlatformData
{
	private const int HasPoint = 1;
	private const int TypeShift = 1;
	private const int TypeMask = 0b11110;

	private const int Empty = (int) PlatformType.Empty;
	private const int Regular = (int) PlatformType.Regular << TypeShift;
	private const int Goal = (int) PlatformType.Goal << TypeShift;

	public static int GetPlatformType(int platformValue) => (platformValue & TypeMask) >> TypeShift;
	public static bool HasPoints(int platformValue) => (platformValue & HasPoint) != 0;

	public static int CreatePlatform(int platformType, bool hasPoints)
	{
		int value = platformType << TypeShift;
		if (hasPoints)
		{
			value |= HasPoint;
		}
		return value;
	}

	public static int SetPoints(int platformValue, bool hasPoints)
	{
		if (platformValue == Empty)
		{
			GD.PrintErr("Empty platforms cannot have points!");
		}
		if (hasPoints)
		{
			return platformValue | HasPoint;
		}

		return platformValue & ~HasPoint;
	}
}
