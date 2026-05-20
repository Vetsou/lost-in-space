public static class PlatformRegistry
{
	// TODO: modify this to allow user generated content, store levels in JSONs and make this a dynamic dictionary based on loaded levels
	public static Platform CreatePlaform(int id) => id switch
	{
		1 => new RegularPlatform(),
		2 => new GoalPlatform(),
		_ => null
	};
}
