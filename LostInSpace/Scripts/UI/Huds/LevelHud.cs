using Godot;
using LostInSpace.Scripts.Gameplay;

namespace LostInSpace.Scripts.UI;

public partial class LevelHud : CanvasLayer
{
	[Export] private Label _levelLabel;
	[Export] private Label _stepsLabel;
	[Export] private Label _bestLabel;
	[Export] private Label _pointsLabel;
	[Export] private Label _optionalPointsLabel;

	[Export] private Button _resetButton;
	[Export] private Button _menuButton;

	private Level _level;

	public void Bind(Level level)
	{
		Unbind();

		_level = level;
		_level.LevelLoaded += OnLevelLoaded;
		_level.StepsChanged += OnStepsChanged;
		_level.BestStepsChanged += OnBestStepsChanged;
		_level.PrimaryPointsChanged += OnPrimaryPointsChanged;
		_level.OptionalPointsChanged += OnOptionalPointsChanged;

		_resetButton.Pressed += OnResetPressed;
	}

	public void Unbind()
	{
		if (_level == null)
		{
			return;
		}

		_level.LevelLoaded -= OnLevelLoaded;
		_level.StepsChanged -= OnStepsChanged;
		_level.BestStepsChanged -= OnBestStepsChanged;
		_level.PrimaryPointsChanged -= OnPrimaryPointsChanged;
		_level.OptionalPointsChanged -= OnOptionalPointsChanged;

		_resetButton.Pressed -= OnResetPressed;
		_level = null;
	}

	private void OnLevelLoaded(string id) => _levelLabel.Text = $"{Tr("HUD_LEVEL")} {id}";
	private void OnStepsChanged(int steps) => _stepsLabel.Text = $"{Tr("HUD_STEPS")}: {steps}";
	private void OnPrimaryPointsChanged(int c, int t) => _pointsLabel.Text = $"{Tr("HUD_POINTS")}: {c} / {t}";
	private void OnBestStepsChanged(int best) => _bestLabel.Text = best > 
		0 ? $"{Tr("HUD_BEST")}: {best}" : $"{Tr("HUD_BEST")}: -";

	private void OnResetPressed() => _level.ResetLevel();

	private void OnOptionalPointsChanged(int c, int t)
	{
		bool hasOptionalPoints = t > 0;
		_optionalPointsLabel.Text =
			hasOptionalPoints ? $"{Tr("HUD_OPTIONAL")}: {c} / {t}" : $"{Tr("HUD_OPTIONAL")}: - / -";
	}
}
