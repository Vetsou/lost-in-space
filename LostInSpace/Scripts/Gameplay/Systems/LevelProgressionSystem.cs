namespace LostInSpace.Scripts.Gameplay.Systems;

public class LevelProgression
{
	private ushort StepCount { get; set; }
	private ushort BestStepCount { get; set; }

	public event Action<ushort> StepsChanged;
	public event Action<ushort> BestStepsChanged;
	public event Action LevelCompleted;

	public void StartLevel()
	{
		StepCount = 0;
		StepsChanged?.Invoke(StepCount);
		BestStepsChanged?.Invoke(BestStepCount);
	}

	public void IncrementStep()
	{
		StepCount++;
		StepsChanged?.Invoke(StepCount);
	}

	public void RegisterCompletion()
	{
		if (BestStepCount == 0 || StepCount < BestStepCount)
		{
			BestStepCount = StepCount;
			BestStepsChanged?.Invoke(BestStepCount);
		}
		LevelCompleted?.Invoke();
	}

	public void Reset()
	{
		StepCount = 0;
		StepsChanged?.Invoke(StepCount);
	}
}
