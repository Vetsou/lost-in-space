using Godot;
using LostInSpace.Scripts.Rendering;

namespace LostInSpace.Scripts.Gameplay.Platforms.Variants;

public class TeleporterPlatform(byte teleportLinkId) : IPlatform
{
	public byte TeleportLinkId => teleportLinkId;
	public VisualData VisualData { get; } = ResourceLoader.Load<VisualData>("uid://dva7a0u42vhvn");
	public bool IsDisabled { get; set; }

	public TileResult OnEnter(PlatformContext ctx)
	{
		if (IsDisabled)
		{
			return TileResult.Nothing;
		}

		if (ctx.TeleporterPartner is not { } partner)
		{
			return TileResult.Nothing;
		}

		return TileResult.Teleport(partner);
	}

	public TileResult OnExit(PlatformContext ctx)
	{
		IsDisabled = false;
		return TileResult.Nothing;
	}
}
