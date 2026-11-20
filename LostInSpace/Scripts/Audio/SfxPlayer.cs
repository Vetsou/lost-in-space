using Godot;

namespace LostInSpace.Scripts.Audio;

public partial class SfxPlayer : Node
{
	private const int POOL_SIZE = 8;

	public static SfxPlayer Instance { get; private set; }

	private readonly List<AudioStreamPlayer> _pool = [];

	public override void _EnterTree()
	{
		Instance = this;

		for (int i = 0; i < POOL_SIZE; i++)
		{
			var player = new AudioStreamPlayer();
			AddChild(player);
			_pool.Add(player);
		}
	}

	public override void _ExitTree() => Instance = null;

	public void Play(AudioStream stream)
	{
		if (stream == null)
		{
			return;
		}

		AudioStreamPlayer player = _pool.Find(p => !p.Playing) ?? _pool[0];
		_pool.Remove(player);
		_pool.Add(player);

		player.Stop();
		player.Stream = stream;
		player.Play();
	}
}
