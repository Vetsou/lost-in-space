using Godot;
using LostInSpace.Scripts.Gameplay.Data;

namespace LostInSpace.Scripts.Gameplay.Systems;

public class LevelSession(LevelRenderer renderer)
{
	private LevelState _state;

	public ushort StepCount { get; private set; }
	public ushort BestStepCount { get; private set; }
	public ushort PrimaryCurrent { get; private set; }
	public ushort OptionalCurrent { get; private set; }

	private bool AllPrimaryCollected => PrimaryCurrent == _state.PrimaryPointsTotal;

	public event Action PrimaryChanged;
	public event Action OptionalChanged;
	public event Action<ushort> StepsChanged;
	public event Action<ushort> BestStepsChanged;
	public event Action LevelCompleted;

	public void StartSession(LevelState state)
	{
		_state = state;

		StepCount = 0;
		BestStepCount = 0; // TODO: Load this from file
		PrimaryCurrent = 0;
		OptionalCurrent = 0;

		StepsChanged?.Invoke(StepCount);
		BestStepsChanged?.Invoke(BestStepCount);
		PrimaryChanged?.Invoke();
		OptionalChanged?.Invoke();
	}

	public void RecordStep()
	{
		StepCount++;
		StepsChanged?.Invoke(StepCount);
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

	public void TryCompleteLevel()
	{
		if (!AllPrimaryCollected)
		{
			return;
		}

		if (BestStepCount == 0 || StepCount < BestStepCount)
		{
			BestStepCount = StepCount;
			BestStepsChanged?.Invoke(BestStepCount);
		}

		LevelCompleted?.Invoke();
	}
}
