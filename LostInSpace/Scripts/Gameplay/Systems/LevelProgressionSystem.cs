namespace LostInSpace.Scripts.Gameplay.Systems;

public class LevelProgression
{
	public string LevelId { get; set; }
	public ushort StepCount { get; private set; }
	public ushort BestStepCount { get; private set; }

	public event Action<ushort> StepsChanged;
	public event Action<ushort> BestStepsChanged;
	public event Action LevelCompleted;

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
