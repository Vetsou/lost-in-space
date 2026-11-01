namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class TeleporterPlatform(byte teleportLinkId) : IPlatform
{
	public byte TeleportLinkId => teleportLinkId;
	public PlatformType Type => PlatformType.Teleporter;

	public TileResult OnEnter(PlatformContext ctx)
	{
		if (ctx.TeleporterPartner is not { } partner)
		{
			return TileResult.None;
		}

		return TileResult.Teleport(partner);
	}

	public TileResult OnExit(PlatformContext ctx) => TileResult.None;
}
