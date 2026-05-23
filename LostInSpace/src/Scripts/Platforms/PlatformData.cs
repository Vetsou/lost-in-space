using Godot;

public static class PlatformData
{
	private const int HasPoint = 1;
	private const int TypeShift = 1;
	private const int TypeMask = 0b11110;

	public const int Spacing = 1;

	public static int GetPlatformType(int platformValue) => (platformValue & TypeMask) >> TypeShift;

	public static bool HasPoints(int platformValue) => (platformValue & HasPoint) != 0;
}
