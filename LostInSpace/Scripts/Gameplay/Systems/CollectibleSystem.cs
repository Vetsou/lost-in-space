using Godot;

namespace LostInSpace.Scripts.Gameplay.Systems;

public class CollectibleSystem(LevelState state, LevelRenderer renderer)
{
	public ushort PrimaryCurrent { get; private set; }
	public ushort OptionalCurrent { get; private set; }
	public bool AllPrimaryCollected => PrimaryCurrent == state.PrimaryPointsTotal;

	public bool TryPickUp(Vector2I pos)
	{
		bool changed = false;

		if (state.HasPrimaryPoint(pos))
		{
			PrimaryCurrent++;
			state.ConsumePrimaryPoint(pos);
			renderer.FreeCollectible(pos);
			changed = true;
		}

		if (state.HasOptionalPoint(pos))
		{
			OptionalCurrent++;
			state.ConsumeOptionalPoint(pos);
			renderer.FreeCollectible(pos);
			changed = true;
		}

		return changed;
	}
}
