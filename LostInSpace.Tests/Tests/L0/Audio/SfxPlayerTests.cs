using GdUnit4;
using Godot;
using LostInSpace.Scripts.Audio;
using static GdUnit4.Assertions;

namespace LostInSpace.Tests.L0.Audio;

[TestSuite]
[RequireGodotRuntime]
public class SfxPlayerTests
{
	private const int PoolSize = 8;

	private SfxPlayer _player;

	[BeforeTest]
	public void AddPlayerToTree()
	{
		_player = new SfxPlayer();
		((SceneTree)Engine.GetMainLoop()).Root.AddChild(_player);
	}

	[AfterTest]
	public void RemovePlayerFromTree() => _player.Free();

	[TestCase]
	public void Play_WithFreePlayers_ShouldKeepEarlierSoundsPlaying()
	{
		// Arrange
		List<AudioStream> sounds = CreateSounds(PoolSize);

		// Act
		sounds.ForEach(_player.Play);

		// Assert
		AssertThat(PlayingSounds().Count).IsEqual(PoolSize);
		foreach (AudioStream sound in sounds)
		{
			AssertThat(PlayingSounds().Contains(sound)).IsTrue();
		}
	}

	[TestCase]
	public void Play_WhenPoolIsFull_ShouldEndOldestSounds()
	{
		// Arrange
		List<AudioStream> sounds = CreateSounds(PoolSize + 2);

		// Act
		sounds.ForEach(_player.Play);

		// Assert
		List<AudioStream> playing = PlayingSounds();
		AssertThat(playing.Count).IsEqual(PoolSize);
		AssertThat(playing.Contains(sounds[0])).IsFalse();
		AssertThat(playing.Contains(sounds[1])).IsFalse();
		foreach (AudioStream sound in sounds.Skip(2))
		{
			AssertThat(playing.Contains(sound)).IsTrue();
		}
	}

	[TestCase]
	public void Play_WithNullStream_ShouldDoNothing()
	{
		// Act
		_player.Play(null);

		// Assert
		AssertThat(PlayingSounds().Count).IsEqual(0);
	}

	private List<AudioStream> PlayingSounds() =>
		_player.GetChildren().OfType<AudioStreamPlayer>().Where(p => p.Playing).Select(p => p.Stream).ToList();

	private static List<AudioStream> CreateSounds(int count) =>
		Enumerable.Range(0, count)
			.Select(_ => (AudioStream)new AudioStreamWav { Data = new byte[44100], MixRate = 22050 })
			.ToList();
}
