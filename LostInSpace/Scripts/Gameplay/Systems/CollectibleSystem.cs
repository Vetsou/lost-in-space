using Godot;
using LostInSpace.Scripts.Gameplay.Data;

namespace LostInSpace.Scripts.Gameplay.Systems;

public class CollectibleSystem(LevelRenderer renderer)
{
	private LevelState _state;

	public ushort PrimaryCurrent { get; private set; }
	public ushort OptionalCurrent { get; private set; }
	public bool AllPrimaryCollected => PrimaryCurrent == _state.PrimaryPointsTotal;

	public event Action PrimaryChanged;
	public event Action OptionalChanged;

	public void Attach(LevelState state)
	{
		_state = state;
		PrimaryCurrent = 0;
		OptionalCurrent = 0;
		PrimaryChanged?.Invoke();
		OptionalChanged?.Invoke();
	}

	public void TryPickUp(Vector2I pos)
	{
		CollectibleId id = _state.GetCollectible(pos);
		if (id == CollectibleId.None)
		{
			return;
		}

		switch (id)
		{
			case CollectibleId.Primary:
				PrimaryCurrent++;
				PrimaryChanged?.Invoke();
				break;
			case CollectibleId.Optional:
				OptionalCurrent++;
				OptionalChanged?.Invoke();
				break;
		}

		_state.ConsumeCollectible(pos);
		renderer.RemoveCollectible(pos);
	}
}
