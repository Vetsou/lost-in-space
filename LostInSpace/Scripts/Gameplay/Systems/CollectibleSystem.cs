using Godot;

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
		if (_state.HasPrimaryPoint(pos))
		{
			PrimaryCurrent++;
			_state.ConsumePrimaryPoint(pos);
			renderer.RemoveCollectible(pos);
			PrimaryChanged?.Invoke();
		}

		if (_state.HasOptionalPoint(pos))
		{
			OptionalCurrent++;
			_state.ConsumeOptionalPoint(pos);
			renderer.RemoveCollectible(pos);
			OptionalChanged?.Invoke();
		}
	}
}
