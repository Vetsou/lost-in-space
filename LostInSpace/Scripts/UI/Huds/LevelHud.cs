using Godot;
using LostInSpace.Scripts.Gameplay;
using System;

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
		_level = null;
	}

	private void OnLevelLoaded(string id) => _levelLabel.Text = $"Level {id}";
	private void OnStepsChanged(int steps) => _stepsLabel.Text = $"Steps: {steps}";
	private void OnBestStepsChanged(int best) => _bestLabel.Text = best > 0 ? $"Best: {best}" : "BEST: -";
	private void OnPrimaryPointsChanged(int c, int t) => _pointsLabel.Text = $"Points: {c} / {t}";
	private void OnOptionalPointsChanged(int c, int t)
	{
		bool hasOptionalPoints = t > 0;
		_optionalPointsLabel.Text = hasOptionalPoints ? $"Optional: {c} / {t}" : "Optional: - / -";
	}
}
