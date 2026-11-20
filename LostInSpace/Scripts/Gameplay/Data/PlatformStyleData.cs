using LostInSpace.Scripts.Gameplay.Platforms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LostInSpace.Scripts.Gameplay.Data;

public readonly record struct PlatformStyleData
{
	[JsonProperty(Required = Required.Always)]
	public required Dictionary<PlatformType, PlatformStyleEntry> Platforms { get; init; }

	public void Validate()
	{
		if (Platforms == null)
		{
			throw new Exception("Style has no platforms");
		}

		foreach (PlatformType type in Enum.GetValues<PlatformType>())
		{
			if (!Platforms.TryGetValue(type, out PlatformStyleEntry entry))
			{
				throw new Exception($"Style is missing platform type {type}");
			}

			if (string.IsNullOrWhiteSpace(entry.Model))
			{
				throw new Exception($"Style has no model for platform type {type}");
			}
		}
	}
}

public readonly record struct PlatformStyleEntry
{
	[JsonProperty(Required = Required.Always)]
	public required string Model { get; init; }

	public string Color { get; init; }
	public string Shader { get; init; }
	public Dictionary<string, JToken> ShaderParameters { get; init; }
	public PlatformSoundsData? Sounds { get; init; }
}

public readonly record struct PlatformSoundsData
{
	public string OnEnter { get; init; }
	public string OnExit { get; init; }
}
