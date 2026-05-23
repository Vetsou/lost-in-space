using Godot;

public partial class Point : Node3D, ICollectible
{
	public void OnCollect()
	{
		QueueFree();
	}
}
