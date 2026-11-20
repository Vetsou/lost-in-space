using Godot;
using LostInSpace.Scripts.Audio;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FileAccess = Godot.FileAccess;

namespace LostInSpace.Scripts.Gameplay.Platforms;

public class PlatformStyle
{
	private const string BASE_MATERIAL_UID = "uid://cyu7m68i6lh30";
	private const string DEFAULT_STYLE_PATH = "res://Resources/Skins/Default/Style.json";

	private readonly record struct Entry(VisualData Visual, AudioData Audio);

	private readonly Dictionary<PlatformType, Entry> _entries;

	private PlatformStyle(Dictionary<PlatformType, Entry> entries) => _entries = entries;

	public VisualData GetVisualData(PlatformType type) => _entries[type].Visual;
	public AudioData GetAudioData(PlatformType type) => _entries[type].Audio;

	public static PlatformStyle LoadOrDefault(string path)
	{
		try
		{
			return Load(path);
		}
		catch (Exception e)
		{
			GD.PushError($"{e.Message}. Falling back to the default style");
			return Load(DEFAULT_STYLE_PATH);
		}
	}

	public static PlatformStyle Load(string path)
	{
		if (!FileAccess.FileExists(path))
		{
			throw new Exception($"Style file '{path}' does not exist");
		}

		PlatformStyleData data;
		try
		{
			data = JsonConvert.DeserializeObject<PlatformStyleData>(FileAccess.GetFileAsString(path));
			data.Validate();
		}
		catch (Exception e)
		{
			throw new Exception($"Style file '{path}' is invalid: {e.Message}", e);
		}

		ShaderMaterial baseMaterial = ResourceLoader.Load<ShaderMaterial>(BASE_MATERIAL_UID);

		Dictionary<PlatformType, Entry> entries = [];
		foreach (PlatformType type in Enum.GetValues<PlatformType>())
		{
			entries[type] = LoadEntry(data.Platforms[type], baseMaterial, path.GetBaseDir(), $"Style '{path}', platform {type}");
		}

		return new PlatformStyle(entries);
	}

	public static Mesh ExtractMesh(PackedScene scene)
	{
		Node root = scene.Instantiate();
		try
		{
			MeshInstance3D instance = root as MeshInstance3D
				?? root.FindChildren("*", nameof(MeshInstance3D), true, false).OfType<MeshInstance3D>().FirstOrDefault();
			return instance?.Mesh;
		}
		finally
		{
			root.Free();
		}
	}

	private static Entry LoadEntry(PlatformStyleEntry entry, ShaderMaterial baseMaterial, string styleDir, string context)
	{
		var visual = new VisualData
		{
			Mesh = LoadMesh(ResolvePath(entry.Model, styleDir, context), context),
			Material = CreateMaterial(entry, baseMaterial, styleDir, context),
			Albedo = ParseColor(entry.Color, context)
		};

		var audio = new AudioData
		{
			EnterSound = LoadOptional<AudioStream>(ResolvePath(entry.Sounds?.OnEnter, styleDir, context), context),
			ExitSound = LoadOptional<AudioStream>(ResolvePath(entry.Sounds?.OnExit, styleDir, context), context)
		};

		return new Entry(visual, audio);
	}

