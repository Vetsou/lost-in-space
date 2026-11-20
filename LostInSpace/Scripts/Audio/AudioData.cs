using Godot;

namespace LostInSpace.Scripts.Audio;

[GlobalClass]
public partial class AudioData : Resource
{
	[Export] public AudioStream EnterSound;
	[Export] public AudioStream ExitSound;
}
