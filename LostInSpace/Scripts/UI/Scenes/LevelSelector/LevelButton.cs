using Godot;

namespace LostInSpace.Scripts.UI.Scenes.LevelSelector;

public partial class LevelButton : Button
{
	public LevelButton(string levelFileName, Action<string> onClickCallback)
	{
		Text = levelFileName;
		Pressed += () => onClickCallback(levelFileName);
		CustomMinimumSize = new Vector2(0, 100);
		SizeFlagsHorizontal = SizeFlags.ExpandFill;
	}
}