	private static string ResolvePath(string path, string styleDir, string context)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			return null;
		}

		if (path.Contains("://"))
		{
			throw new Exception($"{context}: asset '{path}' must be relative to the style folder");
		}

		return styleDir.PathJoin(path).SimplifyPath();
	}

	private static Mesh LoadMesh(string path, string context)
	{
		Resource resource = LoadRequired<Resource>(path, context);
		Mesh mesh = resource switch
		{
			Mesh directMesh => directMesh,
			PackedScene scene => ExtractMesh(scene),
			_ => throw new Exception($"{context}: model '{path}' is neither a mesh nor a scene")
		};

		return mesh ?? throw new Exception($"{context}: model '{path}' contains no mesh");
	}

	private static Material CreateMaterial(PlatformStyleEntry entry, ShaderMaterial baseMaterial, string styleDir, string context)
	{
		string shaderPath = ResolvePath(entry.Shader, styleDir, context);
		bool hasParameters = entry.ShaderParameters is { Count: > 0 };

		if (shaderPath == null && !hasParameters)
		{
			return baseMaterial;
		}

		Shader shader = LoadOptional<Shader>(shaderPath, context) ?? baseMaterial.Shader;
		if (shader.GetMode() != Shader.Mode.Spatial)
		{
			throw new Exception($"{context}: shader '{shader.ResourcePath}' is not a spatial shader");
		}

		Dictionary<string, Variant.Type> uniforms = GetUniforms(shader);
		var material = new ShaderMaterial { Shader = shader };

		if (hasParameters)
		{
			foreach ((string name, JToken value) in entry.ShaderParameters)
			{
				if (!uniforms.TryGetValue(name, out Variant.Type type))
				{
					throw new Exception($"{context}: shader '{shader.ResourcePath}' has no uniform '{name}'");
				}

				material.SetShaderParameter(name, ConvertParameter(name, value, type, styleDir, context));
			}
		}

		return material;
	}

	private static Dictionary<string, Variant.Type> GetUniforms(Shader shader)
	{
		Dictionary<string, Variant.Type> uniforms = [];
		foreach (Variant uniform in shader.GetShaderUniformList())
		{
			Godot.Collections.Dictionary info = uniform.AsGodotDictionary();
			uniforms[info["name"].AsString()] = (Variant.Type)info["type"].AsInt32();
		}

		return uniforms;
	}

	private static Variant ConvertParameter(string name, JToken value, Variant.Type type, string styleDir, string context)
	{
		Exception Invalid() => new($"{context}: parameter '{name}' value '{value.ToString(Formatting.None)}' is not valid for a {type} uniform");

		float[] ReadFloats(params int[] allowedLengths)
		{
			if (value is not JArray array
				|| !allowedLengths.Contains(array.Count)
				|| array.Any(item => item.Type is not (JTokenType.Float or JTokenType.Integer)))
			{
				throw Invalid();
			}

			return array.Select(item => item.Value<float>()).ToArray();
		}

		switch (type)
		{
			case Variant.Type.Float:
				return value.Type is JTokenType.Float or JTokenType.Integer ? value.Value<float>() : throw Invalid();
			case Variant.Type.Int:
				return value.Type == JTokenType.Integer ? value.Value<int>() : throw Invalid();
			case Variant.Type.Bool:
				return value.Type == JTokenType.Boolean ? value.Value<bool>() : throw Invalid();
			case Variant.Type.Vector2:
				float[] v2 = ReadFloats(2);
				return new Vector2(v2[0], v2[1]);
			case Variant.Type.Vector3:
				float[] v3 = ReadFloats(3);
				return new Vector3(v3[0], v3[1], v3[2]);
			case Variant.Type.Vector4:
				float[] v4 = ReadFloats(4);
				return new Vector4(v4[0], v4[1], v4[2], v4[3]);
			case Variant.Type.Color:
				if (value.Type == JTokenType.String)
				{
					return ParseColor(value.Value<string>(), context);
				}

				float[] c = ReadFloats(3, 4);
				return new Color(c[0], c[1], c[2], c.Length == 4 ? c[3] : 1f);
			case Variant.Type.Object:
				return value.Type == JTokenType.String
					? LoadRequired<Texture2D>(ResolvePath(value.Value<string>(), styleDir, context), context)
					: throw Invalid();
			default:
				throw new Exception($"{context}: parameter '{name}' has an unsupported uniform type {type}");
		}
	}

	private static Color ParseColor(string color, string context)
	{
		if (string.IsNullOrWhiteSpace(color))
		{
			return Colors.White;
		}

		return Color.HtmlIsValid(color)
			? Color.FromHtml(color)
			: throw new Exception($"{context}: invalid color '{color}'");
	}

	private static T LoadOptional<T>(string path, string context) where T : Resource =>
		string.IsNullOrWhiteSpace(path) ? null : LoadRequired<T>(path, context);

	private static T LoadRequired<T>(string path, string context) where T : Resource
	{
		if (!ResourceLoader.Exists(path))
		{
			throw new Exception($"{context}: asset '{path}' does not exist");
		}

		return ResourceLoader.Load(path) as T
			?? throw new Exception($"{context}: asset '{path}' is not a {typeof(T).Name}");
	}
}
