using Godot;
using Game.Constants;

public partial class ConveyorTile : Area3D 
{
	[Export] public MoveDirection Direction;

	public override void _Ready()
	{
		BodyEntered += OnPlayerEntered;
	}

	private void OnPlayerEntered(Node3D body)
	{
		GD.Print($"Tile entered by {body.Name} going {Direction}");
	}
}
